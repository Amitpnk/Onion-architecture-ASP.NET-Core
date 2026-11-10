using NUnit.Framework;
using System.Net;
using System.Text.Json;

namespace OA.Test.Integration.ApiEndpoints;

public class ApiInfrastructureTest : TestClientProvider
{
    [Test]
    public async Task Info_ReturnsVersion()
    {
        var response = await Client.GetAsync("/info");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadAsStringAsync(), Does.StartWith("Version: "));
    }

    [Test]
    public async Task SwaggerDocument_IsGenerated_WithBearerSecurity()
    {
        var response = await Client.GetAsync("/swagger/OpenAPISpecification/swagger.json");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        var bearer = root.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer");
        Assert.Multiple(() =>
        {
            Assert.That(root.GetProperty("openapi").GetString(), Does.StartWith("3."));
            Assert.That(root.GetProperty("info").GetProperty("title").GetString(), Is.EqualTo("Onion Architecture WebAPI"));
            Assert.That(bearer.GetProperty("type").GetString(), Is.EqualTo("http"));
            Assert.That(bearer.GetProperty("scheme").GetString(), Is.EqualTo("bearer"));
            Assert.That(root.GetProperty("security")[0].TryGetProperty("Bearer", out _), Is.True,
                "Bearer requirement should apply globally");
        });

        var paths = root.GetProperty("paths").EnumerateObject().Select(p => p.Name).ToList();
        Assert.That(paths, Does.Contain("/api/v{version}/Customer").And.Contain("/api/Account/authenticate"));
    }

    [Test]
    public async Task SwaggerUi_IsServed()
    {
        var response = await Client.GetAsync("/OpenAPI/index.html");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("text/html"));
    }
}
