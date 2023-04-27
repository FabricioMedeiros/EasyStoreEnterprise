using System;
using System.Collections.Generic;
using System.Text;

namespace ESE.Order.Domain.Orders
{
    public class OrderItem
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public string Image { get; set; }

        // EF Rel.
        public Order Order { get; set; }

        public OrderItem(Guid orderId, Guid productId, string productName, int quantity, decimal price, string image = null)
        {
            OrderId = orderId;
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
