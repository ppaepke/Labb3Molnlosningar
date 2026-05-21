using Labb3Molnlösningar.Models;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Labb3Molnlösningar.Services;

public class CosmosDbService
{
    private readonly Container _customerContainer;
    private readonly Container _sellerContainer;

    public CosmosDbService(IConfiguration configuration)
    {
        var connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        var databaseName = "CrmDatabase";

        var clientOptions = new CosmosClientOptions
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
        };

        var client = new CosmosClient(connectionString, clientOptions);
        var database = client.GetDatabase(databaseName);

        _customerContainer = database.GetContainer("Customers");
        _sellerContainer = database.GetContainer("Sellers");
    }

    // ── CUSTOMERS ──

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        var query = _customerContainer.GetItemQueryIterator<Customer>("SELECT * FROM c");
        var results = new List<Customer>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Customer?> GetCustomerByIdAsync(string id)
    {
        try
        {
            var response = await _customerContainer.ReadItemAsync<Customer>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        var response = await _customerContainer.CreateItemAsync(customer, new PartitionKey(customer.Id));
        return response.Resource;
    }

    public async Task<Customer?> UpdateCustomerAsync(string id, Customer customer)
    {
        customer.Id = id;
        var response = await _customerContainer.UpsertItemAsync(customer, new PartitionKey(id));
        return response.Resource;
    }

    public async Task DeleteCustomerAsync(string id)
    {
        await _customerContainer.DeleteItemAsync<Customer>(id, new PartitionKey(id));
    }

    // ── SEARCH ──

    public async Task<IEnumerable<Customer>> SearchByCustomerNameAsync(string name)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE CONTAINS(LOWER(c.name), LOWER(@name))")
            .WithParameter("@name", name);

        var iterator = _customerContainer.GetItemQueryIterator<Customer>(query);
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

        var iterator = _customerContainer.GetItemQueryIterator<Customer>(query);
        var results = new List<Customer>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    // ── SELLERS ──

    public async Task<Seller> CreateSellerAsync(Seller seller)
    {
        var response = await _sellerContainer.CreateItemAsync(seller, new PartitionKey(seller.Id));
        return response.Resource;
    }

    public async Task<IEnumerable<Seller>> GetAllSellersAsync()
    {
        var query = _sellerContainer.GetItemQueryIterator<Seller>("SELECT * FROM c");
        var results = new List<Seller>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }
}