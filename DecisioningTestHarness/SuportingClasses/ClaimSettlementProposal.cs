
using Optima.Net.NegotiatR;


namespace TestHarness
{
    /// <summary>
    /// Represents a proposed insurance claim settlement.
    /// Immutable, expressive, and semantically equivalent
    /// to other IProposal implementations (e.g., CredentialCreationProposal).
    /// </summary>
    public sealed class ClaimSettlementProposal : IProposal
    {
        public ProposalId Id { get; }
        public SettlementType Type { get; }

        public string ClaimId { get; }
        public decimal ClaimedAmount { get; }          // NEW
        public decimal ApprovedAmount { get; }
        public string SettlementNotes { get; }

        private ClaimSettlementProposal(
            ProposalId id,
            SettlementType type,
            string claimId,
            decimal claimedAmount,
            decimal approvedAmount,
            string settlementNotes)
        {
            Id = id;
            Type = type;
            ClaimId = claimId;
            ClaimedAmount = claimedAmount;
            ApprovedAmount = approvedAmount;
            SettlementNotes = settlementNotes;
        }

        // ─────────────────────────────────────────────
        // Factory methods
        // ─────────────────────────────────────────────

        public static IProposal Standard(
            string claimId,
            decimal claimedAmount,
            string notes)
            => new ClaimSettlementProposal(
                ProposalId.New(),
                SettlementType.Standard,
                claimId,
                claimedAmount,
                claimedAmount,
                notes);

        public static IProposal CounterProposal(
            string claimId,
            decimal claimedAmount,
            decimal proposedAmount,
            string justification)
            => new ClaimSettlementProposal(
                ProposalId.New(),
                SettlementType.CounterProposal,
                claimId,
                claimedAmount,
                proposedAmount,
                justification);

        public static IProposal Rejected(
            string claimId,
            decimal claimedAmount,
            string reason)
            => new ClaimSettlementProposal(
                ProposalId.New(),
                SettlementType.Rejected,
                claimId,
                claimedAmount,
                0m,
                reason);
    }



}
