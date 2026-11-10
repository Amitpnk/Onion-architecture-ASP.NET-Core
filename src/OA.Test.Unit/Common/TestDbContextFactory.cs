using Microsoft.EntityFrameworkCore;
using OA.Domain.Entities;
using OA.Persistence;

namespace OA.Test.Unit.Common;

/// <summary>
/// Creates an isolated in-memory <see cref="ApplicationDbContext"/> per test.
/// </summary>
internal static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static async Task<Customer> SeedCustomerAsync(ApplicationDbContext context, string name = "Acme Corp")
    {
        var customer = new Customer
        {
            CustomerName = name,
            ContactName = "Jane Doe",
            ContactTitle = "Owner",
            Address = "1 Main St",
            City = "Springfield",
            Region = "IL",
            PostalCode = "62701",
            Country = "USA",
            Phone = "555-0100",
            Fax = "555-0101"
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        return customer;
    }
}
