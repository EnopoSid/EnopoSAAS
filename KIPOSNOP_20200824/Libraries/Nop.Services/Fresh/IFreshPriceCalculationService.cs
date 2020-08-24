using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Fresh
{
    public partial interface IFreshPriceCalculationService
    {
        decimal GetFreshProductPrice(Product product, decimal price,
            out decimal taxRate);

        decimal GetFreshProductPrice(Product product, decimal price,
            bool includingTax, Customer customer, out decimal taxRate);

        decimal GetFreshProductPrice(Product product, int taxCategoryId,
           decimal price, bool includingTax, Customer customer,
           bool priceIncludesTax, out decimal taxRate);

        decimal GetUnitPriceforFreshItem(ShoppingCartItem shoppingCartItem,  IGrouping<Guid?,FCart> fCart,
            bool includeDiscounts = true);

        decimal GetUnitPriceforFreshItem(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts, IGrouping<Guid?, FCart> fCart,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts);

        decimal GetUnitPriceforFreshItem(Product product,
                    IGrouping<Guid?, FCart> fCart,
                    Customer customer,
                    ShoppingCartType shoppingCartType,
                    int quantity,
                    string attributesXml,
                    decimal customerEnteredPrice,
                    DateTime? rentalStartDate, DateTime? rentalEndDate,
                    bool includeDiscounts,
                    out decimal discountAmount,
                    out List<DiscountForCaching> appliedDiscounts);


        /// <summary>
        /// Get a price adjustment of a product attribute value
        /// </summary>
        /// <param name="value">Product attribute value</param>
        /// <param name="customer">Customer</param>
        /// <param name="productPrice">Product price (null for using the base product price)</param>
        /// <returns>Price adjustment</returns>
        decimal GetProductAttributeValuePriceAdjustment(ProductAttributeValue value, Customer customer, decimal? productPrice = null);

        /// <summary>
        /// Round a product or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency; pass null to use the primary store currency</param>
        /// <returns>Rounded value</returns>
        decimal RoundPrice(decimal value, Currency currency = null);

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="roundingType">The rounding type</param>
        /// <returns>Rounded value</returns>
        decimal Round(decimal value, RoundingType roundingType);


        decimal GetSubTotalForFreshCart(ShoppingCartItem shoppingCartItem, IGrouping<Guid?, FCart> fCart,
            bool includeDiscounts = true);

        decimal GetSubTotalForFreshCart(ShoppingCartItem shoppingCartItem,
            IGrouping<Guid?, FCart> fCart,
            bool includeDiscounts,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts,
            out int? maximumDiscountQty);



    }
}
