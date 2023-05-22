using ESE.Core.Messages;
using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using ESE.Orders.API.Application.DTO;
using ESE.Orders.API.Application.Events;
using ESE.Orders.Domain.Orders;
using ESE.Orders.Domain.Vouchers;
using ESE.Orders.Domain.Vouchers.Specs;
using FluentValidation.Results;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Orders.API.Application.Commands
{
    public class OrderCommandHandler : CommandHandler,
        IRequestHandler<AddOrderCommand, ValidationResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMessageBus _bus;

        public OrderCommandHandler(IOrderRepository orderRepository, IVoucherRepository voucherRepository, IMessageBus bus)
        {
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _bus = bus;
        }

        public async Task<ValidationResult> Handle(AddOrderCommand message, CancellationToken cancellationToken)
        {
            // Validação do comando
            if (!message.IsValid()) return message.ValidationResult;

            // Mapear Pedido
            var order = MapOrder(message);

            // Aplicar voucher se houver
            if (!await ApplyVoucher(message, order)) return ValidationResult;

            // Validar pedido
            if (!ValidateOrder(order)) return ValidationResult;

            // Processar pagamento
            if (!await ProcessPayment(order, message)) return ValidationResult;

            // Se pagamento tudo ok!
            order.AuthorizeOrder();

            // Adicionar Evento
            order.AddEvent(new OrderRegisteredEvent(order.Id, order.ClientId));

            // Adicionar Pedido Repositorio
            _orderRepository.Add(order);

            // Persistir dados de pedido e voucher
            return await SaveData(_orderRepository.UnitOfWork);
        }

        private Order MapOrder(AddOrderCommand message)
        {
            var address = new Address
            {
                Street = message.Address.Street,
                Number = message.Address.Number,
                Complement = message.Address.Complement,
                Neighborhood = message.Address.Neighborhood,
                ZipCode = message.Address.ZipCode,
                City = message.Address.City,
                State = message.Address.State
            };

            var order = new Order(message.ClientId, 
                message.TotalPrice, 
                message.OrderItems.Select(OrderItemDTO.ToOrderItem).ToList(),
                message.VoucherUsed, 
                message.Discount);

            order.SetAddress(address);
            return order;
        }

        private async Task<bool> ApplyVoucher(AddOrderCommand message, Order order)
        {
            if (!message.VoucherUsed) return true;

            var voucher = await _voucherRepository.GetVoucherByCode(message.VoucherCode);

            if (voucher == null)
            {
                AddError("O voucher informado não existe!");
                return false;
            }

            var voucherValidation = new VoucherValidation().Validate(voucher);

            if (!voucherValidation.IsValid)
            {
                voucherValidation.Errors.ToList().ForEach(m => AddError(m.ErrorMessage));
                return false;
            }

            order.SetVoucher(voucher);
            voucher.DecrementQuantity();

            _voucherRepository.Update(voucher);

            return true;
        }

        private bool ValidateOrder(Order order)
        {
            var orderOriginalValue = order.TotalPrice;
            var orderDiscount = order.Discount;

            order.CalculateOrderPrice();

            if (order.TotalPrice != orderOriginalValue)
            {
                AddError("O valor total do pedido não confere com o cálculo do pedido");
                return false;
            }

            if (order.Discount != orderDiscount)
            {
                AddError("O valor total não confere com o cálculo do pedido");
                return false;
            }

            return true;
        }

        public async Task<bool> ProcessPayment(Order order, AddOrderCommand message)
        {
            var orderStarted = new OrderStartedIntegrationEvent
            {
                OrderId = order.Id,
                ClientId = order.ClientId,
                Price = order.TotalPrice,
                TypePayment = 1, // fixo. Alterar se tiver mais tipos
                NameCard = message.NameCard,
                NumberCard = message.NumberCard,
                MonthYearExpiry = message.ExpirationCard,
                CVV = message.CvvCard
            };

            var result = await _bus.RequestAsync<OrderStartedIntegrationEvent, ResponseMessage>(orderStarted);

            if (result.ValidationResult.IsValid) return true;

            foreach (var error in result.ValidationResult.Errors)
            {
                AddError(error.ErrorMessage);
            }

            return false;
        }
    }
}
