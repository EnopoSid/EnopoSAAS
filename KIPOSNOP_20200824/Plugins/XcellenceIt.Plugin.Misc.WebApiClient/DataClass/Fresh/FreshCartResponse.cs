using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public partial class FreshCartResponse
    {
        public FreshCartResponse()
        {
            freshCartDetails freshCart = new freshCartDetails();
        }
        public List<freshCartDetails> freshCartDetails { get; set; }

        public Guid CustomerGUID { get; set; }

        public freshCartDetails freshCartDetail {get;set;}
    }

    public class freshCartDetails
    {
        public int ShoppinCartId { get; set; }

        public int MealNo { get; set; }
    }
}
