using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.POS
{
    public class POSConfirmOrderRequest:AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public CustomerGuids customerGuids { get; set; }

        public List<CheckoutAddressDetails> CheckoutAddressDetails { get; set; }

        public string PaymentMethodType { get; set; }

        public bool IsRegisteredUser { get; set; }

        //Added By Surakshith to display memberId in mail for register user strat on 15-06-2020

        public string MemberId { get; set; }

        //Added By Surakshith to display memberId in mail for register user end on 15-06-2020

        public decimal OrderTax { get; set; }

    }

    public class CustomerGuids
    {
        public Guid CustomerGuid { get; set; }
        public Guid POSUserGuid { get; set; }
    }
}
