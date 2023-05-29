using ESE.Core.Messages.Integration;
using ESE.Payments.API.Facede;
using ESE.Payments.API.Models;
using FluentValidation.Results;
using System.Threading.Tasks;

namespace ESE.Payments.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentFacade _paymentFacade;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentFacade paymentFacade, IPaymentRepository paymentRepository)
        {
            _paymentFacade = paymentFacade;
            _paymentRepository = paymentRepository;
        }

        public async Task<ResponseMessage> AuthorizePayment(Payment payment)
        {
            var transaction = await _paymentFacade.AuthorizePayment(payment);
            var validationResult = new ValidationResult();

            if (transaction.Status != PaymentTransactionStatus.Authorized)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                        "Pagamento recusado, entre em contato com a sua operadora de cartão"));

                return new ResponseMessage(validationResult);
            }

            payment.AddTransaction(transaction);
            _paymentRepository.AddPayment(payment);

            if (!await _paymentRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    "Houve um erro ao realizar o pagamento."));        

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }
    }
}
