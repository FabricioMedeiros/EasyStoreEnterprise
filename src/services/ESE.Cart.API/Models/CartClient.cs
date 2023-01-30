using System;
using System.Collections.Generic;
using System.Linq;

namespace ESE.Cart.API.Models
{
    public class CartClient
    {
        internal const int MAX_QUANTITY_ITEMS = 5;
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

        internal void CalculateTotalValueCart()
        {
            TotalPrice = Items.Sum(p => p.CalculateValue());
        }

        internal bool ItemExistsCart(CartItem item)
        {
            return Items.Any(p => p.ProductId == item.ProductId);
        }
        internal CartItem GetProdutById(Guid productId)
        {
            return Items.FirstOrDefault(p => p.ProductId == productId);
        }

        internal void AddItem(CartItem item)
        {
            if (!item.IsValid()) return;

            item.linkCart(Id);

            if (ItemExistsCart(item))
            {
                var itemExists = GetProdutById(item.ProductId);
                itemExists.AddUnits(item.Quantity);

                item = itemExists;
                Items.Remove(itemExists);
            }

            Items.Add(item);
            CalculateTotalValueCart();
        }

    }

}
