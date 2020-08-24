using System;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class TopMenuRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        [Required]
        public Guid? CustomerGUID { get; set; }

        public int LanguageId { get; set; }

    }
}
