using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
     public class AddNewAddressRequest : AuthenticationEntity
    {
        public AddNewAddressRequest()
        {
            this.AddressModel = new AddressModel();  
        }
       
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int CurrencyId { get; set; }

        public bool PickUpInStore { get; set; }

        public int LanguageId { get; set; }

        public AddressModel AddressModel { get; set; }

        public List<string> AttributeControlIds { get; set; }
    }
}
