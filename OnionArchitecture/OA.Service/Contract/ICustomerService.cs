using OA.Domain.Entities;

namespace OA.Service.Contract
{
    public interface ICustomerService
    {
        bool SaveChangesAsync();
        void AddCustomer(Customer customer);
        void DeleteCustomer(int customerId);
    }
}
