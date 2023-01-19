using ESE.Cart.API.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ESE.Cart.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<CartDbContext>();
        }
    }
}
