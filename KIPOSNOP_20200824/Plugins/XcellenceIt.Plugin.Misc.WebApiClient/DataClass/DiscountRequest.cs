using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class DiscountRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public string DiscountCouponCode { get; set; }

        public Guid CustomerGUID { get; set; }

        public decimal OrderTotal { get; set; }
    }

    public class MultipleDiscountRequest: AuthenticationEntity
    {
        public int StoreId { get; set; }
        public List<DiscountRequest> DiscountsInfo { get; set; }
        public Guid CustomerGUID { get; set; }
    }
}
