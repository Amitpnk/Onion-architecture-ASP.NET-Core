using OnionArchitecture.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionArchitecture.Persistence.Contract
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeOrders = false);
        Task<Customer> GetCustomerAsync(string customerName, bool includeOrders = false);
    }
}
