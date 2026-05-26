// CustomerRepository - Repository-lagret för kunder

// Detta lager sköter all kommunikation med Cosmos DB för kunder.
// Använder Native Provider-mönstret, dvs CosmosClient direkt
// utan Entity Framework.
//
// Native Provider-flöde:
// CosmosClient → Database → Container → CRUD-operationer
//
// Genom att implementera ICustomerRepository kan vi enkelt
// byta databas i framtiden utan att ändra övriga lager.



using Labb3Molnlösningar.Interface;
using Labb3Molnlösningar.Models;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Labb3Molnlösningar.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly Container _container;

    public CustomerRepository(IConfiguration configuration)
    {
        var client = new CosmosClient(
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new CosmosClientOptions
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });

        _container = client.GetDatabase("CrmDatabase").GetContainer("Customers");
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Customer>("SELECT * FROM c");
        var results = new List<Customer>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Customer>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        var response = await _container.CreateItemAsync(customer, new PartitionKey(customer.Id));
        return response.Resource;
    }

    public async Task<Customer?> UpdateAsync(string id, Customer customer)
    {
        customer.Id = id;
        var response = await _container.UpsertItemAsync(customer, new PartitionKey(id));
        return response.Resource;
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
    }

    public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE CONTAINS(LOWER(c.name), LOWER(@name))")
            .WithParameter("@name", name);

        var iterator = _container.GetItemQueryIterator<Customer>(query);
        var results = new List<Customer>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<IEnumerable<Customer>> SearchBySellerNameAsync(string sellerName)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE CONTAINS(LOWER(c.assignedSeller.name), LOWER(@name))")
            .WithParameter("@name", sellerName);

        var iterator = _container.GetItemQueryIterator<Customer>(query);
        var results = new List<Customer>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }
}