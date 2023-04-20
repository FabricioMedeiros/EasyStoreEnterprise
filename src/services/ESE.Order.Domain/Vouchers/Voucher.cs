using ESE.Core.DomainObjects;
using ESE.Order.Domain.Vouchers.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESE.Order.Domain.Vouchers
{
    public class Voucher : Entity, IAggregateRoot
    {
        public string Code { get; private set; }
        public decimal? Percentage { get; private set; }
        public decimal? DiscountValue { get; private set; }
        public int Quantity { get; private set; }
        public VoucherDiscountType DiscountType { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? UtilizationDate { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public bool Active { get; private set; }
        public bool Used { get; private set; }

        public bool IsValidToUse()
        {
            return new VoucherActiveSpecification()
                .And(new VoucherDateSpecification())
                .And(new VoucherQuantitySpecification())
                .IsSatisfiedBy(this);
        }
        public void SetUsed()
        {
            Active = false;
            Used = true;
            Quantity = 0;
            UtilizationDate = DateTime.Now;
        }

        public void DecrementQuantity()
        {
            Quantity -= 1;
            if (Quantity >= 1) return;

            SetUsed();
        }
    }
}
