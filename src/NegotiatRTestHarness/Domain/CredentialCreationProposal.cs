using NegotiatRTestHarness.Domain.Biometric.ValueObjects;
using NegotiatRTestHarness.Domain.Password.ValueObjects;
using NegotiatRTestHarness.Domain.Shared;
using Optima.Net;
using Optima.Net.NegotiatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain
{

    /// <summary>
    /// Represents an evaluation aggregate root.
    /// 
    /// A proposal is an immutable snapshot of all facts required
    /// to evaluate a single decision atomically.
    /// 
    /// ProposalId uniquely identifies the aggregate for the lifetime
    /// of the evaluation.
    /// </summary>
    public sealed class CredentialCreationProposal : IProposal
    {
        public ProposalId Id { get; }
        public CredentialType Type { get; }

        public Optional<PasswordCreationAttempt> PasswordCreationAttempt { get; }
        public BiometricEnrollmentContext BiometricContext { get; }

        private CredentialCreationProposal(
            ProposalId id,
            CredentialType type,
            Optional<PasswordCreationAttempt> passwordCreationAttempt,
            BiometricEnrollmentContext biometricContext)
        {
            Id = id;
            Type = type;
            PasswordCreationAttempt = passwordCreationAttempt;
            BiometricContext = biometricContext;
        }

        public static IProposal Password(
            Optional<PasswordCreationAttempt> attempt,
            BiometricEnrollmentContext biometricsContext)
            => new CredentialCreationProposal(
                ProposalId.New(),
                CredentialType.Password,
                attempt,
                biometricsContext);

        public static IProposal Biometrics(
            BiometricEnrollmentContext biometricsContext,
            Optional<PasswordCreationAttempt> attempt)
            => new CredentialCreationProposal(
                ProposalId.New(),
                CredentialType.Biometrics,
                attempt,
                biometricsContext);
    }
}
