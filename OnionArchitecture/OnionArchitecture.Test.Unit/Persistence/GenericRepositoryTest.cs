using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnionArchitecture.Data;
using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Persistence.Repository;
using System.Linq;

namespace OnionArchitecture.Test.Unit.Persistence
{
    public class GenericRepositoryTest
    {
        private Mock<IGenericRepository<Customer>> customerRepositoryMock;
        private DbContextOptionsBuilder builder;
        private Mock<Customer> customerMock;

        Customer customer;

        [SetUp]
        public void Setup()
        {
            customerRepositoryMock = new Mock<IGenericRepository<Customer>>();

            builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("InMemoryCustomerDB");
            customerMock = new Mock<Customer>();

            customer = new Customer
            {
                CustomerName = "Shweta Naik",
                Address = "Bangalore"
            };
        }

        [Test]
        public void CheckGenenricRepositoryAddCustomer()
        {
            using (var context = new CustomerContext(builder.Options))
            {
                var customerRepository = new GenericRepository<Customer>(context);
                customerRepository.Add(customer);
                var result = customerRepository.SaveChanges();
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void CheckGenenricRepositoryUpdateCustomer()
        {
            using (var context = new CustomerContext(builder.Options))
            {
                var customerRepository = new GenericRepository<Customer>(context);
                customerRepository.Update(customer);
                var result = customerRepository.SaveChanges();
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void CheckGenenricRepositoryDeleteCustomer()
        {
            using (var context = new CustomerContext(builder.Options))
            {
                var customerRepository = new GenericRepository<Customer>(context);

                customerRepository.Add(customer);
                customerRepository.SaveChanges();

                customerRepository.Delete(customer.Id);
                var result = customerRepository.SaveChanges();
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void CheckGenenricRepositoryGetCustomer()
        {
            using (var context = new CustomerContext(builder.Options))
            {

                var customerRepository = new GenericRepository<Customer>(context);
                customerRepository.Add(new Customer { CustomerName = "Shweta Naik", Address = "Bangalore" });
                customerRepository.Add(new Customer { CustomerName = "Amit Naik", Address = "Bangalore" });
                customerRepository.SaveChanges();

                var cust = customerRepository.GetAll();

                Assert.LessOrEqual(2, cust.Count());
            }
        }

        [Test]
        public void CheckGenenricRepositoryGetByIdCustomer()
        {
            using (var context = new CustomerContext(builder.Options))
            {
                var customerRepository = new GenericRepository<Customer>(context);
                customerRepository.Add(new Customer { CustomerName = "Shweta Naik", Address = "Bangalore" });
                customerRepository.Add(new Customer { CustomerName = "Amit Naik", Address = "Bangalore" });
                customerRepository.SaveChanges();

                var cust = customerRepository.GetById(1);

                Assert.AreEqual(1, cust.Id);
            }
        }

    }
}
