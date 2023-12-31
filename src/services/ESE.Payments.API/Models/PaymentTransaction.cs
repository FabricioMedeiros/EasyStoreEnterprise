﻿using ESE.Core.DomainObjects;
using System;

namespace ESE.Payments.API.Models
{
    public class PaymentTransaction : Entity
    {
        public string AuthorizeCode { get; set; }
        public string CardBrand { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Cost { get; set; }
        public PaymentTransactionStatus Status { get; set; }
        public string TID { get; set; } // Id
        public string NSU { get; set; } // Meio (paypal)

        public Guid PaymentId { get; set; }

        // EF Relation
        public Payment Payment { get; set; }
    }
}
