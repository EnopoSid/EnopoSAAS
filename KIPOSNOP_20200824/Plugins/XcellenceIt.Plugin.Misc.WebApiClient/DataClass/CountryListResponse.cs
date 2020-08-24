namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class CountryListResponse
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public bool LimitedToStore { get; set; }

        public string TwoLetterIsoCode { get; set; }

        public int NumericIsoCode { get; set; }
    }
}
