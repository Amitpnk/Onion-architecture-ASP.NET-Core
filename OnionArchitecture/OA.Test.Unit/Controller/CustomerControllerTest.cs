using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OA.Controllers;
using OA.Domain.Entities;
using OA.Infrastructure.ViewModel;
using OA.Persistence.Contract;
using OA.Service.Contract;
using System.Threading.Tasks;

namespace OA.Test.Unit.Controller
{
    public class CustomerControllerTest
    {
        private CustomerController controller;
        private Mock<ICustomerRepository> customerRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ICustomerService> customerServiceMock;

        [SetUp]
        public void Setup()
        {
            customerRepositoryMock = new Mock<ICustomerRepository>();
            mapperMock = new Mock<IMapper>();
            customerServiceMock = new Mock<ICustomerService>();

            controller = new CustomerController(mapperMock.Object, customerServiceMock.Object, customerRepositoryMock.Object);

            customerServiceMock.Setup(x => x.SaveChangesAsync()).Returns(true);

            CustomerModel customerModel = new CustomerModel
            { CustomerName = "Shweta Naik", Address = "Bangalore" };
            Customer customer = new Customer
            { CustomerName = "Shweta Naik", Address = "Bangalore" };

            mapperMock.Setup(x => x.Map<CustomerModel>(customer)).Returns(customerModel);
        }

        [Test]
        public async Task GetAllCustomerTestAsyncOkResult()
        {

            var response = await controller.Get();
            Assert.IsInstanceOf<OkObjectResult>(response);

        }

        [Test]
        public async Task PostCustomerTestAsync()
        {
            CustomerModel customerModel = new CustomerModel
            { CustomerName = "Shweta Naik", Address = "Bangalore" };

            var response = await controller.Post(customerModel);

            var item = response.Value;
            Assert.IsInstanceOf<CustomerModel>(customerModel);
            Assert.AreEqual("Shweta Naik", item.CustomerName);
        }

    }
}
