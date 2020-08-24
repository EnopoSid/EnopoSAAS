
namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class PopularProductTagsRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int LanguageId { get; set; }
    }
}
