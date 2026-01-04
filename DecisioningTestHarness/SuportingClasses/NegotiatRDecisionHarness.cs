
using Optima.Net;
using Optima.Net.Decisioning;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.Domain.Specification;
using Optima.Net.Domain.Specification.Evaluators;
using Optima.Net.NegotiatR;
using Optima.Net.NegotiatR.NegotiatROutcomes;
using Optima.Net.NegotiatR.NegotiatRRules;
using System;
using System.Collections.Generic;
using System.Linq;
using TestHarness;

namespace TestHarness
{
    /// <summary>
    /// Demonstrates an integrated domain + negotiation flow
    /// where Optima.Net.Domain (policies) and Optima.Net.NegotiatR (rules)
    /// combine to produce an immutable Decision record.
    /// 
    /// NOTE:
    /// This harness operates *without intents* — it starts from a proposal
    /// and produces a final, immutable decision.
    /// </summary>
    public static class NegotiatRDecisionHarness
    {
        public static Decision<ClaimSettlementProposal, ClaimSettlementProposal> RecordNegotiatedClaimDecision(
            ClaimSettlementProposal proposal)
        {
            var metadata = new DecisionMetadata
            {
                Actor = Optional<string>.Some("Claims.Negotiation.Service"),
                Source = Optional<string>.Some("Decisioning.Harness.NegotiatR"),
                CorrelationId = Optional<string>.Some(Guid.NewGuid().ToString())
            };

            // ─────────────────────────────────────────────
            // STEP 1 — Domain evaluation (EvaluateAll)
            // ─────────────────────────────────────────────
            var specificationEvaluator = new SpecificationEvaluator();
            var diagnosticEvaluator = new PolicyDiagnosticEvaluator(specificationEvaluator);

            var claimApprovalPolicy = new ClaimApprovalPolicy();
            var claimApprovalJustification = new ClaimApprovalJustification();

            var policies = new[]
            {
                (
                    Policy: (IPolicy<IProposal>)claimApprovalPolicy,
                    Justification: (IPolicyJustification<IProposal>)claimApprovalJustification
                )
            };

            var evaluation = diagnosticEvaluator.EvaluateAll(policies, proposal);

            if (evaluation.Fulfilled)
            {
                // Domain approved proposal
                var settlement = (ClaimSettlementProposal)ClaimSettlementProposal.Standard(
                    proposal.ClaimId,
                    proposal.ClaimedAmount,
                    "Approved under standard domain evaluation."
                );

                return new Decision<ClaimSettlementProposal, ClaimSettlementProposal>(
                    proposal,
                    DecisionOutcome.Approved,
                    Optional<ClaimSettlementProposal>.Some(settlement),
                    Optional<IReadOnlyCollection<object>>.None(),
                    metadata,
                    Optional<INegotiationOutcome>.None());
            }

            // ─────────────────────────────────────────────
            // STEP 2 — Extract domain failures
            // ─────────────────────────────────────────────
            var failures = ExtractFailures(evaluation).ToArray();

            // ─────────────────────────────────────────────
            // STEP 3 — Invoke Optima.Net.NegotiatR
            // ─────────────────────────────────────────────
            var negotiator = new NegotiatR(new INegotiatRRule[]
            {
                new ClaimValueReductionRule()
            });

            var negotiationOutcome = negotiator.Negotiate(proposal, failures);

            // ─────────────────────────────────────────────
            // STEP 4 — Map NegotiatR outcome → Decision
            // ─────────────────────────────────────────────
            return negotiationOutcome switch
            {
                NegotiatRAccepted accepted => new Decision<ClaimSettlementProposal, ClaimSettlementProposal>(
                    proposal,
                    DecisionOutcome.Approved,
                    Optional<ClaimSettlementProposal>.Some(
                        (ClaimSettlementProposal)ClaimSettlementProposal.Standard(
                            proposal.ClaimId,
                            proposal.ClaimedAmount,
                            "Accepted as-is.")
                    ),
                    Optional<IReadOnlyCollection<object>>.Some(failures.Cast<object>().ToArray()),
                    metadata,
                    Optional<INegotiationOutcome>.None()),

                NegotiatRCounterProposed counter => new Decision<ClaimSettlementProposal, ClaimSettlementProposal>(
                    proposal,
                    DecisionOutcome.CounterProposed,
                    Optional<ClaimSettlementProposal>.Some(
                        (ClaimSettlementProposal)counter.CounterProposal),
                    Optional<IReadOnlyCollection<object>>.Some(failures.Cast<object>().ToArray()),
                    metadata,
                    Optional<INegotiationOutcome>.Some(new NegotiationOutcomeAdapter(
                        NegotiationDisposition.Modified,
                        Optional<object>.Some(counter.CounterProposal),
                        failures.Cast<object>().ToArray()))),

                NegotiatRRejected rejected => new Decision<ClaimSettlementProposal, ClaimSettlementProposal>(
                    proposal,
                    DecisionOutcome.Rejected,
                    Optional<ClaimSettlementProposal>.None(),
                    Optional<IReadOnlyCollection<object>>.Some(failures.Cast<object>().ToArray()),
                    metadata,
                    Optional<INegotiationOutcome>.Some(new NegotiationOutcomeAdapter(
                        NegotiationDisposition.Unchanged,
                        Optional<object>.None(),
                        failures.Cast<object>().ToArray()))),

                _ => throw new InvalidOperationException("Unknown negotiation outcome.")
            };
        }

        // Recursively flatten nested diagnostic tree into failures
        private static IEnumerable<PolicyFailure> ExtractFailures(PolicyDiagnosticResult<IProposal> result)
        {
            if (result.Fulfilled)
                yield break;

            yield return new PolicyFailure(
                result.PolicyType,
                result.Code ?? "Unknown",
                result.Semantics,
                result.Data);

            foreach (var child in result.Children)
            {
                foreach (var sub in ExtractFailures(child))
                    yield return sub;
            }
        }
    }

    // ─────────────────────────────────────────────
    // Demo Policy + Justification + Negotiation Rule
    // ─────────────────────────────────────────────

    public sealed class ClaimApprovalPolicy : IPolicy<IProposal>, INamedPolicy
    {
        public string PolicyName => "ClaimApprovalPolicy";

        public PolicyFailureSemantics FailureSemantics => PolicyFailureSemantics.Replaceable;

        public bool IsSatisfiedBy(IProposal proposal)
        {
            if (proposal is not ClaimSettlementProposal claim)
                return false;

            // Only approve up to $10,000 automatically
            return claim.ClaimedAmount <= 10000m;
        }
    }

    public sealed class ClaimApprovalJustification : IPolicyJustification<IProposal>
    {
        public string PolicyName => "ClaimApprovalPolicy";

        public IReadOnlyCollection<ISpecification<IProposal>> Specifications { get; }

        public ClaimApprovalJustification()
        {
            Specifications = Array.Empty<ISpecification<IProposal>>();
        }
    }

    /// <summary>
    /// NegotiatR rule: proposes 85% payout for claims exceeding auto-approval limit.
    /// </summary>
    public sealed class ClaimValueReductionRule : INegotiatRRule
    {
        public int Priority => 1;
        public Optional<RuleExecutionPhase> Group => Optional<RuleExecutionPhase>.None();

        public bool CanApply(IProposal proposal, IReadOnlyCollection<PolicyFailure> failures)
        {
            return proposal is ClaimSettlementProposal claim && claim.ClaimedAmount > 10000m;
        }

        public IEnumerable<IProposal> ProposeAlternatives(IProposal proposal, IReadOnlyCollection<PolicyFailure> failures)
        {
            if (proposal is ClaimSettlementProposal claim)
            {
                yield return ClaimSettlementProposal.CounterProposal(
                    claim.ClaimId,
                    claim.ClaimedAmount,
                    claim.ClaimedAmount * 0.85m,
                    "Counterproposal: partial approval (85%) under high-value policy rule.");
            }
        }
    }
}
