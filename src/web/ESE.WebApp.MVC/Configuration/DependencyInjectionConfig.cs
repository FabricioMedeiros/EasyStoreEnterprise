using ESE.WebAPI.Core.User;
using ESE.WebApp.MVC.Extensions;
using ESE.WebApp.MVC.Services;
using ESE.WebApp.MVC.Services.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Net.Http;

namespace ESE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            #region HttpServices

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAuthService, AuthService>().
                     AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                     .AddPolicyHandler(PollyExtensions.WaitTry())
                     .AddTransientHttpErrorPolicy(
                          p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICatalogService, CatalogService>().
                     AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                     .AddPolicyHandler(PollyExtensions.WaitTry())
                     .AddTransientHttpErrorPolicy(
                          p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IShoppingBffService, ShoppingBffService>().
                    AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                    .AddPolicyHandler(PollyExtensions.WaitTry())
                    .AddTransientHttpErrorPolicy(
                         p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            #endregion
        }
    }

    #region PollyExtensions
    public static class PollyExtensions
    {
        public static AsyncRetryPolicy<HttpResponseMessage> WaitTry()
        {
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tentando pela {retryCount} vez!");
                    Console.ForegroundColor = ConsoleColor.White;
                });

            return retry;
        }
    }
    #endregion
}
