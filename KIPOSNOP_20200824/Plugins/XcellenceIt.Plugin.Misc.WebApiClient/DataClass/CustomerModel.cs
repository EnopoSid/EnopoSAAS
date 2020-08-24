using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class CustomerModel: ResponseSuccess
    {
        public int CustomerId { get; set; }
        public Guid CustomerGuid { get; set; }
    }
}
