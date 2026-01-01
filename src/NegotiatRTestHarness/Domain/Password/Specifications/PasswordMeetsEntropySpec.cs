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
    public sealed class PasswordMeetsEntropySpec
    : ISpecification<IProposal>
    {
        public bool IsSatisfiedBy(IProposal proposal)
        {
            if (proposal is not CredentialCreationProposal method)
                return true;

            if (method.Type != CredentialType.Password)
                return true;

            var attempt = method.PasswordCreationAttempt
                .ValueOrThrow("Password authentication requires a password attempt.");

            var password = method.PasswordCreationAttempt.Value.RawPassword;

            return password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit)
                && password.Any(char.IsSymbol);
        }
    }
}
