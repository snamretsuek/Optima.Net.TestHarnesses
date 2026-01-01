using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.ValueObjects
{
    public sealed record PasswordCreationAttempt(
    Guid UserId,
    string RawPassword,
    DateTime LastRotatedAt
    );
}
