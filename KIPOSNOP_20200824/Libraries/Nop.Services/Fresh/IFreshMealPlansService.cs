using Nop.Core.Domain.Fresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Fresh
{
    public partial interface  IFreshMealPlansService
    {
        FreshMealsPlans GetFreshMealPlanById(int freshMealPlanId);
    }
}
