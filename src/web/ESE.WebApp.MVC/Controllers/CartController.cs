using ESE.WebApp.MVC.Models;
using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Controllers
{
    public class CartController : MainController
    { 
        private readonly IShoppingBffService _shoppingBffService;

        public CartController(IShoppingBffService shoppingBffService)
        {
            _shoppingBffService = shoppingBffService;
        }

        [Route("cart")]
        public async Task<IActionResult> Index()
        {
            return View(await _shoppingBffService.GetCart());
        }

        [HttpPost]
        [Route("cart/add-item")]
        public async Task<IActionResult> AddCartItem(ItemCartViewModel itemProduct)
        {
            var response = await _shoppingBffService.AddItemCart(itemProduct);

            if (HasErrorsResponse(response)) return View("Index", await _shoppingBffService.GetCart());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("cart/update-item")]
        public async Task<IActionResult> UpdateItemCart(Guid productId, int quantity)
        {
            var itemProduct = new ItemCartViewModel { ProductId = productId, Quantity = quantity };
            var response = await _shoppingBffService.UpdateItemCart(productId, itemProduct);

            if (HasErrorsResponse(response)) return View("Index", await _shoppingBffService.GetCart());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("cart/remove-item")]
        public async Task<IActionResult> RemoveItemCart(Guid productId)
        {
            var response = await _shoppingBffService.RemoveItemCart(productId);

            if (HasErrorsResponse(response)) return View("Index", await _shoppingBffService.GetCart());

            return RedirectToAction("Index");
        }
    }
}
