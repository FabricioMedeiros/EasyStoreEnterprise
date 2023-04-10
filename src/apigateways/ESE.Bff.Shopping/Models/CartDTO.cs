using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Bff.Shopping.Models
{
    public class CartDTO
    {
        public decimal TotalPrice { get; set; }
        public VoucherDTO Voucher { get; set; }
        public bool VoucherUsed { get; set; }
        public decimal Discount { get; set; }
        public List<ItemCartDTO> Items { get; set; } = new List<ItemCartDTO>();
    }
}
