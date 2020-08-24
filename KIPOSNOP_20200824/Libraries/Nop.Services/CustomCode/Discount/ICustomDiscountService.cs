using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services
{
    public interface ICustomDiscountService
    {
        /// <summary>
        /// Get Applied discounts for customer
        /// </summary>
        /// <param name="discount">Discount</param>
        List<Discount> GetDiscountsByName(List<String> discounts);
    }
}
