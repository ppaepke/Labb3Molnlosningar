// ISellerRepository - Interface för säljarrepository

// Definierar kontraktet för vad ett säljarrepository måste kunna göra.

// Säljare har färre operationer än kunder då de främst används
// som inbäddade objekt i kunddokumenten.


using Labb3Molnlösningar.Models;

namespace Labb3Molnlösningar.Interface
{
    public interface ISellerRepository
    {
        Task<IEnumerable<Seller>> GetAllAsync();
        Task<Seller> CreateAsync(Seller seller);
    }
}
