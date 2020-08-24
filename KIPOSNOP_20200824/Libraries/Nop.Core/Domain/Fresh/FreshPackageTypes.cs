using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Fresh
{
    public partial class FreshPackageTypes:BaseEntity
    {
        #region Properties

        public string PackageType { get; set; }

        public int? CreatedBy { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public Nullable<DateTime> ModifiedDate { get; set; }

        public bool StatusId { get; set; }

        public string PackageDesc { get; set; }

        public decimal Price { get; set; }

        #endregion
    }
}
