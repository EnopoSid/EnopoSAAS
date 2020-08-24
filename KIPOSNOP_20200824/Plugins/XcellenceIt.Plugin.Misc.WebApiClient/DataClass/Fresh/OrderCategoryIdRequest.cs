using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public partial class OrderCategoryIdRequest:AuthenticationEntity
    {
        public List<int> orderIds { get; set; }
    }
}
