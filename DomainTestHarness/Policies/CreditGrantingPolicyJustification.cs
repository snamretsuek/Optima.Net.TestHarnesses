using DomainTestHarness.Specification;
using Optima.Net.Domain.Policy;
using Optima.Net.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTestHarness.Policies
{
    public sealed class CreditGrantingPolicyJustification
    : IPolicyJustification<CreditApplication>
    {
        public string PolicyName => nameof(CreditGrantingPolicy);

        public IReadOnlyCollection<ISpecification<CreditApplication>> Specifications { get; }

        public CreditGrantingPolicyJustification(
            CreditLimitSpec creditLimitSpec,
            NotBlacklistedSpec notBlackListedSpec,
            NoPriorDefaultsSpec noPriorDefaultSpec)
        {
            Specifications = new ISpecification<CreditApplication>[]
            {
            creditLimitSpec,
            notBlackListedSpec,
            noPriorDefaultSpec
            };
        }
    }
}
