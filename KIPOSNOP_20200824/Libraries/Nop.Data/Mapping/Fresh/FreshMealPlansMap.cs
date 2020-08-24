using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Fresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Fresh
{
    public partial class FreshMealPlansMap : NopEntityTypeConfiguration<FreshMealsPlans>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FreshMealsPlans> builder)
        {
            builder.ToTable("FreshMealsPlans");
            builder.HasKey(meals => meals.Id);
        }
        #endregion
    }
}
