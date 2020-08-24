using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Catalog
{
    public partial interface IKiposCategorySIteStatusService
    {
        // added by Phanendra on 04-05-2020 to get only Gourmet related products 
        IList<Kipos_Category_SIteStatus> GetAllKiposCategorySiteStatus();
    }
}
