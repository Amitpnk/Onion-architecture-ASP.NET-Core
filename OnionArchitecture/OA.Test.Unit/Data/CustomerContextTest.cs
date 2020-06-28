using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OA.Data;
using OA.Domain.Entities;

namespace OA.Test.Unit.Data
{
    public class CustomerContextTest
    {

        [Test]
        public void CanInsertCustomerIntoDatabasee()
        {

            using var context = new CustomerContext();
            var customer = new Customer();
            context.Customers.Add(customer);
            Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
        }
    }
}
