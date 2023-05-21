using ESE.Clients.API.Application.Commands;
using ESE.Clients.API.Application.Events;
using ESE.Clients.API.Data;
using ESE.Clients.API.Data.Repository;
using ESE.Clients.API.Models;
using ESE.Clients.API.Services;
using ESE.Core.Mediator;
using ESE.WebAPI.Core.User;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace ESE.Clients.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IRequestHandler<RegisterClientCommand, ValidationResult>, ClientCommandHandler>();
            services.AddScoped<IRequestHandler<AddAddressCommand, ValidationResult>, ClientCommandHandler>();

            services.AddScoped<INotificationHandler<ClientRegisteredEvent>, ClientEventHandler>();

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ClientDbContext>();          
        }
    }
}
