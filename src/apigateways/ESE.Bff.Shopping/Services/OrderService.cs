using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ESE.Bff.Shopping.Extensions;
using ESE.Bff.Shopping.Models;
using ESE.Bff.Shopping.Services;
using Microsoft.Extensions.Options;

namespace ESE.Bff.Shopping.Services
{
    public interface IOrderService
    {
        Task<VoucherDTO> GetVoucherByCode(string code);
    }

    public class OrderService : Service, IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.OrderUrl);
        }

        public async Task<VoucherDTO> GetVoucherByCode(string code)
        {
            var response = await _httpClient.GetAsync($"/voucher/{code}/");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<VoucherDTO>(response);
        }
    }
}