using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESE.Payments.API.Models
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        void AddPayment(Payment payment);
        void AddTransaction(PaymentTransaction paymentTransaction);
        Task<Payment> GetPaymentByOrderId(Guid orderId);
        Task<IEnumerable<PaymentTransaction>> GetTransactionsByOrderId(Guid orderId);
    }
}
