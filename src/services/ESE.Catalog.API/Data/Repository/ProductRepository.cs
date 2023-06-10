using Dapper;
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

        public async Task<PagedResult<Product>> GetAll(int pageSize, int pageIndex, string query = null)
        {
            var sql = @$"SELECT * FROM PRODUCTS 
                      WHERE (@Name IS NULL OR Name LIKE '%' + @Name + '%') 
                      ORDER BY [Name] 
                      OFFSET {pageSize * (pageIndex - 1)} ROWS 
                      FETCH NEXT {pageSize} ROWS ONLY 
                      SELECT COUNT(Id) FROM PRODUCTS 
                      WHERE (@Name IS NULL OR Name LIKE '%' + @Name + '%')";

            var multi = await _context.Database.GetDbConnection()
                .QueryMultipleAsync(sql, new { Name = query });

            var products = multi.Read<Product>();
            var total = multi.Read<int>().FirstOrDefault();

            return new PagedResult<Product>()
            {
                List = products,
                TotalResults = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query
            };
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
