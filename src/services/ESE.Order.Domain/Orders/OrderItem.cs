using ESE.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESE.Orders.Domain.Orders
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public string Image { get; set; }

        // EF Rel.
        public Order Order { get; set; }

        public OrderItem(Guid productId, string productName, int quantity, decimal price, string image = null)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
            Image = image;
        }

        // EF ctor
        protected OrderItem() { }
        internal decimal CalculateTotalPrice()
        {
            return Quantity * Price;
        }
    }
}
