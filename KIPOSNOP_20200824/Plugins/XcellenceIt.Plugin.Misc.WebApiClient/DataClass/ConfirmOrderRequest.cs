using Nop.Services.Payments;
using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
   public class ConfirmOrderRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public string PaymentToken { get; set; }

        //public string CardHolderName { get; set; }

        //Added By Surakshith to display memberId in mail for register user start on 13-06-2020

        public string MemberId { get; set; }

        //Added By Surakshith to display memberId in mail for register user end on 13-06-2020

        public decimal OrderTotal { get; set; }

        public List<CheckoutAddressDetails> CheckoutAddressDetails { get; set; }

        public bool IsMember { get; set; }

        public string PaymentMethodType { get; set; }

        public decimal OrderTax { get; set; }
    }
}
