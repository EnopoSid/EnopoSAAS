using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class PasswordRecoveryRequest : AuthenticationEntity
    {
        [Required]
        public string EmailId { get; set; }

        public int LanguageId { get; set; }

        public string Token { get; set; }

        public string NewPassword { get; set; }

        public int StoreId { get; set; }
    }
}
