using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Common
{
    public partial class Order_Pickup_CustDetailsMap : NopEntityTypeConfiguration<Order_Pickup_CustDetails>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Order_Pickup_CustDetails> builder)
        {
            builder.ToTable(nameof(Order_Pickup_CustDetails));
            builder.HasKey(order_Pickup_CustDetails => order_Pickup_CustDetails.Id);

            base.Configure(builder);
        }

        #endregion
    }
}
