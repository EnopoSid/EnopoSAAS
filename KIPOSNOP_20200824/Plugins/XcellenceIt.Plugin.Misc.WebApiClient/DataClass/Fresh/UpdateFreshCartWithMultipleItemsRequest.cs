using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public partial class UpdateFreshCartWithMultipleItemsRequest:AuthenticationEntity
    {
        public int StoreId { get; set; }

        public int CurrencyId { get; set; }

        public Guid CustomerGUID { get; set; }

        public int ShoppingCartTypeId { get; set; }

        public int ParentCategoryId { get; set; }

        public List<UpdateToCartMealPlanModel> mealDetails = new List<UpdateToCartMealPlanModel>();
    }

    public class UpdateToCartMealPlanModel
    {
        public Guid MealOrderId { get; set; }
        public string MealPlanName { get; set; }
        public int ProductId { get; set; }
        public int MealPlanPrice { get; set; }
        public int MealPlanId { get; set; }
        public List<mealDetail> Items { get; set; }
    }
}
