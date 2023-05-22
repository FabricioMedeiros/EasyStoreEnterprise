using ESE.Core.Mediator;
using ESE.Orders.API.Application.Commands;
using ESE.Orders.API.Application.Queries;
using ESE.WebAPI.Core.Controllers;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ESE.Orders.API.Controllers
{
    [Authorize]
    public class OrderController : MainController
    {
        private readonly IMediatorHandler _mediator;
        private readonly IAspNetUser _user;
        private readonly IOrderQueries _orderQueries;

        public OrderController(IMediatorHandler mediator,
            IAspNetUser user,
            IOrderQueries orderQueries)
        {
            _mediator = mediator;
            _user = user;
            _orderQueries = orderQueries;
        }

        [HttpPost("order")]
        public async Task<IActionResult> AddOrder(AddOrderCommand order)
        {
            order.ClientId = _user.GetUserId();
            return CustomResponse(await _mediator.SendCommand(order));
        }

        [HttpGet("order/last")]
        public async Task<IActionResult> GetLastOrder()
        {
            var order = await _orderQueries.GetLastOrder(_user.GetUserId());

            return order == null ? NotFound() : CustomResponse(order);
        }

        [HttpGet("order/list-client")]
        public async Task<IActionResult> ListByClient()
        {
            var orders = await _orderQueries.GetListByClientId(_user.GetUserId());

            return orders == null ? NotFound() : CustomResponse(orders);
        }
    }
}
