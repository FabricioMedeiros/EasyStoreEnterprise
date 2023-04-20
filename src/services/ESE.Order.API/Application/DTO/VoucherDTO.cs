using ESE.Order.Domain.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Order.API.Application.DTO
{
    public class VoucherDTO
    {
        public string Code { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? DiscountValue { get;  set; }
        public VoucherDiscountType DiscountType { get; set; }
    }
}
