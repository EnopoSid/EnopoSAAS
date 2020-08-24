namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class GetSettingBySettingKeyRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public string SettingKey { get; set; }
    }
}
