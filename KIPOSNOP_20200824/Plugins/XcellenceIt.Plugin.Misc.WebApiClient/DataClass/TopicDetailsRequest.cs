
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class TopicDetailsRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int TopicId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }
    }
}
