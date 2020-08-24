using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductDetailsRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ProductId { get; set; }
    }
}
