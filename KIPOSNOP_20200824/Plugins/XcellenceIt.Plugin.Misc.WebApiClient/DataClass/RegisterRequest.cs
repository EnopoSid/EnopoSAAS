using System;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class RegisterRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string EmailId { get; set; }

        [Required]
        public string Password { get; set; }

        public string Gender { get; set; } 

        public int LanguageId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //public DateTime DateOfBirth { get; set; }

        public string CompanyName { get; set; }

        /*mobileNumber*/
        public string MobileNumber { get; set; }
        /*mobileNumber*/

        /*Added by surakshith for IsGuestUser Start*/
        public bool IsGuestUser { get; set; }
       /*Added by surakshith for IsGuestUser End*/

        public string PhoneNumber { get; set; }

        public bool NewsLetter { get; set; }

        public int IsGuestCustomerId { get; set; }

        //new added
        public string TimeZoneId { get; set; }

        public string VatNumber { get; set; }

        public int? DateOfBirthDay { get; set; }

        public int? DateOfBirthMonth { get; set; }

        public int? DateOfBirthYear { get; set; }

        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }
    }
}
