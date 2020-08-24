namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class AdvancedSearchRequest : AuthenticationEntity
    {
        public AdvancedSearchRequest()
        {
            this.AdvanceSearchResponse = new AdvanceSearchResponse();
            this.CatalogPagingResponse = new CatalogPagingResponse();
        }
       
        public int LanguageId { get; set; }

        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public int CustomerId { get; set; }

        public string Message { get; set; }

        public AdvanceSearchResponse AdvanceSearchResponse { get; set; }

        public CatalogPagingResponse CatalogPagingResponse { get; set; }
    }
}
