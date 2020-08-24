using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Common
{
    public partial class Order_Pickup_CustDetails:BaseEntity
    {
        /// <summary>
        /// Gets or sets the Order Id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the Customer Id
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the MobileNumber
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the EmailId
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// Gets or sets the CreatedBy
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the CreatedDate
        /// </summary>
        public Nullable<System.DateTime> CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the ModifiedBy
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the ModifiedDate
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        public bool IsActive { get; set; }

    }
}
