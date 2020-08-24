using System;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class GetAddressRequest : AuthenticationEntity
    {
        [Required]
        public Guid CustomerGUID { get; set; }

        public int StoreId { get; set; }

    }
}
