using ESE.Cart.API.Data;
using ESE.Cart.API.Models;
using ESE.WebAPI.Core.Controllers;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

            if (HasError()) return CustomResponse();

            var result = await _context.SaveChangesAsync();
            if (result < 1) AddProcessingError('Não foi possível salvar os dados no banco.');

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
                _context.CartItens.Update(cart.GetProdutById(item.ProductId));
            }
            else
            {
                _context.CartItens.Add(item);
            }

            _context.CartClients.Update(cart);
        }

    }
}
