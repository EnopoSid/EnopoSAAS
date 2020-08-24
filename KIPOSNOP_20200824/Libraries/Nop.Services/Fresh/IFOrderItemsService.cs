using Nop.Core.Domain.Fresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Fresh
{
    public partial interface IFOrderItemsService
    {

        /// <summary>
        /// Inserts Item into FOrderItems
        /// </summary>
        /// <param name="fOrderItems">FOrderItems</param>
        void InsertFreshCart(FOrderItems fOrderItems);

        IList<FOrderItems> GetFOrderItemsByOrderId(int OrderId);
    }
}
