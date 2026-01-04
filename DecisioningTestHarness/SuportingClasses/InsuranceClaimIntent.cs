namespace TestHarness
{
    /// <summary>
    /// Represents an attempt by a policyholder to submit a claim for processing.
    /// </summary>
    public sealed record InsuranceClaimIntent(string ClaimId, string PolicyNumber, decimal ClaimedAmount, string Reason);
}
