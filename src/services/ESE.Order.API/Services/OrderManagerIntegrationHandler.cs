using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using ESE.Orders.API.Application.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Orders.API.Services
{
    public class OrderManagerIntegrationHandler : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderManagerIntegrationHandler> _logger;
        private Timer _timer;

        public OrderManagerIntegrationHandler(ILogger<OrderManagerIntegrationHandler> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de pedidos iniciado.");

            _timer = new Timer(ProcessOrders, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private async void ProcessOrders(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var orderQueries = scope.ServiceProvider.GetRequiredService<IOrderQueries>();
                var order = await orderQueries.GetAuthorizedOrders();

                if (order == null) return;

                var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var orderAuthorized = new OrderAuthorizedIntegrationEvent(order.ClientId, order.Id,
                    order.OrderItems.ToDictionary(p => p.ProductId, p => p.Quantity));

                await bus.PublishAsync(orderAuthorized);

                _logger.LogInformation($"Pedido ID: {order.Id} foi encaminhado para baixa no estoque.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de pedidos finalizado.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
