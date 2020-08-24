using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class LoginRequest : AuthenticationEntity
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }

        public int? GuestCustomerId { get; set; }
   
        public int? StoreId { get; set; }
    }
}
