using Labb3Molnlösningar.Models;

namespace Labb3Molnlösningar.Interface
{
    public interface ISellerRepository
    {
        Task<IEnumerable<Seller>> GetAllAsync();
        Task<Seller> CreateAsync(Seller seller);
    }
}
