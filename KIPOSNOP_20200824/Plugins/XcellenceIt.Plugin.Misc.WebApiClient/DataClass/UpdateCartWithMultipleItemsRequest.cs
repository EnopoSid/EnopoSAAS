
using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class UpdateCartWithMultipleItemsRequest : AuthenticationEntity
    {
        public UpdateCartWithMultipleItemsRequest()
        {
            this.CartItems = new List<UpdateShoppingCartItems>();
        }       
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public string RemoveFromCart { get; set; }

        public IList<UpdateShoppingCartItems> CartItems { get; set; }

    }
}
