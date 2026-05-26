using Labb3Molnlösningar.Interface;
using Labb3Molnlösningar.Models;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Labb3Molnlösningar.Repositories;

public class SellerRepository : ISellerRepository
{
    private readonly Container _container;

    public SellerRepository(CosmosClient client)
    {
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