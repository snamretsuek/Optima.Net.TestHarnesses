using Optima.Net.Domain.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTestHarness.Policies
{
    internal class CreditGrantingPolicy : IPolicy<CreditApplication>, INamedPolicy
    {

        public const string PolicyId = "CreditGrantingPolicy";

        public string PolicyName => PolicyId;
        public PolicyFailureSemantics FailureSemantics => PolicyFailureSemantics.Terminal;

        public bool IsSatisfiedBy(CreditApplication candidate) => true;

    }
}
