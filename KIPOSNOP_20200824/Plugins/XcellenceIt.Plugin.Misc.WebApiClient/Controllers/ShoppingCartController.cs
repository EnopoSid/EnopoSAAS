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
using Nop.Web.Models.Checkout;
using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh;
using Nop.Services.Fresh;
using Nop.Services.Shipping.Pickup;
using System.Xml;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class ShoppingCartController : Controller
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
        private readonly IAddressService _addressService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IFreshMealPlansService _freshMealPlansService = EngineContext.Current.Resolve<IFreshMealPlansService>();
        private readonly IFcartService _fCartService = EngineContext.Current.Resolve<IFcartService>();
        private readonly IFreshPriceCalculationService _freshPriceCalculationService = EngineContext.Current.Resolve<IFreshPriceCalculationService>();

        #endregion

        #region Ctor

        public ShoppingCartController(
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
        IUrlRecordService urlRecordService,
         IAddressService addressService,
         ICheckoutModelFactory checkoutModelFactory
         //IFreshCartService freshCartService
        )
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
            this._addressService = addressService;
            this._checkoutModelFactory = checkoutModelFactory;
            //this._freshCartService = freshCartService;
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
                    ParentCategoryId = sci.ParentCategoryId,
                    //added by sree for cart update issue
                    ShoppingCartId= sci.Id,
                    
                    AttributeInfoAsArrayList = _productAttributeFormatter.FormatAttributesInArrayFormat(sci.Product, sci.AttributesXml)
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
                            //Added By sree for category Discount Fixed Amount issue 18_08_2020 start
                            decimal tempDiscountBase = 0;

                            if (shoppingCartItemSubTotalWithDiscount == 0)
                            {
                                decimal productPrice= _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, false), out decimal txRate);
                                tempDiscountBase = productPrice;
                            }
                            else
                            {
                                tempDiscountBase = shoppingCartItemDiscountBase;
                            }
                            //Added By sree for category Discount Fixed Amount issue 18_08_2020 end
                            //decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, workingCurrency);
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(tempDiscountBase, workingCurrency);
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
                    ParentCategoryId = sci.ParentCategoryId
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
        public virtual IActionResult DetailAddProductToCart([FromBody]AddProductToCartRequest addProductToCartRequest)
        {

            if (addProductToCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(addProductToCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(addProductToCartRequest.CurrencyId);

            var product = _productService.GetProductById(addProductToCartRequest.ProductId);
            if (product == null)
            {
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFound"), addProductToCartRequest.ProductId)).NotFound();
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SimpleProductOnly")).BadRequest();
            }

            var shoppingCartResponse = new ShoppingCartResponse();

            //product has attribute then return attribute type for reference
            if (product.ProductAttributeMappings.Count > 0)
            {
                shoppingCartResponse.AttributeControlType = new List<string>();
                foreach (var item in product.ProductAttributeMappings)
                {
                    foreach (var pvav in item.ProductAttributeValues)
                    {
                        shoppingCartResponse.AttributeControlType.Add("AttributeId##" + item.AttributeControlTypeId + "#;AttributeValueId##" + pvav.Id + "#;AttributeValue##" + pvav.Name + "#;IsRequired##" + item.IsRequired + "#;");
                    }
                }
            }

            #region Start and End Date
            DateTime? rentalStartDt = null, rentalEndDt = null;
            #endregion

            if (product.IsRental)
            {
                ParseRentalDates(product, addProductToCartRequest.RentalStartDate, addProductToCartRequest.RentalEndDate, out rentalStartDt, out rentalEndDt);
            }

            //save item
            var addToCartWarnings = new List<string>();
            var cartType = (ShoppingCartType)addProductToCartRequest.ShoppingCartTypeId;

            //add to the cart
            if (product.ProductAttributeMappings.Count > 0 && addProductToCartRequest.AttributeControlIds != null)
            {
                string attributes = ParseProductAttributes(product, addProductToCartRequest.AttributeControlIds);
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(currentCustomer,
                    product, cartType, addProductToCartRequest.StoreId,
                    attributes, decimal.Zero,
                    rentalStartDt, rentalEndDt, addProductToCartRequest.Quantity, true, addProductToCartRequest.ParentCategoryId));
            }
            else
            {
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(currentCustomer,
                    product, cartType, addProductToCartRequest.StoreId,
                    null, decimal.Zero,
                    rentalStartDt, rentalEndDt, addProductToCartRequest.Quantity, true, addProductToCartRequest.ParentCategoryId));
            }

            #region Return result

            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                shoppingCartResponse.Messages = addToCartWarnings.ToArray();
                return Ok(shoppingCartResponse);
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name));

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckWishList")).BadRequest();
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(
                           _localizationService.GetResource("Wishlist.HeaderQuantity"),
                           currentCustomer.ShoppingCartItems
                               .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                               .LimitPerStore(addProductToCartRequest.StoreId)
                               .Sum(item => item.Quantity));

                        CustomerModel customerModel = new CustomerModel
                        {
                            Message = string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Products.ProductHasBeenAddedToTheWishlist"), updatetopwishlistsectionhtml),
                            CustomerGuid = currentCustomer.CustomerGuid
                        };
                        return Ok(customerModel);

                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name));

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            return Ok(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckCart"));

                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(
                           _localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                           currentCustomer.ShoppingCartItems
                               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                               .LimitPerStore(addProductToCartRequest.StoreId)
                               .Sum(item => item.Quantity));

                        CustomerModel customerModel = new CustomerModel
                        {
                            Message = string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Products.ProductHasBeenAddedToTheCart"), updatetopcartsectionhtml),
                            CustomerGuid = currentCustomer.CustomerGuid
                        };
                        return Ok(customerModel);
                    }
            }

            #endregion

        }

        // added by Phanendra on 18-2-2020 to add default ingragents while add to cart start
        [HttpPost]
        public virtual IActionResult POSDetailAddProductToCart([FromBody]AddProductToCartRequest addProductToCartRequest)
        {

            if (addProductToCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(addProductToCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(addProductToCartRequest.CurrencyId);

            var product = _productService.GetProductById(addProductToCartRequest.ProductId);
            if (product == null)
            {
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFound"), addProductToCartRequest.ProductId)).NotFound();
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SimpleProductOnly")).BadRequest();
            }

            var shoppingCartResponse = new ShoppingCartResponse();

            var ProductAttributeRequired = new List<string>(); // added by Phanendra on 18-2-2020 to add default ingragents while add to cart

            //product has attribute then return attribute type for reference
            if (product.ProductAttributeMappings.Count > 0)
            {
                shoppingCartResponse.AttributeControlType = new List<string>();
                foreach (var item in product.ProductAttributeMappings)
                {
                    foreach (var pvav in item.ProductAttributeValues)
                    {
                        shoppingCartResponse.AttributeControlType.Add("AttributeId##" + item.AttributeControlTypeId + "#;AttributeValueId##" + pvav.Id + "#;AttributeValue##" + pvav.Name + "#;IsRequired##" + item.IsRequired + "#;");
                        // added by Phanendra on 18-2-2020 to add default ingragents while add to cart start
                        if (addProductToCartRequest.AttributeControlIds.Count == 0 && pvav.IsPreSelected == true)
                        {
                            ProductAttributeRequired.Add(string.Format("product_attribute_{0}_{1}_{2}_{3}", item.ProductId, item.ProductAttributeId, item.Id, pvav.Id));
                        }
                        else if (addProductToCartRequest.AttributeControlIds.Count > 0 && pvav.IsPreSelected == true)
                        {
                            string product_attribute = string.Format("product_attribute_{0}_{1}_{2}_{3}", item.ProductId, item.ProductAttributeId, item.Id, pvav.Id);
                            List<string> numbers = addProductToCartRequest.AttributeControlIds;
                            int itemCount = 0;
                            foreach (string items in numbers)
                            {
                                if (items == product_attribute)
                                {
                                    itemCount = itemCount + 1;
                                }
                            }
                            if (itemCount > 1)
                            {
                                addProductToCartRequest.AttributeControlIds.Remove(product_attribute);
                            }
                        }
                        // added by Phanendra on 18-2-2020 to add default ingragents while add to cart end
                    }
                }
            }

            // added by Phanendra on 18-2-2020 to add default ingragents while add to cart start
            if (ProductAttributeRequired.Count > 0)
                addProductToCartRequest.AttributeControlIds = ProductAttributeRequired;
            // added by Phanendra on 18-2-2020 to add default ingragents while add to cart end

            #region Start and End Date
            DateTime? rentalStartDt = null, rentalEndDt = null;
            #endregion

            if (product.IsRental)
            {
                ParseRentalDates(product, addProductToCartRequest.RentalStartDate, addProductToCartRequest.RentalEndDate, out rentalStartDt, out rentalEndDt);
            }

            //save item
            var addToCartWarnings = new List<string>();
            var cartType = (ShoppingCartType)addProductToCartRequest.ShoppingCartTypeId;

            //add to the cart
            if (product.ProductAttributeMappings.Count > 0 && addProductToCartRequest.AttributeControlIds != null)
            {
                string attributes = ParseProductAttributes(product, addProductToCartRequest.AttributeControlIds);
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(currentCustomer,
                    product, cartType, addProductToCartRequest.StoreId,
                    attributes, decimal.Zero,
                    rentalStartDt, rentalEndDt, addProductToCartRequest.Quantity, true, addProductToCartRequest.ParentCategoryId));
            }
            else
            {
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(currentCustomer,
                    product, cartType, addProductToCartRequest.StoreId,
                    null, decimal.Zero,
                    rentalStartDt, rentalEndDt, addProductToCartRequest.Quantity, true, addProductToCartRequest.ParentCategoryId));
            }

            #region Return result

            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                shoppingCartResponse.Messages = addToCartWarnings.ToArray();
                return Ok(shoppingCartResponse);
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name));

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckWishList")).BadRequest();
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(
                           _localizationService.GetResource("Wishlist.HeaderQuantity"),
                           currentCustomer.ShoppingCartItems
                               .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                               .LimitPerStore(addProductToCartRequest.StoreId)
                               .Sum(item => item.Quantity));

                        CustomerModel customerModel = new CustomerModel
                        {
                            Message = string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Products.ProductHasBeenAddedToTheWishlist"), updatetopwishlistsectionhtml),
                            CustomerGuid = currentCustomer.CustomerGuid
                        };
                        return Ok(customerModel);

                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name));

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            return Ok(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckCart"));

                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(
                           _localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                           currentCustomer.ShoppingCartItems
                               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                               .LimitPerStore(addProductToCartRequest.StoreId)
                               .Sum(item => item.Quantity));

                        CustomerModel customerModel = new CustomerModel
                        {
                            Message = string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Products.ProductHasBeenAddedToTheCart"), updatetopcartsectionhtml),
                            CustomerGuid = currentCustomer.CustomerGuid
                        };
                        return Ok(customerModel);
                    }
            }

            #endregion

        }
        // added by Phanendra on 18-2-2020 to add default ingragents while add to cart end

        //Cart Response For Angular Site POS WEB Start
        [HttpPost]
        public virtual IActionResult Cart([FromBody]CartRequest cartRequest)
        {
            if (cartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(cartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NotAuthorize")).BadRequest();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(cartRequest.StoreId)
                .ToList();

            /*Added by Surakshith for getting Cart Count Start*/
            if (cart.Count == 0)
            {
                CartResponse cartres = new CartResponse();
                cartres.cartCount = cart.Count;
                cartres.items = cart;
                return Ok(cartres);
            }
            /*Added by Surakshith for getting Cart Count End*/

            var model = new ShoppingCartModelResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            PrepareShoppingCartModel(model, cart, currentCustomer, cartRequest.CurrencyId, cartRequest.StoreId, cartRequest.LanguageId);

            return Ok(model);
        }
        //Cart Response For Angular Site POS WEB End
        //Cart Response For AngularJS Site Online Site Start
        [HttpPost]
        public virtual IActionResult OnlineCart([FromBody]CartRequest cartRequest)
        {
            if (cartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(cartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NotAuthorize")).BadRequest();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(cartRequest.StoreId)
                .ToList();

            /*Added by Surakshith for getting Cart Count Start*/
            
            if (cart.Count == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();
            }
            /*Added by Surakshith for getting Cart Count End*/

            var model = new ShoppingCartModelResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            PrepareShoppingCartModel(model, cart, currentCustomer, cartRequest.CurrencyId, cartRequest.StoreId, cartRequest.LanguageId);

            return Ok(model);
        }
        //Cart Response For AngularJS Site Online Site End
        [HttpPost]
        public virtual IActionResult WishList([FromBody]WishListRequest wishListRequest)
        {
            if (wishListRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            Customer currentCustomer = _customerService.GetCustomerByGuid(wishListRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NotAuthorize")).BadRequest();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(wishListRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyWishList")).NotFound();
            }

            var model = new WishlistModelResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };
            PrepareWishlistModel(model, cart, currentCustomer, wishListRequest.CurrencyId, wishListRequest.StoreId, wishListRequest.LanguageId);
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult ApplyDiscount([FromBody]DiscountRequest discountRequest)
        {
            if (discountRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            return ApplyDiscountLocal(discountRequest);
        }

        [HttpPost]
        public virtual List<IActionResult> ApplyMultipleDiscounts([FromBody]MultipleDiscountRequest discountRequests)
        {
            List<IActionResult> returnResponse = new List<IActionResult>();

            foreach (DiscountRequest discountRequest in discountRequests.DiscountsInfo)
            {
                discountRequest.ApiSecretKey = discountRequests.ApiSecretKey;
                discountRequest.CustomerGUID = discountRequests.CustomerGUID;
                discountRequest.StoreId = discountRequests.StoreId;
                returnResponse.Add(ApplyDiscountLocal(discountRequest));
            }

            return returnResponse;
        }

        [NonAction]
        public IActionResult ApplyDiscountLocal(DiscountRequest discountRequest)
        {
            if (discountRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            var currentCustomer = _customerService.GetCustomerByGuid(discountRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(discountRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            var model = new DiscountBoxResponse();
            if (!string.IsNullOrWhiteSpace(discountRequest.DiscountCouponCode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountRequest.DiscountCouponCode, showHidden: true)
                .Where(d => d.RequiresCouponCode)
                .ToList();

                var userErrors = new List<string>();
                if (discount.Any())
                {
                    var anyValidDiscount = discount.Any(d =>
                        {
                            var validationResult = _discountService.ValidateDiscount(d, currentCustomer, new[] { discountRequest.DiscountCouponCode });
                            userErrors.AddRange(validationResult.Errors);

                            return validationResult.IsValid;
                        });

                        if (anyValidDiscount)
                        {
                            //valid
                            //added following lines for applying one time coupon for new users except existing users start
                            
                                    _customerService.ApplyDiscountCouponCode(currentCustomer, discountRequest.DiscountCouponCode);
                                    CustomerModel customerModel = new CustomerModel
                                    {
                                        CustomerGuid = currentCustomer.CustomerGuid,
                                        Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied")
                                    };

                                    return Ok(customerModel);
                           
                            //added following lines for applying coupon for new users except existing users end
                        }
                        else
                        {
                            if (userErrors.Any())
                            {
                                if (userErrors[0] == null)
                                {
                                    return new ResponseObject("Invalid CouponCode").BadRequest();
                                }
                                else
                                {
                                    var newUserRole = currentCustomer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.NewUserRoleName);
                                    if (newUserRole != null)
                                    {
                                        //request.Customer.CustomerRoles.Remove(guestRole);
                                        currentCustomer.CustomerCustomerRoleMappings
                                            .Remove(currentCustomer.CustomerCustomerRoleMappings.FirstOrDefault(mapping => mapping.CustomerRoleId == newUserRole.Id));
                                    }
                                    _customerService.UpdateCustomer(currentCustomer);
                                    return new ResponseObject(userErrors[0].ToString()).BadRequest();
                                }
                                
                            }
                            else
                            {
                                //general error text
                                return new ResponseObject(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount")).BadRequest();
                            }
                        }
                }
                else
                {
                    // return new ResponseObject(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount")).BadRequest();
                    return new ResponseObject("Coupon Code doesn't exists").BadRequest();
                }

            }
            else
            {
                //empty coupon code
                return new ResponseObject(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount")).BadRequest();
            }
        }

        [HttpPost]
        public virtual IActionResult RemoveDiscount([FromBody]DiscountRequest discountRequest)
        {
            var currentCustomer = _customerService.GetCustomerByGuid(discountRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(discountRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            var model = new DiscountBoxResponse();
            if (!string.IsNullOrWhiteSpace(discountRequest.DiscountCouponCode))
            {
                var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountRequest.DiscountCouponCode)
                          .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, currentCustomer).IsValid);
                bool isDiscountValid = discount != null &&
                    discount.RequiresCouponCode &&
                   _discountService.ValidateDiscount(discount, currentCustomer).IsValid;
                if (!isDiscountValid)
                {
                    return new ResponseObject(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount")).BadRequest();
                }
            }
            _customerService.RemoveDiscountCouponCode(currentCustomer, discountRequest.DiscountCouponCode);

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.RemoveCoupon")
            };
            return Ok(customerModel);
        }

        [HttpPost]
        public virtual IActionResult ApplyGiftCard([FromBody]GiftCardRequest giftCardRequest)
        {
            if (giftCardRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(giftCardRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(giftCardRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            var model = new GiftBoxResponse();
            if (!_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                if (!string.IsNullOrWhiteSpace(giftCardRequest.GiftCardCouponCode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftCardRequest.GiftCardCouponCode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && _giftCardService.IsGiftCardValid(giftCard);
                    if (isGiftCardValid)
                    {
                        _customerService.ApplyGiftCardCouponCode(currentCustomer, giftCardRequest.GiftCardCouponCode);
                        _customerService.UpdateCustomer(currentCustomer);

                        CustomerModel customerModel = new CustomerModel
                        {
                            CustomerGuid = currentCustomer.CustomerGuid,
                            Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ApplyGiftCard")
                        };
                        return Ok(customerModel);
                    }
                    else
                    {
                        return new ResponseObject(_localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard")).BadRequest();
                    }
                }
                else
                {
                    return new ResponseObject(_localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard")).BadRequest();

                }
            }
            else
            {
                return new ResponseObject(_localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts")).BadRequest();
            }
        }

        [HttpPost]
        public virtual IActionResult RemoveGiftCard([FromBody]GiftCardRequest giftCardRequest)
        {

            if (giftCardRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(giftCardRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(giftCardRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            if (!_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                if (!string.IsNullOrWhiteSpace(giftCardRequest.GiftCardCouponCode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftCardRequest.GiftCardCouponCode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && _giftCardService.IsGiftCardValid(giftCard);
                    if (!isGiftCardValid)
                    {
                        return new ResponseObject(_localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard")).BadRequest();
                    }
                }
            }

            if (giftCardRequest.GiftCardCouponCode != null)
            {
                _customerService.RemoveGiftCardCouponCode(currentCustomer, giftCardRequest.GiftCardCouponCode);
                _customerService.UpdateCustomer(currentCustomer);
            }
            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.RemoveGiftcard")
            };
            return Ok(customerModel);
        }

        [HttpPost]
        public virtual IActionResult EstimateShipping([FromBody]EstimateShippingRequest estimateShippingRequest)
        {
            var currentCustomer = _customerService.GetCustomerByGuid(estimateShippingRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(estimateShippingRequest.CurrencyId);

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(estimateShippingRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            var model = new EstimateShippingResponse
            {
                CountryId = estimateShippingRequest.CountryId,
                StateProvinceId = estimateShippingRequest.StateProvinceId,
                ZipPostalCode = estimateShippingRequest.ZipPostalCode,
                Warnings = new List<string>(),
                ShippingOptions = new List<EstimateShippingResponse.ShippingOptionModel>(),
                CustomerGuid = currentCustomer.CustomerGuid
            };

            if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                var address = new Address
                {
                    CountryId = estimateShippingRequest.CountryId,
                    Country = estimateShippingRequest.CountryId.HasValue ? _countryService.GetCountryById(estimateShippingRequest.CountryId.Value) : null,
                    StateProvinceId = estimateShippingRequest.StateProvinceId,
                    StateProvince = estimateShippingRequest.StateProvinceId.HasValue ? _stateProvinceService.GetStateProvinceById(estimateShippingRequest.StateProvinceId.Value) : null,
                    ZipPostalCode = estimateShippingRequest.ZipPostalCode,
                };
                GetShippingOptionResponse getShippingOptionResponse = _shippingService
                    .GetShippingOptions(cart, address, currentCustomer, "", estimateShippingRequest.StoreId);
                if (!getShippingOptionResponse.Success)
                {
                    foreach (var error in getShippingOptionResponse.Errors)
                        model.Warnings.Add(error);
                }
                else
                {
                    if (getShippingOptionResponse.ShippingOptions.Count > 0)
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            var soModel = new EstimateShippingResponse.ShippingOptionModel
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,

                            };
                            //calculate discounted and taxed rate
                            decimal shippingTotal = _orderTotalCalculationService.AdjustShippingRate(shippingOption.Rate,
                                cart, out List<DiscountForCaching> appliedDiscounts);

                            decimal rateBase = _taxService.GetShippingPrice(shippingTotal, currentCustomer);
                            decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, workingCurrency);
                            soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                            model.ShippingOptions.Add(soModel);
                        }
                    }
                    else
                    {
                        return new ResponseObject(_localizationService.GetResource("Checkout.ShippingIsNotAllowed")).BadRequest();
                    }
                }
            }
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult OrderTotal([FromBody]OrderTotalRequest orderTotalRequest)
        {
            if (orderTotalRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(orderTotalRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(orderTotalRequest.CurrencyId);
            var workingLanguage = _languageService.GetLanguageById(orderTotalRequest.LanguageId);

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(orderTotalRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest(); ;

            var model = PrepareOrderTotalsModel(currentCustomer, workingCurrency, workingLanguage, orderTotalRequest.StoreId, cart, orderTotalRequest.IsEditable);
            model.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateCart([FromBody]UpdateCartRequest updateCartRequest)
        {
            if (updateCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(updateCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            // Check item id exist in cart
            bool IsItemExistInCart = currentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .Where(sci => sci.Id == updateCartRequest.ItemId)
               .LimitPerStore(updateCartRequest.StoreId)
               .Any();

            if (!IsItemExistInCart)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ItemNotExist")).BadRequest();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(updateCartRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                if (updateCartRequest.ItemId == sci.Id)
                {
                    var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(currentCustomer,
                                sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                updateCartRequest.NewQuantity, true);
                    innerWarnings.Add(sci.Id, currSciWarnings);
                }
            }

            //updated cart
            cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(updateCartRequest.StoreId)
                .ToList();
            var model = new ShoppingCartModelResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };
            PrepareShoppingCartModel(model, cart, currentCustomer, updateCartRequest.CurrencyId, updateCartRequest.StoreId, updateCartRequest.LanguageId);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (warnings.Count() != 0)
                {
                    sciModel.Warnings = new List<string>();
                    if (sciModel != null)
                        foreach (var w in warnings)
                            if (!sciModel.Warnings.Contains(w))
                                sciModel.Warnings.Add(w);
                }
            }
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateCartWithMultipleItems([FromBody]UpdateCartWithMultipleItemsRequest updateCartWithMultipleItemsRequest)
        {
            if (updateCartWithMultipleItemsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(updateCartWithMultipleItemsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(updateCartWithMultipleItemsRequest.StoreId)
                .ToList();
            //commented by sree for first costumer registration
            if (cart.Count == 0)
            {
                foreach(UpdateShoppingCartItems item in updateCartWithMultipleItemsRequest.CartItems)
                {
                    AddProductToCartRequest addProductToCartRequest = new AddProductToCartRequest();

                    addProductToCartRequest.ApiSecretKey = updateCartWithMultipleItemsRequest.ApiSecretKey;
                    addProductToCartRequest.CurrencyId = updateCartWithMultipleItemsRequest.CurrencyId;
                    addProductToCartRequest.CustomerGUID = updateCartWithMultipleItemsRequest.CustomerGUID;
                    addProductToCartRequest.ParentCategoryId = item.ParentCategoryId;
                    addProductToCartRequest.ProductId = item.ItemId;
                    addProductToCartRequest.Quantity = item.Quantity;
                    addProductToCartRequest.ShoppingCartTypeId = item.ShoppingCartTypeId;
                    addProductToCartRequest.StoreId = updateCartWithMultipleItemsRequest.StoreId;
                    addProductToCartRequest.AttributeControlIds = item.AttributeControlIds;
                    DetailAddProductToCart(addProductToCartRequest);
                }
                return Ok("Items added to cart successfully");
            }
            else
            {
                //var allIdsToRemove = updateCartWithMultipleItemsRequest.RemoveFromCart != null ? updateCartWithMultipleItemsRequest.RemoveFromCart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

                //current warnings <cart item identifier, warnings>
                var innerWarnings = new Dictionary<int, IList<string>>();
                foreach (var sci in cart)
                {
                        foreach (UpdateShoppingCartItems item in updateCartWithMultipleItemsRequest.CartItems)
                        {
                           //Modified by surakshith to update multiple cart items with shopping cart id start on 30-06-2020
                            if (sci.Id == item.ItemId)
                            {
                            //Modified by surakshith to update multiple cart items with shopping cart id end on 30-06-2020
                            int newQuantity = item.Quantity;
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(currentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                                break;
                        }
                        }
                }

                foreach (UpdateShoppingCartItems item in updateCartWithMultipleItemsRequest.CartItems)
                {
                    AddProductToCartRequest addProductToCartRequest = new AddProductToCartRequest();
                    addProductToCartRequest.ApiSecretKey = updateCartWithMultipleItemsRequest.ApiSecretKey;
                    addProductToCartRequest.CurrencyId = updateCartWithMultipleItemsRequest.CurrencyId;
                    addProductToCartRequest.CustomerGUID = updateCartWithMultipleItemsRequest.CustomerGUID;
                    addProductToCartRequest.ParentCategoryId = item.ParentCategoryId;
                    addProductToCartRequest.ProductId = item.ItemId;
                    addProductToCartRequest.Quantity = item.Quantity;
                    addProductToCartRequest.ShoppingCartTypeId = item.ShoppingCartTypeId;
                    addProductToCartRequest.StoreId = updateCartWithMultipleItemsRequest.StoreId;
                    addProductToCartRequest.AttributeControlIds = item.AttributeControlIds;
                    DetailAddProductToCart(addProductToCartRequest);
                }

                    return Ok("Updated cart successfully");
            }
        }

        [HttpPost]
        public virtual IActionResult RemoveFromCart([FromBody]RemoveFromCartRequest removeFromCartRequest)
        {
            if (removeFromCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(removeFromCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(removeFromCartRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            var allIdsToRemove = removeFromCartRequest.ItemIds != null ? removeFromCartRequest.ItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //check cutomer and cart items ids 
            foreach (var items in cart)
            {
                foreach (var ids in allIdsToRemove)
                {
                    if (ids == items.Id)
                    {
                        //current warnings <cart item identifier, warnings>
                        var innerWarnings = new Dictionary<int, IList<string>>();
                        foreach (var sci in cart)
                        {
                            bool remove = allIdsToRemove.Contains(sci.Id);
                            if (remove)
                            {
                                _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                                var objFCart = _fCartService.GetFreshCartByShoppingCartId(sci.Id);
                                if (objFCart != null)
                                    _fCartService.RemoveCartByShoppingCartId(objFCart.Id);
                            }
                        }

                        //updated cart
                        cart = currentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(removeFromCartRequest.StoreId)
                            .ToList();
                        var model = new ShoppingCartModelResponse();
                        PrepareShoppingCartModel(model, cart, currentCustomer, removeFromCartRequest.CurrencyId, removeFromCartRequest.StoreId, removeFromCartRequest.LanguageId);
                        //update current warnings
                        foreach (var kvp in innerWarnings)
                        {
                            //kvp = <cart item identifier, warnings>
                            var sciId = kvp.Key;
                            var warnings = kvp.Value;
                            //find model
                            var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                            if (sciModel != null)
                                foreach (var w in warnings)
                                    if (!sciModel.Warnings.Contains(w))
                                        sciModel.Warnings.Add(w);
                        }
                        CustomerModel customerModel = new CustomerModel
                        {
                            CustomerGuid = currentCustomer.CustomerGuid,
                            Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CartRemove")
                        };
                        return Ok(customerModel);
                    }
                }
            }
            return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ItemNotMatch")).BadRequest();
        }

        [HttpPost]
        public virtual IActionResult RemoveFromWishList([FromBody]RemoveFromCartRequest removeFromCartRequest)
        {
            if (removeFromCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(removeFromCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(removeFromCartRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyWishList")).BadRequest();

            var allIdsToRemove = removeFromCartRequest.ItemIds != null ? removeFromCartRequest.ItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //check cutomer and wishlist items ids 
            bool isItemMatched = false;
            foreach (var items in cart)
            {
                foreach (var ids in allIdsToRemove)
                {
                    if (ids == items.Id)
                    {
                        isItemMatched = true;
                        //current warnings <wishlist item identifier, warnings>
                        var innerWarnings = new Dictionary<int, IList<string>>();
                        foreach (var sci in cart)
                        {
                            bool remove = allIdsToRemove.Contains(sci.Id);
                            if (remove)
                                _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                        }

                        //updated cart
                        cart = currentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(removeFromCartRequest.StoreId)
                            .ToList();
                        var model = new ShoppingCartModelResponse();
                        PrepareShoppingCartModel(model, cart, currentCustomer, removeFromCartRequest.CurrencyId, removeFromCartRequest.StoreId, removeFromCartRequest.LanguageId);
                        //update current warnings
                        foreach (var kvp in innerWarnings)
                        {
                            //kvp = <cart item identifier, warnings>
                            var sciId = kvp.Key;
                            var warnings = kvp.Value;
                            //find model
                            var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                            if (sciModel != null)
                                foreach (var w in warnings)
                                    if (!sciModel.Warnings.Contains(w))
                                        sciModel.Warnings.Add(w);
                        }
                    }
                }
            }

            cart = currentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
               .LimitPerStore(removeFromCartRequest.StoreId)
               .ToList();
            var wishlistModel = new WishlistModelResponse();
            PrepareWishlistModel(wishlistModel, cart, currentCustomer, removeFromCartRequest.CurrencyId, removeFromCartRequest.StoreId, removeFromCartRequest.LanguageId);

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            if (isItemMatched)
            {
                customerModel.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.WishListRemove");
                return Ok(customerModel);
            }

            else
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ItemNotMatch")).BadRequest();
        }

        [HttpPost]
        public virtual IActionResult SetCheckOutAttribute([FromBody]SetCheckOutAttributeRequest setCheckOutAttributeRequest)
        {
            if (setCheckOutAttributeRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(setCheckOutAttributeRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var model = new ShoppingCartModelResponse();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(setCheckOutAttributeRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //parse and save checkout attributes
            model.Message = ParseAndSaveCheckoutAttributes(currentCustomer, setCheckOutAttributeRequest.StoreId, setCheckOutAttributeRequest.CheckoutAttributeResponse, cart);

            //validate attributes
            string checkoutAttributes = _genericAttributeService
                .GetAttribute<string>(currentCustomer, NopCustomerDefaults.CheckoutAttributes, setCheckOutAttributeRequest.StoreId);
            var checkoutAttributeWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Count > 0)
            {
                //something wrong, redisplay the page with warnings
                PrepareShoppingCartModel(model, cart, currentCustomer, setCheckOutAttributeRequest.CurrencyId, setCheckOutAttributeRequest.StoreId, setCheckOutAttributeRequest.LanguageId, validateCheckoutAttributes: true);

                return new ResponseObject(checkoutAttributeWarnings.ToString()).BadRequest(); ;
            }

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckOutAttribute")
            };
            return Ok(customerModel);
        }

        [HttpPost]
        public virtual IActionResult CartCount([FromBody]ShoppingCartRequest shoppingCartCount)
        {
            if (shoppingCartCount == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(shoppingCartCount.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var updatetopcartsectionhtml = string.Format(
                             _localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                             currentCustomer.ShoppingCartItems
                                 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                 .LimitPerStore(shoppingCartCount.StoreId)
                                 .Sum(item => item.Quantity));

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CartCount"), updatetopcartsectionhtml)
            };
            return Ok(customerModel);
        }

        [HttpPost]
        public virtual IActionResult CatalogAddProductToCart([FromBody]CatalogAddProductToCartRequest catalogAddProductToCartRequest)
        {
            if (catalogAddProductToCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(catalogAddProductToCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(catalogAddProductToCartRequest.CurrencyId);

            var cartType = (ShoppingCartType)catalogAddProductToCartRequest.ShoppingCartTypeId;

            var product = _productService.GetProductById(catalogAddProductToCartRequest.ProductId);
            if (product == null)
                //no product found
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFound"), catalogAddProductToCartRequest.ProductId)).NotFound();

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SimpleProductOnly")).BadRequest();
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > catalogAddProductToCartRequest.Quantity)
            {
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MinimumQtyRequired")).BadRequest();
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerEnterPrice")).BadRequest();
            }

            if (product.IsRental)
            {
                //rental products require start/end dates to be entered
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.RequiredStartEndDate")).BadRequest();
            }

            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SelectQuantity")).BadRequest();
            }

            if (product.ProductAttributeMappings.Count > 0)
            {
                //product has some attributes. let a customer see them
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckProductAttribute")).BadRequest();

            }

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == cartType)
                .LimitPerStore(catalogAddProductToCartRequest.StoreId)
                .ToList();
            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + catalogAddProductToCartRequest.Quantity : catalogAddProductToCartRequest.Quantity;
            var addToCartWarnings = _shoppingCartService
               .GetShoppingCartItemWarnings(currentCustomer, cartType,
               product, catalogAddProductToCartRequest.StoreId, string.Empty,
               decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart
                //let's display standard warnings
                return new ResponseObject(addToCartWarnings.ToArray().ToString()).BadRequest();
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = _shoppingCartService.AddToCart(customer: currentCustomer,
                product: product,
                shoppingCartType: cartType,
                storeId: catalogAddProductToCartRequest.StoreId,
                quantity: catalogAddProductToCartRequest.Quantity);
            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return null;
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name));

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || catalogAddProductToCartRequest.ForceRedirection)
                        {
                            //redirect to the wishlist page
                            return null;
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(
                            _localizationService.GetResource("Wishlist.HeaderQuantity"),
                            currentCustomer.ShoppingCartItems
                                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                                .LimitPerStore(catalogAddProductToCartRequest.StoreId)
                                .Sum(item => item.Quantity));
                        CustomerModel customerModel = new CustomerModel
                        {
                            CustomerGuid = currentCustomer.CustomerGuid,
                            Message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist"))
                        };
                        return Ok(customerModel);
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name));

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || catalogAddProductToCartRequest.ForceRedirection)
                        {
                            //redirect to the shopping cart page
                            return null;
                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                            currentCustomer.ShoppingCartItems
                                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                .LimitPerStore(catalogAddProductToCartRequest.StoreId)
                                .Sum(item => item.Quantity));
                        CustomerModel customerModel = new CustomerModel
                        {
                            CustomerGuid = currentCustomer.CustomerGuid,
                            Message = _localizationService.GetResource("Products.ProductHasBeenAddedToTheCart")
                        };
                        return Ok(customerModel);

                    }
            }
        }

        [HttpPost]
        public virtual IActionResult WishListCount([FromBody]ShoppingCartRequest shoppingCartCount)
        {
            if (shoppingCartCount == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(shoppingCartCount.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                            currentCustomer.ShoppingCartItems
                                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                                .LimitPerStore(shoppingCartCount.StoreId)
                                .Sum(item => item.Quantity));
            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.WishListCount"), updatetopwishlistsectionhtml)
            };
            return Ok(customerModel);
        }

        //Added by surakshith to Differntiate online and web pos method start
        [HttpPost]
        public virtual IActionResult InsertDiscountForOnline([FromBody]AddDiscountRequest discount)
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
                RequiresCouponCode = true,
                CouponCode = discountCouponName,
                IsCumulative = true,
                DiscountLimitationId = (int)DiscountLimitationType.NTimesPerCustomer,
                LimitationTimes = 1,
                AppliedToSubCategories = false
            };

            _discountService.InsertDiscount(tempDiscount);
            return Ok(tempDiscount);
        }
        //Added by surakshith to Differntiate online and web pos method End
       
        [HttpPost]
        public virtual IActionResult InsertDiscount([FromBody]AddDiscountRequest discount)
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

            string discountCouponName = "NQ_" + customer.Id + "_" + discount.KPointsToRedeem + "_"
                  + DateTime.Now.ToString("yyyyMMddHHmmss");

            Discount tempDiscount = new Discount
            {
                Name = discountCouponName,
                DiscountTypeId = (int)DiscountType.AssignedToOrderSubTotal,
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = discount.DiscountAmount,
                RequiresCouponCode = true,
                CouponCode = discountCouponName,
                IsCumulative = true,
                DiscountLimitationId = (int)DiscountLimitationType.NTimesPerCustomer,
                LimitationTimes = 1,
                AppliedToSubCategories = false
            };

            _discountService.InsertDiscount(tempDiscount);
            return Ok(tempDiscount);
        }

        [HttpPost]
        public virtual IActionResult DeleteDiscount([FromBody]AddDiscountRequest discount)
        {
            if (discount == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var tempDiscount = _discountService.GetDiscountById(discount.DiscountId);
            
            _discountService.DeleteDiscount(tempDiscount);
            return Ok("Deleted Coupon successfully");
        }


        [HttpPost]
        public virtual IActionResult GetPickupAddress([FromBody]CartRequest requestObject)
        {
            if (requestObject == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }


            Customer customer = _customerService.GetCustomerByGuid(requestObject.CustomerGUID);
            GetPickupPointsResponse pickupPointsResponse = new GetPickupPointsResponse();

            if (customer == null)
            {
                 pickupPointsResponse = _shippingService.GetPickupPoints(null,
                        null, storeId: requestObject.StoreId);
            }
            else
            {
                pickupPointsResponse = _shippingService.GetPickupPoints(customer.BillingAddress,
                      customer, storeId: requestObject.StoreId);
            }
  
            IList<CheckoutPickupPointModel> PickupPoints = new List<CheckoutPickupPointModel>();

            if (pickupPointsResponse.Success)
                PickupPoints = pickupPointsResponse.PickupPoints.Select(point =>
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(point.CountryCode);
                    var state = _stateProvinceService.GetStateProvinceByAbbreviation(point.StateAbbreviation, country?.Id);

                    var pickupPointModel = new CheckoutPickupPointModel
                    {
                        Id = point.Id,
                        Name = point.Name,
                        Description = point.Description,
                        ProviderSystemName = point.ProviderSystemName,
                        Address = point.Address,
                        City = point.City,
                        County = point.County,
                        StateName = state != null ? _localizationService.GetLocalized(state, x => x.Name, requestObject.LanguageId) : string.Empty,
                        CountryName = country != null ? _localizationService.GetLocalized(country, x => x.Name, requestObject.LanguageId) : string.Empty,
                        ZipPostalCode = point.ZipPostalCode,
                        Latitude = point.Latitude,
                        Longitude = point.Longitude,
                        OpeningHours = point.OpeningHours
                    };

                    return pickupPointModel;
                }).ToList();


            return Ok(PickupPoints);

        }

        [HttpPost]
        public virtual IActionResult SelectDeliveryMethod([FromBody]SelectDeliveryMethodRequest requestObject)
        {
            if (requestObject == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }


            Customer customer = _customerService.GetCustomerByGuid(requestObject.CustomerGUID);

            if (customer == null)
            {
                return NotFound();
            }

            //pickup point
            if (_shippingSettings.AllowPickUpInStore)
            {
                if (requestObject.PickUpInStore)
                {
                    //no shipping address selected
                    customer.ShippingAddress = null;
                    _customerService.UpdateCustomer(customer);

                    var pickupPoint = requestObject.DeliveryOption.ToString().Split(new[] { "___" }, StringSplitOptions.None);

                    var pickupPoints = _shippingService.GetPickupPoints(customer.BillingAddress,
                        customer, pickupPoint[1], requestObject.StoreId).PickupPoints.ToList();
                    var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));
                    if (selectedPoint == null)
                        throw new Exception("Pickup point is not allowed");

                    var pickUpInStoreShippingOption = new ShippingOption
                    {
                        Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                        Rate = selectedPoint.PickupFee,
                        Description = selectedPoint.Description,
                        ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName
                    };
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, requestObject.StoreId);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SelectedPickupPointAttribute, selectedPoint, requestObject.StoreId);

                    //load next step
                    return Ok("Delivery Option selected");
                }

                //set value indicating that "pick up in store" option has not been chosen
                _genericAttributeService.SaveAttribute<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, requestObject.StoreId);

                int.TryParse(requestObject.DeliveryOption, out int shippingAddressId);

                if (shippingAddressId > 0)
                {
                    //existing address
                    var address = customer.Addresses.FirstOrDefault(a => a.Id == shippingAddressId);
                    if (address == null)
                        throw new Exception("Address can't be loaded");

                    customer.ShippingAddress = address;
                    _customerService.UpdateCustomer(customer);

                    return Ok("Delivery Option selected");
                }
                else
                {
                    return NotFound("Shipping Address not found");
                }
            }

            else
            {
                return NotFound("Method not selected");
            }


        }

        //Added by Phani to for applying custom amount discount for user in POS module total on 25-10-2019 start
        [HttpPost]
        public virtual IActionResult InsertCustomAmtDiscount([FromBody]AddDiscountRequest discount)
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
                RequiresCouponCode = true,
                CouponCode = discountCouponName,
                IsCumulative = true,
                DiscountLimitationId = (int)DiscountLimitationType.NTimesPerCustomer,
                LimitationTimes = 1,
                AppliedToSubCategories = false
            };

            _discountService.InsertDiscount(tempDiscount);
            return Ok(tempDiscount);
        }
        //Added by Phani to for applying custom amount discount for user in POS module total on 25-10-2019 end
        #endregion


        public virtual IActionResult InsertCustomAmtDiscountSingleService([FromBody]AddDiscountRequest discount)
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
                RequiresCouponCode = true,
                CouponCode = discountCouponName,
                IsCumulative = true,
                DiscountLimitationId = (int)DiscountLimitationType.NTimesPerCustomer,
                LimitationTimes = 1,
                AppliedToSubCategories = false
            };

            _discountService.InsertDiscount(tempDiscount);

            DiscountRequest discountRequest = new DiscountRequest();
            discountRequest.ApiSecretKey = discount.ApiSecretKey;
            discountRequest.CustomerGUID = discount.CustomerGUID;
            discountRequest.DiscountCouponCode = tempDiscount.CouponCode;
            discountRequest.OrderTotal = discount.OrderTotal;
            discountRequest.StoreId = discount.StoreId;

            return ApplyDiscountSingleService(discountRequest);
        }


        [NonAction]
        public IActionResult ApplyDiscountSingleService([FromBody]DiscountRequest discountRequest)
        {
            if (discountRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            return ApplyDiscountLocalSingleService(discountRequest);
        }

        [NonAction]
        public IActionResult ApplyDiscountLocalSingleService(DiscountRequest discountRequest)
        {
            if (discountRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            var currentCustomer = _customerService.GetCustomerByGuid(discountRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(discountRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            var model = new DiscountBoxResponse();
            if (!string.IsNullOrWhiteSpace(discountRequest.DiscountCouponCode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountRequest.DiscountCouponCode, showHidden: true)
                .Where(d => d.RequiresCouponCode)
                .ToList();

                var userErrors = new List<string>();
                if (discount.Any())
                {
                    if ((discountRequest.OrderTotal >= discount[0].DiscountAmount && discount[0].UsePercentage == false)
                        ||
                        (discount[0].UsePercentage == true)
                        )
                    {
                        var anyValidDiscount = discount.Any(d =>
                        {
                            var validationResult = _discountService.ValidateDiscount(d, currentCustomer, new[] { discountRequest.DiscountCouponCode });
                            userErrors.AddRange(validationResult.Errors);

                            return validationResult.IsValid;
                        });

                        if (anyValidDiscount)
                        {
                            _customerService.ApplyDiscountCouponCode(currentCustomer, discountRequest.DiscountCouponCode);
                            CustomerModel customerModel = new CustomerModel
                            {
                                CustomerId = currentCustomer.Id,
                                CustomerGuid = currentCustomer.CustomerGuid,
                                Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied")
                            };
                            GetAppiledDiscountRequest getAppiledDiscountRequest = new GetAppiledDiscountRequest();
                            getAppiledDiscountRequest.ApiSecretKey = discountRequest.ApiSecretKey;
                            getAppiledDiscountRequest.CustomerGuid = customerModel.CustomerGuid;
                            getAppiledDiscountRequest.CustomerId = customerModel.CustomerId;
                            getAppiledDiscountRequest.OrderTotal = discountRequest.OrderTotal;
                            getAppiledDiscountRequest.DiscountCouponCode = discountRequest.DiscountCouponCode;

                            return GetAppliedDiscountCouponList(getAppiledDiscountRequest);
                        }
                        else
                        {
                            if (userErrors.Any())
                            {
                                if (userErrors[0] == null)
                                {
                                    return new ResponseObject("Invalid CouponCode").BadRequest();
                                }
                                else
                                {
                                    var newUserRole = currentCustomer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.NewUserRoleName);
                                    if (newUserRole != null)
                                    {
                                        currentCustomer.CustomerCustomerRoleMappings
                                            .Remove(currentCustomer.CustomerCustomerRoleMappings.FirstOrDefault(mapping => mapping.CustomerRoleId == newUserRole.Id));
                                    }
                                    _customerService.UpdateCustomer(currentCustomer);
                                    return new ResponseObject(userErrors[0].ToString()).BadRequest();
                                }

                            }
                            else
                            {
                                //general error text
                                return new ResponseObject(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount")).BadRequest();
                            }
                        }

                    }
                    else
                    {
                        return new ResponseObject("Discount Code Not Applied as Discount Amount should be less than the Order Amount").BadRequest();
                    }
                }
                else
                {
                    return new ResponseObject("Coupon Code doesn't exists").BadRequest();
                }

            }
            else
            {
                //empty coupon code
                return new ResponseObject(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount")).BadRequest();
            }
        }

        [NonAction]
        public IActionResult GetAppliedDiscountCouponList(GetAppiledDiscountRequest getAppiledDiscountRequest)
        {
            GenericAttribute genericAttribute = new GenericAttribute();
            genericAttribute = _genericAttributeService.GetAttributesForEntity(getAppiledDiscountRequest.CustomerId, "Customer").Where(x => x.Key == "DiscountCouponCode").SingleOrDefault();
            List<string> AppliedCouponCode = new List<string>();
            if (genericAttribute != null)
            {
                var existingCouponCodes = genericAttribute.Value;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");

                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;
                    var code = node1.Attributes["Code"].InnerText.Trim();
                    //  couponCodes.Add(code);
                    AppliedCouponCode.Add(code);
                }
            }
            else
            {

            }
            return Ok(AppliedCouponCode);
        }

    }
}