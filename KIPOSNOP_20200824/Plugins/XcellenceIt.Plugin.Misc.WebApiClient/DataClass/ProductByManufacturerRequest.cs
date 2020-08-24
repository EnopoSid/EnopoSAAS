using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductByManufacturerRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ManufacturerId { get; set; }

        public CatalogPagingResponse CatalogPagingResponse { get; set; }

        public CatalogPagingFilteringResponse PagingFilteringContext { get; set; }

    }
  
}

  
