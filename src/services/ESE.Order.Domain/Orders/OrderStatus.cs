using System;
using System.Collections.Generic;
using System.Text;

namespace ESE.Order.Domain.Orders
{
    public enum OrderStatus
    {
        Authorized = 1,
        Paid = 2,
        Declined = 3,
        Delivered = 4,
        Canceled = 5
    }
}
