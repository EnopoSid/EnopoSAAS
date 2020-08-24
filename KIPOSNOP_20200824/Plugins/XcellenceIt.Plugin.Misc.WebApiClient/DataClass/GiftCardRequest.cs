using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class GiftCardRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public string GiftCardCouponCode { get; set; }
    }
}
