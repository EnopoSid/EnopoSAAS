﻿using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class OrderTotalRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public bool IsEditable { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }
    }
}
