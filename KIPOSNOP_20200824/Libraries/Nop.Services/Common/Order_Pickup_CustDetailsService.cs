using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Common
{
    public partial class Order_Pickup_CustDetailsService: IOrder_Pickup_CustDetailsService
    {
        private readonly IRepository<Order_Pickup_CustDetails> _order_Pickup_CustDetailsRepository = EngineContext.Current.Resolve<IRepository<Order_Pickup_CustDetails>>();

        public Order_Pickup_CustDetailsService()
        {

        }

        /// <summary>
        /// Inserts Item into Order_Pickup_CustDetails
        /// </summary>
        /// <param name="details">Order_Pickup_CustDetails</param>
        public virtual void InsertPickUpDetails(Order_Pickup_CustDetails details)
        {
            if (details == null)
                throw new ArgumentNullException(nameof(Order_Pickup_CustDetails));

            //if (freshCart is IEntityForCaching)
            //    throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _order_Pickup_CustDetailsRepository.Insert(details);

        }

        /// <summary>
        /// Gets Order_Pickup_CustDetails by orderId
        /// </summary>
        /// <param name="orderId">orderId</param>
        public virtual Order_Pickup_CustDetails GetCustomerDetails(int orderId)
        {
            if (orderId == 0)
                throw new ArgumentNullException(nameof(Order_Pickup_CustDetails));

            //if (freshCart is IEntityForCaching)
            //    throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            var query = _order_Pickup_CustDetailsRepository.Table.Where(x => x.OrderId == orderId && x.IsActive == true).FirstOrDefault();

              return query;

        }
    }
}
