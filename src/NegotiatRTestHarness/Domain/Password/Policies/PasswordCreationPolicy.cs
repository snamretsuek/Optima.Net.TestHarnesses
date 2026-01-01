using NegotiatRTestHarness.Domain.Shared;
using Optima.Net.Domain.Policy;
using Optima.Net.NegotiatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.Policies
{
    public sealed class PasswordCreationPolicy : IPolicy<IProposal>, INamedPolicy
    {
        private const string _policyId = "PasswordCreationPolicy";

        public string PolicyName => _policyId;

        public bool IsSatisfiedBy(IProposal proposal)
        {
            if (proposal is not CredentialCreationProposal method)
                return false;

            return method.Type == CredentialType.Password;
        }

        public PolicyFailureSemantics FailureSemantics =>
            PolicyFailureSemantics.Correctable | PolicyFailureSemantics.Replaceable;
    }
}
