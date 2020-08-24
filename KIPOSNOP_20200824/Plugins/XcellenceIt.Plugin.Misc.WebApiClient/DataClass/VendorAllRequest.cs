using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
   public class VendorAllRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }
    }
}
