using System;

namespace ESE.Core.Messages.Integration
{
    public class OrderRegisteredIntegrationEvent : IntegrationEvent
    {
        public Guid ClientId { get; private set; }

        public OrderRegisteredIntegrationEvent(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}
