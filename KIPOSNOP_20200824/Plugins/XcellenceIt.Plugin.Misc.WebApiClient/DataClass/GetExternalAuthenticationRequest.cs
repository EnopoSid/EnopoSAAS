using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class GetExternalAuthenticationRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }
    }
}
