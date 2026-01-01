using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.ValueObjects
{
    /// <summary>
    /// Domain value object representing a hashed credential secret.
    ///
    /// PasswordHash encapsulates the result of a password hashing operation
    /// and exists to prevent primitive obsession and accidental misuse of
    /// raw strings within the domain model.
    ///
    /// This type does not perform hashing itself and does not expose any
    /// behavior beyond holding the hashed value. It represents a fact that
    /// is safe to store, compare, and reason about within the domain.
    /// </summary>
    public sealed record PasswordHash(string Value);
}
