using ESE.Core.Messages.Itegration;
using System;
using System.Collections.Generic;
using System.Text;

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
