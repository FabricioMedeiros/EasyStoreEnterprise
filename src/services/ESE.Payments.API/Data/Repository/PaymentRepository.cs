using ESE.Core.Data;
using ESE.Payments.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Payments.API.Data.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
