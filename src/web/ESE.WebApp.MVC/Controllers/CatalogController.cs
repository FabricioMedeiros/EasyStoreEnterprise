using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Controllers
{
    public class CatalogController : MainController
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        [Route("")]
        [Route("products")]
        public async Task<IActionResult> Index()
        {
            var products = await _catalogService.GetAll();

            return View(products);
        }

        [HttpGet]
        [Route("product-details/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            var product = await _catalogService.GetById(id);

            return View(product);
        }
    }
}
