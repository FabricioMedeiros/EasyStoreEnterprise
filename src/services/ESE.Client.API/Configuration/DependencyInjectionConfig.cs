using ESE.Clients.API.Data;
using ESE.Clients.API.Data.Repository;
using ESE.Clients.API.Models;
using Microsoft.Extensions.DependencyInjection;


namespace ESE.Clients.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ClientDbContext>();
        }
    }
}
