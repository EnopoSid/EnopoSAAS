using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{
    public partial class FCart : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the shoppingCartId identifier
        /// </summary>
        public int ShoppingCartId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the product id identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the card type
        /// </summary>
        public int MealNo { get; set; }

        ///// <summary>
        ///// Gets or sets the card name
        ///// </summary>
        //public int orderId { get; set; }

        /// <summary>
        /// Gets or sets the card number
        /// </summary>
        public bool? Status { get; set; }

        ///// <summary>
        ///// Gets or sets the masked credit card number
        ///// </summary>
        //public int CreatedBy { get; set; }

        ///// <summary>
        ///// Gets or sets the masked credit card number
        ///// </summary>
        //public int ModifiedBy { get; set; }

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
        public int? IsReorder { get; set; }

        /// <summary>
        /// Gets or sets the masked credit card number
        /// </summary>
        public int? PackageType { get; set; }
        #endregion
    }

}