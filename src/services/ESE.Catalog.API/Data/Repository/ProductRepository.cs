using ESE.Catalog.API.Models;
using ESE.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Catalog.API.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _context;

        public ProductRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product> GetById(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GestListProductsById(string ids)
        {
            var idsGuid = ids.Split(',')
                .Select(id => (Ok: Guid.TryParse(id, out var x), Value: x));

            if (!idsGuid.All(nid => nid.Ok)) return new List<Product>();

            var idsValue = idsGuid.Select(id => id.Value);

            return await _context.Products.AsNoTracking()
                .Where(p => idsValue.Contains(p.Id) && p.Active).ToListAsync();
        }

        public void Add(Product produto)
        {
            _context.Products.Add(produto);
        }

        public void Update(Product produto)
        {
            _context.Products.Update(produto);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}
