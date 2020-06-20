using Microsoft.AspNetCore.Builder;
using OnionArchitecture.Middleware;

namespace OnionArchitecture.Extension
{
    public static class Middleware
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }


}
