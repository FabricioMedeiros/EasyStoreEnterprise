using System;
using System.Collections.Generic;

namespace ESE.Core.Messages.Integration
{
    public class OrderAuthorizedIntegrationEvent : IntegrationEvent
    {
        public Guid ClientId { get; private set; }
        public Guid OrderId { get; private set; }
        public IDictionary<Guid, int> Items { get; private set; }

        public OrderAuthorizedIntegrationEvent(Guid clientId, Guid orderId, IDictionary<Guid, int> items)
        {
            ClientId = clientId;
            OrderId = orderId;
            Items = items;
        }
    }
}
