
using Optima.Net;
using Optima.Net.Decisioning;

namespace TestHarness
{
    /// <summary>
    /// Simulates the creation of insurance claim decisions without any external dependencies.
    /// Demonstrates Decisioning as a semantic record layer — no evaluation, no negotiation.
    /// </summary>
    public static class DecisionHarness
    {
        public static Decision<InsuranceClaimIntent, ClaimDecisionOutcome> RecordClaimDecision(
            InsuranceClaimIntent intent)
        {
            var metadata = new DecisionMetadata
            {
                Actor = Optional<string>.Some("Claims.Adjudicator.System"),
                Source = Optional<string>.Some("Optima.Net.Decisioning.Harness"),
                CorrelationId = Optional<string>.Some(Guid.NewGuid().ToString())
            };

            // --- CASE: Negotiation-like partial agreement (hail damage) ---
            if (intent.ClaimedAmount == 12000m &&
                intent.Reason.Contains("hail", StringComparison.OrdinalIgnoreCase))
            {
                var counterOutcome = ClaimDecisionOutcome.CounterProposed(
                    intent.ClaimId,
                    9000m,
                    "Partial approval under hail coverage clause #HAIL-2025."
                );

                var negotiation = Optional<INegotiationOutcome>.Some(
                    new NegotiationOutcome(
                        NegotiationDisposition.Modified,
                        Optional<object>.Some(counterOutcome),
                        new List<object>
                        {
                            "Customer initially requested $12,000 for full roof replacement.",
                            "Coverage tier caps hail-related damage at $9,000.",
                            "Customer accepted counterproposal."
                        }
                    )
                );

                var evidence = Optional<IReadOnlyCollection<object>>.Some(new List<object>
                {
                    "Negotiation concluded with partial payout agreement."
                });

                return new Decision<InsuranceClaimIntent, ClaimDecisionOutcome>(
                    intent,
                    DecisionOutcome.CounterProposed,
                    Optional<ClaimDecisionOutcome>.Some(counterOutcome),
                    evidence,
                    metadata,
                    negotiation
                );
            }

            // --- CASE: Amount exceeds auto-approval threshold ---
            if (intent.ClaimedAmount > 10000m)
            {
                var evidence = Optional<IReadOnlyCollection<object>>.Some(new List<object>
                {
                    "Claim amount exceeds auto-approval threshold ($10,000).",
                    "Manual review required under risk policy #RC-2025.",
                    "No prior authorization found for high-value claim."
                });

                var deferredOutcome = ClaimDecisionOutcome.Deferred(
                    intent.ClaimId,
                    "Awaiting manual review for high-value claim."
                );

                return new Decision<InsuranceClaimIntent, ClaimDecisionOutcome>(
                    intent,
                    DecisionOutcome.Deferred,
                    Optional<ClaimDecisionOutcome>.Some(deferredOutcome),
                    evidence,
                    metadata,
                    Optional<INegotiationOutcome>.None()
                );
            }

            // --- CASE: Flood exclusion ---
            if (intent.Reason.Contains("flood", StringComparison.OrdinalIgnoreCase))
            {
                var evidence = Optional<IReadOnlyCollection<object>>.Some(new List<object>
                {
                    "Flood damage not covered under current policy tier.",
                    "Refer to exclusion clause #17B."
                });

                var rejectedOutcome = ClaimDecisionOutcome.Rejected(
                    intent.ClaimId,
                    "Flood exclusion applied."
                );

                return new Decision<InsuranceClaimIntent, ClaimDecisionOutcome>(
                    intent,
                    DecisionOutcome.Rejected,
                    Optional<ClaimDecisionOutcome>.Some(rejectedOutcome),
                    evidence,
                    metadata,
                    Optional<INegotiationOutcome>.None()
                );
            }

            // --- CASE: Standard approval ---
            var approvedOutcome = ClaimDecisionOutcome.Approved(
                intent.ClaimId,
                intent.ClaimedAmount * 0.9m,
                "Approved under standard coverage terms."
            );

            return new Decision<InsuranceClaimIntent, ClaimDecisionOutcome>(
                intent,
                DecisionOutcome.Approved,
                Optional<ClaimDecisionOutcome>.Some(approvedOutcome),
                Optional<IReadOnlyCollection<object>>.None(),
                metadata,
                Optional<INegotiationOutcome>.None()
            );
        }
    }
}
