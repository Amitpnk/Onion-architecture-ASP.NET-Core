using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Model;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionArchitecture.Controllers
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
            try
            {
                var result = await _customerRepository.GetCustomerAsync(customerName, includeOrders);

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
