using Nop.Web.Models.Media;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class VendorAllResponse 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }

        public PictureModel PictureModel { get; set; }
    }
}
