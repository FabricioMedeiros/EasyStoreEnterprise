using ESE.Order.API.Application.DTO;
using ESE.Order.Domain.Vouchers;
using System.Threading.Tasks;

namespace ESE.Order.API.Application.Queries
{
    public interface IVoucherQueries
    {
        Task<VoucherDTO> GetVoucherByCode(string code);
    }

    public class VoucherQueries : IVoucherQueries
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherQueries(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<VoucherDTO> GetVoucherByCode(string code)
        {
            var voucher = await _voucherRepository.GetVoucherByCode(code);

            if (voucher == null) return null;

            if (!voucher.IsValidToUse()) return null;

            return new VoucherDTO
            {
                Code = voucher.Code,
                DiscountType = (VoucherDiscountType)(int)voucher.DiscountType,
                Percentage = voucher.Percentage,
                DiscountValue = voucher.DiscountValue
            };
        }
    }

}
