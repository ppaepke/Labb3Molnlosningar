using Labb3Molnlösningar.Models;

namespace Labb3Molnlösningar.Interface
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> UpdateAsync(string id, Customer customer);
        Task DeleteAsync(string id);
        Task<IEnumerable<Customer>> SearchByNameAsync(string name);
        Task<IEnumerable<Customer>> SearchBySellerNameAsync(string sellerName);
    }
}