using ESE.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ESE.Bff.Shopping.Controllers
{
    public class CartController : Controller
    {
        [Authorize]
        public class CarrinhoController : MainController
        {
            [HttpGet]
            [Route("shopping/cart")]
            public async Task<IActionResult> Index()
            {
                return CustomResponse();
            }

            [HttpGet]
            [Route("shopping/quantity-cart")]
            public async Task<IActionResult> GetQuantityCart()
            {
                return CustomResponse();
            }

            [HttpPost]
            [Route("shopping/cart/items")]
            public async Task<IActionResult> AddItemCart()
            {
                return CustomResponse();
            }

            [HttpPut]
            [Route("shopping/cart/items/{productId}")]
            public async Task<IActionResult> UpdateItemCart()
            {
                return CustomResponse();
            }

            [HttpDelete]
            [Route("shopping/cart/items/{productId}")]
            public async Task<IActionResult> DeleteItemCart()
            {
                return CustomResponse();
            }
        }
    }
}
