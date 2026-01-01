using Optima.Net.Events.Models;
using Optima.Net.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Application.IntegrationEvents
{
    public static class CredentialCreationMethodRejectedEvent
    {
        public static GenericEvent<DynamicPayload> Create(
            IReadOnlyCollection<string> failureReasons,
            string source,
            DateTime occurredAt)
        {
            var payload = new DynamicPayload("PasswordAuthenticationRejected");
            payload.Add("CreationMethod", "Password");
            payload.Add("FailureReasons", failureReasons);
            payload.Add("Source", source);
            payload.Add("OccurredAt", occurredAt);

            return new GenericEvent<DynamicPayload>
            {
                EventId = Guid.NewGuid(),
                EventType = payload.PayloadName,
                Source = source,
                SchemaVersion = "V1.0.0",
                Timestamp = occurredAt,
                Payload = payload
            };
        }
    }
}
