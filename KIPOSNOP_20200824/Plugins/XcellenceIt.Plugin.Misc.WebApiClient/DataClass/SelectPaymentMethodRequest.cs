
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SelectPaymentMethodRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public string PaymentMethod { get; set; }

        public bool UseRewardPoints { get; set; }
    }
}
