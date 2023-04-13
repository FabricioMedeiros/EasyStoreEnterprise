using ESE.WebApp.MVC.Models;
using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Extensions
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IShoppingBffService _shoppingBffService;

        public CartViewComponent(IShoppingBffService shoppingBffService)
        {
            _shoppingBffService = shoppingBffService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _shoppingBffService.GetQuantityCart());
        }
    }
}
