using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnionArchitecture.Data;
using OnionArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Test.Unit.Data
{
    public class CustomerContextTest
    {

        [Test]
        public void CanInsertCustomerIntoDatabasee()
        {
          
            using (var context = new CustomerContext())
            {
                var customer = new Customer();
                context.Customers.Add(customer);
                Assert.AreEqual(EntityState.Added, context.Entry(customer).State);
            }
        }
    }
}
