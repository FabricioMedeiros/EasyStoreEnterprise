using ESE.Core.Messages.Integration;
using System;

namespace ESE.Core.Messages.Integration
{
    public class OrderCanceledIntegrationEvent : IntegrationEvent
    {
        public Guid ClientId { get; private set; }
        public Guid OrderId { get; private set; }

        public OrderCanceledIntegrationEvent(Guid clientId, Guid orderId)
        {
            ClientId = clientId;
            OrderId = orderId;
        }
    }
}
