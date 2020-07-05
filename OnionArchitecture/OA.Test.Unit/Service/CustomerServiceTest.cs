using Moq;
using NUnit.Framework;
using OA.Domain.Entities;
using OA.Persistence.Contract;

namespace OA.Test.Unit.Service
{
    public class CustomerServiceTest
    {
        private CustomerService customerService;
        private Mock<IGenericRepository<Customer>> genericRepositoryMock;
        Customer customer;

        [SetUp]
        public void Setup()
        {
            genericRepositoryMock = new Mock<IGenericRepository<Customer>>();
            customerService = new CustomerService(genericRepositoryMock.Object);

            genericRepositoryMock.Setup(x => x.SaveChanges()).Returns(true);

            customer = new Customer
            {
                CustomerName = "Shweta Naik",
                Address = "Bangalore"
            };
        }

        [Test]
        public void CheckCustomerServiceAddCustomer()
        {
            customerService.AddCustomer(customer);
            var result = customerService.SaveChangesAsync();
            Assert.IsTrue(result);

        }

        [Test]
        public void CheckCustomerServiceDeleteCustomer()
        {
            customerService.DeleteCustomer(customer.Id);
            var result = customerService.SaveChangesAsync();
            Assert.IsTrue(result);

        }
    }
}
