using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnionArchitecture.Test.Integration
{
    public class ApiCustomerTest
    {
        [TestCase("Get", "api/Customer")]
        [TestCase("Get", "api/Customer/Amazon")]
        public async Task GetAllCustomerTestAsync(string method, string URL)
        {
            using (var client = new TestClientProvider().Client)
            {
                var request = new HttpRequestMessage(new HttpMethod(method), URL);
                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}