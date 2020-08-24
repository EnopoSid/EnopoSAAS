using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class ProductCategoryResponse
    {
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public ShoppingCartItem ShoppingCartItem { get; set; }
        public Product Product { get; set; }

    }
}
