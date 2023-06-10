using ESE.Core.DomainObjects;
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

        public PaymentIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
        }

        private void SetRespond()
        {
            _bus.RespondAsync<OrderStartedIntegrationEvent, ResponseMessage>(async request =>
                await AuthorizePayment(request));
        }
        private void SetSubscribers()
        {
            _bus.SubscribeAsync<OrderCanceledIntegrationEvent>("OrderCanceled", async request =>
            await CancelPayment(request));

            _bus.SubscribeAsync<OrderDecrementedIntegrationEvent>("OrderDecremented", async request =>
            await CapturePayment(request));
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetRespond();
            SetSubscribers();
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

        private async Task CancelPayment(OrderCanceledIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                var response = await paymentService.CancelPayment(message.OrderId);

                if (!response.ValidationResult.IsValid)
                    throw new DomainException($"Falha ao cancelar pagamento do pedido {message.OrderId}");
            }
        }

        private async Task CapturePayment(OrderDecrementedIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                var response = await paymentService.CapturePayment(message.OrderId);

                if (!response.ValidationResult.IsValid)
                    throw new DomainException($"Falha ao capturar pagamento do pedido {message.OrderId}");

                await _bus.PublishAsync(new OrderPaidIntegrationEvent(message.ClientId, message.OrderId));
            }
        }
    }
}
