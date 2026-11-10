using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;
using OA.Domain.Settings;
using OA.Infrastructure.Extension;
using OA.Service;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Bind AppSettings
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScopedServices();
builder.Services.AddTransientServices();
builder.Services.AddSwaggerOpenAPI();
builder.Services.AddMailSetting(builder.Configuration);
builder.Services.AddServiceLayer();
builder.Services.AddVersion();
builder.Services.AddHealthCheck(appSettings, builder.Configuration);
builder.Services.AddFeatureManagement();

//Setup Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Setup CORS
app.UseCors(options =>
    options.WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod());

// Create the databases if they don't exist (enabled via InitializeDatabaseOnStartup)
app.InitializeDatabase(builder.Configuration);

// Configure custom exception middleware
app.ConfigureCustomExceptionMiddleware();

// Setup Serilog logging
app.Logger.LogInformation("Starting the application with Serilog logging.");

// Health check configuration
app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

app.UseHealthChecksUI(setup =>
{
    setup.ApiPath = "/healthcheck";
    setup.UIPath = "/healthcheck-ui";
    // setup.AddCustomStylesheet("Customization/custom.css");
});

// Setup routing
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger UI
app.ConfigureSwagger();

// Map controllers
app.MapControllers();

// Run the application
app.Run();


public partial class Program
{
}