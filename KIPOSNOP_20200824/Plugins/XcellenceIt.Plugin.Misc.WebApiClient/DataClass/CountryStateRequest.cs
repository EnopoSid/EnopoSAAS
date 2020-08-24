namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class CountryStateRequest : AuthenticationEntity
    {
        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int LanguageId { get; set; }

    }
}
