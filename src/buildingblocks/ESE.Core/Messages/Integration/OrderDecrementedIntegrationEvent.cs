using System;

namespace ESE.Core.Messages.Integration
{
    public class OrderDecrementedIntegrationEvent : IntegrationEvent
    {
        public Guid ClientId { get; private set; }
        public Guid OrderId { get; private set; }

        public OrderDecrementedIntegrationEvent(Guid clientId, Guid orderId)
        {
            ClientId = clientId;
            OrderId = orderId;
        }
    }
}
