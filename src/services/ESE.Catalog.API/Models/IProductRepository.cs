using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESE.Catalog.API.Models
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetById(Guid id);
        Task<List<Product>> GestListProductsById(string ids);

        void Add(Product product);
        void Update(Product product);
    }
}
