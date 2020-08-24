namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class CurrencyRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }
    }
}
