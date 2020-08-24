using Nop.Web.Models.Media;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public partial class ManufacturerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public PictureModel PictureModel { get; set; }

        public IList<PriceRangeFilters> PriceRangeFilters { get; set; }

        public CatalogPagingFilteringResponse PagingFilteringContext { get; set; }

        public IList<ProductListingResponse> FeaturedProducts { get; set; }

        public IList<ProductListingResponse> Products { get; set; }
    }
}
