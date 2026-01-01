using NegotiatRTestHarness.Domain.Password.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Application.Commands
{
    /// <summary>
    /// Application-layer command representing a request to create new
    /// password-based credentials.
    ///
    /// This command carries a password creation attempt into the application
    /// workflow. It does not perform validation, policy evaluation, or credential
    /// creation itself. Those responsibilities belong to the domain and supporting
    /// services.
    ///
    /// The command exists solely to initiate an application use case that will
    /// evaluate the attempt against domain policies and, if successful, create
    /// a credential within the domain.
    /// </summary>
    public sealed record CreatePasswordCredentialsCommand(
    PasswordCreationAttempt Attempt);
}
