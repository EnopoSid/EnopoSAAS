using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public class FreshConfirmOrderRequest:AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public string PaymentToken { get; set; }

        public int languageId { get; set; }

        public string ShippingOption { get; set; }

        public string PaymentMethod { get; set; }

        //public string CardHolderName { get; set; }

        public List<CheckoutAddressDetails> CheckoutAddressDetails { get; set; }

        public string PaymentMethodType { get; set; }

        public int PackagingType { get; set; }

        public bool IsAutoRenewal { get; set; }

        public bool IsTinPackageSelected { get; set; }

    }
}
