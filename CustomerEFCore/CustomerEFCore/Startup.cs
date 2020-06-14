using AutoMapper;
using CustomerEFCore.Data;
using CustomerEFCore.Mapping;
using CustomerEFCore.Repository;
using CustomerEFCore.Repository.Contract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace CustomerEFCore
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

            services.AddDbContext<CustomerContext>(opt =>
                   opt.UseSqlServer(config["ConnectionStrings:CustomerConnx"])
                   .EnableSensitiveDataLogging()
                );

            // Configure connection string in dbcontext
            //services.AddDbContext<CustomerContext>(opt =>
            //       //opt.UseSqlServer("Data Source=(local)\\SQLexpress;Initial Catalog=CustomerEFCoreDB;Integrated Security=True")
            //       opt.UseSqlServer(Configuration.GetConnectionString("CustomerConnx"))
            //       //opt.UseSqlServer(Configuration["ConnectionStrings:CustomerConnx"].ConnectionString)

            //          .EnableSensitiveDataLogging()
            //    );

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
