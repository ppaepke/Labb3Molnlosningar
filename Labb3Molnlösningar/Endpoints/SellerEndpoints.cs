
// SellerEndpoints - API-lagret för säljare
// Extension method som utökar WebApplication med säljarrelaterade
// endpoints. Säljare hanteras enklare än kunder då de inte
// behöver samma affärslogik och validering.

// Flöde för varje anrop:
// HTTP-anrop → Endpoint → SellerRepository (databasåtkomst) → Cosmos DB


using Labb3Molnlösningar.Interface;
using Labb3Molnlösningar.Models;

namespace Labb3Molnlösningar.Endpoints;

public static class SellerEndpoints
{
    public static void MapSellerEndpoints(this WebApplication app)
    {
        app.MapGet("/sellers", async (ISellerRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));
    
        app.MapPost("/sellers", async (Seller seller, ISellerRepository repo) =>
        {
            var created = await repo.CreateAsync(seller);
            return Results.Created($"/sellers/{created.Id}", created);
        });
    }
}