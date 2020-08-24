using System;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class InfoRequest : AuthenticationEntity
    {
        [Required]
        public Guid CustomerGUID { get; set; }
    }
}
