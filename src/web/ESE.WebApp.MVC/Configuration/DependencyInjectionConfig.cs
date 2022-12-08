using ESE.WebApp.MVC.Extensions;
using ESE.WebApp.MVC.Services;
using ESE.WebApp.MVC.Services.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ESE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAuthService, AuthService>();

            services.AddHttpClient<ICatalogService, CatalogService>().
                     AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();
        }

    }
} 
