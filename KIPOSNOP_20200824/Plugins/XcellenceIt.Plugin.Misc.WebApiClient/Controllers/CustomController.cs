using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using System.Reflection;
using Nop.Core.Domain.Discounts;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom;
using Nop.Services;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class CustomController : Controller
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly TaxSettings _taxSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPaymentService _paymentService;
        private readonly ILanguageService _languageService;
        private readonly OrderSettings _orderSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IShippingService _shippingService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILogger _logger;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IDiscountService _discountService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly AddressSettings _addressSettings;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly IGiftCardService _giftCardService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IUrlRecordService _urlRecordService;
        #endregion

        #region Ctor

        public CustomController(
        ICustomerService customerService,
        IShoppingCartService shoppingCartService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IGenericAttributeService genericAttributeService,
        TaxSettings taxSettings,
        ICountryService countryService,
        IStateProvinceService stateProvinceService,
        IOrderProcessingService orderProcessingService,
        IPermissionService permissionService,
        IProductService productService,
        ICurrencyService currencyService,
        IPriceFormatter priceFormatter,
        IPaymentService paymentService,
        ILanguageService languageService,
        OrderSettings orderSettings,
        CatalogSettings catalogSettings,
        IProductAttributeParser productAttributeParser,
        IDownloadService downloadService,
        ICacheManager cacheManager,
        IPriceCalculationService priceCalculationService,
        ITaxService taxService,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IWebHelper webHelper,
        ShoppingCartSettings shoppingCartSettings,
        IShippingService shippingService,
        IProductAttributeService productAttributeService,
        ILogger logger,
        ICheckoutAttributeFormatter checkoutAttributeFormatter,
        IDiscountService discountService,
        ICheckoutAttributeService checkoutAttributeService,
        ICheckoutAttributeParser checkoutAttributeParser,
        AddressSettings addressSettings,
        IProductAttributeFormatter productAttributeFormatter,
        IAddressAttributeFormatter addressAttributeFormatter,
        ShippingSettings shippingSettings,
        IGiftCardService giftCardService,
        IOrderTotalCalculationService orderTotalCalculationService,
        RewardPointsSettings rewardPointsSettings,
        IAddressAttributeService addressAttributeService,
        IAddressAttributeParser addressAttributeParser,
        IUrlRecordService urlRecordService)
        {
            this._customerService = customerService;
            this._shoppingCartService = shoppingCartService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._taxSettings = taxSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._productService = productService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._paymentService = paymentService;
            this._languageService = languageService;
            this._orderSettings = orderSettings;
            this._catalogSettings = catalogSettings;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._priceCalculationService = priceCalculationService;
            this._mediaSettings = mediaSettings;
            this._webHelper = webHelper;
            this._pictureService = pictureService;
            this._taxService = taxService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._shippingService = shippingService;
            this._productAttributeService = productAttributeService;
            this._logger = logger;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._discountService = discountService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._addressSettings = addressSettings;
            this._productAttributeFormatter = productAttributeFormatter;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._shippingSettings = shippingSettings;
            this._giftCardService = giftCardService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities 

        [NonAction]
        protected virtual void ParseRentalDates(Product product, string startControl, string endControl, out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;
            try
            {
                //currenly we support only "mm/dd/yy" format
                var formatProvider = new CultureInfo("en-US");
                startDate = DateTime.Parse(startControl.ToString(), formatProvider);
                endDate = DateTime.Parse(endControl.ToString(), formatProvider);
            }
            catch
            {
            }
        }

        [NonAction]
        protected virtual string PrepareShoppingCartModel(ShoppingCartModelResponse model,
           IList<ShoppingCartItem> cart, Customer currentCustomer, int currencyId, int storeId, int languageId, bool isEditable = true,
           bool validateCheckoutAttributes = false, bool prepareAndDisplayOrderReviewData = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);
            if (cart == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCart";

            if (model == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCartModel";

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;
            model.Items = new List<ShoppingCartModelResponse.ShoppingCartItemModel>();

            if (cart.Count == 0)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCart";

            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.CheckoutAttributes, storeId);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, currentCustomer);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, workingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //gift card and gift card boxes
            model.DiscountBox = new ShoppingCartModelResponse.DiscountBoxModel
            {
                Display = _shoppingCartSettings.ShowDiscountBox
            };
            var discountCouponCode = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.DiscountCouponCodeAttribute, storeId);
            var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountCouponCode)
                    .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, currentCustomer).IsValid);
            if (discount != null)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox = new ShoppingCartModelResponse.GiftCardBoxModel
            {
                Display = _shoppingCartSettings.ShowGiftCardBox
            };

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            model.Warnings = new List<string>();
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(storeId, !_shoppingCartService.ShoppingCartRequiresShipping(cart));
            model.CheckoutAttributes = new List<ShoppingCartModelResponse.CheckoutAttributeModel>();
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModelResponse.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name, languageId: languageId),
                    TextPrompt = _localizationService.GetLocalized(attribute, x => x.TextPrompt, languageId: languageId),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = new List<string>();
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        attributeModel.Values = new List<ShoppingCartModelResponse.CheckoutAttributeValueModel>();
                        var attributeValueModel = new ShoppingCartModelResponse.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attribute, x => x.Name, languageId: languageId),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attributeValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, workingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                var selectedCheckoutAttributes = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.CheckoutAttributes, storeId);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Count > 0)
                            {
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out DateTime selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }

                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var downloadGuidStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                                Guid.TryParse(downloadGuidStr, out Guid downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                    attributeModel.DefaultValue = download.DownloadGuid.ToString();
                            }
                        }
                        break;
                    default:
                        break;
                }
                model.CheckoutAttributes.Add(attributeModel);
            }

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModelResponse.ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                    ProductId = sci.Product.Id,
                    ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name, languageId: languageId),
                    ProductSeName = _urlRecordService.GetSeName(sci.Product, languageId: languageId),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                    sci.Product.ProductType == ProductType.SimpleProduct &&
                    (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Product.IsGiftCard) &&
                    sci.Product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = _productService.ParseAllowedQuantities(sci.Product);
                cartItemModel.AllowedQuantities = new List<SelectListItem>();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(sci.Product.RecurringCyclePeriod, languageId));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue
                         ? _productService.FormatRentalDate(sci.Product, sci.RentalStartDateUtc.Value)
                         : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(sci.Product, sci.RentalEndDateUtc.Value)
                        : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, workingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out decimal shoppingCartItemDiscountBase, out List<DiscountForCaching> scDiscounts, out int? maximumDiscountQty), out decimal taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, workingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, workingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName, languageId, storeId);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    currentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);
                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = new ShoppingCartModelResponse.OrderReviewDataModel
                {
                    Display = true
                };

                //billing info
                var billingAddress = currentCustomer.BillingAddress;
                if (billingAddress != null)
                {
                    PrepareAddressModel(model.OrderReviewData.BillingAddress,
                            address: billingAddress,
                            excludeProperties: false,
                            languageId: languageId,
                            addressSettings: _addressSettings);
                }

                //shipping info
                if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
                {
                    model.OrderReviewData.IsShippable = true;
                    var pickupPoint = _genericAttributeService
                        .GetAttribute<PickupPoint>(currentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, storeId);

                    if (_shippingSettings.AllowPickUpInStore)
                        model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = currentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            PrepareAddressModel(model.OrderReviewData.ShippingAddress,
                                languageId: languageId,
                               address: currentCustomer.ShippingAddress,
                               excludeProperties: false,
                               addressSettings: _addressSettings);
                        }
                    }

                    //selected shipping method
                    var shippingOption = _genericAttributeService
                        .GetAttribute<ShippingOption>(currentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, storeId);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = _genericAttributeService
                    .GetAttribute<string>(currentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, storeId);

                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, languageId: languageId) : "";
            }
            #endregion

            return "";
        }

        [NonAction]
        protected virtual string PrepareWishlistModel(WishlistModelResponse model,
         IList<ShoppingCartItem> cart, Customer currentCustomer, int currencyId, int storeId, int languageId, bool isEditable = true)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);
            if (cart == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyWishList";

            if (model == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyWishListModel";

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart.SystemName, currentCustomer);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            #region Simple properties

            var customer = currentCustomer;
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = _customerService.GetCustomerFullName(customer);
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;

            if (!cart.Any())
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyWishList";

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new WishlistModelResponse.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                    ProductId = sci.Product.Id,
                    ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name, languageId: languageId),
                    ProductSeName = _urlRecordService.GetSeName(sci.Product, languageId: languageId),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                    sci.Product.ProductType == ProductType.SimpleProduct &&
                    (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Product.IsGiftCard) &&
                    sci.Product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = _productService.ParseAllowedQuantities(sci.Product);
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(sci.Product.RecurringCyclePeriod, languageId));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue
                         ? _productService.FormatRentalDate(sci.Product, sci.RentalStartDateUtc.Value)
                         : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(sci.Product, sci.RentalEndDateUtc.Value)
                        : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out decimal _);
                    var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, workingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out decimal shoppingCartItemDiscountBase, out List<DiscountForCaching> scDiscounts, out int? maximumDiscountQty), out decimal taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, workingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, workingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                       _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName, languageId, storeId);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    customer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion

            return "";
        }

        [NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci,
           int pictureSize, bool showDefaultPicture, string productName, int languageId, int storeId)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, languageId, _webHelper.IsCurrentConnectionSecured(), storeId);
            //as we cache per user (shopping cart item identifier)
            //let's cache just for 3 minutes
            var cacheTime = 3;
            var model = _cacheManager.Get(pictureCacheKey, () =>
            {
                //shopping cart item picture
                Picture sciPicture = null;

                //first, let's see whether a shopping cart item has some attribute values with custom pictures
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(sci.AttributesXml);
                foreach (var attributeValue in attributeValues)
                {
                    var attributePicture = _pictureService.GetPictureById(attributeValue.PictureId);
                    if (attributePicture != null)
                    {
                        sciPicture = attributePicture;
                        break;
                    }
                }

                //now let's load the default product picture
                var product = sci.Product;
                if (sciPicture == null)
                {
                    sciPicture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                }

                //let's check whether this product has some parent "grouped" product
                if (sciPicture == null && !product.VisibleIndividually && product.ParentGroupedProductId > 0)
                {
                    sciPicture = _pictureService.GetPicturesByProductId(product.ParentGroupedProductId, 1).FirstOrDefault();
                }
                return new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(sciPicture),
                    ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                    ThumbImageUrl = _pictureService.GetPictureUrl(sciPicture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                };
            }, cacheTime);
            return model;
        }

        [NonAction]
        protected virtual OrderTotalsResponse PrepareOrderTotalsModel(Customer currentCustomer, Currency workingCurrency, Language workingLanguage, int storeId, IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsResponse
            {
                IsEditable = isEditable,
                TaxRates = new List<OrderTotalsResponse.TaxRate>()
            };

            if (cart.Count > 0)
            {
                var subTotalIncludingTax = _taxSettings.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                    out decimal orderSubTotalDiscountAmountBase, out List<DiscountForCaching> orderSubTotalAppliedDiscounts,
                    out decimal subTotalWithoutDiscountBase, out decimal subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, workingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, workingCurrency, workingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, workingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, workingCurrency, workingLanguage, subTotalIncludingTax);
                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                        orderSubTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !string.IsNullOrEmpty(d.CouponCode));
                }

                //shipping info
                model.RequiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, workingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = _genericAttributeService
                            .GetAttribute<ShippingOption>(currentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, storeId);

                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                var paymentMethodSystemName = _genericAttributeService
                    .GetAttribute<string>(currentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, storeId);
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, currentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, workingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _taxSettings.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    decimal shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out SortedDictionary<decimal, decimal> taxRates);
                    decimal shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, workingCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsResponse.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, workingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
                    out decimal orderTotalDiscountAmountBase, out List<DiscountForCaching> orderTotalAppliedDiscounts,
                    out List<AppliedGiftCard> appliedGiftCards, out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, workingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, workingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    model.AllowRemovingOrderTotalDiscount = model.IsEditable &&
                       orderTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !string.IsNullOrEmpty(d.CouponCode));
                }

                //gift cards
                model.GiftCards = new List<OrderTotalsResponse.GiftCard>();
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsResponse.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, workingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = _giftCardService.GetGiftCardRemainingAmount(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, workingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, workingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled &&
                    _rewardPointsSettings.DisplayHowMuchWillBeEarned &&
                    shoppingCartTotalBase.HasValue)
                {
                    model.WillEarnRewardPoints = _orderTotalCalculationService
                        .CalculateRewardPoints(currentCustomer, shoppingCartTotalBase.Value);
                }

            }

            return model;
        }

        [NonAction]
        protected virtual string ParseAndSaveCheckoutAttributes(Customer currentCustomer, int storeId, List<CheckoutAttributeResponse> checkoutAttributeResponse, List<ShoppingCartItem> cart)
        {
            if (cart == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCart";

            string selectedAttributes = "";

            foreach (var checkoutAttribute in checkoutAttributeResponse)
            {
                var attribute = _checkoutAttributeService.GetCheckoutAttributeById(checkoutAttribute.AttributeId);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = checkoutAttribute.AttributeValue.ToString();
                            if (!string.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = checkoutAttribute.AttributeValue.ToString();
                            if (!string.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var cvaValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in cvaValues
                                .Where(pvav => pvav.IsPreSelected)
                                .Select(pvav => pvav.Id)
                                .ToList())
                            {
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = checkoutAttribute.AttributeValue.ToString();
                            if (!string.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = Convert.ToDateTime(checkoutAttribute.AttributeValue);
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(checkoutAttribute.AttributeId.ToString(), out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.CheckoutAttributes, selectedAttributes, storeId);
            return "";
        }

        [NonAction]
        protected virtual string ParseProductAttributes(Product product, List<string> attributeControlIds)
        {
            string attributes = "";

            #region Product attributes

            string selectedAttributes = string.Empty;
            var productVariantAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productVariantAttributes)
            {
                try
                {
                    string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductId, attribute.ProductAttributeId, attribute.Id);
                    foreach (var customAttributeControlId in attributeControlIds)
                    {
                        var customProductAttributeControlId = customAttributeControlId.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        string customControlId = customProductAttributeControlId[0] + "_" + customProductAttributeControlId[1] + "_" + customProductAttributeControlId[2] + "_" + customProductAttributeControlId[3] + "_" + customProductAttributeControlId[4];

                        string multipleAttributeValues = customProductAttributeControlId[5];
                        string[] attributesValues = multipleAttributeValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        #region attributes Cases

                        foreach (var attributeValue in attributesValues)
                        {
                            if (controlId == customControlId)
                            {
                                switch (attribute.AttributeControlType)
                                {
                                    case AttributeControlType.DropdownList:
                                    case AttributeControlType.RadioList:
                                    case AttributeControlType.ColorSquares:
                                    case AttributeControlType.ImageSquares:
                                        {
                                            var ctrlAttributes = attributeValue;
                                            if (!string.IsNullOrEmpty(ctrlAttributes))
                                            {
                                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                                if (selectedAttributeId > 0)
                                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                                        attribute, selectedAttributeId.ToString());
                                            }
                                        }
                                        break;
                                    case AttributeControlType.Checkboxes:
                                        {
                                            var ctrlAttributes = attributeValue;
                                            if (!string.IsNullOrEmpty(ctrlAttributes))
                                            {
                                                foreach (var item in ctrlAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                                {
                                                    int selectedAttributeId = int.Parse(item);
                                                    if (selectedAttributeId > 0)
                                                        selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                                            attribute, selectedAttributeId.ToString());
                                                }
                                            }
                                        }
                                        break;
                                    case AttributeControlType.ReadonlyCheckboxes:
                                        {
                                            //load read-only (already server-side selected) values
                                            var pvaValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                                            foreach (var selectedAttributeId in pvaValues
                                                .Where(pvav => pvav.IsPreSelected)
                                                .Select(pvav => pvav.Id)
                                                .ToList())
                                            {
                                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                                    attribute, selectedAttributeId.ToString());
                                            }
                                        }
                                        break;
                                    case AttributeControlType.TextBox:
                                    case AttributeControlType.MultilineTextbox:
                                        {
                                            var ctrlAttributes = attributeValue;
                                            if (!string.IsNullOrEmpty(ctrlAttributes))
                                            {
                                                string enteredText = ctrlAttributes.Trim();
                                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                                    attribute, enteredText);
                                            }
                                        }
                                        break;
                                    case AttributeControlType.Datepicker:
                                        {
                                            DateTime? selectedDate = null;
                                            try
                                            {
                                                selectedDate = Convert.ToDateTime(attributeValue);
                                            }
                                            catch { }
                                            if (selectedDate.HasValue)
                                            {
                                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                                    attribute, selectedDate.Value.ToString("D"));
                                            }
                                        }
                                        break;
                                    case AttributeControlType.FileUpload:
                                        {
                                            Guid.TryParse(attributeValue, out Guid downloadGuid);
                                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                            if (download != null)
                                            {
                                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                                        attribute, download.DownloadGuid.ToString());
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
            attributes = selectedAttributes;

            #endregion

            return attributes;
        }

        [NonAction]
        public virtual void PrepareAddressModel(AddressModel model, Address address, bool excludeProperties, int languageId,
          AddressSettings addressSettings, Func<IList<Country>> loadCountries = null,
          bool prePopulateWithCustomerFields = false, Customer customer = null, string overrideAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null ? _localizationService.GetLocalized(address.Country, x => x.Name) : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = address.StateProvince != null ? _localizationService.GetLocalized(address.StateProvince, x => x.Name) : null;
                model.City = address.City;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
            }

            if (address == null && prePopulateWithCustomerFields)
            {
                if (customer == null)
                    throw new Exception("Customer cannot be null when prepopulating an address");
                model.Email = customer.Email;
                model.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                model.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
                model.Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute);
                model.Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                model.Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                model.ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                model.City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute);
                //ignore country and state for prepopulation. it can cause some issues when posting pack with errors, etc
                //model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                //model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                model.PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
                model.FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute);
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name, languageId: languageId),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    var states = _stateProvinceService
                        .GetStateProvincesByCountryId(model.CountryId ?? 0, languageId)
                        .ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem
                            {
                                Text = _localizationService.GetLocalized(s, x => x.Name, languageId: languageId),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;

            //customer attribute services
            if (_addressAttributeService != null && _addressAttributeParser != null)
            {
                PrepareCustomAddressAttributes(model, address, languageId, overrideAttributesXml);
            }
            if (_addressAttributeFormatter != null && address != null)
            {
                model.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(address.CustomAttributes);
            }
        }

        [NonAction]
        protected virtual void PrepareCustomAddressAttributes(AddressModel model,
            Address address, int languageId, string overrideAttributesXml = "")
        {
            var attributes = _addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name, languageId: languageId),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name, languageId: languageId),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    (address != null ? address.CustomAttributes : null);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        #endregion

        #region Method

        [HttpPost]
        public virtual IActionResult AddDiscount([FromBody]AddDiscountRequest discount)
        {
            if (discount == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            Customer customer = _customerService.GetCustomerByGuid(discount.CustomerGUID);

            if (customer == null)
            {
                return NotFound();
            }

            string discountCouponName = "KP_" + customer.Id + "_" + discount.KPointsToRedeem + "_"
                  + DateTime.Now.ToString("yyyyMMddHHmmss");

            Discount tempDiscount = new Discount
            {
                Name = discountCouponName,
                DiscountTypeId = (int)DiscountType.AssignedToOrderSubTotal,
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = discount.DiscountAmount,
                RequiresCouponCode = false,
                CouponCode = null,
                IsCumulative = true,
                DiscountLimitationId = (int)DiscountLimitationType.NTimesPerCustomer, 
                LimitationTimes = 1,
                AppliedToSubCategories = false
            };

            _discountService.InsertDiscount(tempDiscount);
            return Ok(tempDiscount);
        }

        [NonAction]
        public int GetCustomerIdByGUID(Guid customerGuid)
        {
            var customerInfo = _customerService.GetCustomerByGuid(customerGuid);
            return customerInfo.Id;
        }

        #endregion
    }
}
