using CustomerEFCore.Data;
using CustomerEFCore.Domain;
using CustomerEFCore.Repository;
using CustomerEFCore.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CustomerEFCore.Test
{
    public class RepositoryTest
    {
        private Mock<ICustomerRepository> customerRepositoryMock;
        private DbContextOptionsBuilder builder;
        private Mock<Customer> customerMock;

        [SetUp]
        public void Setup()
        {
            customerRepositoryMock = new Mock<ICustomerRepository>();

            builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("InMemoryCustomerDB");
            customerMock = new Mock<Customer>();
        }

        [Test]
        public void CheckCustomerRepositoryAddCustomer()
        {

            using (var context = new CustomerContext(builder.Options))
            {
                Customer customer = new Customer
                {
                    CustomerName = "Shweta Naik",
                    Address = "Bangalore"
                };
                var customerRepository = new CustomerRepository(context);
                customerRepository.AddCustomer(customer);
                var result = context.SaveChanges();
                Assert.AreEqual(1, result);
            }
        }
    }
}
