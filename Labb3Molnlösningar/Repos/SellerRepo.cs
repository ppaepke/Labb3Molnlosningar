// SellerRepository - Repository-lagret för säljare

// Sköter kommunikationen med Cosmos DB för säljare.
// Använder samma Native Provider-mönster som CustomerRepository
// men mot Sellers-containern.

using Labb3Molnlösningar.Interface;
using Labb3Molnlösningar.Models;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Labb3Molnlösningar.Repositories;

public class SellerRepository : ISellerRepository
{
    private readonly Container _container;

    public SellerRepository(IConfiguration configuration)
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

        _container = client.GetDatabase("CrmDatabase").GetContainer("Sellers");
    }

    public async Task<IEnumerable<Seller>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Seller>("SELECT * FROM c");
        var results = new List<Seller>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Seller> CreateAsync(Seller seller)
    {
        var response = await _container.CreateItemAsync(seller, new PartitionKey(seller.Id));
        return response.Resource;
    }
}