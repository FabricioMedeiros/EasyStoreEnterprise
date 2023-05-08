using ESE.Cart.API.Data;
using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Cart.API.Services
{
    public class CartIntegrationHander : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public CartIntegrationHander(IServiceProvider serviceProvider, IMessageBus bus)
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
            _bus.SubscribeAsync<OrderRegisteredIntegrationEvent>("OrderRegistered", async request =>
                await DeleteCart(request));
        }
        private async Task DeleteCart(OrderRegisteredIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CartDbContext>();

            var cart = await context.CartClients
                .FirstOrDefaultAsync(c => c.ClientId == message.ClientId);

            if (cart != null)
            {
                context.CartClients.Remove(cart);
                await context.SaveChangesAsync();
            }
        }
    }
}
