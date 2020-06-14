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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace CustomerEFCore.Test
{
    public class CustomerApiIntegrationTest
    {
        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        [TestCase("Get")]
        public async Task GetAllCustomerTestAsync(string method)
        {
            using (var client = new TestClientProvider().Client)
            {
                var request = new HttpRequestMessage(new HttpMethod(method), "api/Customer");
                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
