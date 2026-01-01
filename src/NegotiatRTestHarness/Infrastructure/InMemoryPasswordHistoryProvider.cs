using NegotiatRTestHarness.Application.Contracts;
using NegotiatRTestHarness.Domain.Password.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Infrastructure
{
    /// <summary>
    /// In-memory infrastructure implementation of <see cref="IPasswordHistoryProvider"/>
    /// used by the test harness.
    ///
    /// This implementation intentionally ignores the supplied user identifier and
    /// returns a fixed password history for all calls. It exists to support
    /// deterministic execution of example scenarios and to keep the test harness
    /// focused on policy evaluation and negotiation behavior rather than user
    /// management.
    ///
    /// This type is not intended for production use. Consumers of the test harness
    /// may replace or extend it to introduce per-user behavior if required.
    /// </summary>
    public sealed class InMemoryPasswordHistoryProvider
    : IPasswordHistoryProvider
    {
        // implementation
        public PasswordHistory GetFor(Guid userId)
        {
            return new PasswordHistory(new[]
            {
                new PasswordHash("HASH::password123"),
                new PasswordHash("HASH::P@ssw0rd!"),
                new PasswordHash("HASH::Winter2023")
            });
        }
    }
}
