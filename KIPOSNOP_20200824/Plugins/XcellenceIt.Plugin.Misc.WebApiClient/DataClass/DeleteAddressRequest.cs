
using System;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class DeleteAddressRequest : AuthenticationEntity
    {
        [Required]
        public Guid CustomerGUID { get; set; }

        [Required]
        public int AddressId { get; set; }
    }
}
