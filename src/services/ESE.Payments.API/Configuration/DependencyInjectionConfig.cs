using ESE.Payments.API.Data;
using ESE.Payments.API.Data.Repository;
using ESE.Payments.API.Facede;
using ESE.Payments.API.Models;
using ESE.Payments.API.Services;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ESE.Payments.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentFacade, PaymentCreditCardFacede>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<PaymentDbContext>();
        }
    }
}
