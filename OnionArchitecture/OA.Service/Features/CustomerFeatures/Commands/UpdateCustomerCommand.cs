using MediatR;
using OA.Domain.Entities;
using OA.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OA.Service.Features.CustomerFeatures.Commands
{
    public class UpdateCustomerCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public UpdateCustomerCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
            {
                var cust = _context.Customers.Where(a => a.Id == request.Id).FirstOrDefault();

                if (cust == null)
                {
                    return default;
                }
                else
                {
                    var customer = new Customer();
                    customer.CustomerName = request.CustomerName;
                    customer.ContactName = request.ContactName;
                    await _context.SaveChangesAsync();
                    return cust.Id;
                }
            }
        }
    }
}
