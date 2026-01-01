using NegotiatRTestHarness.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Events
{
    /// <summary>
    /// Domain event raised when a credential has been successfully registered
    /// and accepted by the domain as valid.
    ///
    /// This event represents a committed change in domain state: a new
    /// credential now exists and may be used for authentication according
    /// to the domain’s invariants and policies.
    ///
    /// It is a domain event because it records a fact that became true
    /// inside the domain, independent of application workflows, user
    /// interfaces, or integration concerns.
    /// </summary>
    public sealed record CredentialRegistered
    {
        public CredentialId CredentialId { get; }
        public CredentialType Type { get; }
        public Guid UserId { get; }
        public DateTime OccurredAt { get; }
    }
}
