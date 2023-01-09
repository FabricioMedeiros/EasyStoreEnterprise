using ESE.Clients.API.Application.Commands;
using ESE.Core.Mediator;
using ESE.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ESE.Clients.API.Controllers
{
    public class ClientsController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;

        public ClientsController(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
        }

        [HttpGet("clients")]
        public async Task<IActionResult> IndexAsync()
        {
            var result = await _mediatorHandler.SendCommand(
                 new RegisterClientCommand(Guid.NewGuid(), "Fabricio - Teste", "fa@yahoo.com.br", "30314299076"));

            return CustomResponse(result);
        }
    }
}
