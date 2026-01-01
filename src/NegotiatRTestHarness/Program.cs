// See https://aka.ms/new-console-template for more information
using NegotiatRTestHarness.Application.Commands;
using NegotiatRTestHarness.Application.Decisioning;
using NegotiatRTestHarness.Application.Handlers;
using NegotiatRTestHarness.Application.NegotiatRRules;
using NegotiatRTestHarness.Domain.Biometric.ValueObjects;
using NegotiatRTestHarness.Domain.Password.Justifications;
using NegotiatRTestHarness.Domain.Password.Policies;
using NegotiatRTestHarness.Domain.Password.Specifications;
using NegotiatRTestHarness.Domain.Password.ValueObjects;
using NegotiatRTestHarness.Infrastructure;
using NegotiatRTestHarness.TestHarnessHelpers;
using Optima.Net;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.Domain.Specification.Evaluators;
using Optima.Net.NegotiatR;
using Optima.Net.NegotiatR.NegotiatRRules;


// -----------------------------------------------------
// Domain specifications
// -----------------------------------------------------
var lengthSpec = new PasswordMeetsLengthSpec();
var rotationSpec = new PasswordMeetsRotationSpec();
var entropySpec = new PasswordMeetsEntropySpec();

// -----------------------------------------------------
// Policy + justification
// -----------------------------------------------------
var passwordCreationPolicy = new PasswordCreationPolicy();

var passwordCreationJustification =
    new PasswordCreationPolicyJustification(
        lengthSpec,
        rotationSpec,
        entropySpec);

var policies =
    new[]
    {
        (
            Policy: (IPolicy<IProposal>)passwordCreationPolicy,
            Justification: (IPolicyJustification<IProposal>)passwordCreationJustification
        )
    };



// -----------------------------------------------------
// Negotiation rules (application layer)
// -----------------------------------------------------
var rules = new INegotiatRRule[]
{
    new PasswordCreationToBiometricsRule()
};

// -----------------------------------------------------
// Evaluators & engines
// -----------------------------------------------------
var specificationEvaluator =
    new SpecificationEvaluator();

var policyEvaluator =
    new PolicyDiagnosticEvaluator(specificationEvaluator);

var negotiatR =
    new NegotiatR(rules);


// -----------------------------------------------------
// Infrastructure
// -----------------------------------------------------
var eventPublisher =
    new MockEventPublisher();

// -----------------------------------------------------
// Application handler
// -----------------------------------------------------
var handler =
    new CreatePasswordCredentialsHandler(
        policyEvaluator,
        policies,
        negotiatR,
        eventPublisher);

// -----------------------------------------------------
// Scenario input
// -----------------------------------------------------



var historyProvider = new InMemoryPasswordHistoryProvider();
var history = historyProvider.GetFor(Guid.NewGuid());

var attempt =
    new PasswordCreationAttempt(
        UserId: Guid.NewGuid(),
        RawPassword: "M1a{f*&pp",
        LastRotatedAt: DateTime.UtcNow.AddDays(-5));

var command =
    new CreatePasswordCredentialsCommand(attempt);


var passwordContext = new PasswordCreationContext(
               policy: passwordCreationPolicy,
               attempt: Optional<PasswordCreationAttempt>.Some(command.Attempt),
               history: history,
               isResetFlow: false);

var biometricsContext = new BiometricEnrollmentContext(true, true, true);

// -----------------------------------------------------
// Execute
// -----------------------------------------------------
CredentialCreationDecision decision =
    handler.Handle(command, DateTime.UtcNow,passwordContext,biometricsContext);

// -----------------------------------------------------
// Output (presentation boundary)
// -----------------------------------------------------
CredentialCreationDecisionPrinter.Print(decision);
