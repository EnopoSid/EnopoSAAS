using System;
namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
   public class PersonalInfoRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }      

        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public string CompanyName { get; set; }

        public string Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set;}
    }
}
