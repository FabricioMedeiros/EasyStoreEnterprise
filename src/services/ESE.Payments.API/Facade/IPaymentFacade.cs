using ESE.Payments.API.Models;
using System.Threading.Tasks;

namespace ESE.Payments.API.Facede
{
    public interface IPaymentFacade
    {
        Task<PaymentTransaction> AuthorizePayment(Payment payment);
        Task<PaymentTransaction> CapturePayment(PaymentTransaction transaction);
        Task<PaymentTransaction> CancelAuthorization(PaymentTransaction transaction);
    }
}
