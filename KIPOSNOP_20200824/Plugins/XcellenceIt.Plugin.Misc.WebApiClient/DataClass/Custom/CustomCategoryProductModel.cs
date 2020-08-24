using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class CustomCategoryProductModel : AuthenticationEntity
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ParentSubCategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public int ReviewCustomerId { get; set; }
        public int Rating { get; set; }
        public int ApprovedRatingSum { get; set; }
        public int ApprovedTotalReviews { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        //Added by surakshith to get priceinclusion of tax start on 16-07-2020
        public decimal PriceIncludingTax { get; set; }
        //Added by surakshith to get priceinclusion of tax end on 16-07-2020
        public int PictureId { get; set; }
        public bool Published { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public string ManufacturerName { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerDescription { get; set; }
        public decimal AverageProductRating { get; set; }

        public IList<CustomCategoryProductModel> FeaturedProducts { get; set; }
        public IList<CustomCategoryProductModel> Products { get; set; }
    }
}
