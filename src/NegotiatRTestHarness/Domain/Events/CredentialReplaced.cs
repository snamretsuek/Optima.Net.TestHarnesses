using NegotiatRTestHarness.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Events
{
    /// <summary>
    /// Domain event raised when an existing credential has been replaced
    /// by a new credential of the same logical purpose.
    ///
    /// This event indicates a meaningful domain state transition: the old
    /// credential is no longer authoritative, and the new credential is now
    /// the active representation of identity within the domain.
    ///
    /// It is a domain event because it captures an invariant-preserving
    /// change in domain truth, not a request, workflow step, or external
    /// notification.
    /// </summary>

    public sealed record CredentialReplaced
    {
        public CredentialId OldCredentialId { get; }
        public CredentialId NewCredentialId { get; }
        public CredentialType Type { get; }
        public DateTime OccurredAt { get; }
    }
}
