using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Catalog
{
    public partial class Kipos_Category_SIteStatusMap: NopEntityTypeConfiguration<Kipos_Category_SIteStatus>
    {
        #region Methods
        // added by Phanendra on 04-05-2020 to get only Gourmet related products 
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Kipos_Category_SIteStatus> builder)
        {
            builder.ToTable("Kipos_Category_SIteStatus");
            builder.HasKey(kipos_Category_SIteStatus => kipos_Category_SIteStatus.Id);

        }
        #endregion
    }
}
