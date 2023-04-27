using ESE.Core.Communication;
using ESE.WebApp.MVC.Extensions;
using ESE.WebApp.MVC.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace ESE.WebApp.MVC.Services
{
    public interface IShoppingBffService
    {
        Task<CartViewModel> GetCart();

        Task<int> GetQuantityCart();
        Task<ResponseResult> AddItemCart(ItemCartViewModel product);
        Task<ResponseResult> UpdateItemCart(Guid productId, ItemCartViewModel product);
        Task<ResponseResult> RemoveItemCart(Guid productId);

        Task<ResponseResult> ApplyVoucherCart(string voucher);
    }
    public class ShoppingBffService : Service, IShoppingBffService
    {
        private readonly HttpClient _httpClient;
        public ShoppingBffService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.ShoppingBffUrl);
        }

        public async Task<CartViewModel> GetCart()
        {
            var response = await _httpClient.GetAsync("/shopping/cart");

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<CartViewModel>(response);
        }

        public async Task<int> GetQuantityCart()
        {
            var response = await _httpClient.GetAsync("/shopping/cart/quantity-cart");

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<int>(response);
        }

        public async Task<ResponseResult> AddItemCart(ItemCartViewModel product)
        {
            var itemContent = JsonSerialize(product);

            var response = await _httpClient.PostAsync("/shopping/cart/items", itemContent);

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }

        public async Task<ResponseResult> UpdateItemCart(Guid productId, ItemCartViewModel product)
        {
           var itemContent = JsonSerialize(product);

           var response = await _httpClient.PutAsync($"/shopping/cart/items/{productId}", itemContent);

           if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

           return ReturnOk();
        }

        public async Task<ResponseResult> RemoveItemCart(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"/shopping/cart/items/{productId}");

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }

        public async Task<ResponseResult> ApplyVoucherCart(string voucher)
        {
            var itemContent = JsonSerialize(voucher);

            var response = await _httpClient.PostAsync("/shopping/cart/apply-voucher/", itemContent);

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }
    }
}
