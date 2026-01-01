using NegotiatRTestHarness.Domain.Password.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Application.Contracts
{
    public interface IPasswordHistoryProvider
    {
        PasswordHistory GetFor(Guid userId);
    }
}
