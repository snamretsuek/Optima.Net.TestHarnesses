// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Optima.Net.DomainModel.Invariants;
using TestHarness;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Optima.Net.DomainModel Test Harness ===");

try
{
    // Create a new valid order.
    var order = Order.Create(Guid.NewGuid(), "ORD-1001", 249.99m);

    // Display emitted domain facts.
    foreach (var fact in order.DomainFacts)
    {
        Console.WriteLine($"Emitted Fact: {fact.EventType} @ {fact.Timestamp:u}");
    }

    //simulate a pause as user updates the order
    Console.WriteLine("\nSimulating order update...");
    Thread.Sleep(2000); // Sleep for 2 seconds

    // Update the order.
    order.UpdateTotal(349.99m);

    // Display emitted domain facts again.
    Console.WriteLine("\nAfter update:");
    foreach (var fact in order.DomainFacts)
    {
        Console.WriteLine($"Emitted Fact: {fact.EventType} @ {fact.Timestamp:u}");
    }

    // Trigger an invariant violation to show fail-fast behavior.
    Console.WriteLine("\nAttempting to create invalid order...");
    var invalid = Order.Create(Guid.NewGuid(), "", -100m);
}
catch (InvariantViolationException ex)
{
    Console.WriteLine($"Invariant Violation: {ex.Message}");
}

Console.WriteLine("\n=== Test Harness Completed ===");
