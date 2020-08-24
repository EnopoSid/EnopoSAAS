using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a manufacturer mapping configuration
    /// </summary>
    public partial class FCartMap : NopEntityTypeConfiguration<FCart>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FCart> builder)
        {
            builder.ToTable("FCart");
            builder.HasKey(manufacturer => manufacturer.Id);

        }
        #endregion
    }
}
