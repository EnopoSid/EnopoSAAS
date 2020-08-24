using Nop.Web.Models.Catalog;
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    class ProductReviewCustomModel : ProductReviewModel
    {
        public Guid CustomerGuid { get; set; }
    }
}
