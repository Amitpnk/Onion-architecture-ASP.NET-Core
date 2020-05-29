using AutoMapper;
using CustomerEFCore.Domain;
using CustomerEFCore.Model;
using CustomerEFCore.Repository.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CustomerEFCore.Controllers
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
        public async Task<ActionResult> Get(string customerName, bool includeOrders = false)
        {
            try
            {
                // Decoupling dal
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
        public async Task<ActionResult> Post(CustomerModel model)
        {
            try
            {
                if (await _customerRepo.GetCustomerAsync(model.CustomerName) != null)
                {
                    ModelState.AddModelError("CustomerName", "Customer Name in use");
                }

                if (ModelState.IsValid)
                {
                    // Mapping CampModel to Camp
                    var customer = _mapper.Map<Customer>(model);

                    // Insert to DB
                    _customerRepo.AddCustomer(customer);

                    // Commit to DB
                    if (await _customerRepo.SaveChangesAsync())
                    {
                        // Get the inserted CampModel
                        var newModel = _mapper.Map<CustomerModel>(customer);

                        // Pass to Route with new value
                        return CreatedAtRoute("GetCustomer",
                            new { CustomerName = newModel.CustomerName }, newModel);
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
