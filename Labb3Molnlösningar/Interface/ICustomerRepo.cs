
// ICustomerRepository - Interface för kundrepository

// Definierar kontraktet för vad ett kundrepository måste kunna göra.
// Genom att använda ett interface istället för en konkret klass
// uppnår vi löskoppling - övriga lager vet bara om interfacet,
// inte om den specifika implementationen.

// Fördelar:
// - Enkelt att byta databas (bara skapa ny implementation)
// - Lättare att testa (kan mocka interfacet)
// - Tydligt kontrakt för vad repositoryt erbjuder





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