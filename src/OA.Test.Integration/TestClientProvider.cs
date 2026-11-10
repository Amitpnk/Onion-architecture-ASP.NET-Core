using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OA.Persistence;
using OA.Service.Contract;

namespace OA.Test.Integration;

public class TestClientProvider : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; }
    public FakeEmailService EmailService { get; } = new();

    public TestClientProvider()
    {
        // Initialize WebApplicationFactory with the Program class
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // Set the environment to Test (in-memory databases, console logging)
                builder.UseEnvironment("Test");

                builder.ConfigureTestServices(services =>
                {
                    // Never talk to a real SMTP server from tests
                    services.RemoveAll<IEmailService>();
                    services.AddSingleton<IEmailService>(EmailService);
                });
            });

        // Seed roles and default users (superadmin / basicuser) into the in-memory identity store
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.EnsureCreated();
        }

        Client = CreateClient();
    }

    /// <summary>Creates a fresh client without an Authorization header.</summary>
    public HttpClient CreateClient() => _factory.CreateClient();

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}
