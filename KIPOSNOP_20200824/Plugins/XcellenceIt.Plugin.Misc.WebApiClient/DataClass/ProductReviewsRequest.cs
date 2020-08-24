using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductReviewsRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ProductId { get; set; }

        public int LanguageId { get; set; }

    }
}
