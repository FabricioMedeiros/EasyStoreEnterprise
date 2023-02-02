using ESE.Cart.API.Data;
using ESE.Cart.API.Models;
using ESE.WebAPI.Core.Controllers;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Cart.API.Controllers
{
    public class CartController : MainController
    {
        private readonly IAspNetUser _user;
        private readonly CartDbContext _context;

        public CartController(IAspNetUser user, CartDbContext context)
        {
            _user = user;
            _context = context;
        }

        [HttpGet("cart")]
        public async Task<CartClient> GetCart()
        {
            return await GetCartClient() ?? new CartClient();
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddCartItem(CartItem item)
        {
            var cart = await GetCartClient();

            if (cart == null)
               CreateCart(item);
            else
               UpdateCart(cart, item);

            ValidateCart(cart);
             
            if (HasError()) return CustomResponse();

            await SaveData();

            return CustomResponse();
        }

        [HttpPut("cart/{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid productId, CartItem item)
        {
            var cart = await GetCartClient();
            var cartItem = await GetCartItem(productId, cart, item);
            
            if (cartItem == null) return CustomResponse();

            cart.UpdateUnits(cartItem, item.Quantity);

            ValidateCart(cart);

            if (HasError()) return CustomResponse();

            _context.CartItems.Update(cartItem);
            _context.CartClients.Update(cart);

            await SaveData();
            return CustomResponse();
        }

        [HttpDelete("carrinho/{produtoId}")]
        public async Task<IActionResult> RemoveCartItem(Guid productId)
        {
            var cart = await GetCartClient();

            var cartItem = await GetCartItem(productId, cart);

            if (cartItem == null) return CustomResponse();

            if (HasError()) return CustomResponse();

            cart.RemoveItem(cartItem);

            _context.CartItems.Remove(cartItem);
            _context.CartClients.Update(cart);

            await SaveData();
            return CustomResponse();
        }

        private async Task<CartClient> GetCartClient()
        {
            return await _context.CartClients
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.ClientId == _user.GetUserId());
        }
        private void CreateCart(CartItem item)
        {
            var cart = new CartClient(_user.GetUserId());
            cart.AddItem(item);

            _context.CartClients.Add(cart);
        }
        private void UpdateCart(CartClient cart, CartItem item)
        {
            var productExists = cart.ItemExistsCart(item);

            cart.AddItem(item);

            if (productExists)
            {
                _context.CartItems.Update(cart.GetProdutById(item.ProductId));
            }
            else
            {
                _context.CartItems.Add(item);
            }

            _context.CartClients.Update(cart);
        }
        private async Task<CartItem> GetCartItem(Guid productId, CartClient cart, CartItem item = null)
        {
            if (item != null && productId != item.ProductId)
            {
                AddProcessingError("O item não corresponde ao informado");
                return null;
            }

            if (cart == null)
            {
                AddProcessingError("Carrinho não encontrado");
                return null;
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == productId);

            if (cartItem == null || !cart.ItemExistsCart(cartItem))
            {
                AddProcessingError("O item não está no carrinho");
                return null;
            }

            return cartItem;
        }
        private async Task SaveData()
        {
            var result = await _context.SaveChangesAsync();
            if (result < 1) AddProcessingError("Não foi possível salvar os dados no banco.");
        }
        private bool ValidateCart(CartClient cart)
        {
            if (cart.IsValid()) return true;

            cart.ValidationResult.Errors.ToList().ForEach(e => AddProcessingError(e.ErrorMessage));
            return false;
        }
    }
}
