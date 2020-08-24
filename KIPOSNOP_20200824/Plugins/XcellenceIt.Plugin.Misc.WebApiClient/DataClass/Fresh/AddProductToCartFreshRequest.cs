using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public class AddProductToCartFreshRequest : AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ShoppingCartTypeId { get; set; }

        public int ParentCategoryId { get; set; }

        public int MealPlanId { get; set; }

        public List<mealDetail> mealDetails = new List<mealDetail>();

    }

    public class mealDetail
    {
        public mealDetail()
        {
            this.AttributeControlIds = new List<string>();
        }

        public int MealNumber { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public List<string> AttributeControlIds { get; set; }

        public string RentalStartDate { get; set; }

        public string RentalEndDate { get; set; }

        public string MealTime { get; set; }

        public Nullable<DateTime> MealDate { get; set; }
    }

}
