using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public partial class OrderCategoryIdResponse
    {
        public List<ParentCategoryIdResponse> parentCategoryIdResponses { get; set; }
        public ParentCategoryIdResponse ParentCategoryIdResponse { get; set; }
    }

    public partial class ParentCategoryIdResponse
    {
        public int OrderId { get; set; }
        public int ParentCategoryId { get; set; }
    }
}
