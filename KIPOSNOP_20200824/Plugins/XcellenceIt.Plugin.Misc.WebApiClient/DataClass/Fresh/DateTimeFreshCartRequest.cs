using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public class DateTimeFreshCartRequest:AuthenticationEntity
    {
        public List<DateAndTime> dateAndTimes = new List<DateAndTime>();
    }
    public class DateAndTime
    {
        public int MealNumber { get; set; }
        public DateTime MealDate { get; set; }
        public string MealTime { get; set; }
        public int ShoppingCartId { get; set; }
        public int PackageType { get; set; }
    }
}
