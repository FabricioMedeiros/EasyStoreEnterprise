using ESE.Core.Messages;
using System;

namespace ESE.Orders.API.Application.Events
{
    public class OrderRegisteredEvent : Event
    {
        public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }

        public OrderRegisteredEvent(Guid orderId, Guid clientId)
        {
            OrderId = orderId;
            ClientId = clientId;
        }
    }
}
