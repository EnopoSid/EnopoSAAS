using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public class FOrderRequest:AuthenticationEntity
    {
        public Guid CustomerGUID { get; set; }

        public int LanguageId { get; set; }

        public int OrderId { get; set; }
    }
   
}
