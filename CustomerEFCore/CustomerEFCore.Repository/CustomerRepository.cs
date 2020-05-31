using CustomerEFCore.Data;
using CustomerEFCore.Domain;
using CustomerEFCore.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerEFCore.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;
        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
        }

        public async Task<Customer[]> GetAllCustomersAsync(bool includeOrders = false)
        {
            IQueryable<Customer> query = _context.Customers
               .Include(c => c.Orders);

            if (includeOrders)
            {
                query = query
                  .Include(c => c.Orders.Select(t => new { t.OrderId, t.OrderDate }));
            }

            query = query.OrderByDescending(c => c.CustomerId);

            return await query.ToArrayAsync();
        }

        public async Task<Customer> GetCustomerAsync(string customerName, bool includeOrders = false)
        {
            IQueryable<Customer> query = _context.Customers
                 .Include(c => c.Orders);

            if (includeOrders)
            {
                query = query.Include(c => c.Orders.Select(t => new { t.OrderId, t.OrderDate }));
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
