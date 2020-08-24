using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Fresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Fresh
{
    public partial class FOrderItemsMap : NopEntityTypeConfiguration<FOrderItems>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FOrderItems> builder)
        {
            builder.ToTable("FOrderItems");
            builder.HasKey(fOrdeitems => fOrdeitems.Id);
        }
        #endregion
    }
}
