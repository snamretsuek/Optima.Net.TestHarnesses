using NegotiatRTestHarness.Domain.Password.Policies;
using Optima.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Password.ValueObjects
{
    public sealed record PasswordCreationContext
    {
        public PasswordCreationPolicy Policy { get; }
        public Optional<PasswordCreationAttempt> Attempt { get; }
        public PasswordHistory History { get; }
        public bool IsResetFlow { get; }

        public PasswordCreationContext(
            PasswordCreationPolicy policy,
            Optional<PasswordCreationAttempt> attempt,
            PasswordHistory history,
            bool isResetFlow)
        {
            Policy = policy;
            Attempt = attempt;
            History = history;
            IsResetFlow = isResetFlow;
        }
    }
}
