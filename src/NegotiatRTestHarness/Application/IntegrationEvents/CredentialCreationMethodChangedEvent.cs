using Optima.Net.Events.Models;
using Optima.Net.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Application.IntegrationEvents
{
    /// <summary>
    /// Integration event emitted by the application layer to signal that the
    /// credential creation method has changed as a result of a completed
    /// domain decision.
    ///
    /// This event is not a domain event. The domain is responsible only for
    /// producing the decision outcome. Emitting and publishing this event is
    /// an application concern, used to inform external systems, workflows,
    /// or user interfaces of the change.
    ///
    /// The event represents communication about a decision, not the decision
    /// itself.
    /// </summary>
    public static class CredentialCreationMethodChangedEvent
    {
        public static GenericEvent<DynamicPayload> Create(
            string from,
            string to,
            string source,
            DateTime occurredAt)
        {
            var payload = new DynamicPayload("CredentialCreationMethodChangedEvent");
            payload.Add("From", from);
            payload.Add("To", to);
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
