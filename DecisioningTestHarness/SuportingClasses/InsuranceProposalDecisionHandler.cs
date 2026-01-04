using Optima.Net;
using Optima.Net.Decisioning;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.Domain.Specification.Evaluators;
using Optima.Net.NegotiatR;
using Optima.Net.NegotiatR.NegotiatROutcomes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHarness
{
    /// <summary>
    /// Demonstrates how domain evaluation and negotiation results are transformed
    /// into an immutable Decision record.
    ///
    /// NOTE:
    /// In a production system, this handler would **not** perform domain evaluation
    /// or negotiation itself. Those responsibilities belong to the application layer.
    ///
    /// The application would typically:
    ///   1. Run domain evaluation using PolicyDiagnosticEvaluator
    ///   2. Invoke NegotiatR to produce a NegotiatROutcome
    ///   3. Pass the NegotiatROutcome (and proposal) into this handler
    ///
    /// Here, Steps 1–3 are included only to demonstrate canonical integration flow.
    /// </summary>
    public sealed class InsuranceProposalDecisionHandler
    {
        private readonly PolicyDiagnosticEvaluator _evaluator;
        private readonly INegotiatR _negotiator;

        public InsuranceProposalDecisionHandler(
            PolicyDiagnosticEvaluator evaluator,
            INegotiatR negotiator)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
        }

        /// <summary>
        /// Evaluates a proposal, performs negotiation (for illustration),
        /// and produces a Decision&lt;IProposal, IProposal&gt; record.
        /// </summary>
        public Decision<IProposal, IProposal> Handle(
            IReadOnlyCollection<(IPolicy<IProposal> Policy, IPolicyJustification<IProposal> Justification)> policies,
            IProposal proposal)
        {
            if (proposal is null)
                throw new ArgumentNullException(nameof(proposal));

            if (policies is null)
                throw new ArgumentNullException(nameof(policies));

            var metadata = new DecisionMetadata
            {
                Actor = Optional<string>.Some("Application.PolicyAdjudicator"),
                Source = Optional<string>.Some("Optima.Net.Decisioning.Integration"),
                CorrelationId = Optional<string>.Some(Guid.NewGuid().ToString())
            };

            // ─────────────────────────────────────────────
            // STEP 1 — Domain evaluation (for illustration)
            // ─────────────────────────────────────────────
            // In practice, this would already have been executed outside this handler.
            var result = _evaluator.EvaluateAll(policies, proposal);

            if (result.Fulfilled)
            {
                // Domain approved the proposal.
                return new Decision<IProposal, IProposal>(
                    proposal,
                    DecisionOutcome.Approved,
                    Optional<IProposal>.Some(proposal),
                    Optional<IReadOnlyCollection<object>>.None(),
                    metadata,
                    Optional<INegotiationOutcome>.None());
            }

            // ─────────────────────────────────────────────
            // STEP 2 — Extract policy failures
            // ─────────────────────────────────────────────
            var failures = ExtractFailures(result).ToArray();

            // ─────────────────────────────────────────────
            // STEP 3 — Run negotiation (for illustration)
            // ─────────────────────────────────────────────
            // Normally the NegotiatROutcome would be passed into this handler.
            var negotiation = _negotiator.Negotiate(proposal, failures);

            // ─────────────────────────────────────────────
            // STEP 4 — Map NegotiatR outcome → Decision
            // ─────────────────────────────────────────────
            return MapOutcomeToDecision(proposal, failures, negotiation, metadata);
        }

        private static IEnumerable<PolicyFailure> ExtractFailures(
            PolicyDiagnosticResult<IProposal> result)
        {
            if (result.Fulfilled)
                yield break;

            if (!result.Fulfilled)
            {
                yield return new PolicyFailure(
                    result.PolicyType,
                    result.Code ?? "Unknown",
                    result.Semantics,
                    result.Data);
            }

            foreach (var child in result.Children)
            {
                foreach (var sub in ExtractFailures(child))
                    yield return sub;
            }
        }

        private static Decision<IProposal, IProposal> MapOutcomeToDecision(
            IProposal original,
            IReadOnlyCollection<PolicyFailure> failures,
            NegotiatROutcome outcome,
            DecisionMetadata metadata)
        {
            switch (outcome)
            {
                case NegotiatRAccepted accepted:
                    return new Decision<IProposal, IProposal>(
                        original,
                        DecisionOutcome.Approved,
                        Optional<IProposal>.Some(original),
                        Optional<IReadOnlyCollection<object>>.Some(failures.Cast<object>().ToList()),
                        metadata,
                        Optional<INegotiationOutcome>.None());

                case NegotiatRCounterProposed counter:
                    var negotiation = new NegotiationOutcomeAdapter(
                        NegotiationDisposition.Modified,
                        Optional<object>.Some(counter.CounterProposal),
                        failures.Cast<object>().ToList());
                    return new Decision<IProposal, IProposal>(
                        original,
                        DecisionOutcome.CounterProposed,
                        Optional<IProposal>.Some(counter.CounterProposal),
                        Optional<IReadOnlyCollection<object>>.Some(failures.Cast<object>().ToList()),
                        metadata,
                        Optional<INegotiationOutcome>.Some(negotiation));

                case NegotiatRRejected rejected:
                    var negotiationRejected = new NegotiationOutcomeAdapter(
                        NegotiationDisposition.Unchanged,
                        Optional<object>.None(),
                        failures.Cast<object>().ToList());
                    return new Decision<IProposal, IProposal>(
                        original,
                        DecisionOutcome.Rejected,
                        Optional<IProposal>.None(),
                        Optional<IReadOnlyCollection<object>>.Some(failures.Cast<object>().ToList()),
                        metadata,
                        Optional<INegotiationOutcome>.Some(negotiationRejected));

                default:
                    throw new InvalidOperationException(
                        $"Unknown negotiation outcome type: {outcome.GetType().Name}");
            }
        }
    }

    
}
