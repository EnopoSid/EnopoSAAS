
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SelectShippingRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int CurrencyId { get; set; }

        public int languageId { get; set; }

        public string ShippingOption { get; set; }
    }
}
