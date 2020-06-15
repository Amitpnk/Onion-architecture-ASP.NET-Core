using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnionArchitecture.Data;
using OnionArchitecture.Mapping;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Persistence.Repository;
using System.IO;

namespace OnionArchitecture
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();

            // Configure connection string in dbcontext
            services.AddDbContext<CustomerContext>(opt =>
                   //opt.UseSqlServer(config["ConnectionStrings:CustomerConn"])
                   opt.UseSqlServer(Configuration.GetConnectionString("OnionArchConn"))
                   .EnableSensitiveDataLogging()
                );

            // Auto Mapper Configurations  
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CustomerProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Configure and register for customer repository 
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
