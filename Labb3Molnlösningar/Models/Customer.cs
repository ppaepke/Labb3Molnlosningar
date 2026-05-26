// Customer - Modell för kund

// Representerar strukturen på ett kunddokument i Cosmos DB.

// Varje property motsvarar ett fält i JSON-dokumentet.
//
// Cosmos DB lagrar data som JSON-dokument, och CamelCase-
// serialiseringen i CosmosClientOptions gör att våra C#-properties
// mappas korrekt mot JSON-fälten.
//
// Säljaren lagras inbäddad i kunddokumentet (embedded document)
// vilket är ett vanligt NoSQL-mönster för att undvika joins.





using System.Text.Json.Serialization;

namespace Labb3Molnlösningar.Models;

public class Customer
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Seller AssignedSeller { get; set; } = new();
}