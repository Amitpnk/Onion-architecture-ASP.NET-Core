using AutoMapper;
using NUnit.Framework;
using OA.Domain.Entities;
using OA.Infrastructure.Mapping;
using OA.Infrastructure.ViewModel;

namespace OA.Test.Unit.Mapping;

public class CustomerProfileTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CustomerProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Test]
    public void Map_CustomerModelToCustomer_MapsCustomerIdToId()
    {
        var model = new CustomerModel { CustomerId = 12, CustomerName = "Acme", City = "Paris", Phone = "123" };

        var customer = _mapper.Map<Customer>(model);

        Assert.Multiple(() =>
        {
            Assert.That(customer.Id, Is.EqualTo(12));
            Assert.That(customer.CustomerName, Is.EqualTo("Acme"));
            Assert.That(customer.City, Is.EqualTo("Paris"));
            Assert.That(customer.Phone, Is.EqualTo("123"));
        });
    }

    [Test]
    public void Map_CustomerToCustomerModel_MapsIdBackToCustomerId()
    {
        var customer = new Customer { Id = 34, CustomerName = "Globex", Country = "USA" };

        var model = _mapper.Map<CustomerModel>(customer);

        Assert.Multiple(() =>
        {
            Assert.That(model.CustomerId, Is.EqualTo(34));
            Assert.That(model.CustomerName, Is.EqualTo("Globex"));
            Assert.That(model.Country, Is.EqualTo("USA"));
        });
    }
}
