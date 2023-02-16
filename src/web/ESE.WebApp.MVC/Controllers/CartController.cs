using ESE.WebApp.MVC.Models;
using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Controllers
{
    public class CartController : MainController
    { 
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;

        public CartController(ICartService cartService, ICatalogService catalogService)
        {
            _cartService = cartService;
            _catalogService = catalogService;
        }

        [Route("cart")]
        public async Task<IActionResult> Index()
        {
            return View(await _cartService.GetCart());
        }

        [HttpPost]
        [Route("cart/add-item")]
        public async Task<IActionResult> AddCartItem(ItemProductViewModel itemProduct)
        {
            var product = await _catalogService.GetById(itemProduct.ProductId);

            ValidateItemCart(product, itemProduct.Quantity);
            if (!IsValid()) return View("Index", await _cartService.GetCart());

            itemProduct.Name = product.Name;
            itemProduct.Price = product.Price;
            itemProduct.Image = product.Image;

            var response = await _cartService.AddItemCart(itemProduct);

            if (HasErrorsResponse(response)) return View("Index", await _cartService.GetCart());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("cart/update-item")]
        public async Task<IActionResult> UpdateItemCart(Guid productId, int quantity)
        {
            var product = await _catalogService.GetById(productId);

            ValidateItemCart(product, quantity);
            if (!IsValid()) return View("Index", await _cartService.GetCart());

            var itemProduct = new ItemProductViewModel { ProductId = productId, Quantity = quantity };
            var response = await _cartService.UpdateItemCart(productId, itemProduct);

            if (HasErrorsResponse(response)) return View("Index", await _cartService.GetCart());

            return RedirectToAction("Index");
        }


        [HttpPost]
        [Route("cart/remove-item")]
        public async Task<IActionResult> RemoveItemCart(Guid productId)
        {
            var product = await _catalogService.GetById(productId);

            if (product == null)
            {
                AddProcessingError("Produto inexistente!");
                return View("Index", await _cartService.GetCart());
            }

            var response = await _cartService.RemoveItemCart(productId);

            if (HasErrorsResponse(response)) return View("Index", await _cartService.GetCart());

            return RedirectToAction("Index");
        }

        private void ValidateItemCart(ProductViewModel product, int quantity)
        {
            if (product == null) AddProcessingError("Produto inexistente!");
            if (quantity < 1) AddProcessingError($"Escolha ao menos uma unidade do produto {product.Name}");
            if (quantity > product.Stock) AddProcessingError($"O produto {product.Name} possui {product.Stock} unidades em estoque, você selecionou {quantity}");
        }
    }
}
