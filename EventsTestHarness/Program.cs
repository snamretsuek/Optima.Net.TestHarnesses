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