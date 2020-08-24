using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class EnterCreditCardInfoRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public string PaymentMethodSystemName { get; set; }

        public string CreditCardType { get; set; }

        public string CardholderName { get; set; }

        public string CardNumber { get; set; }

        public string CardCode { get; set; }

        public string ExpireMonth { get; set; }

        public string ExpireYear { get; set; }

        public string PaymentToken { get; set; }
    }
}
