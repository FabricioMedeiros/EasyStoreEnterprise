﻿using ESE.Payments.API.Models;
using ESE.Payments.EasyPay;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ESE.Payments.API.Facede
{
    public class PaymentCreditCardFacede : IPaymentFacade
    {
        private readonly PaymentConfig _paymentConfig;

        public PaymentCreditCardFacede(IOptions<PaymentConfig> paymentConfig)
        {
            _paymentConfig = paymentConfig.Value;
        }

        public async Task<PaymentTransaction> AuthorizePayment(Payment payment)
        {
            var easyPaySvc = new EasyPayService(_paymentConfig.DefaultApiKey,
                _paymentConfig.DefaultEncryptionKey);

            var cardHashGen = new CardHash(easyPaySvc)
            {
                CardNumber = payment.CreditCard.NumberCard,
                CardHolderName = payment.CreditCard.NameCard,
                CardExpirationDate = payment.CreditCard.MonthYearExpiry,
                CardCvv = payment.CreditCard.CVV
            };
            var cardHash = cardHashGen.Generate();

            var transaction = new Transaction(easyPaySvc)
            {
                CardHash = cardHash,
                CardNumber = payment.CreditCard.NumberCard,
                CardHolderName = payment.CreditCard.NameCard,
                CardExpirationDate = payment.CreditCard.MonthYearExpiry,
                CardCvv = payment.CreditCard.CVV,
                PaymentMethod = PaymentMethod.CreditCard,
                Amount = payment.Price
            };

            return ToPaymentTransaction(await transaction.AuthorizeCardTransaction());

        }

        public async Task<PaymentTransaction> CapturePayment(PaymentTransaction paymentTransaction)
        {
            var easyPaySvc = new EasyPayService(_paymentConfig.DefaultApiKey, _paymentConfig.DefaultEncryptionKey);

            var transaction = ToTransaction(paymentTransaction, easyPaySvc);

            return ToPaymentTransaction(await transaction.CaptureCardTransaction());
        }

        public async Task<PaymentTransaction> CancelAuthorization(PaymentTransaction paymentTransaction)
        {
            var easyPaySvc = new EasyPayService(_paymentConfig.DefaultApiKey, _paymentConfig.DefaultEncryptionKey);

            var transaction = ToTransaction(paymentTransaction, easyPaySvc);

            return ToPaymentTransaction(await transaction.CancelAuthorization());
        }
        public static PaymentTransaction ToPaymentTransaction(Transaction transaction)
        {
            return new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Status = (PaymentTransactionStatus)transaction.Status,
                TotalPrice = transaction.Amount,
                CardBrand = transaction.CardBrand,
                AuthorizeCode = transaction.AuthorizationCode,
                Cost = transaction.Cost,
                TransactionDate = transaction.TransactionDate,
                NSU = transaction.Nsu,
                TID = transaction.Tid
            };
        }
        public static Transaction ToTransaction(PaymentTransaction paymentTransaction, EasyPayService easyPayService)
        {
            return new Transaction(easyPayService)
            {
                Status = (TransactionStatus)paymentTransaction.Status,
                Amount = paymentTransaction.TotalPrice,
                CardBrand = paymentTransaction.CardBrand,
                AuthorizationCode = paymentTransaction.AuthorizeCode,
                Cost = paymentTransaction.Cost,
                Nsu = paymentTransaction.NSU,
                Tid = paymentTransaction.TID
            };
        }
    }
}
