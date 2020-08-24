using Nop.Core.Data;
using Nop.Core.Domain.Fresh;
using Nop.Core.Infrastructure;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Fresh
{
    public partial class FreshMealPlansService:IFreshMealPlansService
    {
        private readonly IRepository<FreshMealsPlans> _freshMealsPlansRepository = EngineContext.Current.Resolve<IRepository<FreshMealsPlans>>();

        public FreshMealPlansService()
        {

        }

        /// <summary>
        /// Inserts Item into FreshCart
        /// </summary>
        /// <param name="freshCart">FreshCart</param>
        public virtual FreshMealsPlans GetFreshMealPlanById(int freshMealPlanId)
        {

            var query = _freshMealsPlansRepository.Table;
            query = query.Where(c => c.Id == freshMealPlanId);
            var fCart = query.FirstOrDefault();
            return fCart;
        }
    }
}
