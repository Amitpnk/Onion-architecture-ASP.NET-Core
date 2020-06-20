using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnionArchitecture.Data;
using OnionArchitecture.Mapping;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Persistence.Repository;
using OnionArchitecture.Service.ImplementationBL;
using OnionArchitecture.Service.Interface;
using System.IO;

namespace OnionArchitecture.Extension
{

    public static class ConfigureContainer
    {
        public static void AddDbContext(this IServiceCollection serviceCollection,
            string dataConnectionString)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();

            serviceCollection.AddDbContext<CustomerContext>(opt =>
                   opt.UseSqlServer(dataConnectionString ?? config["ConnectionStrings:OnionArchConn"])
                //.EnableSensitiveDataLogging()
                );
        }
        public static void AddAutoMapper(this IServiceCollection serviceCollection)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CustomerProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            serviceCollection.AddSingleton(mapper);
        }

        public static void AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            serviceCollection.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICustomerService, CustomerService>();
        }



    }
}

