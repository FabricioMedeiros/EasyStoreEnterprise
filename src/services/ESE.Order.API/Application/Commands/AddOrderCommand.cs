using ESE.Core.Messages;
using ESE.Orders.API.Application.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Orders.API.Application.Commands
{
    public class AddOrderCommand : Command
    {
        // Pedido
        public Guid ClientId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }

        // Voucher
        public string VoucherCode { get; set; }
        public bool VoucherUsed { get; set; }
        public decimal Discount { get; set; }

        // Endereco
        public AddressDTO Address { get; set; }

        // Cartao
        public string NumberCard { get; set; }
        public string NameCard { get; set; }
        public string ExpirationCard { get; set; }
        public string CvvCard { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AddOrderValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class AddOrderValidation : AbstractValidator<AddOrderCommand>
        {
            public AddOrderValidation()
            {
                RuleFor(c => c.ClientId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Id do cliente inválido");

                RuleFor(c => c.OrderItems.Count)
                    .GreaterThan(0)
                    .WithMessage("O pedido precisa ter no mínimo 1 item");

                RuleFor(c => c.TotalPrice)
                    .GreaterThan(0)
                    .WithMessage("Valor do pedido inválido");

                RuleFor(c => c.NumberCard)
                    .CreditCard()
                    .WithMessage("Número de cartão inválido");

                RuleFor(c => c.NameCard)
                    .NotNull()
                    .WithMessage("Nome do portador do cartão requerido.");

                RuleFor(c => c.CvvCard.Length)
                    .GreaterThan(2)
                    .LessThan(5)
                    .WithMessage("O CVV do cartão precisa ter 3 ou 4 números.");

                RuleFor(c => c.ExpirationCard)
                    .NotNull()
                    .WithMessage("Data expiração do cartão requerida.");
            }
        }
    }
}
