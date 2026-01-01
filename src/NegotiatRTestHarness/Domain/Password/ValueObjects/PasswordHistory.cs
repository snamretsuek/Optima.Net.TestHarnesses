using NegotiatRTestHarness.TestHarnessHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.ValueObjects
{
    /// <summary>
    /// Domain value object representing the historical record of previously
    /// used credential secrets for a user.
    ///
    /// PasswordHistory is used to enforce password-related invariants such
    /// as reuse prevention. It is immutable and compared by value, but may
    /// contain domain behavior required to answer questions about that history.
    ///
    /// This type does not perform hashing itself; it relies on a provided
    /// hasher to avoid infrastructure concerns leaking into the domain.
    /// </summary>
    public sealed record class PasswordHistory
    {
        private readonly IReadOnlyCollection<PasswordHash> _previous;

        public PasswordHistory(IEnumerable<PasswordHash> previousPasswords)
        {
            _previous = previousPasswords?.ToArray()
                ?? throw new ArgumentNullException(nameof(previousPasswords));
        }

        /// <summary>
        /// Determines whether the supplied raw password has been used before,
        /// according to the provided hashing strategy.
        /// </summary>
        public bool HasBeenUsed(string rawPassword, IPasswordHasher hasher)
        {
            if (rawPassword is null)
                throw new ArgumentNullException(nameof(rawPassword));
            if (hasher is null)
                throw new ArgumentNullException(nameof(hasher));

            var hash = hasher.Hash(rawPassword);
            return _previous.Contains(hash);
        }
    }
}
