using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class UserAvailabilityRequest : AuthenticationEntity
    {
        [Required]
        public string UserName { get; set; }
    }
}
