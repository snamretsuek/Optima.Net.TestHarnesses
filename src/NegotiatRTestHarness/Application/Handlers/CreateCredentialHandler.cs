using NegotiatRTestHarness.Application.Commands;
using NegotiatRTestHarness.Application.Contracts;
using NegotiatRTestHarness.Application.Decisioning;
using NegotiatRTestHarness.Application.IntegrationEvents;
using NegotiatRTestHarness.Domain;
using NegotiatRTestHarness.Domain.Biometric.ValueObjects;
using NegotiatRTestHarness.Domain.Password.Policies;
using NegotiatRTestHarness.Domain.Password.ValueObjects;
using NegotiatRTestHarness.Domain.Shared;
using NegotiatRTestHarness.Infrastructure;
using Optima.Net;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.Domain.Policy.Extensions;
using Optima.Net.NegotiatR;
using Optima.Net.NegotiatR.NegotiatROutcomes;
using Optima.Net.Result;


namespace NegotiatRTestHarness.Application.Handlers
{
    /// <summary>
    /// Application-layer handler responsible for orchestrating the password
    /// credential creation use case.
    ///
    /// This handler receives a <see cref="CreatePasswordCredentialsCommand"/>,
    /// assembles the required domain proposal and contexts, evaluates domain
    /// policies via the policy diagnostic evaluator, and applies negotiation
    /// when necessary.
    ///
    /// It does not contain business rules, policy logic, or specification
    /// evaluation. Those responsibilities belong to the domain. This handler
    /// coordinates the flow and reacts to domain outcomes, including publishing
    /// integration events when appropriate.
    /// </summary>
    public class CreatePasswordCredentialsHandler
    {
        private readonly PolicyDiagnosticEvaluator _policyEvaluator;
        private readonly IReadOnlyCollection<
            (IPolicy<IProposal> Policy,
             IPolicyJustification<IProposal> Justification)
        > _policies;

        private readonly INegotiatR _negotiatR;
        private readonly IEventPublisher _eventPublisher;

        public CreatePasswordCredentialsHandler(
            PolicyDiagnosticEvaluator policyEvaluator,
            IReadOnlyCollection<
                (IPolicy<IProposal>,
                 IPolicyJustification<IProposal>)
            > policies,
            INegotiatR negotiatR,
            IEventPublisher eventPublisher)
        {
            _policyEvaluator = policyEvaluator
                ?? throw new ArgumentNullException(nameof(policyEvaluator));

            _policies = policies
               ?? throw new ArgumentNullException(nameof(policies));

            _negotiatR = negotiatR
                ?? throw new ArgumentNullException(nameof(negotiatR));
            _eventPublisher = eventPublisher
                ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public CredentialCreationDecision Handle(
            CreatePasswordCredentialsCommand command, 
            DateTime now,
            PasswordCreationContext passwordCreationContext,
            BiometricEnrollmentContext biometricEnrollmentContext)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));


            var passwordPolicy = new PasswordCreationPolicy();
            
            var passwordContext = passwordCreationContext;

            var biometricsContext = biometricEnrollmentContext;

            // -------------------------------------------------
            // Build proposal (application-level enrichment)
            // -------------------------------------------------

            var proposal = CredentialCreationProposal.Password(passwordContext.Attempt,biometricsContext);

            // -------------------------------------------------
            // 1. Evaluate domain policies
            // -------------------------------------------------
            var diagnostics =
                _policyEvaluator.EvaluateAll(_policies, proposal);

            var failures =
                diagnostics.GetFailures().ToArray();

            var outcome =
               _negotiatR.Negotiate(proposal, failures);

            // -------------------------------------------------
            // 3. Handle outcome + publish events
            // -------------------------------------------------

            switch (outcome)
            {
                case NegotiatRAccepted accepted:
                    {
                        var method =
                            (CredentialCreationProposal)accepted.Proposal;

                        _eventPublisher.Publish(
                            CredentialCreationMethodSucceededEvent.Create(
                                credentialType:CredentialType.Password,
                                source: "CredentialService",
                                occurredAt: now));

                        return new CredentialCreationDecision(
                            CredentialType: CredentialType.Password,
                            Outcome: outcome,
                            Result: Result<CredentialCreationProposal>.Ok(method),
                            Failures: Optional<IReadOnlyCollection<PolicyFailure>>.None());
                    }

                case NegotiatRCounterProposed counter:
                    {
                        var original =
                            (CredentialCreationProposal)counter.OriginalProposal;

                        var alternative =
                            (CredentialCreationProposal)counter.CounterProposal;

                        _eventPublisher.Publish(
                            CredentialCreationMethodChangedEvent.Create(
                                from: original.Type.ToString(),
                                to: alternative.Type.ToString(),
                                source: "AuthenticationService",
                                occurredAt: now));

                        return new CredentialCreationDecision(
                            CredentialType: CredentialType.Password,
                            Outcome: outcome,
                            Result: Result<CredentialCreationProposal>.Ok(alternative),
                            Failures: Optional<IReadOnlyCollection<PolicyFailure>>.Some(counter.Failures));
                    }

                case NegotiatRRejected rejected:
                    {
                        _eventPublisher.Publish(
                            CredentialCreationMethodRejectedEvent.Create(
                                rejected.Failures.Select(f => f.Code).ToArray(),
                                source: "AuthenticationService",
                                occurredAt: now));

                        return new CredentialCreationDecision(
                            CredentialType: CredentialType.Password,
                            Outcome: outcome,
                            Result: Result<CredentialCreationProposal>.Fail("Authentication rejected"),
                            Failures: Optional<IReadOnlyCollection<PolicyFailure>>.Some(rejected.Failures));
                    }

                default:
                    throw new InvalidOperationException(
                        $"Unhandled NegotiatR outcome: {outcome.GetType().Name}");
            }
        }
    }
}
