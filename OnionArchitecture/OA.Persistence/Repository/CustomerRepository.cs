using Microsoft.EntityFrameworkCore;
using OA.Data;
using OA.Domain.Entities;
using OA.Persistence.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA.Persistence.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;
        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

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

    }
}