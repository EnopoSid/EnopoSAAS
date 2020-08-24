using Nop.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Common
{
    public partial interface IOrder_Pickup_CustDetailsService
    {
        /// <summary>
        /// Inserts Item into Order_Pickup_CustDetails
        /// </summary>
        /// <param name="details">Order_Pickup_CustDetails</param>
        void InsertPickUpDetails(Order_Pickup_CustDetails details);

        /// <summary>
        /// Gets Order_Pickup_CustDetails by orderId
        /// </summary>
        /// <param name="orderId">orderId</param>
        Order_Pickup_CustDetails GetCustomerDetails(int orderId);
    }
}
