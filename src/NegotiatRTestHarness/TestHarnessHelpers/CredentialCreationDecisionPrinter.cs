using NegotiatRTestHarness.Application.Decisioning;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.NegotiatR;
using Optima.Net.NegotiatR.NegotiatROutcomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.TestHarnessHelpers
{
    public static class CredentialCreationDecisionPrinter
    {
        public static void Print(CredentialCreationDecision decision)
        {
            if (decision is null)
                throw new ArgumentNullException(nameof(decision));

            Console.WriteLine("=== AUTHENTICATION DECISION ===");

            switch (decision.Outcome)
            {
                case NegotiatRAccepted:
                    PrintAccepted(decision);
                    break;

                case NegotiatRCounterProposed:
                    PrintCounterProposed(decision);
                    break;

                case NegotiatRRejected:
                    PrintRejected(decision);
                    break;

                default:
                    Console.WriteLine(
                        $"Unknown outcome: {decision.Outcome.GetType().Name}");
                    break;
            }

            Console.WriteLine("================================");
        }

        private static void PrintAccepted(CredentialCreationDecision decision)
        {
            Console.WriteLine("STATUS : ACCEPTED");

            if (decision.Result.IsSuccess)
            {
                Console.WriteLine(
                    $"METHOD : {decision.Result.Value!.Type}");
            }
        }

        private static void PrintCounterProposed(CredentialCreationDecision decision)
        {
            Console.WriteLine("STATUS : COUNTER-PROPOSED");

            if (decision.Result.IsSuccess)
            {
                Console.WriteLine(
                    $"NEW METHOD : {decision.Result.Value!.Type}");
            }

            PrintFailures(
                decision.Failures.HasValue
                    ? decision.Failures.Value
                    : Array.Empty<PolicyFailure>());
        }

        private static void PrintRejected(CredentialCreationDecision decision)
        {
            Console.WriteLine("STATUS : REJECTED");

            if (decision.Result.IsFailure)
            {
                Console.WriteLine(
                    $"ERROR : {decision.Result.Error}");
            }

            PrintFailures(
                decision.Failures.HasValue
                    ? decision.Failures.Value
                    : Array.Empty<PolicyFailure>());
        }

        private static void PrintFailures(
    IReadOnlyCollection<PolicyFailure> failures)
        {
            if (failures.Count == 0)
                return;

            Console.WriteLine("FAILURES:");

            foreach (var failure in failures)
            {
                Console.WriteLine($"- Policy   : {failure.PolicyType}");
                Console.WriteLine($"  Code     : {failure.Code}");
                Console.WriteLine($"  Semantics: {failure.Semantics}");

                if (failure.Data.TryGetValue(
                        "SpecificationDiagnostics",
                        out var value)
                    && value is IEnumerable<object> specs)
                {
                    Console.WriteLine("  Failed Specifications:");

                    foreach (var spec in specs)
                    {
                        if (spec is PolicyDiagnosticResult<IProposal> d
                            && !d.Fulfilled)
                        {
                            Console.WriteLine($"    - {d.PolicyType}");
                        }
                    }
                }
            }
        }
    }
}
