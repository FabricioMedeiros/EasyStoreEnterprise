using ESE.Core.Messages;
using FluentValidation;
using System;

namespace ESE.Clients.API.Application.Commands
{
    public class AddAddressCommand : Command
    {
        public Guid ClientId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public AddAddressCommand()
        {
        }

        public AddAddressCommand(Guid clientId, string street, string number, string complement, string neighborhood, string city, string state, string zipCode)
        {
            ClientId = clientId;
            Street = street;
            Number = number;
            Complement = complement;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public override bool IsValid()
        {
            ValidationResult = new AddressValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class AddressValidation : AbstractValidator<AddAddressCommand>
        {
            public AddressValidation()
            {
                RuleFor(c => c.Street)
                    .NotEmpty()
                    .WithMessage("Informe o Logradouro");

                RuleFor(c => c.Number)
                    .NotEmpty()
                    .WithMessage("Informe o Número");

                RuleFor(c => c.ZipCode)
                    .NotEmpty()
                    .WithMessage("Informe o CEP");

                RuleFor(c => c.Neighborhood)
                    .NotEmpty()
                    .WithMessage("Informe o Bairro");

                RuleFor(c => c.City)
                    .NotEmpty()
                    .WithMessage("Informe o Cidade");

                RuleFor(c => c.State)
                    .NotEmpty()
                    .WithMessage("Informe o Estado");
            }
        }
    }
}
