using ESE.Clients.API.Application.Commands;
using ESE.Clients.API.Application.Events;
using ESE.Clients.API.Data;
using ESE.Clients.API.Data.Repository;
using ESE.Clients.API.Models;
using ESE.Core.Mediator;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace ESE.Clients.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IRequestHandler<RegisterClientCommand, ValidationResult>, ClientCommandHandler>();

            services.AddScoped<INotificationHandler<ClientRegisteredEvent>, ClientEventHandler>();

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ClientDbContext>();
            
        }
    }
}
