using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Fresh
{
    public partial class FOrderItems:BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// Gets or sets the product id identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the card type
        /// </summary>
        public int MealNo { get; set; }


        /// <summary>
        /// Gets or sets the card number
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public Nullable<DateTime> ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the custom order number without prefix
        /// </summary>
        public string MealTime { get; set; }

        /// <summary>
        /// Gets or sets the delivery date without prefix
        /// </summary>
        public Nullable<DateTime> MealDate { get; set; }

        ///<summary>
        ///Gets or sets the MealOrderId without prefix
        ///</summary>
        public Nullable<Guid> MealOrderId { get; set; }

        ///<summary>
        ///Gets or sets the MealPlanId without prefix
        ///</summary>
        public int? MealPlanId { get; set; }

        /// <summary>
        /// Gets or sets the masked credit card number
        /// </summary>
        public decimal? MealPrice { get; set; }

        /// <summary>
        /// Gets or sets the masked credit card number
        /// </summary>
        public int? PackageTypeId { get; set; }


        #endregion
    }
}
