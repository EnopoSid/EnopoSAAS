using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class AddProductReviewRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ProductId { get; set; }

        public bool CaptchaValid { get; set; }

        public ProductReviewRequest ProductReviewRequest { get; set; }

    }
    public class ProductReviewRequest
    {
        public string Title { get; set; }

        public string ReviewText { get; set; }

        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentCustomerLeaveReview { get; set; }

        public bool SuccessfullyAdded { get; set; }

        public string Result { get; set; }
    }
}
  
