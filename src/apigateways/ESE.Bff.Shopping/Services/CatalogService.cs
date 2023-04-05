using ESE.Bff.Shopping.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace ESE.Bff.Shopping.Services
{
    public interface ICatalogService
    {
    }

    public class CatalogService : Service, ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CatalogUrl);
        }
    }
}