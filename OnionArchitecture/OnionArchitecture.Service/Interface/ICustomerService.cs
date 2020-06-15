using OnionArchitecture.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionArchitecture.Service.Interface
{
    public interface ICustomerService
    {
        void AddCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        Task<Customer[]> GetAllCustomers(bool includeOrders = false);
        Task<Customer> GetCustomer(string customerName, bool includeOrders = false);
    }
}
