// See https://aka.ms/new-console-template for more information
using Optima.Net;
using Optima.Net.Events.Helpers;
using Optima.Net.Events.Models;
using Optima.Net.Events.Payloads;
using Optima.Net.Events.Services;
using System.Text.Json;



var payload = new DynamicPayload("PaymentRequested");
payload.Add("SourceAccountNumber", "123456");
payload.Add("SourceFirstName", "Alice");
payload.Add("SourceLastName", "Walker");
payload.Add("SourceTitle", "Ms");
payload.Add("SourceReference", "Salary2025");

payload.Add("TargetAccountNumber", "987654");
payload.Add("TargetFirstName", "Bob");
payload.Add("TargetLastName", "Johnson");
payload.Add("TargetTitle", "Mr");
payload.Add("TargetReference", "Invoice-2025-11");
payload.Add("TargetProofOfPaymentRequired", true);
payload.Add("TargetEmail", "bob.johnson@example.com");
payload.Add("TargetCellNumber", "+27123456789");

payload.Add("Amount", 1520.75);
payload.Add("Currency", "ZAR");

var avroEvt = new GenericEvent<DynamicPayload>
{
    EventId = Guid.NewGuid(),
    EventType = payload.PayloadName,
    Source = "payment-service",
    SchemaVersion = "V1.0.0",
    Timestamp = DateTime.UtcNow,
    CorrelationId = Optional<Guid>.Some(Guid.NewGuid()),
    Payload = payload
};

Console.WriteLine("▶ Original Generic Event:");
Console.WriteLine(JsonSerializer.Serialize(avroEvt, JsonHelper.DefaultOptions));
Console.WriteLine();

var schemaJson = AvroSchemaGenerator.GenerateSchemaFromGenericEvent(avroEvt);
Console.WriteLine("Generated Avro Schema:");
Console.WriteLine(schemaJson);
Console.WriteLine();

var schemaHash = AvroSchemaGenerator.ComputeSchemaHash(schemaJson);
Console.WriteLine("Computed the Avro Schema Hash:");
Console.WriteLine(schemaHash);
Console.WriteLine();

Console.WriteLine("You can use the AvroSchemaGenarator.GenerateSchemaFromGenericEvent and the AvroSchemaGenerator.ComputeSchemaHash");
Console.WriteLine("methods to generate Avro schemas and compute their hashes for your events");
Console.WriteLine("The generated schema and hash can then be saved to a registry (or configuration) with the hash being the unique identifier for the schema.\n");
Console.WriteLine("When serializing the serialization method will recalculate the hash based on the schema that was used.");
Console.WriteLine("This is by design, to ensure that the correct schema hash is used for deserialserialization, on the receivers end.");
Console.WriteLine("This is also how the desarializer can establish if the schema was altered or not, by comparing the calculated hash \nwith the hash that was stored in the registry (or configuration) when the schema was registered.\n");
Console.WriteLine("If the hash does not exist in the registry or configuration it can be assumed that the scham was altered.");
Console.WriteLine("SchemaHash usage for schema retrieval is best done with a transport pattern such as the Envelope Pattern.");
Console.WriteLine("For more information on the Envelope Pattern, please refer to the Optima.Net.Events documentation.\n");


var avroData = AvroEventSerializer.Serialize(avroEvt, schemaJson);
Console.WriteLine($"Serialized Avro Data: " + string.Join(", ", avroData) + "\n");
Console.WriteLine($"Serialized Avro Data: {avroData.Length} bytes\n");


var deserialized = AvroEventSerializer.Deserialize(avroData, schemaJson);

Console.WriteLine("Deserialized Generic Event:");
Console.WriteLine(JsonSerializer.Serialize(deserialized, JsonHelper.DefaultOptions));

Console.WriteLine("\nRound-trip successful!");


AvroEventSerializer.PrintDebug(avroEvt, schemaJson);

Console.WriteLine("\nProtobuf");

var protoSchema = ProtobufSchemaGenerator.GenerateSchemaProto(
            payload.Fields,
            payload.PayloadName,
            "events"
        );

Console.WriteLine("\nGenerated Protobuf Schema:\n");
Console.WriteLine(protoSchema);

var protoEvt = new GenericEvent<DynamicPayload>
{
    EventId = Guid.NewGuid(),
    EventType = payload.PayloadName,
    Source = "payment-service",
    SchemaVersion = "V1.0.0",
    Timestamp = DateTime.UtcNow,
    CorrelationId = Optional<Guid>.Some(Guid.NewGuid()),
    Payload = payload
};
Console.WriteLine("\nOriginal Event:");
Console.WriteLine(JsonHelper.Serialize(protoEvt));

// -------------------------
// Serialize using Protobuf
// -------------------------
var data = ProtobufEventSerializer.Serialize(protoEvt, protoSchema);
Console.WriteLine($"\nSerialized Event Size: {data.Length} bytes");

// -------------------------
// Deserialize back
// -------------------------
var result = ProtobufEventSerializer.Deserialize(data, protoSchema);
Console.WriteLine("\nDeserialized Event:");
Console.WriteLine(JsonHelper.Serialize(result));

Console.WriteLine("\n Completed successfully.");