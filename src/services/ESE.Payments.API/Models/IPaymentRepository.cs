using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Payments.API.Models
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        void AddPayment(Payment payment);
    }
}
