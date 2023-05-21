using ESE.Bff.Shopping.Extensions;
using ESE.Bff.Shopping.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ESE.Bff.Shopping.Services
{
    public interface ICatalogService
    {
        Task<ItemProductDTO> GetById(Guid id);
        Task<IEnumerable<ItemProductDTO>> GetItems(IEnumerable<Guid> ids);
    }

    public class CatalogService : Service, ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CatalogUrl);
        }

        public async Task<ItemProductDTO> GetById(Guid id)
        {
            var response = await _httpClient.GetAsync($"/catalog/products/{id}");

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<ItemProductDTO>(response);
        }

        public async Task<IEnumerable<ItemProductDTO>> GetItems(IEnumerable<Guid> ids)
        {
            var idsRequest = string.Join(",", ids);

            var response = await _httpClient.GetAsync($"/catalog/products/list/{idsRequest}/");

            CheckErrorsResponse(response);

            return await DeserializeObjectResponse<IEnumerable<ItemProductDTO>>(response);
        }
    }
}