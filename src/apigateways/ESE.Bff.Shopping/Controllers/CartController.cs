using ESE.Bff.Shopping.Models;
using ESE.Bff.Shopping.Services;
using ESE.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Bff.Shopping.Controllers
{

    [Authorize]
    public class CartController : MainController
    {
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, ICatalogService catalogService)
        {
            _cartService = cartService;
            _catalogService = catalogService;
        }

        [HttpGet]
        [Route("/shopping/cart")]
        public async Task<IActionResult> Index()
        {
            return CustomResponse(await _cartService.GetCart());
        }

        [HttpGet]
        [Route("/shopping/cart/quantity-cart")]
        public async Task<int> GetQuantityCart()
        {
            var quantity = await _cartService.GetCart();
            return quantity?.Items.Sum(i => i.Quantity) ?? 0;
        }

        [HttpPost]
        [Route("/shopping/cart/items")]
        public async Task<IActionResult> AddItemCart(ItemCartDTO itemProduct)
        {
            var product = await _catalogService.GetById(itemProduct.ProductId);

            await ValidateItemCart(product, itemProduct.Quantity, true);
           
            if (HasError()) return CustomResponse();

            itemProduct.Name = product.Name;
            itemProduct.Price = product.Price;
            itemProduct.Image = product.Image;

            var response = await _cartService.AddItemCart(itemProduct);

            return CustomResponse(response);
        }

        [HttpPut]
        [Route("/shopping/cart/items/{productId}")]
        public async Task<IActionResult> UpdateItemCart(Guid productId, ItemCartDTO itemProduct)
        {
            var product = await _catalogService.GetById(productId);

            await ValidateItemCart(product, itemProduct.Quantity);
            
            if (HasError()) return CustomResponse();

            var response = await _cartService.UpdateItemCart(productId, itemProduct);

            return CustomResponse(response);
        }

        [HttpDelete]
        [Route("/shopping/cart/items/{productId}")]
        public async Task<IActionResult> DeleteItemCart(Guid productId)
        {
            var product = await _catalogService.GetById(productId);

            if (product == null)
            {
                AddProcessingError("Produto inexistente!");
                return CustomResponse();
            }

            var response = await _cartService.RemoveItemCart(productId);

            return CustomResponse(response);
        }

        [HttpPost]
        [Route("shopping/cart/apply-voucher")]
        public async Task<IActionResult> ApplyVoucher([FromBody] string voucherCode)
        {
            var voucher = await _orderService.GetVoucherByCode(voucherCode);
            if (voucher is null)
            {
                AddProcessingError("Voucher inválido ou não encontrado!");
                return CustomResponse();
            }

            var response = await _cartService.ApplyVoucherCart(voucher);

            return CustomResponse(response);
        }

        private async Task ValidateItemCart(ItemProductDTO product, int quantity, bool addProduct = false)
        {
            if (product == null) AddProcessingError("Produto inexistente!");
            if (quantity < 1) AddProcessingError($"Escolha ao menos uma unidade do produto {product.Name}");

            var carrinho = await _cartService.GetCart();
            var itemCarrinho = carrinho.Items.FirstOrDefault(p => p.ProductId == product.Id);

            if (itemCarrinho != null && addProduct && itemCarrinho.Quantity + quantity > product.Stock)
            {
                AddProcessingError($"O produto {product.Name} possui {product.Stock} unidades em estoque, você selecionou {quantity}");
                return;
            }

            if (quantity > product.Stock) AddProcessingError($"O produto {product.Name} possui {product.Stock} unidades em estoque, você selecionou {quantity}");
        }
    }
}

