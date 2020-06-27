using OA.Domain.Entities;
using OA.Persistence.Contract;
using OA.Service.Contract;

namespace OA.Service.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<Customer> _repo;

        public CustomerService(IGenericRepository<Customer> repo)
        {
            _repo = repo;
        }
        public void AddCustomer(Customer customer)
        {
            _repo.Add(customer);
        }

        public void DeleteCustomer(int customerId)
        {
            _repo.Delete(customerId);
        }

        public bool SaveChangesAsync()
        {
            return _repo.SaveChanges();
        }

    }
}
