using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Entities;
using OA.Infrastructure.ViewModel;
using OA.Persistence.Contract;
using OA.Service.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OA.Controllers
{
    [ApiController]
    [Route("api/Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomerController(IMapper mapper, ICustomerService customerService, ICustomerRepository customerRepository)
        {
            _customerService = customerService;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _customerRepository.GetAllCustomersAsync();
            var mappedResult = _mapper.Map<IEnumerable<Customer>>(result);
            return Ok(mappedResult);
        }


        // GET: api/Customer/name
        [HttpGet]
        [Route("{customerName}", Name = "GetCustomer")]
        public async Task<ActionResult> Get(string customerName, bool includeOrders = false)
        {
            var result = await _customerRepository.GetCustomerAsync(customerName, includeOrders);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CustomerModel>(result));
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult<CustomerModel>> Post([FromBody] CustomerModel model)
        {

            if (await _customerRepository.GetCustomerAsync(model.CustomerName) != null)
            {
                ModelState.AddModelError("CustomerName", "Customer Name in use");
            }

            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Customer>(model);

                _customerService.AddCustomer(customer);

                if (_customerService.SaveChangesAsync())
                {
                    // Get the inserted CustomerModel 
                    var newModel = _mapper.Map<CustomerModel>(customer);

                    return CreatedAtRoute("GetCustomer",
                        new { newModel.CustomerName }, newModel);
                }
            }
            return BadRequest(ModelState);
        }

    }
}
