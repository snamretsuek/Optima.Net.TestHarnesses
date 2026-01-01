using NegotiatRTestHarness.Domain.Password.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.TestHarnessHelpers
{
    internal sealed class FakePasswordHasher : IPasswordHasher
    {
        public PasswordHash Hash(string rawPassword)
            => new($"HASH::{rawPassword}");
    }
}
