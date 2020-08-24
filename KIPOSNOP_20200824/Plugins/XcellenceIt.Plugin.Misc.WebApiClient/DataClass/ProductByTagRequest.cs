
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductByTagRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ProductTagId { get; set; }

        public int CurrencyId { get; set; }

        public CatalogPagingResponse CatalogPagingResponse { get; set; }

    }

}
