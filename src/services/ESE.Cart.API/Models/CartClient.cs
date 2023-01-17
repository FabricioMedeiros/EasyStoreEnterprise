using System;
using System.Collections.Generic;

namespace ESE.Cart.API.Models
{
    public class CartClient
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public CartClient(Guid clientId)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
        }

        public CartClient() { }

    }

}
