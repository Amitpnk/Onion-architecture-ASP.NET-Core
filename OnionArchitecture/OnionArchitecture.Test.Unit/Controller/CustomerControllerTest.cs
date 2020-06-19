using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnionArchitecture.Controllers;
using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Test.Unit.Controller
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
        }

        [Test]
        public async Task GetAllCustomerTestAsyncOkResult()
        {

            //Act  
            var response = await controller.Get();

            //response ?? Assert.IsInstanceOf<BadRequestResult>(response);
                
            Assert.IsInstanceOf<OkObjectResult>(response);

        }

        //[Test]
        //public async Task GetAllCustomerTestAsyncMatchResult()
        //{
        //    //Act  
        //    var response = await controller.Get();
        //    Assert.IsInstanceOf<OkObjectResult>(response);
        //}

    }
}
