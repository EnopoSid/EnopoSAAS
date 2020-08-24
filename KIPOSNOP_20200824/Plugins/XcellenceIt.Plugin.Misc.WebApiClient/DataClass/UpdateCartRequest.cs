
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class UpdateCartRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ItemId { get; set; }

        public int NewQuantity { get; set; }

    }
}
