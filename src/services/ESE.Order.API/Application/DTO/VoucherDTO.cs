using ESE.Orders.Domain.Vouchers;

namespace ESE.Orders.API.Application.DTO
{
    public class VoucherDTO
    {
        public string Code { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? DiscountValue { get;  set; }
        public VoucherDiscountType DiscountType { get; set; }
    }
}
