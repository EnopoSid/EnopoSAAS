using System;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class SettingUpdateRequest : AuthenticationEntity
    {
        public SettingUpdateRequest()
        {
            this.SettingModelRequest = new SettingModelResponse();
        }       
        public int StoreId { get; set; }

        [Required]
        public Guid CustomerGUID { get; set; }

        public SettingModelResponse SettingModelRequest { get; set; }
    }
}
