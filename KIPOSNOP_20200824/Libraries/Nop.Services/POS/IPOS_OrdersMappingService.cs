using Nop.Core.Domain.POS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.POS
{
    public interface IPOS_OrdersMappingService
    {
        /// <summary>
        /// Inserts Item into FOrderItems
        /// </summary>
        /// <param name="POS_OrdersMapping">POS_OrdersMapping</param>
        void InsertPOSOrder(POS_OrdersMapping pOS_OrdersMapping);

        POS_OrdersMapping GetPOSOrderByOrderId(int OrderId);
    }
}
