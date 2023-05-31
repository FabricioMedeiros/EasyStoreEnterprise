using ESE.Catalog.API.Models;
using ESE.Core.DomainObjects;
using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Catalog.API.Services
{
    public class CatalogIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public CatalogIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<OrderAuthorizedIntegrationEvent>("OrderAuthorized", async request =>
                await DecrementStock(request));
        }

        private async Task DecrementStock(OrderAuthorizedIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var productsHasStock = new List<Product>();
                var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

                var idsProducts = string.Join(",", message.Items.Select(c => c.Key));
                var products = await productRepository.GestListProductsById(idsProducts);

                if (products.Count != message.Items.Count)
                {
                    CancelOrderOutofStock(message);
                    return;
                }

                foreach (var product in products)
                {
                    var quantityProduct = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;

                    if (product.Available(quantityProduct))
                    {
                        product.DecrementStock(quantityProduct);
                        productsHasStock.Add(product);
                    }
                }

                if (productsHasStock.Count != message.Items.Count)
                {
                    CancelOrderOutofStock(message);
                    return;
                }

                foreach (var product in productsHasStock)
                {
                    productRepository.Update(product);
                }

                if (!await productRepository.UnitOfWork.Commit())
                {
                    throw new DomainException($"Problemas ao atualizar estoque do pedido {message.OrderId}");
                }

                var orderDecremented = new OrderDecrementedIntegrationEvent(message.ClientId, message.OrderId);
                await _bus.PublishAsync(orderDecremented);
            }
        }

        public async void CancelOrderOutofStock(OrderAuthorizedIntegrationEvent message)
        {
            var orderCanceled = new OrderCanceledIntegrationEvent(message.ClientId, message.OrderId);
            await _bus.PublishAsync(orderCanceled);
        }
    }
}
