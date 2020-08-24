using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class AddressRequest : AuthenticationEntity
    {
        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public string  FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public int CountryId { get; set; }

        public int StateProvinceId { get; set; }

        public string City { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string ZipPostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public int AddressId { get; set; }

    }
}
