using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionArchitecture.Service.ImplementationBL
{
    public class CustomerService : ICustomerService //, ICustomerRepository
    {
        private readonly IGenericRepository<Customer> _repo;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(IGenericRepository<Customer> repo, ICustomerRepository customerRepository)
        {
            _repo = repo;
            _customerRepository = customerRepository;
        }
        public void AddCustomer(Customer customer)
        {
            _repo.Add(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            _repo.Delete(customer);
        }

        //public Task<Customer> GetCustomerAsync(string customerName, bool includeOrders = false)
        //{
        //    throw new System.NotImplementedException();
        //}

        public bool SaveChangesAsync()
        {
            return _repo.SaveChanges();
        }

        //public Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeOrders = false)
        //{
        //    return _customerRepository.GetAllCustomersAsync();
        //}
    }
}
