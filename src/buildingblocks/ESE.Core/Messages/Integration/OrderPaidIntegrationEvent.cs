﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESE.Core.Messages.Integration
{
    public class OrderPaidIntegrationEvent : IntegrationEvent
    {
        public Guid ClientId { get; private set; }
        public Guid OrderId { get; private set; }

        public OrderPaidIntegrationEvent(Guid clientId, Guid orderId)
        {
            ClientId = clientId;
            OrderId = orderId;
        }
    }
}
