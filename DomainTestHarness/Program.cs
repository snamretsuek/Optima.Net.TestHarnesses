// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using DomainTestHarness;
using DomainTestHarness.Policies;
using DomainTestHarness.Specification;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.Domain.Policy.Extensions;
using Optima.Net.Domain.Specification.Evaluators;


var application = new CreditApplication(
    "Jane Smith",
    85000,//income
    120000,//credit limit
    false,
    false
    );

CreditGrantingPolicy creditGrantingPolicy = new CreditGrantingPolicy();
CreditGrantingPolicyJustification policyJustification = new CreditGrantingPolicyJustification(new CreditLimitSpec(),
                                                                    new NotBlacklistedSpec(),
                                                                    new NoPriorDefaultsSpec());

var policies = new[]
{
    (Policy: (IPolicy<CreditApplication>)creditGrantingPolicy,
     Justification: (IPolicyJustification<CreditApplication>)policyJustification)
};

PolicyDiagnosticEvaluator evaluator = new PolicyDiagnosticEvaluator(new SpecificationEvaluator());

var diagnostic = evaluator.EvaluateAll(policies, application);

var failuresText = string.Join(
    ", ",
    diagnostic.GetFailures()
        .Select(f => $"{f.PolicyType}:{f.Code}"));

Console.WriteLine(
    $"Credit Application for {application.ApplicantName} is " +
    $"{(diagnostic.Fulfilled ? "Approved." : "Denied.")} Reason(s): " +
    failuresText + ", failed.");


