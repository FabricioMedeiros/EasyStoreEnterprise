using ESE.Core.Messages;
using FluentValidation;
using System;

namespace ESE.Clients.API.Application.Commands
{
    public class RegisterClientCommand : Command
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Cpf { get; private set; }

        public RegisterClientCommand(Guid id, string name, string email, string cpf)
        {
            AggregateId = id;
            Id = id;
            Name = name;
            Email = email;
            Cpf = cpf;
        }

        public override bool IsValid()
        {
            ValidationResult = new RegisterClientValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class RegisterClientValidation : AbstractValidator<RegisterClientCommand>
        {
            public RegisterClientValidation()
            {
                RuleFor(c => c.Id)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Id do cliente inválido");

                RuleFor(c => c.Name)
                    .NotEmpty()
                    .WithMessage("O nome do cliente não foi informado");

                RuleFor(c => c.Cpf)
                    .Must(ValidateCpf)
                    .WithMessage("O CPF informado não é válido.");

                RuleFor(c => c.Email)
                    .Must(ValidateEmail)
                    .WithMessage("O e-mail informado não é válido.");
            }

            protected static bool ValidateCpf(string cpf)
            {
                return Core.DomainObjects.Cpf.Validate(cpf);
            }

            protected static bool ValidateEmail(string email)
            {
                return Core.DomainObjects.Email.Validate(email);
            }
        }
    }
}
