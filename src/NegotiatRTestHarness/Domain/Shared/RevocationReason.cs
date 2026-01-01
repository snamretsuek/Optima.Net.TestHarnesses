using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Shared
{
    
    /// <summary>
    /// Enumerates the domain-recognized reasons for credential revocation.
    ///
    /// RevocationReason captures the business justification for invalidating
    /// an existing credential. It is used by the domain to record intent,
    /// support auditing, and drive downstream behavior without embedding
    /// procedural logic.
    ///
    /// This enum represents domain facts, not technical failures or
    /// infrastructure concerns.
    /// </summary>
    public enum RevocationReason
    {
        /// <summary>
        /// The credential was revoked due to suspected or confirmed compromise.
        /// </summary>
        Compromised,

        /// <summary>
        /// The credential was revoked at the explicit request of the user.
        /// </summary>
        UserRequested,

        /// <summary>
        /// The credential was revoked due to a security or policy violation.
        /// </summary>
        PolicyViolation,

        /// <summary>
        /// The credential was revoked because it was replaced by a newer credential.
        /// </summary>
        Replaced,

        /// <summary>
        /// The credential was revoked for administrative or operational reasons.
        /// </summary>
        Administrative
    }
}
