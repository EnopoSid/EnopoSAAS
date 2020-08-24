using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class CategoryListProductModel
    {
        public CategoryListProductModel()
        {
            ProductSubCategory categoryProduct = new ProductSubCategory();
        }
        public List<ProductSubCategory> subCategories { get; set; }
         public ProductSubCategory productSubCategory { get; set; }
    }

    public class ProductSubCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfProducts { get; set; }

        public IList<CustomCategoryProductModel> Products { get; set; }
    }
}
