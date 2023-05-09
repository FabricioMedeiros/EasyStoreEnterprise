using ESE.Clients.API.Application.Events;
using ESE.Clients.API.Models;
using ESE.Core.Messages;
using FluentValidation.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Clients.API.Application.Commands
{
    public class ClientCommandHandler : CommandHandler, IRequestHandler<RegisterClientCommand, ValidationResult>,
        IRequestHandler<AddAddressCommand, ValidationResult>
    {
        private readonly IClientRepository _clientRepository;

        public ClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public async Task<ValidationResult> Handle(RegisterClientCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var client = new Client(message.Id, message.Name, message.Email, message.Cpf);

            var clientexist = await _clientRepository.GetByCpf(client.Cpf.Number);

            if (clientexist != null)
            {
                AddError("Este CPF já está em uso.");
                return ValidationResult;
            }

            _clientRepository.Add(client);

            client.AddEvent(new ClientRegisteredEvent(message.Id, message.Name, message.Email, message.Cpf));

            return await SaveData(_clientRepository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(AddAddressCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var address = new Address(message.Street, message.Number, message.Complement, message.Neighborhood, message.City, message.State, message.ZipCode, message.ClientId);
            _clientRepository.AddAddress(address);

            return await SaveData(_clientRepository.UnitOfWork);
        }
    }
}

