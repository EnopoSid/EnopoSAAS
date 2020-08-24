using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class CheckoutParameters
    {
        public int NumberOfCheckoutCategories { get; set; }
        public int CheckOutOrderCount { get; set; }
        public int IsPickUpInStoreFalseCount { get; set; }
        public string CaptureTransactionId { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal GrocerOrderTotal { get; set; }
        public decimal GourmetOrderTotal { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
