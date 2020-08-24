
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductByCategoryRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }
       
        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int CategoryId { get; set; }

        public CatalogPagingResponse CatalogPagingResponse { get; set; }
    }
}
