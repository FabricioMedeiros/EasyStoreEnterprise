using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESE.Catalog.API.Models
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedResult<Product>> GetAll(int pageSize, int pageIndex, string query = null);
        Task<Product> GetById(Guid id);
        Task<List<Product>> GestListProductsById(string ids);

        void Add(Product product);
        void Update(Product product);
    }
}
