using NUnit.Framework;
using OA.Persistence;
using OA.Service.Features.CustomerFeatures.Queries;
using OA.Test.Unit.Common;

namespace OA.Test.Unit.Features.Customers;

public class CustomerQueryHandlerTests
{
    private ApplicationDbContext _context = null!;

    [SetUp]
    public void SetUp() => _context = TestDbContextFactory.Create();

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task GetAllCustomers_EmptyDatabase_ReturnsEmptyList()
    {
        var handler = new GetAllCustomerQueryHandler(_context);

        var result = await handler.Handle(new GetAllCustomerQuery(), CancellationToken.None);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllCustomers_ReturnsEveryCustomer()
    {
        await TestDbContextFactory.SeedCustomerAsync(_context, "First");
        await TestDbContextFactory.SeedCustomerAsync(_context, "Second");
        var handler = new GetAllCustomerQueryHandler(_context);

        var result = await handler.Handle(new GetAllCustomerQuery(), CancellationToken.None);

        Assert.That(result.Select(c => c.CustomerName), Is.EquivalentTo(new[] { "First", "Second" }));
    }

    [Test]
    public async Task GetCustomerById_ExistingId_ReturnsCustomer()
    {
        var existing = await TestDbContextFactory.SeedCustomerAsync(_context, "Target");
        await TestDbContextFactory.SeedCustomerAsync(_context, "Other");
        var handler = new GetCustomerByIdQueryHandler(_context);

        var result = await handler.Handle(new GetCustomerByIdQuery { Id = existing.Id }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existing.Id));
        Assert.That(result.CustomerName, Is.EqualTo("Target"));
    }

    [Test]
    public async Task GetCustomerById_UnknownId_ReturnsNull()
    {
        await TestDbContextFactory.SeedCustomerAsync(_context);
        var handler = new GetCustomerByIdQueryHandler(_context);

        var result = await handler.Handle(new GetCustomerByIdQuery { Id = 9999 }, CancellationToken.None);

        Assert.That(result, Is.Null);
    }
}
