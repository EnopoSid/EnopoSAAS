using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class EmailAvailabilityRequest : AuthenticationEntity
    {
        [Required]
        public string EmailId { get; set; }
    }
}
