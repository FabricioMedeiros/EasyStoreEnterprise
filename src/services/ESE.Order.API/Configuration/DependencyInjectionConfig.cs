

using ESE.Core.Mediator;
using ESE.Orders.API.Application.Commands;
using ESE.Orders.API.Application.Events;
using ESE.Orders.API.Application.Queries;
using ESE.Orders.Domain.Orders;
using ESE.Orders.Domain.Vouchers;
using ESE.Orders.Infra.Data;
using ESE.Orders.Infra.Data.Repository;
using ESE.WebAPI.Core.User;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ESE.Orders.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            // Commands
            services.AddScoped<IRequestHandler<AddOrderCommand, ValidationResult>, OrderCommandHandler>();

            // Events
            services.AddScoped<INotificationHandler<OrderRegisteredEvent>, OrderEventHandler>();

            // Application
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IVoucherQueries, VoucherQueries>();
            services.AddScoped<IOrderQueries, OrderQueries>();

            // Data
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<OrderDbContext>();
        }
    }
}
