using System;
using System.Net.Http;
using System.Threading.Tasks;
using ESE.Bff.Shopping.Extensions;
using ESE.Bff.Shopping.Models;
using ESE.Core.Communication;
using Microsoft.Extensions.Options;

namespace ESE.Bff.Shopping.Services
{
    public interface ICartService
    {
        Task<CartDTO> GetCart();
        Task<ResponseResult> AddItemCart(ItemCartDTO product);
        Task<ResponseResult> UpdateItemCart(Guid productId, ItemCartDTO product);
        Task<ResponseResult> RemoveItemCart(Guid productId);
    }

    public class CartService : Service, ICartService
    {
        private readonly HttpClient _httpClient;

        public CartService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CartUrl);
        }

        public async Task<CartDTO> GetCart()
        {
            var response = await _httpClient.GetAsync("/cart/");

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<CartDTO>(response);
        }

        public async Task<ResponseResult> AddItemCart(ItemCartDTO product)
        {
            var itemContent = JsonSerialize(product);

            var response = await _httpClient.PostAsync("/cart/", itemContent);

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }

        public async Task<ResponseResult> UpdateItemCart(Guid produtoId, ItemCartDTO product)
        {
            var itemContent = JsonSerialize(product);

            var response = await _httpClient.PutAsync($"/cart/{product.ProductId}", itemContent);

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }

        public async Task<ResponseResult> RemoveItemCart(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"/cart/{productId}");

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }

        
    }
}