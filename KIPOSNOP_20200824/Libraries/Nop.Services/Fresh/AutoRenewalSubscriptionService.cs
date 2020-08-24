using Nop.Core.Data;
using Nop.Core.Domain.Fresh;
using Nop.Core.Infrastructure;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Fresh
{
    public partial class AutoRenewalSubscriptionService: IAutoRenewalSubscriptionService
    {
        private readonly IRepository<AutoRenewalSubscription> _autoRenewalSubscriptionRepository = EngineContext.Current.Resolve<IRepository<AutoRenewalSubscription>>();

        public AutoRenewalSubscriptionService()
        {

        }

        /// <summary>
        /// Inserts Item into AutoRenewalSubscription
        /// </summary>
        /// <param name="autoRenewalSubscription">AutoRenewalSubscription</param>
        public void InsertAutorenewalData(AutoRenewalSubscription  autoRenewalSubscription)
        {
            if (autoRenewalSubscription == null)
                throw new ArgumentNullException(nameof(AutoRenewalSubscription));

            _autoRenewalSubscriptionRepository.Insert(autoRenewalSubscription);
        }

        /// <summary>
        /// Gets Item from AutoRenewalSubscription
        /// </summary>
        /// <param name="customerId">CustomerId</param>
        public AutoRenewalSubscription GetAutorenewalDataByCustomerId(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentNullException(nameof(AutoRenewalSubscription));

            var query = _autoRenewalSubscriptionRepository.Table.Where(x => x.CustomerId == customerId && x.StausId == true).OrderByDescending(x => x.Id).FirstOrDefault();

            return query;
        }
    }
}
