using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
   public class ManufactureRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }

        public Guid CustomerGuid { get; set; }
    }
}
