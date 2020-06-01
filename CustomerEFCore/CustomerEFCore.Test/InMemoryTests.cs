using CustomerEFCore.Data;
using CustomerEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CustomerEFCore.Test
{
    public class InMemoryTests
    {

        private DbContextOptionsBuilder builder;

        [SetUp]
        public void Setup()
        {
            builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("InMemoryCustomerDB");

        }

        [Test]
        public void CanInsertCustomerIntoDatabase()
        {
            using (var context = new CustomerContext())
            {
                var customer = new Customer();
                context.Customers.Add(customer);
                Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
            }
        }

        [Test]
        public void CanInsertOrderIntoDatabase()
        {
            using (var context = new CustomerContext())
            {
                var order = new Order();
                context.Orders.Add(order);
                Assert.AreEqual(EntityState.Added, context.Entry(order).State);
            }
        }

    }
}