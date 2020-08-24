using Castle.Core.Logging;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using Nop.Core.Caching;
using Nop.Core.Domain.Discounts;
using Nop.Core.Infrastructure;

namespace Nop.Services.Fresh
{
    public partial class FreshPriceCalculationService:IFreshPriceCalculationService
    {
        #region Feilds
        private readonly AddressSettings _addressSettings = EngineContext.Current.Resolve<AddressSettings>();
        private readonly CustomerSettings _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
        private readonly IAddressService _addressService = EngineContext.Current.Resolve<IAddressService>();
        private readonly ICountryService _countryService = EngineContext.Current.Resolve<ICountryService>();
        private readonly IGenericAttributeService _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        private readonly IGeoLookupService _geoLookupService = EngineContext.Current.Resolve<IGeoLookupService>();
        private readonly IPluginFinder _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
        private readonly IStateProvinceService _stateProvinceService = EngineContext.Current.Resolve<IStateProvinceService>();
        private readonly IStoreContext _storeContext = EngineContext.Current.Resolve<IStoreContext>();
        private readonly IWebHelper _webHelper = EngineContext.Current.Resolve<IWebHelper>();
        private readonly IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
        private readonly ShippingSettings _shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
        private readonly TaxSettings _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
        private readonly ITaxService _taxService = EngineContext.Current.Resolve<ITaxService>();
        private readonly IProductAttributeParser _productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
        private readonly ShoppingCartSettings _shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        private readonly CatalogSettings _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
        private readonly IStaticCacheManager _cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
        private readonly IDiscountService _discountService = EngineContext.Current.Resolve<IDiscountService>();
        private readonly ICategoryService _categoryService = EngineContext.Current.Resolve<ICategoryService>();
        private readonly IManufacturerService _manufacturerService = EngineContext.Current.Resolve<IManufacturerService>();
        private readonly ICurrencyService _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
        private readonly CurrencySettings _currencySettings = EngineContext.Current.Resolve<CurrencySettings>();
        #endregion


        //#region Ctor
        //public FreshPriceCalculationService(AddressSettings addressSettings,
        //  CustomerSettings customerSettings,
        //  IAddressService addressService,
        //  ICountryService countryService,
        //  IGenericAttributeService genericAttributeService,
        //  IGeoLookupService geoLookupService,
        //  ILogger logger,
        //  IPluginFinder pluginFinder,
        //  IStateProvinceService stateProvinceService,
        //  IStoreContext storeContext,
        //  IWebHelper webHelper,
        //  IWorkContext workContext,
        //  ShippingSettings shippingSettings,
        //  TaxSettings taxSettings,
        //  ITaxService taxService,
        //  IProductAttributeParser productAttributeParser,
        //  ShoppingCartSettings shoppingCartSettings,
        //  IProductService productService,
        //  CatalogSettings catalogSettings,
        //  IStaticCacheManager cacheManager,
        //  IDiscountService discountService,
        //  ICategoryService categoryService,
        //  IManufacturerService manufacturerService,
        //  ICurrencyService currencyService,
        //  CurrencySettings currencySettings)
        //{
        //    this._addressSettings = addressSettings;
        //    this._customerSettings = customerSettings;
        //    this._addressService = addressService;
        //    this._countryService = countryService;
        //    this._genericAttributeService = genericAttributeService;
        //    this._geoLookupService = geoLookupService;
        //    this._logger = logger;
        //    this._pluginFinder = pluginFinder;
        //    this._stateProvinceService = stateProvinceService;
        //    this._storeContext = storeContext;
        //    this._webHelper = webHelper;
        //    this._workContext = workContext;
        //    this._shippingSettings = shippingSettings;
        //    this._taxSettings = taxSettings;
        //    this._taxService = taxService;
        //    this._productAttributeParser = productAttributeParser;
        //    this._shoppingCartSettings = shoppingCartSettings;
        //    this._productService = productService;
        //    this._catalogSettings = catalogSettings;
        //    this._cacheManager = cacheManager;
        //    this._discountService = discountService;
        //    this._categoryService = categoryService;
        //    this._manufacturerService = manufacturerService;
        //    this._currencyService = currencyService;
        //    this._currencySettings = currencySettings;
        //}

        //#endregion

        public FreshPriceCalculationService()
        {

        }

        #region Nested classes

        /// <summary>
        /// Product price (for caching)
        /// </summary>
        [Serializable]
        protected class ProductPriceForCaching
        {
            public ProductPriceForCaching()
            {
                this.AppliedDiscounts = new List<DiscountForCaching>();
            }

            /// <summary>
            /// Price
            /// </summary>
            public decimal Price { get; set; }

            /// <summary>
            /// Applied discount amount
            /// </summary>
            public decimal AppliedDiscountAmount { get; set; }

            /// <summary>
            /// Applied discounts
            /// </summary>
            public List<DiscountForCaching> AppliedDiscounts { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets allowed discounts applied to product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscountsAppliedToProduct(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            if (!product.HasDiscountsApplied)
                return allowedDiscounts;

            //we use this property ("HasDiscountsApplied") for performance optimization to avoid unnecessary database calls
            foreach (var discount in product.AppliedDiscounts)
            {
                if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                    discount.DiscountType == DiscountType.AssignedToSkus)
                    allowedDiscounts.Add(_discountService.MapDiscount(discount));
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to categories
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscountsAppliedToCategories(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //load cached discount models (performance optimization)
            foreach (var discount in _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToCategories))
            {
                //load identifier of categories with this discount applied to
                var discountCategoryIds = _discountService.GetAppliedCategoryIds(discount, customer);

                //compare with categories of this product
                var productCategoryIds = new List<int>();
                if (discountCategoryIds.Any())
                {
                    //load identifier of categories of this product
                    var cacheKey = string.Format(NopCatalogDefaults.ProductCategoryIdsModelCacheKey,
                        product.Id,
                        string.Join(",", customer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id);
                    productCategoryIds = _cacheManager.Get(cacheKey, () =>
                        _categoryService
                        .GetProductCategoriesByProductId(product.Id)
                        .Select(x => x.CategoryId)
                        .ToList());
                }

                foreach (var categoryId in productCategoryIds)
                {
                    if (!discountCategoryIds.Contains(categoryId))
                        continue;

                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !_discountService.ContainsDiscount(allowedDiscounts, discount))
                        allowedDiscounts.Add(discount);
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscounts(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //discounts applied to products
            foreach (var discount in GetAllowedDiscountsAppliedToProduct(product, customer))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to categories
            foreach (var discount in GetAllowedDiscountsAppliedToCategories(product, customer))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to manufacturers
            foreach (var discount in GetAllowedDiscountsAppliedToManufacturers(product, customer))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to manufacturers
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscountsAppliedToManufacturers(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            foreach (var discount in _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers))
            {
                //load identifier of manufacturers with this discount applied to
                var discountManufacturerIds = _discountService.GetAppliedManufacturerIds(discount, customer);

                //compare with manufacturers of this product
                var productManufacturerIds = new List<int>();
                if (discountManufacturerIds.Any())
                {
                    //load identifier of manufacturers of this product
                    var cacheKey = string.Format(NopCatalogDefaults.ProductManufacturerIdsModelCacheKey,
                        product.Id,
                        string.Join(",", customer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id);
                    productManufacturerIds = _cacheManager.Get(cacheKey, () =>
                        _manufacturerService
                        .GetProductManufacturersByProductId(product.Id)
                        .Select(x => x.ManufacturerId)
                        .ToList());
                }

                foreach (var manufacturerId in productManufacturerIds)
                {
                    if (!discountManufacturerIds.Contains(manufacturerId))
                        continue;

                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        !_discountService.ContainsDiscount(allowedDiscounts, discount))
                        allowedDiscounts.Add(discount);
                }
            }

            return allowedDiscounts;
        }


        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="productPriceWithoutDiscount">Already calculated product price without discount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Discount amount</returns>
        protected virtual decimal GetDiscountAmount(Product product,
            Customer customer,
            decimal productPriceWithoutDiscount,
            out List<DiscountForCaching> appliedDiscounts)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            appliedDiscounts = new List<DiscountForCaching>();
            var appliedDiscountAmount = decimal.Zero;

            //we don't apply discounts to products with price entered by a customer
            if (product.CustomerEntersPrice)
                return appliedDiscountAmount;

            //discounts are disabled
            if (_catalogSettings.IgnoreDiscounts)
                return appliedDiscountAmount;

            var allowedDiscounts = GetAllowedDiscounts(product, customer);

            //no discounts
            if (!allowedDiscounts.Any())
                return appliedDiscountAmount;

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, productPriceWithoutDiscount, out appliedDiscountAmount);
            return appliedDiscountAmount;
        }
        #endregion

        public virtual decimal GetFreshProductPrice(Product product, decimal price,
            out decimal taxRate)
        {
            var customer = _workContext.CurrentCustomer;
            var includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            var priceIncludesTax = _taxSettings.PricesIncludeTax;
            var taxCategoryId = 0;
            return GetFreshProductPrice(product, taxCategoryId, price, includingTax,
                customer, priceIncludesTax, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public virtual decimal GetFreshProductPrice(Product product, decimal price,
            bool includingTax, Customer customer, out decimal taxRate)
        {
            var priceIncludesTax = _taxSettings.PricesIncludeTax;
            var taxCategoryId = 0;
            return GetFreshProductPrice(product, taxCategoryId, price, includingTax,
                customer, priceIncludesTax, out taxRate);
        }

        public virtual decimal GetFreshProductPrice(Product product, int taxCategoryId,
           decimal price, bool includingTax, Customer customer,
           bool priceIncludesTax, out decimal taxRate)
        {
            //no need to calculate tax rate if passed "price" is 0
            if (price == decimal.Zero)
            {
                taxRate = decimal.Zero;
                return taxRate;
            }

            _taxService.GetTaxRate(product, taxCategoryId, customer, price, out taxRate, out var isTaxable);

            if (priceIncludesTax)
            {
                //"price" already includes tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    if (!isTaxable)
                    {
                        //but our request is not taxable
                        //hence we should calculate price WITHOUT tax
                        price = CalculatePrice(price, taxRate, false);
                    }
                }
                else
                {
                    //we should calculate price WITHOUT tax
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            else
            {
                //"price" doesn't include tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    //do it only when price is taxable
                    if (isTaxable)
                    {
                        price = CalculatePrice(price, taxRate, true);
                    }
                }
            }

            if (!isTaxable)
            {
                //we return 0% tax rate in case a request is not taxable
                taxRate = decimal.Zero;
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;

            return price;
        }



        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPriceforFreshItem(ShoppingCartItem shoppingCartItem, IGrouping<Guid?, FCart> fCart,
            bool includeDiscounts = true)
        {
            return GetUnitPriceforFreshItem(shoppingCartItem, includeDiscounts, fCart, out _, out _);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPriceforFreshItem(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts, IGrouping<Guid?, FCart> fCart,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            return GetUnitPriceforFreshItem(shoppingCartItem.Product,
                fCart,
                shoppingCartItem.Customer,
                shoppingCartItem.ShoppingCartType,
                shoppingCartItem.Quantity,
                shoppingCartItem.AttributesXml,
                shoppingCartItem.CustomerEnteredPrice,
                shoppingCartItem.RentalStartDateUtc,
                shoppingCartItem.RentalEndDateUtc,
                includeDiscounts,
                out discountAmount,
                out appliedDiscounts);
        }


        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Product attributes (XML format)</param>
        /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
        /// <param name="rentalStartDate">Rental start date (null for not rental products)</param>
        /// <param name="rentalEndDate">Rental end date (null for not rental products)</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPriceforFreshItem(Product product,
            IGrouping<Guid?, FCart> fCart,
            Customer customer,
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate, DateTime? rentalEndDate,
            bool includeDiscounts,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            discountAmount = decimal.Zero;
            appliedDiscounts = new List<DiscountForCaching>();

            decimal finalPrice;

            var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            if (combination?.OverriddenPrice.HasValue ?? false)
            {
                finalPrice = GetFinalPriceforFreshItem(product,
                        customer,
                        combination.OverriddenPrice.Value,
                        decimal.Zero,
                        includeDiscounts,
                        quantity,
                        product.IsRental ? rentalStartDate : null,
                        product.IsRental ? rentalEndDate : null,
                        out discountAmount, out appliedDiscounts);
            }
            else
            {
                //summarize price of all attributes
                var attributesTotalPrice = decimal.Zero;
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        attributesTotalPrice += GetProductAttributeValuePriceAdjustment(attributeValue, customer, product.CustomerEntersPrice ? (decimal?)customerEnteredPrice : null);
                    }
                }

                //get price of a product (with previously calculated price of all attributes)
                if (product.CustomerEntersPrice)
                {
                    finalPrice = customerEnteredPrice;
                }
                else
                {
                    int qty;
                    if (_shoppingCartSettings.GroupTierPricesForDistinctShoppingCartItems)
                    {
                        //the same products with distinct product attributes could be stored as distinct "ShoppingCartItem" records
                        //so let's find how many of the current products are in the cart
                        //var a = fCart.GroupBy(x => x.MealOrderId).ToArray();
                        //var tempQuantity = 0;
                        //foreach (var temp in a)
                        //{
                        //    tempQuantity = temp.Count();

                        //}
                        //qty = tempQuantity;
                        //qty = customer.ShoppingCartItems
                        //    .Where(x => x.ProductId == product.Id)
                        //    .Where(x => x.ShoppingCartType == shoppingCartType)
                        //    .Sum(x => x.Quantity);

                        var tempQuantity = 0;
                        if (fCart.Count() != 0)
                        {
                            tempQuantity = fCart.Count();
                        }

                        qty = tempQuantity;
                        //fCart
                        if (qty == 0)
                        {
                            qty = quantity;
                        }
                    }
                    else
                    {
                        qty = quantity;
                    }

                    finalPrice = GetFinalPriceforFreshItem(product,
                        customer,
                        attributesTotalPrice,
                        includeDiscounts,
                        qty,
                        product.IsRental ? rentalStartDate : null,
                        product.IsRental ? rentalEndDate : null,
                        out discountAmount, out appliedDiscounts);
                }
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                finalPrice = this.RoundPrice(finalPrice);

            return finalPrice;
        }

        public virtual decimal GetProductAttributeValuePriceAdjustment(ProductAttributeValue value, Customer customer, decimal? productPrice = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var adjustment = decimal.Zero;
            switch (value.AttributeValueType)
            {
                case AttributeValueType.Simple:
                    //simple attribute
                    if (value.PriceAdjustmentUsePercentage)
                    {
                        if (!productPrice.HasValue)
                            productPrice = GetFinalPriceforFreshItem(value.ProductAttributeMapping.Product, customer);

                        adjustment = (decimal)((float)productPrice * (float)value.PriceAdjustment / 100f);
                    }
                    else
                    {
                        adjustment = value.PriceAdjustment;
                    }

                    break;
                case AttributeValueType.AssociatedToProduct:
                    //bundled product
                    var associatedProduct = _productService.GetProductById(value.AssociatedProductId);
                    if (associatedProduct != null)
                    {
                        adjustment = GetFinalPriceforFreshItem(associatedProduct, _workContext.CurrentCustomer) * value.Quantity;
                    }

                    break;
                default:
                    break;
            }

            return adjustment;
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPriceforFreshItem(Product product,
            Customer customer,
            decimal additionalCharge = decimal.Zero,
            bool includeDiscounts = true,
            int quantity = 1)
        {
            return GetFinalPriceforFreshItem(product, customer, additionalCharge, includeDiscounts,
                quantity, out _, out _);
        }


        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPriceforFreshItem(Product product,
            Customer customer,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts)
        {
            return GetFinalPriceforFreshItem(product, customer,
                additionalCharge, includeDiscounts, quantity,
                null, null,
                out discountAmount, out appliedDiscounts);
        }


        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPriceforFreshItem(Product product,
            Customer customer,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts)
        {
            return GetFinalPriceforFreshItem(product, customer, null, additionalCharge, includeDiscounts, quantity,
                rentalStartDate, rentalEndDate, out discountAmount, out appliedDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="overriddenProductPrice">Overridden product price. If specified, then it'll be used instead of a product price. For example, used with product attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPriceforFreshItem(Product product,
            Customer customer,
            decimal? overriddenProductPrice,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            discountAmount = decimal.Zero;
            appliedDiscounts = new List<DiscountForCaching>();

            var cacheKey = string.Format(NopCatalogDefaults.ProductPriceModelCacheKey,
                product.Id,
                overriddenProductPrice?.ToString(CultureInfo.InvariantCulture),
                additionalCharge.ToString(CultureInfo.InvariantCulture),
                includeDiscounts,
                quantity,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var cacheTime = _catalogSettings.CacheProductPrices ? 60 : 0;
            //we do not cache price for rental products
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (product.IsRental)
                cacheTime = 0;
            var cachedPrice = _cacheManager.Get(cacheKey, () =>
            {
                var result = new ProductPriceForCaching();

                //initial price
                var price = overriddenProductPrice ?? product.Price;

                //tier prices
                var tierPrice = _productService.GetPreferredTierPrice(product, customer, _storeContext.CurrentStore.Id, quantity);
                if (tierPrice != null)
                    price = tierPrice.Price;

                //additional charge
                price = price + additionalCharge;

                //rental products
                if (product.IsRental)
                    if (rentalStartDate.HasValue && rentalEndDate.HasValue)
                        price = price * _productService.GetRentalPeriods(product, rentalStartDate.Value, rentalEndDate.Value);

                if (includeDiscounts)
                {
                    //discount
                    var tmpDiscountAmount = GetDiscountAmount(product, customer, price, out var tmpAppliedDiscounts);
                    price = price - tmpDiscountAmount;

                    if (tmpAppliedDiscounts?.Any() ?? false)
                    {
                        result.AppliedDiscounts = tmpAppliedDiscounts;
                        result.AppliedDiscountAmount = tmpDiscountAmount;
                    }
                }

                if (price < decimal.Zero)
                    price = decimal.Zero;

                result.Price = price;
                return result;
            }, cacheTime);

            if (!includeDiscounts)
                return cachedPrice.Price;

            if (!cachedPrice.AppliedDiscounts.Any())
                return cachedPrice.Price;

            appliedDiscounts.AddRange(cachedPrice.AppliedDiscounts);
            discountAmount = cachedPrice.AppliedDiscountAmount;

            return cachedPrice.Price;
        }

        /// <summary>
        /// Round a product or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency; pass null to use the primary store currency</param>
        /// <returns>Rounded value</returns>
        public virtual decimal RoundPrice(decimal value, Currency currency = null)
        {
            //we use this method because some currencies (e.g. Gungarian Forint or Swiss Franc) use non-standard rules for rounding
            //you can implement any rounding logic here

            currency = currency ?? _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            return this.Round(value, currency.RoundingType);
        }


        /// <summary>
        /// Calculated price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="percent">Percent</param>
        /// <param name="increase">Increase</param>
        /// <returns>New price</returns>
        protected virtual decimal CalculatePrice(decimal price, decimal percent, bool increase)
        {
            if (percent == decimal.Zero)
                return price;

            decimal result;
            if (increase)
            {
                result = price * (1 + percent / 100);
            }
            else
            {
                result = price - price / (100 + percent) * percent;
            }

            return result;
        }

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="roundingType">The rounding type</param>
        /// <returns>Rounded value</returns>
        public virtual decimal Round(decimal value, RoundingType roundingType)
        {
            //default round (Rounding001)
            var rez = Math.Round(value, 2);
            var fractionPart = (rez - Math.Truncate(rez)) * 10;

            //cash rounding not needed
            if (fractionPart == 0)
                return rez;

            //Cash rounding (details: https://en.wikipedia.org/wiki/Cash_rounding)
            switch (roundingType)
            {
                //rounding with 0.05 or 5 intervals
                case RoundingType.Rounding005Up:
                case RoundingType.Rounding005Down:
                    fractionPart = (fractionPart - Math.Truncate(fractionPart)) * 10;

                    fractionPart = fractionPart % 5;
                    if (fractionPart == 0)
                        break;

                    if (roundingType == RoundingType.Rounding005Up)
                        fractionPart = 5 - fractionPart;
                    else
                        fractionPart = fractionPart * -1;

                    rez += fractionPart / 100;
                    break;
                //rounding with 0.10 intervals
                case RoundingType.Rounding01Up:
                case RoundingType.Rounding01Down:
                    fractionPart = (fractionPart - Math.Truncate(fractionPart)) * 10;

                    if (roundingType == RoundingType.Rounding01Down && fractionPart == 5)
                        fractionPart = -5;
                    else
                        fractionPart = fractionPart < 5 ? fractionPart * -1 : 10 - fractionPart;

                    rez += fractionPart / 100;
                    break;
                //rounding with 0.50 intervals
                case RoundingType.Rounding05:
                    fractionPart *= 10;
                    fractionPart = fractionPart < 25 ? fractionPart * -1 : fractionPart < 50 || fractionPart < 75 ? 50 - fractionPart : 100 - fractionPart;

                    rez += fractionPart / 100;
                    break;
                //rounding with 1.00 intervals
                case RoundingType.Rounding1:
                case RoundingType.Rounding1Up:
                    fractionPart *= 10;

                    if (roundingType == RoundingType.Rounding1Up && fractionPart > 0)
                        rez = Math.Truncate(rez) + 1;
                    else
                        rez = fractionPart < 50 ? Math.Truncate(rez) : Math.Truncate(rez) + 1;

                    break;
                case RoundingType.Rounding001:
                default:
                    break;
            }

            return rez;
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public virtual decimal GetSubTotalForFreshCart(ShoppingCartItem shoppingCartItem, IGrouping<Guid?, FCart> fCart,
            bool includeDiscounts = true)
        {
            return GetSubTotalForFreshCart(shoppingCartItem, fCart, includeDiscounts, out var _, out var _, out var _);
        }


        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="maximumDiscountQty">Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items</param>
        /// <returns>Shopping cart item sub total</returns>
        public virtual decimal GetSubTotalForFreshCart(ShoppingCartItem shoppingCartItem,
            IGrouping<Guid?, FCart> fCart,
            bool includeDiscounts,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts,
            out int? maximumDiscountQty)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            decimal subTotal;
            maximumDiscountQty = null;

            //unit price
            var unitPrice = GetUnitPriceforFreshItem(shoppingCartItem,includeDiscounts, fCart, out discountAmount,out appliedDiscounts);

            //var unitPrice = GetUnitPriceforFreshItem(shoppingCartItem, fCarts, includeDiscounts,
            //   out discountAmount, out appliedDiscounts);

            //discount
            if (appliedDiscounts.Any())
            {
                //we can properly use "MaximumDiscountedQuantity" property only for one discount (not cumulative ones)
                DiscountForCaching oneAndOnlyDiscount = null;
                if (appliedDiscounts.Count == 1)
                    oneAndOnlyDiscount = appliedDiscounts.First();

                if ((oneAndOnlyDiscount?.MaximumDiscountedQuantity.HasValue ?? false) &&
                    shoppingCartItem.Quantity > oneAndOnlyDiscount.MaximumDiscountedQuantity.Value)
                {
                    maximumDiscountQty = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                    //we cannot apply discount for all shopping cart items
                    var discountedQuantity = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                    var discountedSubTotal = unitPrice * discountedQuantity;
                    discountAmount = discountAmount * discountedQuantity;

                    var notDiscountedQuantity = shoppingCartItem.Quantity - discountedQuantity;
                    var notDiscountedUnitPrice = GetUnitPriceforFreshItem(shoppingCartItem,fCart,false);
                    var notDiscountedSubTotal = notDiscountedUnitPrice * notDiscountedQuantity;

                    subTotal = discountedSubTotal + notDiscountedSubTotal;
                }
                else
                {
                    //discount is applied to all items (quantity)
                    //calculate discount amount for all items
                    discountAmount = discountAmount * shoppingCartItem.Quantity;

                    subTotal = unitPrice * shoppingCartItem.Quantity;
                }
            }
            else
            {
                subTotal = unitPrice * shoppingCartItem.Quantity;
            }

            return subTotal;
        }

    }
}
