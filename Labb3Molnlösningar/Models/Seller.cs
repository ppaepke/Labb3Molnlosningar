// Seller - Modell för säljare

// Representerar strukturen på ett säljardokument i Cosmos DB.
// Används på två ställen:
// 1. Som eget dokument i Sellers-containern
// 2. Som inbäddat objekt i Customer-dokumentet (AssignedSeller)
//
// Detta är ett medvetet val av NoSQL-design där vi duplicerar
// säljarens data i varje kunddokument för att slippa joins
// och få snabbare läsningar.


using System.Text.Json.Serialization;

namespace Labb3Molnlösningar.Models;

public class Seller
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}