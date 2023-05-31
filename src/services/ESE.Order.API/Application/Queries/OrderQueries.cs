using Dapper;
using ESE.Orders.API.Application.DTO;
using ESE.Orders.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Orders.API.Application.Queries
{
    public interface IOrderQueries
    {
        Task<OrderDTO> GetLastOrder(Guid clientId);
        Task<IEnumerable<OrderDTO>> GetListByClientId(Guid clientId);

        Task<OrderDTO> GetAuthorizedOrders();
    }

    public class OrderQueries : IOrderQueries
    {
        private readonly IOrderRepository _orderRepository;

        public OrderQueries(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }      

        public async Task<OrderDTO> GetLastOrder(Guid clientId)
        {
            const string sql = @"SELECT
                                P.ID AS 'PRODUCTID', P.CODE, P.VOUCHERUSED, P.DISCOUNT, P.TOTALPRICE, P.ORDERSTATUS,
                                P.STREET,P.NUMBER, P.NEIGHBORHOOD, P.ZIPCODE, P.COMPLEMENT, P.CITY, P.STATE,
                                PIT.ID AS 'PRODUCTITEMID', PIT.PRODUCTNAME, PIT.QUANTITY, PIT.IMAGE, PIT.PRICE 
                                FROM ORDERS P 
                                INNER JOIN ORDERITEMS PIT ON P.ID = PIT.ORDERID 
                                WHERE P.CLIENTID = @clientId 
                                AND P.CREATIONDATE between DATEADD(minute, -3,  GETDATE()) and DATEADD(minute, 0,  GETDATE())
                                AND P.ORDERSTATUS = 1 
                                ORDER BY P.CREATIONDATE DESC";

            var order = await _orderRepository.GetConnection().QueryAsync<dynamic>(sql, new { clientId });

            return MapOrder(order);
        }

        public async Task<IEnumerable<OrderDTO>> GetListByClientId(Guid clientId)
        {
            var orders = await _orderRepository.GetListByClientId(clientId);

            return orders.Select(OrderDTO.ToOrderDTO);
        }

        private OrderDTO MapOrder(dynamic result)
        {
            var order = new OrderDTO
            {
                Code = result[0].CODE,
                Status = result[0].ORDERSTATUS,
                TotalPrice = result[0].TOTALPRICE,
                Discount = result[0].DISCOUNT,
                VoucherUsed = result[0].VOUCHERUSED,

                OrderItems = new List<OrderItemDTO>(),
                Address = new AddressDTO
                {
                    Street = result[0].STREET,
                    Neighborhood = result[0].NEIGHBORHOOD,
                    ZipCode = result[0].ZIPCODE,
                    City = result[0].CITY,
                    Complement = result[0].COMPLEMENT,
                    State = result[0].STATE,
                    Number = result[0].NUMBER
                }
            };

            foreach (var item in result)
            {
                var orderItem = new OrderItemDTO
                {
                    Name = item.PRODUCTNAME,
                    Price = item.PRICE,
                    Quantity = item.QUANTITY,
                    Image = item.IMAGE
                };

                order.OrderItems.Add(orderItem);
            }

            return order;
        }

        public async Task<OrderDTO> GetAuthorizedOrders()
        {
            const string sql = @"SELECT 
                                P.ID as 'OrderId', P.ID, P.CLIENTID, 
                                PI.ID as 'OrderItemId', PI.ID, PI.PRODUCTID, PI.QUANTITY 
                                FROM ORDERS P 
                                INNER JOIN ORDERITEMS PI ON P.ID = PI.ORDERID 
                                WHERE P.ORDERSTATUS = 1                                
                                ORDER BY P.CREATIONDATE";

            // Utilizacao do lookup para manter o estado a cada ciclo de registro retornado
            var lookup = new Dictionary<Guid, OrderDTO>();

            await _orderRepository.GetConnection().QueryAsync<OrderDTO, OrderItemDTO, OrderDTO>(sql,
                (p, pi) =>
                {
                    if (!lookup.TryGetValue(p.Id, out var orderDTO))
                        lookup.Add(p.Id, orderDTO = p);

                    orderDTO.OrderItems ??= new List<OrderItemDTO>();
                    orderDTO.OrderItems.Add(pi);

                    return orderDTO;

                }, splitOn: "OrderId,OrderItemId");

            // Obtendo dados o lookup
            return lookup.Values.OrderBy(p => p.Date).FirstOrDefault();
        }
    }
}
