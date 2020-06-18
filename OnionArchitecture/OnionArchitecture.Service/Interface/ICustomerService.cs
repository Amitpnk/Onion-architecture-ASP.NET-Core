using OnionArchitecture.Domain.Entities;

namespace OnionArchitecture.Service.Interface
{
    public interface ICustomerService
    {
        bool SaveChangesAsync();
        void AddCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        //Task< IEnumerable< Customer>> GetAllCustomers(bool includeOrders = false);
        //Task<Customer> GetCustomer(string customerName, bool includeOrders = false);
    }
}
