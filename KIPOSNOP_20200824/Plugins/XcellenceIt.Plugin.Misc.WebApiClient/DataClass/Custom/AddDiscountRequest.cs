using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class AddDiscountRequest: AuthenticationEntity
    {
        public Guid CustomerGUID { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal KPointsToRedeem { get; set; }

        public string CouponCode { get; set; }

        public int DiscountId { get; set; }

        public decimal OrderTotal { get; set; }

        public int StoreId { get; set; }
    }
}
