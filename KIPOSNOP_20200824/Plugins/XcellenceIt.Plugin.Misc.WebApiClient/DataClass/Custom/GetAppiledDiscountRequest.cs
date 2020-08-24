using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class GetAppiledDiscountRequest : AuthenticationEntity
    {
        public int CustomerId { get; set; }
        public Guid CustomerGuid { get; set; }
        public string DiscountCouponCode { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
