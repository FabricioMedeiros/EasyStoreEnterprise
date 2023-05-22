using System;

namespace ESE.Core.Messages.Integration
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public Guid ClientId { get; set; }
        public Guid OrderId { get; set; }
        public int TypePayment { get; set; }
        public decimal Price { get; set; }

        public string NameCard { get; set; }
        public string NumberCard { get; set; }
        public string MonthYearExpiry { get; set; }
        public string CVV { get; set; }
    }
}
