using EasyNetQ;
using ESE.Clients.API.Application.Commands;
using ESE.Core.Mediator;
using ESE.Core.Messages.Itegration;
using FluentValidation.Results;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ESE.Clients.API.Services
{
    public class RegisterClientIntegrationHandle : BackgroundService
    {
        private IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegisterClientIntegrationHandle(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           _bus = RabbitHutch.CreateBus("host=localhost:5672");

           _bus.RespondAsync<UserRegisteredIntegrationEvent, ResponseMessage>(async request =>
              new ResponseMessage(await RegisterClient(request)));

            return Task.CompletedTask;
        }

        private async Task<ValidationResult> RegisterClient(UserRegisteredIntegrationEvent message)
        {
            var clientCommand = new RegisterClientCommand(message.Id, message.Name, message.Email, message.Cpf);
            ValidationResult success;

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                success = await mediator.SendCommand(clientCommand);
            }

            return success;
        }
    }
}
