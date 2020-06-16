using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnionArchitecture.Data;
using OnionArchitecture.Mapping;
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

        //public static void AddRepository(this IServiceCollection serviceCollection)
        //{
        //    serviceCollection.AddScoped(typeof(IRepository<>), typeof(DataRepository<>));
        //}

        //public static void AddTransientServices(this IServiceCollection serviceCollection)
        //{
        //    serviceCollection.AddTransient<IBookService, BookService>();

        //    serviceCollection.AddTransient<IEmailSender, EmailSender>();
        //}

    }
}

