using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OA.Persistence;

namespace OA.Infrastructure.Extension;

public static class DatabaseInitializer
{
    /// <summary>
    /// Creates the application and identity databases when "InitializeDatabaseOnStartup" is true.
    /// If the project has EF Core migrations they are applied; otherwise the schema is created
    /// directly from the model. This avoids the SQL error 4060 ("Cannot open database ... requested
    /// by the login") that happens when the API is called before the database exists.
    /// </summary>
    public static void InitializeDatabase(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase") ||
            !configuration.GetValue<bool>("InitializeDatabaseOnStartup"))
        {
            return;
        }

        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseInitializer));

        Initialize(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), logger);
        Initialize(scope.ServiceProvider.GetRequiredService<IdentityContext>(), logger);
    }

    private static void Initialize(DbContext context, ILogger logger)
    {
        var name = context.GetType().Name;
        try
        {
            if (context.Database.GetMigrations().Any())
            {
                logger.LogInformation("Applying migrations for {Context}", name);
                context.Database.Migrate();
            }
            else
            {
                CreateSchemaFromModel(context, logger, name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Could not initialize database for {Context}. Check the connection string and that the SQL login " +
                "has permission to create/open the database.", name);
            throw;
        }
    }

    // EnsureCreated() skips schema creation when the database already contains any table. The
    // Serilog MSSqlServer sink creates the database and its EventLogging.Logs table while the host
    // is built, so check for this context's own tables instead.
    private static void CreateSchemaFromModel(DbContext context, ILogger logger, string name)
    {
        var creator = context.Database.GetService<IRelationalDatabaseCreator>();
        if (!creator.Exists())
        {
            logger.LogInformation("Creating database for {Context}", name);
            creator.Create();
        }

        var entity = context.Model.GetEntityTypes().First(e => e.GetTableName() != null);
        var table = $"[{entity.GetSchema() ?? "dbo"}].[{entity.GetTableName()}]";
        var exists = context.Database
            .SqlQuery<int>($"SELECT CASE WHEN OBJECT_ID({table}, 'U') IS NULL THEN 0 ELSE 1 END AS [Value]")
            .Single() == 1;

        if (!exists)
        {
            logger.LogInformation("No migrations found for {Context}; creating tables from model", name);
            creator.CreateTables();
        }
    }
}
