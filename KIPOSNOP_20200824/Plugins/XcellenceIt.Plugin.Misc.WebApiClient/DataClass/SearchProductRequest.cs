
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SearchProductRequest : AuthenticationEntity
    {
        public SearchProductRequest()
        {
            this.SearchListResponse = new SearchListResponse();
            this.CatalogPagingResponse = new CatalogPagingResponse();
        }

        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public string Message { get; set; }
      
        public SearchListResponse SearchListResponse { get; set; }

        public CatalogPagingResponse CatalogPagingResponse { get; set; }

    }
}
