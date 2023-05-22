using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ESE.Bff.Shopping.Extensions;
using ESE.Bff.Shopping.Models;
using ESE.Bff.Shopping.Services;
using ESE.Core.Communication;
using Microsoft.Extensions.Options;

namespace ESE.Bff.Shopping.Services
{
    public interface IOrderService
    {
        Task<ResponseResult> FinalizeOrder(OrderDTO order);
        Task<OrderDTO> GetLastOrder();
        Task<IEnumerable<OrderDTO>> GetListByClientId();

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

        public async Task<ResponseResult> FinalizeOrder(OrderDTO order)
        {
            var orderContent = JsonSerialize(order);

            var response = await _httpClient.PostAsync("/order/", orderContent);

            if (!CheckErrorsResponse(response)) return await DeserializeObjectResponse<ResponseResult>(response);

            return ReturnOk();
        }

        public async Task<OrderDTO> GetLastOrder()
        {
            var response = await _httpClient.GetAsync("/order/last/");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<OrderDTO>(response);
        }

        public async Task<IEnumerable<OrderDTO>> GetListByClientId()
        {
            var response = await _httpClient.GetAsync("/order/list-client/");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<IEnumerable<OrderDTO>>(response);
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