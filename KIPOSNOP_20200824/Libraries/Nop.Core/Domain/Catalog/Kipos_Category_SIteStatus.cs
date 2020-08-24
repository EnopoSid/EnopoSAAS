using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{ 
    public partial class Kipos_Category_SIteStatus : BaseEntity
    {
        #region Properties
        // added by Phanendra on 04-05-2020 to get only Gourmet related products 
        /// <summary>
        /// Gets or sets the Category Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the Parent Category Id
        /// </summary>
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the IsPOS
        /// </summary>
        public int IsPOS { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        public Boolean IsActive { get; set; }

        ///// <summary>
        ///// Gets or sets the masked credit card number
        ///// </summary>
        //public int CreatedBy { get; set; }

        ///// <summary>
        ///// Gets or sets the masked credit card number
        ///// </summary>
        //public int ModifiedBy { get; set; }

        ///// <summary>
        ///// Gets or sets the date and time of order creation
        ///// </summary>
        //public DateTime CreatedDate { get; set; }

        ///// <summary>
        ///// Gets or sets the date and time of order creation
        ///// </summary>
        //public Nullable<DateTime> ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the IsOnline
        /// </summary>
        public int IsOnline { get; set; }       

        #endregion
    }
}
