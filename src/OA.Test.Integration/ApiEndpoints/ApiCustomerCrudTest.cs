using NUnit.Framework;
using OA.Service.Features.CustomerFeatures.Commands;
using System.Net;

namespace OA.Test.Integration.ApiEndpoints;

public class ApiCustomerCrudTest : TestClientProvider
{
    private const string BaseUrl = "api/v1/Customer";

    [TestCase("GET", BaseUrl)]
    [TestCase("GET", BaseUrl + "/1")]
    [TestCase("POST", BaseUrl)]
    [TestCase("PUT", BaseUrl + "/1")]
    [TestCase("DELETE", BaseUrl + "/1")]
    public async Task CustomerEndpoints_WithoutToken_ReturnUnauthorized(string method, string url)
    {
        using var client = CreateClient();
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (method is "POST" or "PUT")
            request.Content = HttpClientExtensions.SerializeAndCreateContent(NewCustomer());

        var response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        var body = await response.ReadAsAsync<ApiResponse<string>>();
        Assert.That(body.Succeeded, Is.False);
        Assert.That(body.Message, Is.EqualTo("You are not Authorized"));
    }

    [Test]
    public async Task CreateThenGetById_ReturnsAllSubmittedFields()
    {
        var command = NewCustomer("Created Customer");
        var id = await CreateAsync(command);

        var response = await AuthorizedClient.GetAsync($"{BaseUrl}/{id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var customer = await response.ReadAsAsync<CustomerDto>();
        Assert.Multiple(() =>
        {
            Assert.That(customer.Id, Is.EqualTo(id));
            Assert.That(customer.CustomerName, Is.EqualTo(command.CustomerName));
            Assert.That(customer.ContactName, Is.EqualTo(command.ContactName));
            Assert.That(customer.ContactTitle, Is.EqualTo(command.ContactTitle));
            Assert.That(customer.Address, Is.EqualTo(command.Address));
            Assert.That(customer.City, Is.EqualTo(command.City));
            Assert.That(customer.Region, Is.EqualTo(command.Region));
            Assert.That(customer.PostalCode, Is.EqualTo(command.PostalCode));
            Assert.That(customer.Country, Is.EqualTo(command.Country));
            Assert.That(customer.Phone, Is.EqualTo(command.Phone));
            Assert.That(customer.Fax, Is.EqualTo(command.Fax));
        });
    }

    [Test]
    public async Task GetAll_IncludesNewlyCreatedCustomer()
    {
        var name = $"Listed {Guid.NewGuid():N}";
        var id = await CreateAsync(NewCustomer(name));

        var response = await AuthorizedClient.GetAsync(BaseUrl);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var customers = await response.ReadAsAsync<List<CustomerDto>>();
        Assert.That(customers.Any(c => c.Id == id && c.CustomerName == name), Is.True);
    }

    [Test]
    public async Task Update_PersistsChanges()
    {
        var id = await CreateAsync(NewCustomer("Before Update"));
        var update = UpdateFor(id, "After Update");

        var updateResponse = await AuthorizedClient.PutJsonAsync($"{BaseUrl}/{id}", update);
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await updateResponse.ReadAsAsync<int>(), Is.EqualTo(id));

        var customer = await (await AuthorizedClient.GetAsync($"{BaseUrl}/{id}")).ReadAsAsync<CustomerDto>();
        Assert.Multiple(() =>
        {
            Assert.That(customer.CustomerName, Is.EqualTo("After Update"));
            Assert.That(customer.ContactName, Is.EqualTo("New Contact"));
            Assert.That(customer.City, Is.EqualTo("Los Angeles"));
            Assert.That(customer.Region, Is.EqualTo("CA"));
        });
    }

    [Test]
    public async Task Update_RouteIdDoesNotMatchBody_ReturnsBadRequest_AndLeavesCustomerUnchanged()
    {
        var id = await CreateAsync(NewCustomer("Unchanged"));
        var update = UpdateFor(id + 1000, "Should Not Apply"); // valid body, mismatched id

        var response = await AuthorizedClient.PutJsonAsync($"{BaseUrl}/{id}", update);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var customer = await (await AuthorizedClient.GetAsync($"{BaseUrl}/{id}")).ReadAsAsync<CustomerDto>();
        Assert.That(customer.CustomerName, Is.EqualTo("Unchanged"));
    }

    [Test]
    public async Task Update_UnknownCustomer_ReturnsZero()
    {
        const int unknownId = 987654;
        var update = UpdateFor(unknownId, "Ghost");

        var response = await AuthorizedClient.PutJsonAsync($"{BaseUrl}/{unknownId}", update);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.ReadAsAsync<int>(), Is.EqualTo(0));
    }

    [Test]
    public async Task Delete_UnknownCustomer_ReturnsZero()
    {
        var response = await AuthorizedClient.DeleteAsync($"{BaseUrl}/987654");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.ReadAsAsync<int>(), Is.EqualTo(0));
    }

    [Test]
    public async Task Delete_RemovesCustomerFromList()
    {
        var id = await CreateAsync(NewCustomer("To Delete"));

        var deleteResponse = await AuthorizedClient.DeleteAsync($"{BaseUrl}/{id}");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await deleteResponse.ReadAsAsync<int>(), Is.EqualTo(id));

        var customers = await (await AuthorizedClient.GetAsync(BaseUrl)).ReadAsAsync<List<CustomerDto>>();
        Assert.That(customers.Any(c => c.Id == id), Is.False);
    }

    private HttpClient AuthorizedClient => Client.AddBearerTokenAuthorization();

    private async Task<int> CreateAsync(CreateCustomerCommand command)
    {
        var response = await AuthorizedClient.PostJsonAsync(BaseUrl, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), await response.Content.ReadAsStringAsync());
        var id = await response.ReadAsAsync<int>();
        Assert.That(id, Is.GreaterThan(0));
        return id;
    }

    [Test]
    public async Task Create_MissingRequiredFields_ReturnsBadRequest()
    {
        // Command properties are non-nullable strings, so MVC treats them as required
        var response = await AuthorizedClient.PostJsonAsync(BaseUrl, new { CustomerName = "Only a name" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("ContactName"));
    }

    private static UpdateCustomerCommand UpdateFor(int id, string name) => new()
    {
        Id = id,
        CustomerName = name,
        ContactName = "New Contact",
        ContactTitle = "Director",
        Address = "456 Another St",
        City = "Los Angeles",
        Region = "CA",
        PostalCode = "90001",
        Country = "USA",
        Phone = "987-654-3210",
        Fax = "123-456-7890"
    };

    private static CreateCustomerCommand NewCustomer(string name = "John Wick") => new()
    {
        CustomerName = name,
        ContactName = "Winston",
        ContactTitle = "Manager",
        Address = "123 Main St",
        City = "New York",
        Region = "NY",
        PostalCode = "10001",
        Country = "USA",
        Phone = "123-456-7890",
        Fax = "987-654-3210"
    };
}
