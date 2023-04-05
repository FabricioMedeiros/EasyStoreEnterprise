using ESE.Bff.Shopping.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace ESE.Bff.Shopping.Services
{
    public interface IPaymentService
    {
    }

    public class PaymentService : Service, IPaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.PaymentUrl);
        }
    }
}