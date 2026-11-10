using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OA.Persistence;
using OA.Service.Features.CustomerFeatures.Commands;
using OA.Test.Unit.Common;

namespace OA.Test.Unit.Features.Customers;

public class CustomerCommandHandlerTests
{
    private ApplicationDbContext _context = null!;

    [SetUp]
    public void SetUp() => _context = TestDbContextFactory.Create();

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task CreateCustomer_PersistsAllFields_AndReturnsNewId()
    {
        var handler = new CreateCustomerCommandHandler(_context);
        var command = new CreateCustomerCommand
        {
            CustomerName = "John Wick",
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

        var id = await handler.Handle(command, CancellationToken.None);

        var saved = await _context.Customers.SingleAsync(c => c.Id == id);
        Assert.Multiple(() =>
        {
            Assert.That(id, Is.GreaterThan(0));
            Assert.That(saved.CustomerName, Is.EqualTo(command.CustomerName));
            Assert.That(saved.ContactName, Is.EqualTo(command.ContactName));
            Assert.That(saved.ContactTitle, Is.EqualTo(command.ContactTitle));
            Assert.That(saved.Address, Is.EqualTo(command.Address));
            Assert.That(saved.City, Is.EqualTo(command.City));
            Assert.That(saved.Region, Is.EqualTo(command.Region));
            Assert.That(saved.PostalCode, Is.EqualTo(command.PostalCode));
            Assert.That(saved.Country, Is.EqualTo(command.Country));
            Assert.That(saved.Phone, Is.EqualTo(command.Phone));
            Assert.That(saved.Fax, Is.EqualTo(command.Fax));
        });
    }

    [Test]
    public async Task UpdateCustomer_ExistingCustomer_UpdatesFields_AndReturnsId()
    {
        var existing = await TestDbContextFactory.SeedCustomerAsync(_context);
        var handler = new UpdateCustomerCommandHandler(_context);
        var command = new UpdateCustomerCommand
        {
            Id = existing.Id,
            CustomerName = "Updated Name",
            ContactName = "New Contact",
            ContactTitle = "Director",
            Address = "456 Another St",
            City = "Los Angeles",
            Region = "CA",
            PostalCode = "90001",
            Country = "USA",
            Phone = "000-000-0000",
            Fax = "111-111-1111"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        var updated = await _context.Customers.SingleAsync(c => c.Id == existing.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(existing.Id));
            Assert.That(updated.CustomerName, Is.EqualTo("Updated Name"));
            Assert.That(updated.ContactName, Is.EqualTo("New Contact"));
            Assert.That(updated.ContactTitle, Is.EqualTo("Director"));
            Assert.That(updated.Address, Is.EqualTo("456 Another St"));
            Assert.That(updated.City, Is.EqualTo("Los Angeles"));
            Assert.That(updated.Region, Is.EqualTo("CA"));
            Assert.That(updated.PostalCode, Is.EqualTo("90001"));
            Assert.That(updated.Phone, Is.EqualTo("000-000-0000"));
            Assert.That(updated.Fax, Is.EqualTo("111-111-1111"));
        });
    }

    [Test]
    public async Task UpdateCustomer_UnknownId_ReturnsZero_AndChangesNothing()
    {
        var existing = await TestDbContextFactory.SeedCustomerAsync(_context);
        var handler = new UpdateCustomerCommandHandler(_context);

        var result = await handler.Handle(new UpdateCustomerCommand { Id = 9999, CustomerName = "Ghost" }, CancellationToken.None);

        Assert.That(result, Is.EqualTo(0));
        var untouched = await _context.Customers.SingleAsync();
        Assert.That(untouched.CustomerName, Is.EqualTo(existing.CustomerName));
    }

    [Test]
    public async Task DeleteCustomer_ExistingCustomer_RemovesIt_AndReturnsId()
    {
        var existing = await TestDbContextFactory.SeedCustomerAsync(_context);
        var handler = new DeleteCustomerByIdCommandHandler(_context);

        var result = await handler.Handle(new DeleteCustomerByIdCommand { Id = existing.Id }, CancellationToken.None);

        Assert.That(result, Is.EqualTo(existing.Id));
        Assert.That(await _context.Customers.AnyAsync(), Is.False);
    }

    [Test]
    public async Task DeleteCustomer_UnknownId_ReturnsZero_AndKeepsOtherCustomers()
    {
        await TestDbContextFactory.SeedCustomerAsync(_context);
        var handler = new DeleteCustomerByIdCommandHandler(_context);

        var result = await handler.Handle(new DeleteCustomerByIdCommand { Id = 9999 }, CancellationToken.None);

        Assert.That(result, Is.EqualTo(0));
        Assert.That(await _context.Customers.CountAsync(), Is.EqualTo(1));
    }
}
