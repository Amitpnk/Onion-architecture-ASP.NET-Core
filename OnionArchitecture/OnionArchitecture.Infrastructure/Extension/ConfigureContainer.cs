using Microsoft.AspNetCore.Builder;
using OnionArchitecture.Infrastructure.Middleware;

namespace OnionArchitecture.Infrastructure.Extension
{
    public static class ConfigureContainer
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
