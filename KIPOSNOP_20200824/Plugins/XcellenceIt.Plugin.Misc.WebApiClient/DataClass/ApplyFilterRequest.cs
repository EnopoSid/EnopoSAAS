using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ApplyFilterRequest : AuthenticationEntity
    {
        public ApplyFilterRequest()
        {
            this.specIds = new List<int>();
        }

        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int CategoryId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public List<int> specIds { get; set; }

        public CatalogPagingResponse CatalogPagingResponse { get; set; }
    }
}
