using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Orders.API.Application.Events
{
    public class OrderEventHandler : INotificationHandler<OrderRegisteredEvent>
    {
        private readonly IMessageBus _bus;

        public OrderEventHandler(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(OrderRegisteredEvent message, CancellationToken cancellationToken)
        {
            await _bus.PublishAsync(new OrderRegisteredIntegrationEvent(message.ClientId));
        }
    }
}
