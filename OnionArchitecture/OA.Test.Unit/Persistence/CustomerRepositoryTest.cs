using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OA.Data;
using OA.Domain.Entities;
using OA.Persistence.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace OA.Test.Unit.Persistence
{
    public class CustomerRepositoryTest
    {
        private DbContextOptionsBuilder builder;

        private ApplicationDbContext context;

        [SetUp]
        public void Setup()
        {
            builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("InMemoryCustomerDB");

            SettingUp();

        }

        [Test]
        public async Task CheckCustomerRepositoryGetAllCustomersAsyn()
        {
            var customerRepository = new CustomerRepository(context);
            var result = await customerRepository.GetAllCustomersAsync();
            Assert.LessOrEqual(2, result.Count());
        }

        [Test]
        public async Task CheckCustomerRepositoryGetCustomerAsync()
        {
            string name = "Shweta Naik";
            var customerRepository = new CustomerRepository(context);
            var result = await customerRepository.GetCustomerAsync(name);
            Assert.AreEqual(name, result.CustomerName);
        }

        void SettingUp()
        {
            // Inserting to inmemory database
            context = new ApplicationDbContext(builder.Options);
            var customerRepository = new GenericRepository<Customer>(context);
            customerRepository.Add(new Customer { CustomerName = "Shweta Naik", Address = "Bangalore" });
            customerRepository.Add(new Customer { CustomerName = "Amit Naik", Address = "Bangalore" });
            customerRepository.SaveChanges();
        }
    }
}
