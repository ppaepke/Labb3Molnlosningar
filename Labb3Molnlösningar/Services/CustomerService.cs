using Labb3Molnlösningar.Interface;
using Labb3Molnlösningar.Models;

namespace Labb3Molnlösningar.Services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        => await _customerRepository.GetAllAsync();

    public async Task<Customer?> GetCustomerByIdAsync(string id)
        => await _customerRepository.GetByIdAsync(id);

    public async Task<(Customer? customer, string? error)> CreateCustomerAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.AssignedSeller?.Name) ||
            string.IsNullOrWhiteSpace(customer.AssignedSeller?.Email))
        {
            return (null, "En kund måste ha en ansvarig säljare med namn och email.");
        }
        var created = await _customerRepository.CreateAsync(customer);
        return (created, null);
    }

    public async Task<Customer?> UpdateCustomerAsync(string id, Customer customer)
        => await _customerRepository.UpdateAsync(id, customer);

    public async Task DeleteCustomerAsync(string id)
        => await _customerRepository.DeleteAsync(id);

    public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        => await _customerRepository.SearchByNameAsync(name);

    public async Task<IEnumerable<Customer>> SearchBySellerNameAsync(string sellerName)
        => await _customerRepository.SearchBySellerNameAsync(sellerName);
}