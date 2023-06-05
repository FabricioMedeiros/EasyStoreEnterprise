using ESE.Core.DomainObjects;
using ESE.Core.Messages.Integration;
using ESE.Payments.API.Facede;
using ESE.Payments.API.Models;
using FluentValidation.Results;
using System;
using System.Linq;
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

        public async Task<ResponseMessage> CapturePayment(Guid orderId)
        {
            var transactions = await _paymentRepository.GetTransactionsByOrderId(orderId);
            var transactionAuthorized = transactions?.FirstOrDefault(t => t.Status == PaymentTransactionStatus.Authorized);
            var validationResult = new ValidationResult();

            if (transactionAuthorized == null) throw new DomainException($"Transação não encontrada para o pedido {orderId}");

            var transaction = await _paymentFacade.CapturePayment(transactionAuthorized);

            if (transaction.Status != PaymentTransactionStatus.Paid)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível capturar o pagamento do pedido {orderId}"));

                return new ResponseMessage(validationResult);
            }

            transaction.PaymentId = transactionAuthorized.PaymentId;
            _paymentRepository.AddTransaction(transaction);

            if (!await _paymentRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível persistir a captura do pagamento do pedido {orderId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CancelPayment(Guid orderId)
        {
            var transacoes = await _paymentRepository.GetTransactionsByOrderId(orderId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == PaymentTransactionStatus.Authorized);
            var validationResult = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para o pedido {orderId}");

            var transacao = await _paymentFacade.CancelAuthorization(transacaoAutorizada);

            if (transacao.Status != PaymentTransactionStatus.Cancelled)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível cancelar o pagamento do pedido {orderId}"));

                return new ResponseMessage(validationResult);
            }

            transacao.PaymentId = transacaoAutorizada.PaymentId;
            _paymentRepository.AddTransaction(transacao);

            if (!await _paymentRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível persistir o cancelamento do pagamento do pedido {orderId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }
    }
}
