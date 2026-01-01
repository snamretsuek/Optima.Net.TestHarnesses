using NegotiatRTestHarness.Domain;
using NegotiatRTestHarness.Domain.Shared;
using Optima.Net;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Policy.Evaluators;
using Optima.Net.NegotiatR;
using Optima.Net.NegotiatR.NegotiatRRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Application.NegotiatRRules
{
    /// <summary>
    /// Application-layer negotiation rule that proposes biometric credential
    /// creation as an alternative when password creation fails with replaceable
    /// policy failures.
    ///
    /// This rule applies only to password-based credential creation proposals and
    /// activates when at least one replaceable policy failure is present and the
    /// environment supports biometric enrollment. It does not validate biometrics
    /// or re-evaluate policies; it merely proposes an alternative intent.
    ///
    /// The rule exists to improve user experience by suggesting a viable fallback
    /// path without altering domain state or enforcing business rules.
    /// </summary>
    public sealed class PasswordCreationToBiometricsRule : INegotiatRRule
    {
        public int Priority => 0;
        public Optional<RuleExecutionPhase> Group => Optional<RuleExecutionPhase>.None();

        public bool CanApply(
                IProposal proposal,
                IReadOnlyCollection<PolicyFailure> failures)
        {
            if (proposal is not CredentialCreationProposal p)
                return false;

            if (p.Type != CredentialType.Password)
                return false;

            if (!p.PasswordCreationAttempt.HasValue)
                return false;

            return
                failures.Any(f => f.Semantics.HasFlag(PolicyFailureSemantics.Replaceable)) &&
                p.BiometricContext.IsDeviceCapable &&
                p.BiometricContext.IsEnrollmentAllowed;
        }

        public IEnumerable<IProposal> ProposeAlternatives(
            IProposal proposal,
            IReadOnlyCollection<PolicyFailure> failures)
        {
            var original = (CredentialCreationProposal)proposal;

            yield return CredentialCreationProposal.Biometrics(
                original.BiometricContext,
                original.PasswordCreationAttempt);
        }
    }
}
