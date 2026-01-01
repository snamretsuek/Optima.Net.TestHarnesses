using NegotiatRTestHarness.Application.Contracts;
using Optima.Net.Events.Models;
using Optima.Net.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiatRTestHarness.Infrastructure
{
    /// <summary>
    /// Test-harness implementation of <see cref="IEventPublisher"/> used for
    /// local execution and demonstration purposes.
    ///
    /// This publisher does not deliver events to any external infrastructure.
    /// Instead, it writes event metadata and payload contents to the console
    /// to make integration behavior observable during manual runs of the
    /// test harness.
    ///
    /// This type is not intended for production use and must not be used as
    /// a real infrastructure implementation.
    /// </summary>
    public sealed class MockEventPublisher : IEventPublisher
    {
        public void Publish(GenericEvent<DynamicPayload> evt)
        {
            Console.WriteLine("=== EVENT PUBLISHED ===");
            Console.WriteLine($"EventType     : {evt.EventType}");
            Console.WriteLine($"Source        : {evt.Source}");
            Console.WriteLine($"SchemaVersion : {evt.SchemaVersion}");
            Console.WriteLine($"Timestamp     : {evt.Timestamp:o}");

            foreach (var item in evt.Payload.Fields)
            {
                Console.WriteLine($"  {item.Key}: {item.Value}");
            }

            Console.WriteLine("=======================");
            Console.WriteLine();
        }
    }

}
