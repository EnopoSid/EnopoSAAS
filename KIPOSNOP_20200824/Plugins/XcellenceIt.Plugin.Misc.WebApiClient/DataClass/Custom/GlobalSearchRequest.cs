using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom
{
    public class GlobalSearchRequest:AuthenticationEntity
    {
        public string KeyWord { get; set; }
        public int StoreId { get; set; }
        public Guid CustomerGUID { get; set; }
        public int LanguageId { get; set; }
        public int CurrencyId { get; set; }
    }
}
