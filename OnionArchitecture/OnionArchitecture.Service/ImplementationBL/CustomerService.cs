using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Service.Interface;

namespace OnionArchitecture.Service.ImplementationBL
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

        public void DeleteCustomer(int id)
        {
            _repo.Delete(id);
        }

        public bool SaveChangesAsync()
        {
            return _repo.SaveChanges();
        }
 
    }
}
