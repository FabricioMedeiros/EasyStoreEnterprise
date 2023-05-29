using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using ESE.Payments.API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Payments.API.Services
{
    public class PaymentIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        private void SetRespond()
        {
            _bus.RespondAsync<OrderStartedIntegrationEvent, ResponseMessage>(async request =>
                await AuthorizePayment(request));
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetRespond();
            return Task.CompletedTask;
        }

        private async Task<ResponseMessage> AuthorizePayment(OrderStartedIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var payment = new Payment
            {
                OrderId = message.OrderId,
                TypePayment = (TypePayment)message.TypePayment,
                Price = message.Price,
                CreditCard = new CreditCard(
                    message.NameCard, message.NumberCard, message.MonthYearExpiry, message.CVV)
            };

            var response = await paymentService.AuthorizePayment(payment);

            return response;
        }
    }
}
