using ESE.WebApp.MVC.Models;
using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Controllers
{
    [Authorize]
    public class ClienteController : MainController
    {
        private readonly IClientService _clientService;

        public ClienteController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> NewAddress(AddressViewModel address)
        {
            var response = await _clientService.AddAddress(address);

            if (HasErrorsResponse(response)) TempData["Errors"] =
                ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

            return RedirectToAction("DeliveryAddress", "Order");
        }
    }
}
