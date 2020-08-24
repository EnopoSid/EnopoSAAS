using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class RemoveExternalAuthenticationRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public string ProviderSystemName { get; set; }
    }
}
