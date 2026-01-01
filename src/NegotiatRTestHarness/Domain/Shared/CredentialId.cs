using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Shared
{
    /// <summary>
    /// Strongly-typed identifier for a credential.
    /// CredentialId is a domain value object that uniquely identifies
    /// a credential within the domain. It enforces GUID-based identity
    /// to guarantee uniqueness and prevent invalid identifiers from
    /// entering the domain.
    /// </summary>
    /// 
    public readonly record struct CredentialId
    {
        public Guid Value { get; }

        private CredentialId(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new unique CredentialId.
        /// </summary>
        public static CredentialId New()
            => new CredentialId(Guid.NewGuid());

        /// <summary>
        /// Rehydrates a CredentialId from an existing GUID value.
        /// </summary>
        public static CredentialId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException(
                    "CredentialId cannot be empty.",
                    nameof(value));

            return new CredentialId(value);
        }

        /// <summary>
        /// Rehydrates a CredentialId from a string representation of a GUID.
        /// Throws if the value is not a valid GUID.
        /// </summary>
        public static CredentialId From(string value)
        {
            if (!Guid.TryParse(value, out var guid))
                throw new ArgumentException(
                    "CredentialId must be a valid GUID.",
                    nameof(value));

            if (guid == Guid.Empty)
                throw new ArgumentException(
                    "CredentialId cannot be empty.",
                    nameof(value));

            return new CredentialId(guid);
        }

        public override string ToString() => Value.ToString("D");
    }

}
