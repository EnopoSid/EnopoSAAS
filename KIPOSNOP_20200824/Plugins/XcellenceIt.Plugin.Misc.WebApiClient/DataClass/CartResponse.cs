using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Orders;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    /*Added by Surakshith for getting Cart Count Start*/
    public class CartResponse
    {
        public int cartCount { get; set; }

        public List<ShoppingCartItem> items { get; set; }
    }
    /*Added by Surakshith for getting Cart Count End*/
}
