using System;
using System.Net.Http;
using ESE.Bff.Shopping.Extensions;
using ESE.Bff.Shopping.Services;
using Microsoft.Extensions.Options;

namespace ESE.Bff.Shopping.Services
{
    public interface IOrderService
    {
    }

    public class OrderService : Service, IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.OrderUrl);
        }
    }
}