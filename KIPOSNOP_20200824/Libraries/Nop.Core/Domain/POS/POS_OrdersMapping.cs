using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.POS
{
    public partial class POS_OrdersMapping:BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the OrderId identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the OrderGuid identifier
        /// </summary>
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the CustomerId identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the POSUserGuid identifier
        /// </summary>
        public Guid POSUserGuid { get; set; }

        /// <summary>
        /// Gets or sets the POSUserId
        /// </summary>
        public int POSUserId { get; set; }

        /// <summary>
        /// Gets or sets the card number
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the card number
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order modification
        /// </summary>
        public Nullable<DateTime> ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the Status without prefix
        /// </summary>
        public bool? Status { get; set; }

        #endregion
    }
}
