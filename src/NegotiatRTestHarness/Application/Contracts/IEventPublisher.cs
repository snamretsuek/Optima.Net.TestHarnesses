using Optima.Net.Events.Models;
using Optima.Net.Events.Payloads;

namespace NegotiatRTestHarness.Application.Contracts
{
    /// <summary>
    /// Application-layer abstraction for publishing integration events.
    ///
    /// Implementations of this interface are responsible for delivering
    /// events to external systems or messaging infrastructure. The application
    /// layer depends only on this abstraction and does not know how events are
    /// transported, serialized, or delivered.
    ///
    /// This interface is intentionally not part of the domain. It exists to
    /// support orchestration and integration concerns after domain decisions
    /// have been made.
    /// </summary>
    public interface IEventPublisher
    {
        void Publish(GenericEvent<DynamicPayload> evt);
    }
}

