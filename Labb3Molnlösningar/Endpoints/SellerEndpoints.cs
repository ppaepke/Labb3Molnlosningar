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