using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESE.Order.Domain.Vouchers
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<Voucher> GetVoucherById(string code);
    }
}
