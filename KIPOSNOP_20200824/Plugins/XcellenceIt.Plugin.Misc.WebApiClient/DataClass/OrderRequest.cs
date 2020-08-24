using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
   public class OrderRequest : AuthenticationEntity
    {
        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public int OrderId { get; set; }
    }
}
