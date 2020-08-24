using Nop.Core.Data;
using Nop.Core.Domain.Fresh;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nop.Services.Fresh
{
    public partial class FOrderItemsService: IFOrderItemsService
    {
        private readonly IRepository<FOrderItems> _fOrderItemsRepository = EngineContext.Current.Resolve<IRepository<FOrderItems>>();

        public FOrderItemsService()
        {

        }

        /// <summary>
        /// Inserts Item into FOrderItems
        /// </summary>
        /// <param name="fOrderItems">FOrderItems</param>
        public virtual void InsertFreshCart(FOrderItems fOrderItems)
        {
            if (fOrderItems == null)
                throw new ArgumentNullException(nameof(FOrderItems));

            //if (freshCart is IEntityForCaching)
            //    throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _fOrderItemsRepository.Insert(fOrderItems);

        }

        public virtual IList<FOrderItems> GetFOrderItemsByOrderId(int OrderId)
        {
            var query = _fOrderItemsRepository.Table;
            query = query.Where(c => c.OrderId == OrderId);
            var fOrderItems = query.ToList();
            return fOrderItems;
        }
    }
}
