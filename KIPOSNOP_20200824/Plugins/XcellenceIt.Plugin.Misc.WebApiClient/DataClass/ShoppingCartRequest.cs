using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ShoppingCartRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }
    }
}
