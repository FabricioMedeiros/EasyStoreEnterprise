using ESE.Core.DomainObjects;
using System;
using System.Collections.Generic;

namespace ESE.Payments.API.Models
{
    public class Payment : Entity, IAggregateRoot
    {
        public Payment()
        {
            Transactions = new List<PaymentTransaction>();
        }

        public Guid OrderId { get; set; }
        public TypePayment TypePayment { get; set; }
        public decimal Price { get; set; }

        public CreditCard CreditCard { get; set; }

        // EF Relation
        public ICollection<PaymentTransaction> Transactions { get; set; }

        public void AddTransaction(PaymentTransaction transaction)
        {
            Transactions.Add(transaction);
        }
    }
}
