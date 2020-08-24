using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Fresh
{
    public partial interface IFreshOrderTotalCalulationService
    {
        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        void GetFreshShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            IGrouping<Guid?, FCart>[] fCart, bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount);

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of order sub total)</param>
        void GetFreshShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            IGrouping<Guid?, FCart>[] fCart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates);

        decimal? GetFreshShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool includingTax,
            out decimal taxRate, out List<DiscountForCaching> appliedDiscounts);


        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>A value indicating whether shipping is free</returns>
        bool IsFreeShipping(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, decimal? subTotal = null);

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Adjusted shipping rate</returns>
        decimal AdjustShippingRate(decimal shippingRate,
            IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, out List<DiscountForCaching> appliedDiscounts);

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Additional shipping charge</returns>
        decimal GetFreshShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart);
        

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>Shipping total</returns>
        decimal? GetFreshShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool includingTax);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        decimal? GetFreshShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool includingTax,
           out decimal taxRate);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, out SortedDictionary<decimal, decimal> taxRates,
            bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetFreshShoppingCartTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true);

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        decimal ConvertRewardPointsToAmount(int rewardPoints);

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        int ConvertAmountToRewardPoints(decimal amount);

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints);


    }
}
