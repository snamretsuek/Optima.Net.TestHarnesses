using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTestHarness
{
    public sealed class CreditApplication
    {
        public string ApplicantName { get; }
        public decimal Monthlyncome { get; }
        public decimal RequestedCreditLimit { get; }
        public bool HasPriorDefaults { get; }
        public bool IsBlacklisted { get; }

        public CreditApplication(
            string applicantName,
            int monthlyIncome,
            decimal requestedCredit,
            bool hasPriorDefaults,
            bool isBlacklisted)
        {
            ApplicantName = applicantName;
            Monthlyncome = monthlyIncome;
            RequestedCreditLimit = requestedCredit;
            HasPriorDefaults = hasPriorDefaults;
            IsBlacklisted = isBlacklisted;
        }
    }
}
