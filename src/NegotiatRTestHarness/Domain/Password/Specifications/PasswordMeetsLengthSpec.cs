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
    public sealed class PasswordMeetsLengthSpec
    : ISpecification<IProposal>
    {
        private const int MinLength = 12;

        public bool IsSatisfiedBy(IProposal proposal)
        {
            if (proposal is not CredentialCreationProposal method)
                return true;

            if (method.Type != CredentialType.Password)
                return true;

            var attempt = method.PasswordCreationAttempt
                .ValueOrThrow("Password creation requires a password attempt.");

            return attempt.RawPassword.Length >= MinLength;
        }
    }
}
