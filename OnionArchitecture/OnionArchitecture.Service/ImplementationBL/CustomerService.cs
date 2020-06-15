using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Service.Interface;
using System.Threading.Tasks;

namespace OnionArchitecture.Service.ImplementationBL
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepo = customerRepository;
        }
        public void AddCustomer(Customer customer)
        {
            _customerRepo.AddCustomer(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            _customerRepo.DeleteCustomer(customer);
        }

        public async Task<Customer[]> GetAllCustomers(bool includeOrders = false)
        {
            return await _customerRepo.GetAllCustomersAsync();
        }

        public async Task<Customer> GetCustomer(string customerName, bool includeOrders = false)
        {
            return await _customerRepo.GetCustomerAsync(customerName, includeOrders);
        }
    }
}
