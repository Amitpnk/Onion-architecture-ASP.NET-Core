using OA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OA.Persistence.Contract
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeOrders = false);
        Task<Customer> GetCustomerAsync(string customerName, bool includeOrders = false);
    }
}
