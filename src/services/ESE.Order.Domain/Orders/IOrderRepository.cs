using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESE.Orders.Domain.Orders
{
    interface IOrderRepository
    {
        /* Pedido */
        Task<Order> GetById(Guid id);
        Task<IEnumerable<Order>> GestListByClientId(Guid clientId);
        void Add(Order order);
        void Update(Order order);

        /* Pedido Item */
        Task<OrderItem> GetItemById(Guid id);
        Task<OrderItem> GetItemByOrder(Guid orderId, Guid productId);
    }
}
