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
    public class CatalogController : MainController
    {
        private readonly IProductRepository _productRepository;

        public CatalogController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("catalog/products")]
        public async Task<PagedResult<Product>> Index([FromQuery] int ps = 8, [FromQuery] int page = 1, [FromQuery] string q = null)
        {
            return await _productRepository.GetAll(ps, page, q);
        }

        [ClaimsAuthorize("Catalog","Read")]
        [HttpGet("catalog/products/{id}")]
        public async Task<Product> ProductDetails(Guid id)
        {
            return await _productRepository.GetById(id);
        }

        [HttpGet("catalog/products/list/{ids}")]
        public async Task<IEnumerable<Product>> GetListProductsById(string ids)
        {
            return await _productRepository.GestListProductsById(ids);
        }
    }
}