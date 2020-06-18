using Microsoft.EntityFrameworkCore;
using OnionArchitecture.Data;
using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Persistence.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitecture.Persistence.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;
        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

        //public void AddCustomer(Customer customer)
        //{
        //    _context.Customers.Add(customer);
        //}

        //public void DeleteCustomer(Customer customer)
        //{
        //    _context.Customers.Remove(customer);
        //}

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(bool includeOrders = false)
        {
            IQueryable<Customer> query = _context.Customers
               .Include(c => c.Orders);

            if (includeOrders)
            {
                query = query
                  .Include(c => c.Orders.Select(t => new { t.Id, t.OrderDate }));
            }

            query = query.OrderByDescending(c => c.Id);

            return await query.ToArrayAsync();
        }

        public async Task<Customer> GetCustomerAsync(string customerName, bool includeOrders = false)
        {
            IQueryable<Customer> query = _context.Customers
                 .Include(c => c.Orders);

            if (includeOrders)
            {
                query = query.Include(c => c.Orders.Select(t => new { t.Id, t.OrderDate }));
            }

            query = query.Where(c => c.CustomerName == customerName);

            return await query.FirstOrDefaultAsync();
        }

        public bool SaveChangesAsync()
        {
            return _context.SaveChanges() > 0;
        }
    }
}