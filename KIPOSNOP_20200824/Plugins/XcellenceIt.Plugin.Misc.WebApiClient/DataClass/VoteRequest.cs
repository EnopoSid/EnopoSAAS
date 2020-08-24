
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class VoteRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int PollAnswerId { get; set; }

        public Guid CustomerGUID { get; set; }
    }
}
