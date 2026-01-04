// See https://aka.ms/new-console-template for more information

using Optima.Net.Decisioning;
using TestHarness;

Console.WriteLine("=== Optima.Net.Decisioning Harness ===");
Console.WriteLine("Scenario: Insurance Claim Decisioning\n");

// ─────────────────────────────────────────────
// SECTION 1 — Pure Decisioning Harness (no negotiation)
// ─────────────────────────────────────────────
Console.WriteLine("▶ RUNNING DecisionHarness (pure Decisioning)\n");

var claimIntents = new[]
{
    new InsuranceClaimIntent("C-1001", "P-00991", 5000m, "Broken windshield due to storm debris"),
    new InsuranceClaimIntent("C-1002", "P-00992", 25000m, "Fire damage to residence"),
    new InsuranceClaimIntent("C-1003", "P-00993", 4000m, "Basement flood due to river overflow"),
    new InsuranceClaimIntent("C-1004", "P-00994", 12000m, "Roof repair due to hail damage")
};

foreach (var intent in claimIntents)
{
    var decision = DecisionHarness.RecordClaimDecision(intent);
    PrintDecision(decision);
    Console.WriteLine(new string('-', 70));
    Console.WriteLine();
    Console.WriteLine(new string('-', 70));
}

// ─────────────────────────────────────────────
// SECTION 2 — NegotiatR Harness (proposal-based negotiation)
// ─────────────────────────────────────────────
Console.WriteLine("\n\n\n\n");
Console.WriteLine("===  RUNNING NegotiatRDecisionHarness (proposal-based negotiation)=== ");

var proposals = new[]
{
    (ClaimSettlementProposal)ClaimSettlementProposal.Standard("C-2001", 8000m, "Standard claim under policy tier A."),
    (ClaimSettlementProposal)ClaimSettlementProposal.Standard("C-2002", 15000m, "High-value claim for partial review."),
    (ClaimSettlementProposal)ClaimSettlementProposal.Standard("C-2003", 9500m, "Moderate damage within auto-approval limit.")
};

foreach (var proposal in proposals)
{
    var decision = NegotiatRDecisionHarness.RecordNegotiatedClaimDecision(proposal);
    PrintDecision(decision);
    Console.WriteLine(new string('-', 70));
    Console.WriteLine();
    Console.WriteLine(new string('-', 70));
}

Console.WriteLine("\nExecution complete. Press any key to exit...");
Console.ReadKey();


// ─────────────────────────────────────────────
// SUPPORTING PRINTERS
// ─────────────────────────────────────────────

static void PrintDecision<TIntent, TResult>(Decision<TIntent, TResult> decision)
{
    Console.WriteLine($"Decision Outcome: {decision.Outcome}");
    Console.WriteLine($"Timestamp: {decision.Metadata.Timestamp:O}");
    Console.WriteLine($"Actor: {decision.Metadata.Actor.ValueOrDefault("Unknown")}");
    Console.WriteLine($"Correlation ID: {decision.Metadata.CorrelationId.ValueOrDefault("N/A")}");
    Console.WriteLine();

    switch (decision.Result)
    {
        case { HasValue: true }:
            Console.WriteLine("Result Details:");
            PrintResult(decision.Result.Value);
            break;
        default:
            Console.WriteLine("No result produced.");
            break;
    }

    if (decision.Evidence.HasValue)
    {
        Console.WriteLine("\nEvidence:");
        foreach (var e in decision.Evidence.Value)
            Console.WriteLine($"  - {e}");
    }

    Console.WriteLine($"\nNegotiation: {(decision.Negotiation.HasValue ? "Recorded" : "[none]")}");
}

static void PrintResult(object result)
{
    switch (result)
    {
        case ClaimDecisionOutcome outcome:
            Console.WriteLine($"  Claim ID: {outcome.ClaimId}");
            Console.WriteLine($"  Outcome Type: {outcome.OutcomeType}");
            Console.WriteLine($"  Approved Amount: {outcome.ApprovedAmount:C}");
            Console.WriteLine($"  Notes: {outcome.Notes}");
            break;

        case ClaimSettlementProposal proposal:
            Console.WriteLine($"  Claim ID: {proposal.ClaimId}");
            Console.WriteLine($"  Claimed: {proposal.ClaimedAmount:C}");
            Console.WriteLine($"  Approved: {proposal.ApprovedAmount:C}");
            Console.WriteLine($"  Notes: {proposal.SettlementNotes}");
            Console.WriteLine($"  Type: {proposal.Type}");
            break;

        default:
            Console.WriteLine($"  [Unknown result type: {result.GetType().Name}]");
            break;
    }
}
