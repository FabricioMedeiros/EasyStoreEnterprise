using ESE.Payments.API.Models;
using System.Threading.Tasks;

namespace ESE.Payments.API.Facede
{
    interface IPaymentFacede
    {
        Task<PaymentTransaction> AuthorizePayment(Payment payment);
    }
}
