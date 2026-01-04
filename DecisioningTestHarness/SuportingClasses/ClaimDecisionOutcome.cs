using System;

namespace TestHarness
{
    /// <summary>
    /// Represents the finalized outcome of a claim as recorded by Decisioning.
    /// Framework-agnostic, side-effect free, and immutable.
    /// </summary>
    public sealed class ClaimDecisionOutcome
    {
        public string ClaimId { get; }
        public decimal ApprovedAmount { get; }
        public string Notes { get; }
        public ClaimDecisionOutcomeType OutcomeType { get; }

        public ClaimDecisionOutcome(string claimId, decimal approvedAmount, string notes, ClaimDecisionOutcomeType outcomeType)
        {
            ClaimId = claimId;
            ApprovedAmount = approvedAmount;
            Notes = notes;
            OutcomeType = outcomeType;
        }

        // ─────────────────────────────────────────────
        // Factory methods for semantic clarity
        // ─────────────────────────────────────────────

        public static ClaimDecisionOutcome Approved(string claimId, decimal amount, string notes)
            => new(claimId, amount, notes, ClaimDecisionOutcomeType.Approved);

        public static ClaimDecisionOutcome Deferred(string claimId, string reason)
            => new(claimId, 0m, reason, ClaimDecisionOutcomeType.Deferred);

        public static ClaimDecisionOutcome Rejected(string claimId, string reason)
            => new(claimId, 0m, reason, ClaimDecisionOutcomeType.Rejected);

        public static ClaimDecisionOutcome CounterProposed(string claimId, decimal offeredAmount, string justification)
            => new(claimId, offeredAmount, justification, ClaimDecisionOutcomeType.CounterProposed);
    }

    public enum ClaimDecisionOutcomeType
    {
        Approved,
        Deferred,
        Rejected,
        CounterProposed
    }
}
