using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnionArchitecture.Data;
using OnionArchitecture.Infrastructure.Mapping;
using OnionArchitecture.Persistence.Contract;
using OnionArchitecture.Persistence.Repository;
using OnionArchitecture.Service.Contract;
using OnionArchitecture.Service.ImplementationBL;

namespace OnionArchitecture.Infrastructure.Extension
{
    public static class ConfigureServiceContainer
    {
        public static void AddDbContext(this IServiceCollection serviceCollection,
             IConfiguration configuration, IConfigurationRoot configRoot)
        {
            serviceCollection.AddDbContext<CustomerContext>(options =>
                   options.UseSqlServer(configuration.GetConnectionString("OnionArchConn") ?? configRoot["ConnectionStrings:OnionArchConn"])
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
