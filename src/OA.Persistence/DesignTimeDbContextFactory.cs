using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OA.Persistence;

// Used only by EF Core tooling (dotnet ef / Package Manager Console). These factories let
// migrations be added and applied without starting the web host, so they work whether the
// startup project is OA or OA.Persistence. See https://go.microsoft.com/fwlink/?linkid=851728

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(DesignTimeConfiguration.GetConnectionString("OnionArchConn"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .Options;
        return new ApplicationDbContext(options);
    }
}

public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IdentityContext>()
            .UseSqlServer(DesignTimeConfiguration.GetConnectionString("IdentityConnection"),
                b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName))
            .Options;
        return new IdentityContext(options);
    }
}

internal static class DesignTimeConfiguration
{
    public static string GetConnectionString(string name)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(FindSettingsDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString(name)
               ?? throw new InvalidOperationException(
                   $"Connection string '{name}' was not found. Add it to OA/appsettings.json or set the " +
                   $"ConnectionStrings__{name} environment variable.");
    }

    // dotnet ef runs with the startup project's folder as the working directory. When that is
    // OA.Persistence (which has no appsettings.json), fall back to the web project's settings.
    private static string FindSettingsDirectory()
    {
        var current = Directory.GetCurrentDirectory();
        if (File.Exists(Path.Combine(current, "appsettings.json")))
        {
            return current;
        }

        var webProject = Path.GetFullPath(Path.Combine(current, "..", "OA"));
        return File.Exists(Path.Combine(webProject, "appsettings.json")) ? webProject : current;
    }
}
