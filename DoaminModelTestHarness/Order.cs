using Optima.Net.DomainModel.Entities;
using Optima.Net.DomainModel.Invariants;
using Optima.Net.Events.Models;
using Optima.Net.Events.Payloads;

namespace TestHarness
{
    /// <summary>
    /// Represents an order aggregate within the domain model.
    /// 
    /// This aggregate enforces core structural invariants and emits domain facts
    /// upon creation and update. The aggregate itself does not dispatch or handle
    /// events — it only asserts their occurrence to describe changes in domain state.
    /// 
    /// In keeping with the design principles of Optima.Net.DomainModel:
    /// - It protects the domain from illegal states.
    /// - It exposes explicit factory methods instead of public constructors.
    /// - It emits domain facts (events) as immutable assertions of truth.
    /// </summary>
    public sealed class Order : AggregateRoot<Guid>
    {
        /// <summary>
        /// Gets the unique, domain-level order number associated with this order.
        /// </summary>
        public string OrderNumber { get; }

        /// <summary>
        /// Gets the total monetary value of the order.
        /// </summary>
        public decimal TotalAmount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// 
        /// The constructor is private to enforce controlled creation through
        /// the <see cref="Create"/> factory method. All invariants are validated
        /// at construction time to ensure that an invalid order cannot exist.
        /// </summary>
        /// <param name="id">The unique identifier of the order.</param>
        /// <param name="orderNumber">The domain-specific order number.</param>
        /// <param name="totalAmount">The total value of the order.</param>
        private Order(Guid id, string orderNumber, decimal totalAmount)
            : base(id)
        {
            // Enforce core structural invariants before state assignment.
            Invariant.MustBeTrue(!string.IsNullOrWhiteSpace(orderNumber), "Order number must not be empty.");
            Invariant.MustBeTrue(totalAmount > 0, "Order total must be greater than zero.");

            OrderNumber = orderNumber;
            TotalAmount = totalAmount;

            // Emit the domain fact that an order has been created.
            EmitOrderCreatedEvent();
        }

        /// <summary>
        /// Creates a new order instance while enforcing domain invariants.
        /// 
        /// This factory method provides the only entry point for creating valid
        /// orders, ensuring that all construction rules are applied consistently.
        /// </summary>
        /// <param name="id">The unique identifier of the order.</param>
        /// <param name="orderNumber">The domain-specific order number.</param>
        /// <param name="totalAmount">The total value of the order.</param>
        /// <returns>A fully initialized and valid <see cref="Order"/> instance.</returns>
        public static Order Create(Guid id, string orderNumber, decimal totalAmount)
        {
            return new Order(id, orderNumber, totalAmount);
        }

        /// <summary>
        /// Updates the total amount of the order while enforcing invariants.
        /// 
        /// The update operation must not violate any domain rules. If the new
        /// total is invalid, an <see cref="InvariantViolationException"/> is thrown.
        /// 
        /// Upon success, a domain fact is emitted describing the update.
        /// </summary>
        /// <param name="newAmount">The new total order amount.</param>
        public void UpdateTotal(decimal newAmount)
        {
            // Enforce invariant: Order total must always be positive.
            Invariant.MustBeTrue(newAmount > 0, "Order total must be greater than zero.");

            TotalAmount = newAmount;

            // Emit the domain fact representing the update.
            EmitOrderUpdatedEvent();
        }

        /// <summary>
        /// Emits a domain fact indicating that a new order has been created.
        /// 
        /// This event records the order’s identity and creation details.
        /// The payload content is opaque to the DomainModel — it only asserts
        /// what has occurred, without reacting to it.
        /// </summary>
        private void EmitOrderCreatedEvent()
        {
            var payload = new DynamicPayload(payloadName: "OrderCreated");
            payload.Add("OrderId", Id);
            payload.Add("OrderNumber", OrderNumber);
            payload.Add("TotalAmount", TotalAmount);
            payload.Add("CreatedAtUtc", DateTime.UtcNow);

            var evt = new GenericEvent<DynamicPayload>
            {
                EventId = Guid.NewGuid(),
                EventType = payload.PayloadName,
                Source = nameof(Order),
                SchemaVersion = "V1.0.0",
                Timestamp = DateTime.UtcNow,
                Payload = payload
            };

            EmitDomainFact(evt);
        }

        /// <summary>
        /// Emits a domain fact indicating that an existing order has been updated.
        /// 
        /// This event records the new state of the order following a successful
        /// update. Like all domain facts, it is immutable and declarative.
        /// </summary>
        private void EmitOrderUpdatedEvent()
        {
            var payload = new DynamicPayload(payloadName: "OrderUpdated");
            payload.Add("OrderId", Id);
            payload.Add("OrderNumber", OrderNumber);
            payload.Add("TotalAmount", TotalAmount);
            payload.Add("UpdatedAtUtc", DateTime.UtcNow);

            var evt = new GenericEvent<DynamicPayload>
            {
                EventId = Guid.NewGuid(),
                EventType = payload.PayloadName,
                Source = nameof(Order),
                SchemaVersion = "V1.0.0",
                Timestamp = DateTime.UtcNow,
                Payload = payload
            };

            EmitDomainFact(evt);
        }
    }
}
