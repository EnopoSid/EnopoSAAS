namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SubscribeNewsletterRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public string Email { get; set; }

        public bool IsSubscribed { get; set; }

        public int LanguageId { get; set; }
    }
}
