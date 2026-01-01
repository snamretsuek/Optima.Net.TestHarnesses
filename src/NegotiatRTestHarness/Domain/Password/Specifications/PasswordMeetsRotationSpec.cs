using NegotiatRTestHarness.Domain.Shared;
using Optima.Net.Domain.Specification;
using Optima.Net.Extensions;
using Optima.Net.NegotiatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.Specifications
{
    public sealed class PasswordMeetsRotationSpec
    : ISpecification<IProposal>
    {
        private static readonly TimeSpan MaxAge = TimeSpan.FromDays(90);

        public bool IsSatisfiedBy(IProposal proposal)
        {
            if (proposal is not CredentialCreationProposal method)
                return true;

            if (method.Type != CredentialType.Password)
                return true;

            var attempt = method.PasswordCreationAttempt
                .ValueOrThrow("Password cration requires a password attempt.");

            return DateTime.UtcNow - attempt.LastRotatedAt <= MaxAge;
        }
    }
}
