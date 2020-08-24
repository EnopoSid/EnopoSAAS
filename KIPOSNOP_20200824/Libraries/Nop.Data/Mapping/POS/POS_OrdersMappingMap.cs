using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.POS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.POS
{
    public partial class POS_OrdersMappingMap : NopEntityTypeConfiguration<POS_OrdersMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<POS_OrdersMapping> builder)
        {
            builder.ToTable("POS_OrdersMapping");
            builder.HasKey(POS_OrdersMapping => POS_OrdersMapping.Id);
        }
        #endregion
    }
}
