using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class EstimateShippingRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int? CountryId { get; set; }

        public int? StateProvinceId { get; set; }

        public string ZipPostalCode { get; set; }
    }
}
