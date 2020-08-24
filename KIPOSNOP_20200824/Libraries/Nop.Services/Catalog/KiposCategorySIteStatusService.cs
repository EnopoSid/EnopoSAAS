using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nop.Services.Catalog
{
    public partial class KiposCategorySIteStatusService: IKiposCategorySIteStatusService
    {
        private readonly IRepository<Kipos_Category_SIteStatus> _kipos_Category_SIteStatuRepository = EngineContext.Current.Resolve<IRepository<Kipos_Category_SIteStatus>>();


        public KiposCategorySIteStatusService()
        {

        }

        /// <summary>
        /// Inserts Item into FreshCart
        /// </summary>
        public virtual IList<Kipos_Category_SIteStatus> GetAllKiposCategorySiteStatus()
        {

            var query = _kipos_Category_SIteStatuRepository.Table;
            query = query.Where(x => x.IsActive == true);
            var catSiteStatus = query.ToList();
            return catSiteStatus;
        }
    }
}
