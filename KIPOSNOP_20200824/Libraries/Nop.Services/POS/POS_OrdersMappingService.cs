using Nop.Core.Data;
using Nop.Core.Domain.POS;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nop.Services.POS
{
    public partial class POS_OrdersMappingService: IPOS_OrdersMappingService
    {
        private readonly IRepository<POS_OrdersMapping> _POS_OrdersMappingrepository = EngineContext.Current.Resolve<IRepository<POS_OrdersMapping>>();

        public POS_OrdersMappingService()
        {

        }

        /// <summary>
        /// Inserts Item into FOrderItems
        /// </summary>
        /// <param name="POS_OrdersMapping">POS_OrdersMapping</param>
        public virtual void InsertPOSOrder(POS_OrdersMapping pOS_OrdersMapping)
        {
            if (pOS_OrdersMapping == null)
                throw new ArgumentNullException(nameof(pOS_OrdersMapping));

            //if (freshCart is IEntityForCaching)
            //    throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _POS_OrdersMappingrepository.Insert(pOS_OrdersMapping);

        }

        public virtual POS_OrdersMapping GetPOSOrderByOrderId(int OrderId)
        {
            var query = _POS_OrdersMappingrepository.Table;
            query = query.Where(c => c.OrderId == OrderId);
            var POsOrder = query.FirstOrDefault();
            return POsOrder;
        }
    }
}
