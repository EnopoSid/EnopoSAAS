namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ContactUsSendRequest : AuthenticationEntity
    {
        public ContactUsSendRequest()
        {
            this.ContactUsRequest = new ContactUsResponse();   
        }      

        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public ContactUsResponse ContactUsRequest { get; set; }
    }
}
