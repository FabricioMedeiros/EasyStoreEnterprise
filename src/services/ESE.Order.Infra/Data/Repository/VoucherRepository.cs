using ESE.Core.Data;
using ESE.Order.Domain.Vouchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESE.Order.Infra.Data.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly OrderDbContext _context;

        public VoucherRepository(OrderDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Voucher> GetVoucherById(string code)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(p => p.Code == code);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
