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
    public partial class FreshCheckoutController : Controller
    {
        #region fields
        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        private readonly ICurrencyService _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
        private readonly ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        private readonly ShoppingCartSettings _shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
        private readonly CatalogSettings _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
        private readonly IGenericAttributeService _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter = EngineContext.Current.Resolve<ICheckoutAttributeFormatter>();
        private readonly IOrderProcessingService _orderProcessingService = EngineContext.Current.Resolve<IOrderProcessingService>();
        private readonly OrderSettings _orderSettings = EngineContext.Current.Resolve<OrderSettings>();
        private readonly IPriceFormatter _priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
        private readonly IDiscountService _discountService = EngineContext.Current.Resolve<IDiscountService>();
        private readonly IShoppingCartService _shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
        private readonly ICheckoutAttributeService _checkoutAttributeService = EngineContext.Current.Resolve<ICheckoutAttributeService>();
        private readonly IPermissionService _permissionService = EngineContext.Current.Resolve<IPermissionService>();
        private readonly ITaxService _taxService = EngineContext.Current.Resolve<ITaxService>();
        private readonly ICheckoutAttributeParser _checkoutAttributeParser = EngineContext.Current.Resolve<ICheckoutAttributeParser>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        private readonly IUrlRecordService _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
        private readonly IProductAttributeFormatter _productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
        private readonly IPriceCalculationService _priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
        private readonly MediaSettings _mediaSettings = EngineContext.Current.Resolve<MediaSettings>();
        private readonly AddressSettings _addressSettings = EngineContext.Current.Resolve<AddressSettings>();
        private readonly ShippingSettings _shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
        private readonly IPaymentService _paymentService = EngineContext.Current.Resolve<IPaymentService>();
        private readonly TaxSettings _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
        private readonly IOrderTotalCalculationService _orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
        private readonly ILanguageService _languageService = EngineContext.Current.Resolve<ILanguageService>();
        private readonly IGiftCardService _giftCardService = EngineContext.Current.Resolve<IGiftCardService>();
        private readonly IWebHelper _webHelper = EngineContext.Current.Resolve<IWebHelper>();
        private readonly ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>();
        private readonly IPictureService _pictureService = EngineContext.Current.Resolve<IPictureService>();
        private readonly IStateProvinceService _stateProvinceService = EngineContext.Current.Resolve<IStateProvinceService>();
        private readonly IAddressAttributeService _addressAttributeService = EngineContext.Current.Resolve<IAddressAttributeService>();
        private readonly IAddressAttributeParser _addressAttributeParser = EngineContext.Current.Resolve<IAddressAttributeParser>();
        private readonly IAddressAttributeFormatter _addressAttributeFormatter = EngineContext.Current.Resolve<IAddressAttributeFormatter>();
        private readonly IFcartService _fCartService = EngineContext.Current.Resolve<IFcartService>();
        private readonly IFreshPriceCalculationService _freshPriceCalculationService = EngineContext.Current.Resolve<IFreshPriceCalculationService>();
        private readonly IFreshMealPlansService _freshMealPlansService = EngineContext.Current.Resolve<IFreshMealPlansService>();
        private readonly IFreshOrderTotalCalulationService _freshOrderTotalCalulationService = EngineContext.Current.Resolve<IFreshOrderTotalCalulationService>();
        private readonly IFreshOrderService _freshOrderService = EngineContext.Current.Resolve<IFreshOrderService>();


        #endregion

        #region cTor

        public FreshCheckoutController()
        {

        }

        #endregion

        #region methods


        [HttpPost]
        public virtual IActionResult OrderSummaryFresh([FromBody]OrderSummaryRequest orderSummaryRequest)
        {
            var currentCustomer = _customerService.GetCustomerByGuid(orderSummaryRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(orderSummaryRequest.CurrencyId);

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(orderSummaryRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //model
            var model = new FreshOrderSummaryResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            var shoppingCartModel = PrepareOrderSummaryModel(model, cart, currentCustomer, orderSummaryRequest.CurrencyId, orderSummaryRequest.StoreId, orderSummaryRequest.LanguageId, prepareAndDisplayOrderReviewData: true);

            //If we got this far, something failed, redisplay form
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult FreshConfirmOrder([FromBody]ConfirmOrderRequest confirmOrderRequest)
        {
            var currentCustomer = _customerService.GetCustomerByGuid(confirmOrderRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(confirmOrderRequest.CurrencyId);

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(confirmOrderRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //model
            var model = PrepareConfirmOrderModel(workingCurrency, cart);
            model.Warnings = new List<string>();
            model.CheckoutCompleteResponses = new CheckoutConfirmResponse.CheckoutCompleteResponse();

            ProcessPaymentRequest processPaymentRequest = new ProcessPaymentRequest
            {
                StoreId = confirmOrderRequest.StoreId,
                CustomerId = currentCustomer.Id,
                PaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(currentCustomer,
                   NopCustomerDefaults.SelectedPaymentMethodAttribute, confirmOrderRequest.StoreId),
            };
            processPaymentRequest.CreditCardName = "Hardik";
            var customValues = new Dictionary<string, object>();
            customValues.Add("PaymentToken", confirmOrderRequest.PaymentToken);
            processPaymentRequest.CustomValues = customValues;

            processPaymentRequest.CheckoutAddressDetails = confirmOrderRequest.CheckoutAddressDetails.Where(x => x.ParentCategoryId == cart[0].ParentCategoryId).SingleOrDefault();

            var placeOrderResult = _freshOrderService.PlaceOrder(processPaymentRequest);
            if (placeOrderResult.Success)
            {
                model.CheckoutCompleteResponses.OrderId = placeOrderResult.PlacedOrder.Id;
                var postProcessPaymentRequest = new PostProcessPaymentRequest()
                {
                    Order = placeOrderResult.PlacedOrder
                };
                _paymentService.PostProcessPayment(postProcessPaymentRequest);
            }
            else
            {
                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
                return Ok(model);
            }
            //order completed
            model.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ConfirmOrder");
            model.CheckoutCompleteResponses.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;
            model.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(model);
        }

        [NonAction]
        protected virtual CheckoutConfirmResponse PrepareConfirmOrderModel(Currency workingCurrency, IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutConfirmResponse
            {
                //terms of service
                TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage
            };
            //min order amount validation
            bool minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, workingCurrency);
                model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }
            return model;
        }

        [NonAction]
        protected virtual string PrepareOrderSummaryModel(FreshOrderSummaryResponse model,
        IList<ShoppingCartItem> cart, Customer currentCustomer, int currencyId, int storeId, int languageId, bool isEditable = true,
        bool validateCheckoutAttributes = false, bool prepareAndDisplayOrderReviewData = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);
            if (cart == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCart";

            if (model == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCartModel";

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
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            //gift card and gift card boxes
            model.DiscountBox = new FreshOrderSummaryResponse.DiscountBoxModel
            {
                Display = _shoppingCartSettings.ShowDiscountBox
            };
            var discountCouponCode = _genericAttributeService
                .GetAttribute<string>(currentCustomer, NopCustomerDefaults.DiscountCouponCodeAttribute, storeId);

            var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountCouponCode)
                     .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, currentCustomer).IsValid);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.ValidateDiscount(discount, currentCustomer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox = new FreshOrderSummaryResponse.GiftCardBoxModel
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
            model.CheckoutAttributes = new List<FreshOrderSummaryResponse.CheckoutAttributeModel>();

            foreach (var attribute in checkoutAttributes)
            {
                var caModel = new FreshOrderSummaryResponse.CheckoutAttributeModel()
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
                    caModel.AllowedFileExtensions = new List<string>();
                    caModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var caValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    caModel.Values = new List<FreshOrderSummaryResponse.CheckoutAttributeValueModel>();
                    foreach (var caValue in caValues)
                    {
                        var pvaValueModel = new FreshOrderSummaryResponse.CheckoutAttributeValueModel()
                        {
                            Id = caValue.Id,
                            Name = _localizationService.GetLocalized(caValue, x => x.Name, languageId: languageId),
                            ColorSquaresRgb = caValue.ColorSquaresRgb,
                            IsPreSelected = caValue.IsPreSelected,
                        };
                        caModel.Values.Add(pvaValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(caValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, workingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                pvaValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                pvaValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                string selectedCheckoutAttributes = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.CheckoutAttributes, storeId);
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
                                foreach (var item in caModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedCaValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var caValue in selectedCaValues)
                                    foreach (var item in caModel.Values)
                                        if (caValue.Id == item.Id)
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
                                    caModel.DefaultValue = enteredText[0];
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
                                    caModel.SelectedDay = selectedDate.Day;
                                    caModel.SelectedMonth = selectedDate.Month;
                                    caModel.SelectedYear = selectedDate.Year;
                                }
                            }

                        }
                        break;
                    default:
                        break;
                }
                model.CheckoutAttributes.Add(caModel);
            }

            #endregion

            #region Cart items

            var fCart = _fCartService.GetFreshCartByCustomerId(currentCustomer.Id).GroupBy(x => x.MealOrderId).ToArray();
            model.mealPlanModels = new List<FreshOrderSummaryResponse.MealPlanModel>();

            foreach (var item in fCart)
            {
                model.mealPlanModel = new FreshOrderSummaryResponse.MealPlanModel();
                model.mealPlanModel.MealOrderId = (Guid)item.Key;
                model.mealPlanModel.MealPlanName = "" + item.Count().ToString() + "-MealPlan";
                model.mealPlanModel.ProductId = cart[0].ProductId;
                model.mealPlanModel.Items = new List<FreshOrderSummaryResponse.FreshShoppingCartItemModel>();
                foreach (var sci in cart)
                {
                    var tempItem = item.Where(x => x.ShoppingCartId == sci.Id).FirstOrDefault();
                    if (tempItem != null)
                    {
                        if (tempItem.MealDate != null && tempItem.MealTime != null)
                        {
                            var mealPlanDetails = _freshMealPlansService.GetFreshMealPlanById((int)tempItem.MealPlanId);
                            var tempPrice = mealPlanDetails.PerMealAmount;
                            var cartItemModel = new FreshOrderSummaryResponse.FreshShoppingCartItemModel()
                            {
                                Id = sci.Id,
                                Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                                ProductId = sci.Product.Id,
                                ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name, languageId: languageId),
                                ProductSeName = _urlRecordService.GetSeName(sci.Product, languageId: languageId),
                                Quantity = sci.Quantity,
                                AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                                ParentCategoryId = sci.ParentCategoryId,
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
                                cartItemModel.AllowedQuantities.Add(new SelectListItem()
                                {
                                    Text = qty.ToString(),
                                    Value = qty.ToString(),
                                    Selected = sci.Quantity == qty
                                });
                            }

                            //recurring info
                            if (sci.Product.IsRecurring)
                                cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"),
                                    sci.Product.RecurringCycleLength,
                                    _localizationService.GetLocalizedEnum(sci.Product.RecurringCyclePeriod, languageId));

                            //unit prices
                            if (sci.Product.CallForPrice)
                            {
                                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                            }
                            else
                            {
                                decimal taxRate = decimal.Zero;
                                decimal shoppingCartUnitPriceWithDiscountBase = _freshPriceCalculationService.GetFreshProductPrice(sci.Product, _freshPriceCalculationService.GetUnitPriceforFreshItem(sci,item,true), out taxRate);
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
                                //sub total
                                decimal taxRate = decimal.Zero;
                                decimal shoppingCartItemSubTotalWithDiscountBase = _freshPriceCalculationService.GetFreshProductPrice(sci.Product, _freshPriceCalculationService.GetSubTotalForFreshCart(sci,item, true), out taxRate);
                                decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, workingCurrency);
                                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                                //display an applied discount amount
                                decimal shoppingCartItemSubTotalWithoutDiscountBase = _freshPriceCalculationService.GetFreshProductPrice(sci.Product, _freshPriceCalculationService.GetSubTotalForFreshCart(sci, item, false), out taxRate);
                                decimal shoppingCartItemDiscountBase = shoppingCartItemSubTotalWithoutDiscountBase - shoppingCartItemSubTotalWithDiscountBase;
                                if (shoppingCartItemDiscountBase > decimal.Zero)
                                {
                                    decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, workingCurrency);
                                    cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                                }
                            }

                            //picture
                            if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                            {
                                cartItemModel.Picture = new PictureModel();
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
                            cartItemModel.Warnings = new List<string>();
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


            #region Order Total

            //order total
            model.IsEditable = isEditable;
            model.TaxRates = new List<FreshOrderSummaryResponse.TaxRate>();

            if (cart.Count > 0)
            {
                //subtotal
                decimal subtotalBase = decimal.Zero;
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                var subTotalIncludingTax = _taxSettings.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _freshOrderTotalCalulationService.GetFreshShoppingCartSubTotal(cart,fCart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out List<DiscountForCaching> orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, workingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, workingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);

                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                        orderSubTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !string.IsNullOrEmpty(d.CouponCode));
                }


                //shipping info
                model.RequiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _freshOrderTotalCalulationService.GetFreshShoppingCartShippingTotal(cart,fCart,false);
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
                string paymentMethodSystemName = _genericAttributeService
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
                    decimal shoppingCartTaxBase = _freshOrderTotalCalulationService.GetTaxTotal(cart,fCart, out SortedDictionary<decimal, decimal> taxRates);
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
                        model.TaxRates = new List<FreshOrderSummaryResponse.TaxRate>();
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new FreshOrderSummaryResponse.TaxRate()
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, workingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                decimal orderTotalDiscountAmountBase = decimal.Zero;
                decimal redeemedRewardPointsAmount = decimal.Zero;
                decimal? shoppingCartTotalBase = _freshOrderTotalCalulationService.GetFreshShoppingCartTotal(cart,fCart,
                    out orderTotalDiscountAmountBase, out List<DiscountForCaching> orderTotalAppliedDiscounts,
                    out List<AppliedGiftCard> appliedGiftCards, out int redeemedRewardPoints, out redeemedRewardPointsAmount);
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
                var gcModel = new FreshOrderSummaryResponse.GiftCard();
                model.GiftCards = new List<FreshOrderSummaryResponse.GiftCard>();
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        gcModel.Id = appliedGiftCard.GiftCard.Id;
                        gcModel.CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode;
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, workingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = _giftCardService.GetGiftCardRemainingAmount(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, workingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, workingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }
            }
            #endregion

            return "";
        }

        [NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci,
            int pictureSize, bool showDefaultPicture, string productName, int languageId, int storeId)
        {
            //This method is similar to ShoppingCartModelFactory >> PrepareCartItemPictureModel() 
            //but we write this method because inbuilt method of nopcommerce use _workContext.WorkingLanguage.Id 
            //so when we uses API we explicitly need to set current customer. 
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, languageId, _webHelper.IsCurrentConnectionSecured(), storeId);
            //as we cache per user (shopping cart item identifier)
            //let's cache just for 3 minutes
            var cacheTime = 3;
            var model = _cacheManager.Get(pictureCacheKey, () =>
            {
                //shopping cart item picture
                var sciPicture = _pictureService.GetProductPicture(sci.Product, sci.AttributesXml);
                return new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                };
            }, cacheTime);
            return model;
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
                model.CountryName = address.Country != null
                    ? _localizationService.GetLocalized(address.Country, x => x.Name, languageId: languageId)
                    : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = address.StateProvince != null
                    ? _localizationService.GetLocalized(address.StateProvince, x => x.Name, languageId: languageId)
                    : null;
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
                            Name = _localizationService.GetLocalized(attribute, x => x.Name, languageId: languageId),
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
    }
}
