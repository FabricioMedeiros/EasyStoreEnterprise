using ESE.WebApp.MVC.Models;
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

        [HttpGet]
        [Route("payment")]
        public async Task<IActionResult> Pagamento()
        {
            var cart = await _shoppingBffService.GetCart();
            if (cart.Items.Count == 0) return RedirectToAction("Index", "Cart");

            var order = _shoppingBffService.MapToOrder(cart, null);

            return View(order);
        }

        [HttpPost]
        [Route("checkout-order")]
        public async Task<IActionResult> FinalizarPedido(OrderTransactionViewModel orderTransaction)
        {
            if (!ModelState.IsValid) 
              return View("Payment", _shoppingBffService.MapToOrder( await _shoppingBffService.GetCart(), null));

            var response = await _shoppingBffService.CheckoutOrder(orderTransaction);

            if (HasErrorsResponse(response))
            {
                var cart = await _shoppingBffService.GetCart();
                if (cart.Items.Count == 0) return RedirectToAction("Index", "Cart");

                var orderMap = _shoppingBffService.MapToOrder(cart, null);
                return View("Payment", orderMap);
            }

            return RedirectToAction("OrderCompleted");
        }

        [HttpGet]
        [Route("order-completed")]
        public async Task<IActionResult> OrderCompleted()
        {
            return View("ConfirmationOrder", await _shoppingBffService.GetLastOrder());
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            return View(await _shoppingBffService.GetListByClientId());
        }
    }
}
