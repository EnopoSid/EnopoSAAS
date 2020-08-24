using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Fresh
{
    public partial class AutoRenewalSubscription : BaseEntity
    {
        #region Properties

        public int CustomerId { get; set; }

        public int OrderId { get; set; }

        public int MealPlanId { get; set; }

        public int ParentCategoryId { get; set; }

        public bool IsAutoRenewal { get; set; }

        public int AutoRenewalStatus { get; set; }

        public int PackagingType { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool StausId { get; set; }

        public bool IsTinCartSelected {get;set;}

        #endregion
    }
}
