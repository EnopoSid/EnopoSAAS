using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SelectAddressRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int CurrencyId { get; set; }

        public int AddressId { get; set; }

        public int LanguageId { get; set; }
    }
}
