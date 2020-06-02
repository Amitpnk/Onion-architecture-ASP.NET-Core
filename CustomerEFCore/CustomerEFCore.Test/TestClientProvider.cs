using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace CustomerEFCore.Test
{
    public class TestClientProvider
    {
        public HttpClient Client { get; private set; }

        public TestClientProvider()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            Client = server.CreateClient();
        }
    }
}
