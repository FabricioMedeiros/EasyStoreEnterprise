using ESE.Core.DomainObjects;
using ESE.Orders.Domain.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESE.Orders.Domain.Orders
{
    public class Order : Entity, IAggregateRoot
    {
        public int Code { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid? VoucherId { get; private set; }
        public bool VoucherUsed { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalPrice { get; private set; }
        public DateTime CreationDate { get; private set; }
        public OrderStatus OrderStatus { get; private set; }

        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
        public Address Address { get; private set; }
        // EF Rel.
        public Voucher Voucher { get; private set; }

        public Order(Guid clientId, decimal totalPrice, List<OrderItem> orderItems,  bool voucherUsed = false, decimal discount = 0, Guid? voucherId = null)
        {
            ClientId = clientId;
            TotalPrice = totalPrice;
            _orderItems = orderItems;

            Discount = discount;
            VoucherUsed = voucherUsed;
            VoucherId = voucherId;
        }

        // EF ctor
        protected Order() { }

        public void AuthorizeOrder()
        {
            OrderStatus = OrderStatus.Authorized;
        }
        public void SetAuthorizedOrder()
        {
            OrderStatus = OrderStatus.Authorized;
        }
        public void SetCanceledOrder()
        {
            OrderStatus = OrderStatus.Canceled;
        }

        public void SetPaidOrder()
        {
            OrderStatus = OrderStatus.Paid;
        }

        public void SetVoucher(Voucher voucher)
        {
            VoucherUsed = true;
            VoucherId = voucher.Id;
            Voucher = voucher;
        }

        public void SetAddress(Address address)
        {
            Address = address;
        }

        public void CalculateOrderPrice()
        {
            TotalPrice = OrderItems.Sum(p => p.CalculateTotalPrice());
            CalculateTotalDiscoun();
        }

        public void CalculateTotalDiscoun()
        {
            if (!VoucherUsed) return;

            decimal discount = 0;
            var price = TotalPrice;

            if (Voucher.DiscountType == VoucherDiscountType.Percentage)
            {
                if (Voucher.Percentage.HasValue)
                {
                    discount = (price * Voucher.Percentage.Value) / 100;
                    price -= discount;
                }
            }
            else
            {
                if (Voucher.DiscountValue.HasValue)
                {
                    discount = Voucher.DiscountValue.Value;
                    price -= discount;
                }
            }

            TotalPrice = price < 0 ? 0 : price;
            Discount = discount;
        }
    }
}
