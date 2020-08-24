using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class MemberUnsubscribeRequest: AuthenticationEntity
    {
        public Guid CustomerGUID { get; set; }
    }
}
