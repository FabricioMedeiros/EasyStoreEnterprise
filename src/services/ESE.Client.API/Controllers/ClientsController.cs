using ESE.Clients.API.Application.Commands;
using ESE.Clients.API.Models;
using ESE.Core.Mediator;
using ESE.WebAPI.Core.Controllers;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ESE.Clients.API.Controllers
{
    public class ClientsController : MainController
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMediatorHandler _mediator;
        private readonly IAspNetUser _user;

        public ClientsController(IClientRepository clientRepository, IMediatorHandler mediator, IAspNetUser user)
        {
            _clientRepository = clientRepository;
            _mediator = mediator;
            _user = user;
        }

        [HttpGet("client/address")]
        public async Task<IActionResult> GetAddress()
        {
            var address = await _clientRepository.GetAddressById(_user.GetUserId());

            return address == null ? NotFound() : CustomResponse(address);
        }

        [HttpPost("client/address")]
        public async Task<IActionResult> AddAddress(AddAddressCommand address)
        {
            address.ClientId = _user.GetUserId();
            return CustomResponse(await _mediator.SendCommand(address));
        }
    }
}
