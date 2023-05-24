using ESE.Clients.API.Application.Commands;
using ESE.Core.Mediator;
using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Clients.API.Services
{
    public class RegisterClientIntegrationHandle : BackgroundService
    {
        private IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegisterClientIntegrationHandle(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        private void SetResponder()
        {
            _bus.RespondAsync<UserRegisteredIntegrationEvent, ResponseMessage>(async request =>
              await RegisterClient(request));

            _bus.AdvancedBus.Connected += OnConnect;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();

            return Task.CompletedTask;
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }

        private async Task<ResponseMessage> RegisterClient(UserRegisteredIntegrationEvent message)
        {
            var clientCommand = new RegisterClientCommand(message.Id, message.Name, message.Email, message.Cpf);
            ValidationResult success;

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                success = await mediator.SendCommand(clientCommand);
            }

            return new ResponseMessage(success);
        }
    }
}
