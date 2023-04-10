using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Bff.Shopping.Models
{
    public class VoucherDTO
    {
        public decimal? Percent { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Code { get; set; }
        public int TypeDiscount { get; set; }
    }
}
