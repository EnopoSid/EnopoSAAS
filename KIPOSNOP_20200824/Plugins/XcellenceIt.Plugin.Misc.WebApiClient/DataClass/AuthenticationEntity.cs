using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public partial class AuthenticationEntity
    {
        [Required]
        public string ApiSecretKey { get; set; }
    }
}
