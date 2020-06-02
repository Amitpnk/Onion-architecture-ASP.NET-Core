using AutoMapper;
using CustomerEFCore.Controllers;
using CustomerEFCore.Repository.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomerEFCore.Test
{
    public class ApiUnitTest
    {
        private CustomerController controller;
        private Mock<ICustomerRepository> customerRepositoryMock;
        private Mock<IMapper> mapperMock;

        [SetUp]
        public void Setup()
        {
            customerRepositoryMock = new Mock<ICustomerRepository>();
            mapperMock = new Mock<IMapper>();

            controller = new CustomerController(mapperMock.Object, customerRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllCustomerTestAsync()
        {

            //Act  
            var response = await controller.Get();

            dynamic okObjectResult = response.Result;
            //Assert  
            Assert.AreEqual((int)HttpStatusCode.OK, okObjectResult.StatusCode);

        }
    }
}
