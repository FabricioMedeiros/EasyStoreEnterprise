using FluentValidation;
using FluentValidation.Results;
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
        public ValidationResult ValidationResult { get; set; }
        public bool VoucherUsed { get; set; }
        public decimal Discount { get; set; }
        public Voucher Voucher { get; set; }

        public CartClient(Guid clientId)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
        }

        public CartClient() { }

        public void ApplyVoucher(Voucher voucher)
        {
            Voucher = voucher;
            VoucherUsed = true;
            CalculateTotalValueCart();
        }

        internal void CalculateTotalValueCart()
        {
            TotalPrice = Items.Sum(p => p.CalculateValue());
            CalculateTotalDiscount();
        }

        private void CalculateTotalDiscount()
        {
            if (!VoucherUsed) return;

            decimal discount = 0;
            var value = TotalPrice;

            if (Voucher.TypeDiscount == TypeDiscountVoucher.Percentage)
            {
                if (Voucher.Percent.HasValue)
                {
                    discount = (value * Voucher.Percent.Value) / 100;
                    value -= discount;
                }
            }
            else
            {
                if (Voucher.DiscountValue.HasValue)
                {
                    discount = Voucher.DiscountValue.Value;
                    value -= discount;
                }
            }

            TotalPrice = value < 0 ? 0 : value;
            Discount = discount;
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

        internal void UpdateItem(CartItem item)
        {
            if (!item.IsValid()) return;

            item.linkCart(Id);

            var itemExists = GetProdutById(item.ProductId);

            Items.Remove(itemExists);
            Items.Add(item);

            CalculateTotalValueCart();
        }

        internal void UpdateUnits(CartItem item, int units)
        {
            item.UpdateUnits(units);
            UpdateItem(item);
        }

        internal void RemoveItem(CartItem item)
        {
            Items.Remove(GetProdutById(item.ProductId));
            CalculateTotalValueCart();
        }

        internal bool IsValid()
        {
            var erros = Items.SelectMany(i => new CartItem.CartItemValidation().Validate(i).Errors).ToList();
            erros.AddRange(new CartClientValidation().Validate(this).Errors);
            ValidationResult = new ValidationResult(erros);

            return ValidationResult.IsValid;
        }

        public class CartClientValidation : AbstractValidator<CartClient>
        {
            public CartClientValidation()
            {
                RuleFor(c => c.ClientId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Cliente não reconhecido");

                RuleFor(c => c.Items.Count)
                    .GreaterThan(0)
                    .WithMessage("O carrinho não possui itens");

                RuleFor(c => c.TotalPrice)
                    .GreaterThan(0)
                    .WithMessage("O valor total do carrinho precisa ser maior que 0");
            }
        }

    }

}
