using System;
using System.Net.Http;
using ESE.Bff.Shopping.Extensions;
using Microsoft.Extensions.Options;

namespace ESE.Bff.Shopping.Services
{
    public interface ICartService
    {
    }

    public class CartService : Service, ICartService
    {
        private readonly HttpClient _httpClient;

        public CartService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CartUrl);
        }
    }
}