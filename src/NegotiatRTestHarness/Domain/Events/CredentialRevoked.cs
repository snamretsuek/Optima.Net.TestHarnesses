using NegotiatRTestHarness.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Events
{
    /// <summary>
/// Domain event raised when a credential has been revoked and is no longer
/// valid for use within the domain.
///
/// Revocation represents a security-relevant change in domain state.
/// the credential ceases to be acceptable under the domain’s rules,
/// regardless of how or why the revocation was triggered.
///
/// This is a domain event because it records a durable fact about domain
/// truth, not an application-level action or integration signal.
/// </summary>


    public sealed record CredentialRevoked
    {
        public CredentialId CredentialId { get; }
        public CredentialType Type { get; }
        public RevocationReason Reason { get; }
        public DateTime OccurredAt { get; }
    }

}
