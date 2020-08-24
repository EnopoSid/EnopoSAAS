using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Fresh
{
    public partial class FreshMealsPlans:BaseEntity
    {
        #region Properties
        
        public string PlanName { get; set; }

        public decimal PlanAmount { get; set; }

        public decimal PerMealAmount { get; set; }

        public decimal DeliveryAmount { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }

        public Nullable<DateTime> ModifiedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public bool? IsActive { get; set; }

        #endregion
    }
}
