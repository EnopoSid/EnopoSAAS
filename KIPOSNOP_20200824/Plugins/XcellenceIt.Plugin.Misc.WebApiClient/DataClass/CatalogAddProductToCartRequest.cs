using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class CatalogAddProductToCartRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int ProductId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int Quantity { get; set; }

        public int ShoppingCartTypeId { get; set; }

        public bool ForceRedirection { get; set; }
    }
}
