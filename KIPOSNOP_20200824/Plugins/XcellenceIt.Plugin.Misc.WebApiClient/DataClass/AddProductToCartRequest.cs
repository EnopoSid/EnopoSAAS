using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class AddProductToCartRequest : AuthenticationEntity
    {
        public AddProductToCartRequest()
        {
            this.AttributeControlIds = new List<string>();
        }
        
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ProductId { get; set; }

        public int ShoppingCartTypeId { get; set; }

        public int Quantity { get; set; }

        public List<string> AttributeControlIds { get; set; }

        public string RentalStartDate { get; set; }

        public string RentalEndDate { get; set; }

        public int ParentCategoryId { get; set; }
    }
}
