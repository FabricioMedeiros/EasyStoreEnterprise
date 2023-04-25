using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Cart.API.Models
{
    public class Voucher
    {
        public decimal? Percent { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Code { get; set; }
        public TypeDiscountVoucher TypeDiscount { get; set; }
    }

    public enum TypeDiscountVoucher
    {
        Percentage = 0,
        Value = 1
    }
}
