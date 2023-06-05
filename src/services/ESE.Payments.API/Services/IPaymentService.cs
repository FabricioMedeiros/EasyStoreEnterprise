using ESE.Core.Messages.Integration;
using ESE.Payments.API.Models;
using System;
using System.Threading.Tasks;

namespace ESE.Payments.API.Services
{
    public interface IPaymentService
    {
        Task<ResponseMessage> AuthorizePayment(Payment payment);
        Task<ResponseMessage> CapturePayment(Guid orderId);
        Task<ResponseMessage> CancelPayment(Guid orderId);
    }
}
