using OnionArchitecture.Domain.Entities;

namespace OnionArchitecture.Service.Contract
{
    public interface ICustomerService
    {
        bool SaveChangesAsync();
        void AddCustomer(Customer customer);
        void DeleteCustomer(int customerId);
    }
}
