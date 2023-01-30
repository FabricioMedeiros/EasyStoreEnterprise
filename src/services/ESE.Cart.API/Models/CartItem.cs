using FluentValidation;
using System;

namespace ESE.Cart.API.Models
{
    public class CartItem
    {
        public Guid CartId { get; set; }
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }        
        public CartClient CartClient { get; set; }

        public CartItem()
        {
            Id = Guid.NewGuid();
        }

        internal void linkCart(Guid cartId)
        {
            CartId = cartId;
        }

        internal decimal CalculateValue()
        {
            return Quantity * Price;
        }

        internal void AddUnits(int units)
        {
            Quantity += units;
        }

        internal void UpdateUnits(int units)
        {
            Quantity = units;
        }
        
    }
}