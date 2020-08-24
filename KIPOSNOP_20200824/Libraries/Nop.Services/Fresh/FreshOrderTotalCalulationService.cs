using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Services.Fresh
{
    public partial class FreshOrderTotalCalulationService:IFreshOrderTotalCalulationService
    {

        #region Fields

        private readonly CatalogSettings _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
        private readonly ICheckoutAttributeParser _checkoutAttributeParser =EngineContext.Current.Resolve<ICheckoutAttributeParser>();
        private readonly IDiscountService _discountService = EngineContext.Current.Resolve<IDiscountService>();
        private readonly IGenericAttributeService _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        private readonly IGiftCardService _giftCardService = EngineContext.Current.Resolve<IGiftCardService>();
        private readonly IPaymentService _paymentService = EngineContext.Current.Resolve<IPaymentService>();
        private readonly IRewardPointService _rewardPointService = EngineContext.Current.Resolve<IRewardPointService>();
        private readonly IShippingService _shippingService = EngineContext.Current.Resolve<IShippingService>();
        private readonly IShoppingCartService _shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
        private readonly IStoreContext _storeContext = EngineContext.Current.Resolve<IStoreContext>();
        private readonly ITaxService _taxService = EngineContext.Current.Resolve<ITaxService>();
        private readonly IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
        private readonly RewardPointsSettings _rewardPointsSettings = EngineContext.Current.Resolve<RewardPointsSettings>();
        private readonly ShippingSettings _shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
        private readonly ShoppingCartSettings _shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
        private readonly TaxSettings _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
        private readonly IFreshPriceCalculationService _freshPriceCalculationService = EngineContext.Current.Resolve<IFreshPriceCalculationService>();

        #endregion

        #region Ctor
        public FreshOrderTotalCalulationService()
        {

        }
        #endregion

        #region Utilities

        /// <summary>
        /// Gets an order discount (applied to order subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderSubtotalDiscount(Customer customer,
            decimal orderSubTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !_discountService.ContainsDiscount(allowedDiscounts, discount))
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderSubTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return discountAmount;
        }

        #endregion

        #region methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetFreshShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            IGrouping<Guid?, FCart>[] fCart, bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            GetFreshShoppingCartSubTotal(cart, fCart, includingTax,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out _);
        }

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
        public virtual void GetFreshShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            IGrouping<Guid?, FCart>[] fCart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates)
        {
            discountAmount = decimal.Zero;
            appliedDiscounts = new List<DiscountForCaching>();
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;
            taxRates = new SortedDictionary<decimal, decimal>();

            if (!cart.Any())
                return;

            //get the customer 
            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;

            //sub totals
            var subTotalExclTaxWithoutDiscount = decimal.Zero;
            var subTotalInclTaxWithoutDiscount = decimal.Zero;

            foreach (var item in fCart)
            {
                foreach (var shoppingCartItem in cart)
                {
                    var tempItem = item.Where(x => x.ShoppingCartId == shoppingCartItem.Id).FirstOrDefault();
                    if (tempItem != null)
                    {
                        if (tempItem.MealDate != null && tempItem.MealTime != null)
                        {
                            var sciSubTotal = _freshPriceCalculationService.GetSubTotalForFreshCart(shoppingCartItem, item);

                            var sciExclTax = _freshPriceCalculationService.GetFreshProductPrice(shoppingCartItem.Product, sciSubTotal, false, customer, out var taxRate);
                            var sciInclTax = _freshPriceCalculationService.GetFreshProductPrice(shoppingCartItem.Product, sciSubTotal, true, customer, out taxRate);
                            subTotalExclTaxWithoutDiscount += sciExclTax;
                            subTotalInclTaxWithoutDiscount += sciInclTax;

                            //tax rates
                            var sciTax = sciInclTax - sciExclTax;
                            if (taxRate <= decimal.Zero || sciTax <= decimal.Zero)
                                continue;

                            if (!taxRates.ContainsKey(taxRate))
                            {
                                taxRates.Add(taxRate, sciTax);
                            }
                            else
                            {
                                taxRates[taxRate] = taxRates[taxRate] + sciTax;
                            }
                        }
                    }
                           
                }
            }

                

            //checkout attributes
            if (customer != null)
            {
                var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        var caExclTax = _taxService.GetCheckoutAttributePrice(attributeValue, false, customer, out var taxRate);
                        var caInclTax = _taxService.GetCheckoutAttributePrice(attributeValue, true, customer, out taxRate);
                        subTotalExclTaxWithoutDiscount += caExclTax;
                        subTotalInclTaxWithoutDiscount += caInclTax;

                        //tax rates
                        var caTax = caInclTax - caExclTax;
                        if (taxRate <= decimal.Zero || caTax <= decimal.Zero)
                            continue;

                        if (!taxRates.ContainsKey(taxRate))
                        {
                            taxRates.Add(taxRate, caTax);
                        }
                        else
                        {
                            taxRates[taxRate] = taxRates[taxRate] + caTax;
                        }
                    }
                }
            }

            //subtotal without discount
            subTotalWithoutDiscount = includingTax ? subTotalInclTaxWithoutDiscount : subTotalExclTaxWithoutDiscount;
            if (subTotalWithoutDiscount < decimal.Zero)
                subTotalWithoutDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithoutDiscount = _freshPriceCalculationService.RoundPrice(subTotalWithoutDiscount);

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            var discountAmountExclTax = GetOrderSubtotalDiscount(customer, subTotalExclTaxWithoutDiscount, out appliedDiscounts);
            if (subTotalExclTaxWithoutDiscount < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTaxWithoutDiscount;
            var discountAmountInclTax = discountAmountExclTax;
            //subtotal with discount (excl tax)
            var subTotalExclTaxWithDiscount = subTotalExclTaxWithoutDiscount - discountAmountExclTax;
            var subTotalInclTaxWithDiscount = subTotalExclTaxWithDiscount;

            //add tax for shopping items & checkout attributes
            var tempTaxRates = new Dictionary<decimal, decimal>(taxRates);
            foreach (var kvp in tempTaxRates)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;

                if (taxValue == decimal.Zero)
                    continue;

                //discount the tax amount that applies to subtotal items
                if (subTotalExclTaxWithoutDiscount > decimal.Zero)
                {
                    var discountTax = taxRates[taxRate] * (discountAmountExclTax / subTotalExclTaxWithoutDiscount);
                    discountAmountInclTax += discountTax;
                    taxValue = taxRates[taxRate] - discountTax;
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        taxValue = _freshPriceCalculationService.RoundPrice(taxValue);
                    taxRates[taxRate] = taxValue;
                }

                //subtotal with discount (incl tax)
                subTotalInclTaxWithDiscount += taxValue;
            }

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                discountAmountInclTax = _freshPriceCalculationService.RoundPrice(discountAmountInclTax);
                discountAmountExclTax = _freshPriceCalculationService.RoundPrice(discountAmountExclTax);
            }

            if (includingTax)
            {
                subTotalWithDiscount = subTotalInclTaxWithDiscount;
                discountAmount = discountAmountInclTax;
            }
            else
            {
                subTotalWithDiscount = subTotalExclTaxWithDiscount;
                discountAmount = discountAmountExclTax;
            }

            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithDiscount = _freshPriceCalculationService.RoundPrice(subTotalWithDiscount);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetFreshShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool includingTax,
            out decimal taxRate, out List<DiscountForCaching> appliedDiscounts)
        {
            decimal? shippingTotal = null;
            decimal? shippingTotalTaxed = null;
            appliedDiscounts = new List<DiscountForCaching>();
            taxRate = decimal.Zero;

            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;

            var isFreeShipping = IsFreeShipping(cart,fCart);
            if (isFreeShipping)
                return decimal.Zero;

            ShippingOption shippingOption = null;
            if (customer != null)
                shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);

            if (shippingOption != null)
            {
                //use last shipping option (get from cache)
                shippingTotal = AdjustShippingRate(shippingOption.Rate, cart,fCart, out appliedDiscounts);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                    shippingAddress = customer.ShippingAddress;

                var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                if (!shippingRateComputationMethods.Any() && !_shippingSettings.AllowPickUpInStore)
                    throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var shippingRateComputationMethod = shippingRateComputationMethods[0];

                    var shippingOptionRequests = _shippingService.CreateShippingOptionRequests(cart,
                        shippingAddress,
                        _storeContext.CurrentStore.Id,
                        out _);
                    decimal? fixedRate = null;
                    foreach (var shippingOptionRequest in shippingOptionRequests)
                    {
                        //calculate fixed rates for each request-package
                        var fixedRateTmp = shippingRateComputationMethod.GetFixedRate(shippingOptionRequest);
                        if (!fixedRateTmp.HasValue)
                            continue;

                        if (!fixedRate.HasValue)
                            fixedRate = decimal.Zero;

                        fixedRate += fixedRateTmp.Value;
                    }

                    if (fixedRate.HasValue)
                    {
                        //adjust shipping rate
                        shippingTotal = AdjustShippingRate(fixedRate.Value, cart,fCart, out appliedDiscounts);
                    }
                }
            }

            if (!shippingTotal.HasValue)
                return null;

            if (shippingTotal.Value < decimal.Zero)
                shippingTotal = decimal.Zero;

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotal = _freshPriceCalculationService.RoundPrice(shippingTotal.Value);

            shippingTotalTaxed = _taxService.GetShippingPrice(shippingTotal.Value,
                includingTax,
                customer,
                out taxRate);

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotalTaxed = _freshPriceCalculationService.RoundPrice(shippingTotalTaxed.Value);

            return shippingTotalTaxed;
        }

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>A value indicating whether shipping is free</returns>
        public virtual bool IsFreeShipping(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, decimal? subTotal = null)
        {
            //check whether customer is in a customer role with free shipping applied
            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            if (customer != null && customer.CustomerRoles.Where(role => role.Active).Any(role => role.FreeShipping))
                return true;

            //check whether all shopping cart items and their associated products marked as free shipping
            if (cart.All(shoppingCartItem => _shippingService.IsFreeShipping(shoppingCartItem)))
                return true;

            //free shipping over $X
            if (!_shippingSettings.FreeShippingOverXEnabled)
                return false;

            if (!subTotal.HasValue)
            {
                GetFreshShoppingCartSubTotal(cart, fCart, _shippingSettings.FreeShippingOverXIncludingTax, out _, out _, out _, out var subTotalWithDiscount);
                subTotal = subTotalWithDiscount;
            }

            //check whether we have subtotal enough to have free shipping
            if (subTotal.Value > _shippingSettings.FreeShippingOverXValue)
                return true;

            return false;
        }

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Adjusted shipping rate</returns>
        public virtual decimal AdjustShippingRate(decimal shippingRate,
            IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();

            //free shipping
            if (IsFreeShipping(cart, fCart))
                return decimal.Zero;

            //with additional shipping charges
            var adjustedRate = shippingRate + GetFreshShoppingCartAdditionalShippingCharge(cart);

            //discount
            var discountAmount = GetFreshShippingDiscount(cart.FirstOrDefault(item => item.Customer != null)?.Customer, adjustedRate, out appliedDiscounts);
            adjustedRate -= discountAmount;

            adjustedRate = Math.Max(adjustedRate, decimal.Zero);
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                adjustedRate = _freshPriceCalculationService.RoundPrice(adjustedRate);

            var count = 0;
            foreach (var item in fCart)
            {
                var tempDateDistinctList = item.GroupBy(x => x.MealDate).ToList();

                foreach(var tempDateDistinct in tempDateDistinctList)
                {
                  var tempCount = tempDateDistinct.Select(x => x.MealTime).Distinct().Count();
                    if (tempCount > 1)
                    {
                        count = count + tempCount - 1;
                    }
                }
                count = count + tempDateDistinctList.Count();
               
            }

            adjustedRate = count * adjustedRate;

            return adjustedRate;
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetFreshShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool includingTax)
        {
            return GetFreshShoppingCartShippingTotal(cart, fCart,includingTax, out _);
        }


        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetFreshShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool includingTax,
            out decimal taxRate)
        {
            return GetFreshShoppingCartShippingTotal(cart, fCart, includingTax, out taxRate, out _);
        }

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Additional shipping charge</returns>
        public virtual decimal GetFreshShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart)
        {
            return cart.Sum(shoppingCartItem => _shippingService.GetAdditionalShippingCharge(shoppingCartItem));
        }


        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping discount</returns>
        protected virtual decimal GetFreshShippingDiscount(Customer customer, decimal shippingTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            var shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return shippingDiscountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !_discountService.ContainsDiscount(allowedDiscounts, discount))
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, shippingTotal, out shippingDiscountAmount);

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingDiscountAmount = _freshPriceCalculationService.RoundPrice(shippingDiscountAmount);

            return shippingDiscountAmount;
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            return GetTaxTotal(cart, fCart, out _, usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart,
            out SortedDictionary<decimal, decimal> taxRates, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            taxRates = new SortedDictionary<decimal, decimal>();

            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            var paymentMethodSystemName = string.Empty;
            if (customer != null)
            {
                paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            }

            //order sub total (items + checkout attributes)
            var subTotalTaxTotal = decimal.Zero;
            GetFreshShoppingCartSubTotal(cart, fCart, false, out _, out _, out _, out _, out var orderSubTotalTaxRates);
            foreach (var kvp in orderSubTotalTaxRates)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;
                subTotalTaxTotal += taxValue;

                if (taxRate <= decimal.Zero || taxValue <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(taxRate))
                    taxRates.Add(taxRate, taxValue);
                else
                    taxRates[taxRate] = taxRates[taxRate] + taxValue;
            }

            //shipping
            var shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                var shippingExclTax = GetFreshShoppingCartShippingTotal(cart, fCart, false, out var taxRate);
                var shippingInclTax = GetFreshShoppingCartShippingTotal(cart, fCart, true, out taxRate);
                if (shippingExclTax.HasValue && shippingInclTax.HasValue)
                {
                    shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                    //ensure that tax is equal or greater than zero
                    if (shippingTax < decimal.Zero)
                        shippingTax = decimal.Zero;

                    //tax rates
                    if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                            taxRates.Add(taxRate, shippingTax);
                        else
                            taxRates[taxRate] = taxRates[taxRate] + shippingTax;
                    }
                }
            }

            //payment method additional fee
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && _taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                var paymentMethodAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, out var taxRate);
                var paymentMethodAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, out taxRate);

                paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
                //ensure that tax is equal or greater than zero
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                //tax rates
                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            var taxTotal = subTotalTaxTotal + shippingTax + paymentMethodAdditionalFeeTax;
            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;
            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = _freshPriceCalculationService.RoundPrice(taxTotal);
            return taxTotal;
        }


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
        public virtual decimal? GetFreshShoppingCartTotal(IList<ShoppingCartItem> cart, IGrouping<Guid?, FCart>[] fCart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            redeemedRewardPoints = 0;
            redeemedRewardPointsAmount = decimal.Zero;

            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            var paymentMethodSystemName = string.Empty;
            if (customer != null)
            {
                paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            }

            //subtotal without tax
            GetFreshShoppingCartSubTotal(cart,fCart, false, out _, out _, out _, out var subTotalWithDiscountBase);
            //subtotal with discount
            var subtotalBase = subTotalWithDiscountBase;

            //shipping without tax
            var shoppingCartShipping = GetFreshShoppingCartShippingTotal(cart,fCart, false);

            //payment method additional fee without tax
            var paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && !string.IsNullOrEmpty(paymentMethodSystemName))
            {
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart,
                    paymentMethodSystemName);
                paymentMethodAdditionalFeeWithoutTax =
                    _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee,
                        false, customer);
            }

            //tax
            var shoppingCartTax = GetTaxTotal(cart,fCart, usePaymentMethodAdditionalFee);

            //order total
            var resultTemp = decimal.Zero;
            resultTemp += subtotalBase;
            if (shoppingCartShipping.HasValue)
            {
                resultTemp += shoppingCartShipping.Value;
            }

            resultTemp += paymentMethodAdditionalFeeWithoutTax;
            resultTemp += shoppingCartTax;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = _freshPriceCalculationService.RoundPrice(resultTemp);

            //order total discount
            discountAmount = GetOrderTotalDiscount(customer, resultTemp, out appliedDiscounts);

            //sub totals with discount        
            if (resultTemp < discountAmount)
                discountAmount = resultTemp;

            //reduce subtotal
            resultTemp -= discountAmount;

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = _freshPriceCalculationService.RoundPrice(resultTemp);

            //let's apply gift cards now (gift cards that can be used)
            appliedGiftCards = new List<AppliedGiftCard>();
            AppliedGiftCards(cart, appliedGiftCards, customer, ref resultTemp);

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = _freshPriceCalculationService.RoundPrice(resultTemp);

            if (!shoppingCartShipping.HasValue)
            {
                //we have errors
                return null;
            }

            var orderTotal = resultTemp;

            //reward points
            SetRewardPoints(ref redeemedRewardPoints, ref redeemedRewardPointsAmount, useRewardPoints, customer, orderTotal);

            orderTotal = orderTotal - redeemedRewardPointsAmount;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                orderTotal = _freshPriceCalculationService.RoundPrice(orderTotal);
            return orderTotal;
        }

        /// <summary>
        /// Apply gift cards
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="customer">Customer</param>
        /// <param name="resultTemp"></param>
        protected virtual void AppliedGiftCards(IList<ShoppingCartItem> cart, List<AppliedGiftCard> appliedGiftCards,
            Customer customer, ref decimal resultTemp)
        {
            if (_shoppingCartService.ShoppingCartIsRecurring(cart))
                return;

            //we don't apply gift cards for recurring products
            var giftCards = _giftCardService.GetActiveGiftCardsAppliedByCustomer(customer);
            if (giftCards == null)
                return;

            foreach (var gc in giftCards)
            {
                if (resultTemp <= decimal.Zero) continue;

                var remainingAmount = _giftCardService.GetGiftCardRemainingAmount(gc);
                var amountCanBeUsed = resultTemp > remainingAmount ? remainingAmount : resultTemp;

                //reduce subtotal
                resultTemp -= amountCanBeUsed;

                var appliedGiftCard = new AppliedGiftCard
                {
                    GiftCard = gc,
                    AmountCanBeUsed = amountCanBeUsed
                };
                appliedGiftCards.Add(appliedGiftCard);
            }
        }


        /// <summary>
        /// Set reward points
        /// </summary>
        /// <param name="redeemedRewardPoints">Redeemed reward points</param>
        /// <param name="redeemedRewardPointsAmount">Redeemed reward points amount</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        protected virtual void SetRewardPoints(ref int redeemedRewardPoints, ref decimal redeemedRewardPointsAmount,
            bool? useRewardPoints, Customer customer, decimal orderTotal)
        {
            if (!_rewardPointsSettings.Enabled)
                return;

            if (!useRewardPoints.HasValue)
                useRewardPoints = _genericAttributeService.GetAttribute<bool>(customer, NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, _storeContext.CurrentStore.Id);

            if (!useRewardPoints.Value)
                return;

            var rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            rewardPointsBalance = _rewardPointService.GetReducedPointsBalance(rewardPointsBalance);

            if (!CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                return;

            var rewardPointsBalanceAmount = ConvertRewardPointsToAmount(rewardPointsBalance);

            if (orderTotal <= decimal.Zero)
                return;

            if (orderTotal > rewardPointsBalanceAmount)
            {
                redeemedRewardPoints = rewardPointsBalance;
                redeemedRewardPointsAmount = rewardPointsBalanceAmount;
            }
            else
            {
                redeemedRewardPointsAmount = orderTotal;
                redeemedRewardPoints = ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        public virtual bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            if (_rewardPointsSettings.MinimumRewardPointsToUse <= 0)
                return true;

            return rewardPoints >= _rewardPointsSettings.MinimumRewardPointsToUse;
        }

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            if (rewardPoints <= 0)
                return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = _freshPriceCalculationService.RoundPrice(result);
            return result;
        }

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        public virtual int ConvertAmountToRewardPoints(decimal amount)
        {
            var result = 0;
            if (amount <= 0)
                return 0;

            if (_rewardPointsSettings.ExchangeRate > 0)
                result = (int)Math.Ceiling(amount / _rewardPointsSettings.ExchangeRate);
            return result;
        }



        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderTotalDiscount(Customer customer, decimal orderTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !_discountService.ContainsDiscount(allowedDiscounts, discount))
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = _freshPriceCalculationService.RoundPrice(discountAmount);

            return discountAmount;
        }

        #endregion
    }
}
