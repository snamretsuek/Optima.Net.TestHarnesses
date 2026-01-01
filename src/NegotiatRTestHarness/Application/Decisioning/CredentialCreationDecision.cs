using NegotiatRTestHarness.Domain;
using NegotiatRTestHarness.Domain.Shared;
using Optima.Net;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.NegotiatR.NegotiatROutcomes;
using Optima.Net.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Application.Decisioning
{
    /// <summary>
    /// Application-layer decision result produced after completing the
    /// password credential creation workflow.
    ///
    /// This record aggregates the outcome of domain evaluation and negotiation,
    /// including the final negotiation outcome, the resulting proposal (if any),
    /// and any policy failures that informed the decision.
    ///
    /// It is not a domain concept and does not represent domain state. Instead,
    /// it exists to allow the application layer to react to domain outcomes,
    /// drive control flow, and trigger side effects such as integration events
    /// or user feedback.
    /// </summary>
    public sealed record CredentialCreationDecision(
        CredentialType CredentialType,
        NegotiatROutcome Outcome,
        Result<CredentialCreationProposal> Result,
        Optional<IReadOnlyCollection<PolicyFailure>> Failures);
        }
