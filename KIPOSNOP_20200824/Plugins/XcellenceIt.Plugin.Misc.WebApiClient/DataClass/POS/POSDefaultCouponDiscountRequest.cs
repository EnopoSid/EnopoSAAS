using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.POS
{
    public class POSDefaultCouponDiscountRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }
        public List<DiscountRequest> DiscountsInfo { get; set; }
        public List<Guid> CustomerGUIDs { get; set; }
    }

    public class POSDiscountRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public string DiscountCouponCode { get; set; }

        public List<Guid> CustomerGUIDs { get; set; }

        public decimal OrderTotal { get; set; }
    }
}
