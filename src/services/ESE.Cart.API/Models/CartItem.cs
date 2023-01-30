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

        internal bool IsValid()
        {
            return new CartItemValidation().Validate(this).IsValid;
        }

        public class CartItemValidation : AbstractValidator<CartItem>
        {
            public CartItemValidation()
            {
                RuleFor(c => c.ProductId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Id do produto inválido");

                RuleFor(c => c.Name)
                    .NotEmpty()
                    .WithMessage("O nome do produto não foi informado");

                RuleFor(c => c.Quantity)
                    .GreaterThan(0)
                    .WithMessage(item => $"A quantidade miníma para o {item.Name} é 1");

                RuleFor(c => c.Quantity)
                    .LessThanOrEqualTo(CartClient.MAX_QUANTITY_ITEMS)
                    .WithMessage(item => $"A quantidade máxima do {item.Name} é {CartClient.MAX_QUANTITY_ITEMS}");

                RuleFor(c => c.Price)
                    .GreaterThan(0)
                    .WithMessage(item => $"O valor do {item.Name} precisa ser maior que 0");
            }
        }

    }
}