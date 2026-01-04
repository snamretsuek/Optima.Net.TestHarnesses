using Optima.Net.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTestHarness.Specification
{
    public sealed class NotBlacklistedSpec
    : ISpecification<CreditApplication>
    {
        public bool IsSatisfiedBy(CreditApplication app)
            => !app.IsBlacklisted;
    }
}
