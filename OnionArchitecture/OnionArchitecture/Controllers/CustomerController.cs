using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Model;
using OnionArchitecture.Persistence.Contract;
using System;
using System.Threading.Tasks;

namespace OnionArchitecture.Controllers
{
    [ApiController]
    [Route("api/Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IMapper _mapper;
        public CustomerController(IMapper mapper, ICustomerRepository customerRepository)
        {
            _customerRepo = customerRepository;
            _mapper = mapper;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<CustomerModel[]>> Get()
        {
            try
            {
                var result = await _customerRepo.GetAllCustomersAsync();

                var mappedResult = _mapper.Map<CustomerModel[]>(result);
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("{customerName}", Name = "GetCustomer")]
        [HttpGet]
        public async Task<ActionResult> Get(string customerName, bool includeOrders = false)
        {
            try
            {
                var result = await _customerRepo.GetCustomerAsync(customerName, includeOrders);

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<CustomerModel>(result));
            }
            catch (Exception ex)
            {
                // TODO Add logging
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [Route("")]
        [HttpPost]
        public async Task<ActionResult<CustomerModel>> Post([FromBody] CustomerModel model)
        {
            try
            {
                if (await _customerRepo.GetCustomerAsync(model.CustomerName) != null)
                {
                    ModelState.AddModelError("CustomerName", "Customer Name in use");
                }

                if (ModelState.IsValid)
                {
                    var customer = _mapper.Map<Customer>(model);

                    _customerRepo.AddCustomer(customer);

                    if (_customerRepo.SaveChangesAsync())
                    {
                        // Get the inserted CustomerModel 
                        var newModel = _mapper.Map<CustomerModel>(customer);

                        return CreatedAtRoute("GetCustomer",
                            new { newModel.CustomerName }, newModel);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO Add logging
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
