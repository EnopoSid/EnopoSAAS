using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SetCheckOutAttributeRequest : AuthenticationEntity
    {
        public SetCheckOutAttributeRequest()
        {
           this.CheckoutAttributeResponse = new List<CheckoutAttributeResponse>();
        }
       
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public List<CheckoutAttributeResponse> CheckoutAttributeResponse { get; set;}
    }
}
