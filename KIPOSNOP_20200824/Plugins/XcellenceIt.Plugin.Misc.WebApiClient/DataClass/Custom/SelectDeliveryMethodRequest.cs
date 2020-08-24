using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class SelectDeliveryMethodRequest : AuthenticationEntity
    {
        public Guid CustomerGUID { get; set; }

        public Boolean PickUpInStore{get;set;}

        public string DeliveryOption { get; set; }

        public int StoreId { get; set; }

    }
}
