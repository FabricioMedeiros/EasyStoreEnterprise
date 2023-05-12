using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Controllers
{
    public class OrderController : MainController
    {
        private readonly IClientService _clientService;
        private readonly IShoppingBffService _shoppingBffService;

        public OrderController(IClientService clientService, IShoppingBffService shoppingBffService)
        {
            _clientService = clientService;
            _shoppingBffService = shoppingBffService;
        }

        [HttpGet]
        [Route("delivery-address")]
        public async Task<IActionResult> DeliveryAddress()
        {
            var cart = await _shoppingBffService.GetCart();
            if (cart.Items.Count == 0) return RedirectToAction("Index", "Cart");

            var address = await _clientService.GetAddress();
            var order = _shoppingBffService.MapToOrder(cart, address);

            return View(order);
        }
    }
}
