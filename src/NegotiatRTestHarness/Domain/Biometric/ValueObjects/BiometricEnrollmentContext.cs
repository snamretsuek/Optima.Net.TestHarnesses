using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Domain.Biometric.ValueObjects
{
    public sealed record BiometricEnrollmentContext(
    bool IsDeviceCapable,
    bool IsUserEnrolled,
    bool IsEnrollmentAllowed);
}
