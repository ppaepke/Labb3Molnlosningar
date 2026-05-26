using Labb3Molnlösningar.Models;
using Labb3Molnlösningar.Services;

namespace Labb3Molnlösningar.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        app.MapGet("/customers", async (CustomerService service) =>
            Results.Ok(await service.GetAllCustomersAsync()));

        app.MapGet("/customers/{id}", async (string id, CustomerService service) =>
        {
            var customer = await service.GetCustomerByIdAsync(id);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });

        app.MapPost("/customers", async (Customer customer, CustomerService service) =>
        {
            var (created, error) = await service.CreateCustomerAsync(customer);
            if (error is not null) return Results.BadRequest(error);
            return Results.Created($"/customers/{created!.Id}", created);
        });

        app.MapPut("/customers/{id}", async (string id, Customer customer, CustomerService service) =>
        {
            var updated = await service.UpdateCustomerAsync(id, customer);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        app.MapDelete("/customers/{id}", async (string id, CustomerService service) =>
        {
            await service.DeleteCustomerAsync(id);
            return Results.NoContent();
        });

        app.MapGet("/customers/search/name/{name}", async (string name, CustomerService service) =>
            Results.Ok(await service.SearchByNameAsync(name)));

        app.MapGet("/customers/search/seller/{sellerName}", async (string sellerName, CustomerService service) =>
            Results.Ok(await service.SearchBySellerNameAsync(sellerName)));
    }
}