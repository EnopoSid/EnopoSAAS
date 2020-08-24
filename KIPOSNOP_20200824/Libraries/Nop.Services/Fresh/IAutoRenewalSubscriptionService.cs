using Nop.Core.Domain.Fresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Fresh
{
     public partial interface IAutoRenewalSubscriptionService
    {
        /// <summary>
        /// Inserts Item into AutoRenewalSubscription
        /// </summary>
        /// <param name="autoRenewalSubscription">AutoRenewalSubscription</param>
        void InsertAutorenewalData(AutoRenewalSubscription  autoRenewalSubscription);

        /// <summary>
        /// Gets Item from AutoRenewalSubscription
        /// </summary>
        /// <param name="customerId">CustomerId</param>
        AutoRenewalSubscription GetAutorenewalDataByCustomerId(int customerId);
    }
}
