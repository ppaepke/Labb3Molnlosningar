using Labb3Molnlösningar.Models;
using Labb3Molnlösningar.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CosmosDbService>();

var app = builder.Build();

app.UseHttpsRedirection();

// ── CUSTOMERS ──────────────────────────────────────────────

app.MapGet("/customers", async (CosmosDbService db) =>
    Results.Ok(await db.GetAllCustomersAsync()));

app.MapGet("/customers/{id}", async (string id, CosmosDbService db) =>
{
    var customer = await db.GetCustomerByIdAsync(id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

app.MapPost("/customers", async (Customer customer, CosmosDbService db) =>
{
    if (string.IsNullOrWhiteSpace(customer.AssignedSeller?.Name) ||
        string.IsNullOrWhiteSpace(customer.AssignedSeller?.Email))
    {
        return Results.BadRequest("En kund måste ha en ansvarig säljare med namn och email.");
    }
    var created = await db.CreateCustomerAsync(customer);
    return Results.Created($"/customers/{created.Id}", created);
});

app.MapPut("/customers/{id}", async (string id, Customer customer, CosmosDbService db) =>
{
    var updated = await db.UpdateCustomerAsync(id, customer);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

app.MapDelete("/customers/{id}", async (string id, CosmosDbService db) =>
{
    await db.DeleteCustomerAsync(id);
    return Results.NoContent();
});

// ── SEARCH ─────────────────────────────────────────────────

app.MapGet("/customers/search/name/{name}", async (string name, CosmosDbService db) =>
    Results.Ok(await db.SearchByCustomerNameAsync(name)));

app.MapGet("/customers/search/seller/{sellerName}", async (string sellerName, CosmosDbService db) =>
    Results.Ok(await db.SearchBySellerNameAsync(sellerName)));

// ── SELLERS ────────────────────────────────────────────────

app.MapGet("/sellers", async (CosmosDbService db) =>
    Results.Ok(await db.GetAllSellersAsync()));

app.MapPost("/sellers", async (Seller seller, CosmosDbService db) =>
{
    var created = await db.CreateSellerAsync(seller);
    return Results.Created($"/sellers/{created.Id}", created);
});

app.Run();