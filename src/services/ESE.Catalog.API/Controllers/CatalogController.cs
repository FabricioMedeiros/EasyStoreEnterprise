using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESE.Catalog.API.Models;
using ESE.WebAPI.Core.Authentication;
using ESE.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ESE.Catalog.API.Controllers
{
    [Authorize]
    public class CatalogController : MainController
    {
        private readonly IProductRepository _productRepository;

        public CatalogController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [AllowAnonymous]
        [HttpGet("catalog/products")]
        public async Task<IEnumerable<Product>> Index()
        {
            return await _productRepository.GetAll();
        }

        [ClaimsAuthorize("Catalog","Read")]
        [HttpGet("catalog/products/{id}")]
        public async Task<Product> ProductDetails(Guid id)
        {
            return await _productRepository.GetById(id);
        }
    }
}