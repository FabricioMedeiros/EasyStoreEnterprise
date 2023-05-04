﻿using ESE.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace ESE.Orders.Domain.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        /* Pedido */
        Task<Order> GetById(Guid id);
        Task<IEnumerable<Order>> GetListByClientId(Guid clientId);
        void Add(Order order);
        void Update(Order order);

        DbConnection GetConnection();

        /* Pedido Item */
        Task<OrderItem> GetItemById(Guid id);
        Task<OrderItem> GetItemByOrder(Guid orderId, Guid productId);
    }
}
