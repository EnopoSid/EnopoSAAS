using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Fresh;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using Nop.Core.Domain.Discounts;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom;
using Nop.Web.Models.Checkout;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public partial class FreshCartController: Controller
    {
        #region fields

        ShoppingCartController _shoppingCartController = EngineContext.Current.Resolve<ShoppingCartController>();
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
        private readonly IFreshOrderTotalCalulationService _freshOrderTotalCalulationService = EngineContext.Current.Resolve<IFreshOrderTotalCalulationService>();

        #endregion

        #region cTor

        public FreshCartController(
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
        #endregion

        #region Methods


        [HttpPost]
        public virtual IActionResult AddMealstoCartFresh([FromBody]AddProductToCartFreshRequest addProductToCartFreshRequest)
        {
            if (addProductToCartFreshRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                //_logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            Customer customer = _customerService.GetCustomerByGuid(addProductToCartFreshRequest.CustomerGUID);

            if (customer == null)
            {
                customer = _customerService.InsertGuestCustomer();
            }

            Dictionary<string, int> returnValues = new Dictionary<string, int>();

            try
            {
                var mealOrderId = Guid.NewGuid();
                FreshCartResponse freshCartResponse = new FreshCartResponse();
                freshCartResponse.freshCartDetails = new List<freshCartDetails>();

                foreach (var meal in addProductToCartFreshRequest.mealDetails)
                {
                    AddProductToCartRequest addProductToCartRequest = new AddProductToCartRequest()
                    {
                        ApiSecretKey = addProductToCartFreshRequest.ApiSecretKey,
                        CurrencyId = addProductToCartFreshRequest.CurrencyId,
                        CustomerGUID = customer.CustomerGuid,
                        StoreId = addProductToCartFreshRequest.StoreId,
                        ParentCategoryId = addProductToCartFreshRequest.ParentCategoryId,
                        ShoppingCartTypeId = addProductToCartFreshRequest.ShoppingCartTypeId,
                        ProductId = meal.ProductId,
                        AttributeControlIds = meal.AttributeControlIds,
                        Quantity = meal.Quantity,
                        RentalEndDate = meal.RentalEndDate,
                        RentalStartDate = meal.RentalStartDate
                    };
                    DetailAddFreshProductToCart(addProductToCartRequest);

                    var cart = customer.ShoppingCartItems
                           .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                           .LimitPerStore(addProductToCartFreshRequest.StoreId)
                           .ToList();
                    var tempcart = cart.OrderByDescending(x => x.Id).ToList();
                    if (cart.Count == 0)
                        return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();
                    var MealPlanInfo = _freshMealPlansService.GetFreshMealPlanById((int)addProductToCartFreshRequest.MealPlanId);
                    if (MealPlanInfo == null)
                    {
                        return BadRequest();
                    }
                    FCart fCart = new FCart()
                    {
                        CustomerId = customer.Id,
                        ShoppingCartId = tempcart[0].Id,
                        ProductId = meal.ProductId,
                        CreatedDate = DateTime.Now,
                        MealNo = meal.MealNumber,
                        Status = true,
                        MealOrderId = mealOrderId,
                        MealPlanId = addProductToCartFreshRequest.MealPlanId,
                        IsReorder = 0,
                        MealDate= meal.MealDate,
                        MealTime=meal.MealTime,
                        MealPrice= MealPlanInfo.PerMealAmount
                    };
                    _fCartService.InsertFreshCart(fCart);
                    freshCartResponse.freshCartDetail = new freshCartDetails();
                    freshCartResponse.freshCartDetail.ShoppinCartId = tempcart[0].Id;
                    freshCartResponse.freshCartDetail.MealNo = meal.MealNumber;
                    freshCartResponse.freshCartDetails.Add(freshCartResponse.freshCartDetail);
                    freshCartResponse.CustomerGUID = customer.CustomerGuid;

                }
                return Ok(freshCartResponse);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public virtual IActionResult AddDateAndTime([FromBody]DateTimeFreshCartRequest dateTimeFreshCartRequest)
        {
            foreach (var dateAndTime in dateTimeFreshCartRequest.dateAndTimes)
            {
                var freshCart = _fCartService.GetFreshCartByShoppingCartId(dateAndTime.ShoppingCartId, dateAndTime.MealNumber);
                if (freshCart == null)
                {
                    return NotFound();
                }
                var MealPlanInfo = _freshMealPlansService.GetFreshMealPlanById((int)freshCart.MealPlanId);
                if (MealPlanInfo == null)
                {
                    return BadRequest();
                }

                freshCart.MealPrice = MealPlanInfo.PerMealAmount;
                freshCart.MealDate = dateAndTime.MealDate;
                freshCart.MealTime = dateAndTime.MealTime;
                freshCart.ModifiedDate = DateTime.Now;
                _fCartService.UpdateFreshCartItem(freshCart);
            }

            return Ok("Updated Successfully");
        }

        [HttpPost]
        public virtual IActionResult UpdateFreshCartWithMultipleItems([FromBody]UpdateFreshCartWithMultipleItemsRequest updateFreshCartWithMultipleItemsRequest)
        {
            if (updateFreshCartWithMultipleItemsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                //_logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(updateFreshCartWithMultipleItemsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            if (updateFreshCartWithMultipleItemsRequest.mealDetails.Count != 0)
            {
                foreach (var mealPlan in updateFreshCartWithMultipleItemsRequest.mealDetails)
                {
                    AddProductToCartFreshRequest addProductToCartFreshRequest = new AddProductToCartFreshRequest();

                    addProductToCartFreshRequest.ApiSecretKey= updateFreshCartWithMultipleItemsRequest.ApiSecretKey;
                    addProductToCartFreshRequest.CurrencyId= updateFreshCartWithMultipleItemsRequest.CurrencyId;
                    addProductToCartFreshRequest.CustomerGUID = updateFreshCartWithMultipleItemsRequest.CustomerGUID;
                    addProductToCartFreshRequest.StoreId = updateFreshCartWithMultipleItemsRequest.StoreId;
                    addProductToCartFreshRequest.mealDetails = mealPlan.Items;
                    addProductToCartFreshRequest.ParentCategoryId = updateFreshCartWithMultipleItemsRequest.ParentCategoryId;
                    addProductToCartFreshRequest.ShoppingCartTypeId = updateFreshCartWithMultipleItemsRequest.ShoppingCartTypeId;
                    addProductToCartFreshRequest.MealPlanId = mealPlan.MealPlanId;
                    AddMealstoCartFresh(addProductToCartFreshRequest);
                }
            }
            return Ok("Items added to cart successfully");
        }

        [HttpPost]
        public virtual IActionResult FreshCart([FromBody]CartRequest cartRequest)
        {
            if (cartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
               // _logger.Error(ModelState.ToString());
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

            if (cart.Count == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();
            }
            var model = new FreshShoppingCartModelResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            PrepareFreshShoppingCartModel(model, cart, currentCustomer, cartRequest.CurrencyId, cartRequest.StoreId, cartRequest.LanguageId);

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult CalculateShipping([FromBody]CartRequest cartRequest)
        {
            if (cartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                // _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(cartRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject("CustomerNotFound").BadRequest();

            var cart = currentCustomer.ShoppingCartItems
            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            .LimitPerStore(cartRequest.StoreId)
            .ToList();

            if (cart.Count == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();
            }

            var fCart = _fCartService.GetFreshCartByCustomerId(currentCustomer.Id).Where(x=>x.MealDate!=null && x.MealTime!=null).GroupBy(x => x.MealOrderId).ToArray();

           var shippingTotal= _freshOrderTotalCalulationService.GetFreshShoppingCartShippingTotal(cart, fCart, true);

            var workingCurrency = _currencyService.GetCurrencyById(cartRequest.CurrencyId);

            decimal tempShippingCharges = _currencyService.ConvertFromPrimaryStoreCurrency((decimal)shippingTotal, workingCurrency);

            //decimal shippingCharges = _priceFormatter.FormatPrice(tempShippingCharges);
            return Ok("ShippingTotal:"+tempShippingCharges+"");

        }

        [NonAction]
        protected virtual string PrepareFreshShoppingCartModel(FreshShoppingCartModelResponse model,
           IList<ShoppingCartItem> cart, Customer currentCustomer, int currencyId, int storeId, int languageId, bool isEditable = true,
           bool validateCheckoutAttributes = false, bool prepareAndDisplayOrderReviewData = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);
            if (cart == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCart";

            if (model == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCartModel";

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;
            model.mealPlanModels = new List<FreshShoppingCartModelResponse.MealPlanModel>();
            //model.mealPlanModels.Items = new List<FreshShoppingCartModelResponse.FreshShoppingCartItemModel>();
            //model.Items = new List<FreshShoppingCartModelResponse.FreshShoppingCartItemModel>();

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
            model.DiscountBox = new FreshShoppingCartModelResponse.DiscountBoxModel
            {
                Display = _shoppingCartSettings.ShowDiscountBox
            };
            var discountCouponCode = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.DiscountCouponCodeAttribute, storeId);
            var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountCouponCode)
                    .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, currentCustomer).IsValid);
            if (discount != null)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox = new FreshShoppingCartModelResponse.GiftCardBoxModel
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
            //model.CheckoutAttributes = new List<FreshShoppingCartModelResponse.CheckoutAttributeModel>();
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new FreshShoppingCartModelResponse.CheckoutAttributeModel
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
                        attributeModel.Values = new List<FreshShoppingCartModelResponse.CheckoutAttributeValueModel>();
                        var attributeValueModel = new FreshShoppingCartModelResponse.CheckoutAttributeValueModel
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
                //model.CheckoutAttributes.Add(attributeModel);
            }

            #endregion

            #region Cart items

            var fCart = _fCartService.GetFreshCartByCustomerId(currentCustomer.Id).GroupBy(x => x.MealOrderId).ToArray();
            var fCartItems = _fCartService.GetFreshCartByCustomerId(currentCustomer.Id).GroupBy(x => x.MealOrderId).ToList();

            foreach (var item in fCart)
            {
                model.mealPlanModel = new FreshShoppingCartModelResponse.MealPlanModel();
                model.mealPlanModel.MealOrderId = (Guid)item.Key;
                model.mealPlanModel.MealPlanName = "" + item.Count().ToString() + "-MealPlan";
                model.mealPlanModel.ProductId = cart[0].ProductId;
                model.mealPlanModel.MealPlanId = (int)item.FirstOrDefault().MealPlanId;
                model.mealPlanModel.Items = new List<FreshShoppingCartModelResponse.FreshShoppingCartItemModel>();
                foreach (var sci in cart)
                {
                    var tempItem = item.Where(x => x.ShoppingCartId == sci.Id).FirstOrDefault();
                    if (tempItem != null)
                    {
                        if (tempItem.MealDate != null && tempItem.MealTime != null)
                        {
                            var mealPlanDetails = _freshMealPlansService.GetFreshMealPlanById((int)tempItem.MealPlanId);
                           var cartItemModel = new FreshShoppingCartModelResponse.FreshShoppingCartItemModel()
                            {
                                Id = sci.Id,
                                Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                                ProductId = sci.Product.Id,
                                ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name, languageId: languageId),
                                ProductSeName = _urlRecordService.GetSeName(sci.Product, languageId: languageId),
                                Quantity = sci.Quantity,
                                AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                                ParentCategoryId = sci.ParentCategoryId,
                                AttributeInfoAsArrayList = _productAttributeFormatter.FormatAttributesInArrayFormat(sci.Product, sci.AttributesXml),
                                MealDate = (DateTime)tempItem.MealDate,
                                MealTime = tempItem.MealTime,
                                MealNumber = tempItem.MealNo
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
                                decimal shoppingCartUnitPriceWithDiscountBase = _freshPriceCalculationService.GetFreshProductPrice(sci.Product, _freshPriceCalculationService.GetUnitPriceforFreshItem(sci, item), out taxRate);
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
                                decimal shoppingCartItemSubTotalWithDiscountBase = _freshPriceCalculationService.GetFreshProductPrice(sci.Product, _freshPriceCalculationService.GetSubTotalForFreshCart(sci, item, true, out decimal shoppingCartItemDiscountBase, out List<DiscountForCaching> scDiscounts, out int? maximumDiscountQty), out decimal taxRate);
                                decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, workingCurrency);
                                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                                //display an applied discount amount
                                if (shoppingCartItemDiscountBase > decimal.Zero)
                                {
                                    shoppingCartItemDiscountBase = _freshPriceCalculationService.GetFreshProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
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
                            model.mealPlanModel.Items.Add(cartItemModel);
                        }
                    }

                }

                if (model.mealPlanModel.Items.Count != 0)
                {
                    model.mealPlanModels.Add(model.mealPlanModel);
                }
                model.mealPlanModel = null;
            }




            #endregion


            return "";
        }

        protected virtual IActionResult DetailAddFreshProductToCart([FromBody]AddProductToCartRequest addProductToCartRequest)
        {

            if (addProductToCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
               // _logger.Error(ModelState.ToString());
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
                    rentalStartDt, rentalEndDt, addProductToCartRequest.Quantity, true, addProductToCartRequest.ParentCategoryId, true));
            }
            else
            {
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(currentCustomer,
                    product, cartType, addProductToCartRequest.StoreId,
                    null, decimal.Zero,
                    rentalStartDt, rentalEndDt, addProductToCartRequest.Quantity, true, addProductToCartRequest.ParentCategoryId, true));
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

        #endregion

    }
}
