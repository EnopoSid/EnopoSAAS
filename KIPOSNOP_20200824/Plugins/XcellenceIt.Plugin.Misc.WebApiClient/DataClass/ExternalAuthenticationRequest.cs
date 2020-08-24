using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ExternalAuthenticationRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public string ResponseEmailAddress { get; set; }

        public string ResponseFirstName { get; set; }

        public string ResponseLastName { get; set; }

        public string ResponseUserName { get; set; }

        public ExternalAuthenticationParameter ExternalAuthenticationParameter { get; set; }
    }
}
