using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Models
{
    public partial class CustomCategorySimpleModel : BaseNopEntityModel
    {
        public CustomCategorySimpleModel()
        {
            SubCategories = new List<CustomCategorySimpleModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfProducts { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public List<CustomCategorySimpleModel> SubCategories { get; set; }

        public PictureModel PictureModel { get; set; }
    }
}
