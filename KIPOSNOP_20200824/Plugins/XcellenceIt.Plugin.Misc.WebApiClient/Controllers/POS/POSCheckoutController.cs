using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using System;
using System.Linq;
using System.Collections.Generic;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Services.POS;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.POS;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class POSCheckoutController: Controller
    {
        #region Fields

        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        private readonly IShoppingCartService _shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
        private readonly ILocalizationService _localizationService=EngineContext.Current.Resolve<ILocalizationService>();
        private readonly IGenericAttributeService _genericAttributeService=EngineContext.Current.Resolve<IGenericAttributeService>();
        private readonly TaxSettings _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
        private readonly ICountryService _countryService = EngineContext.Current.Resolve<ICountryService>();
        private readonly IStateProvinceService _stateProvinceService = EngineContext.Current.Resolve<IStateProvinceService>();
        private readonly IOrderService _orderService = EngineContext.Current.Resolve<IOrderService>();
        private readonly IOrderProcessingService _orderProcessingService = EngineContext.Current.Resolve<IOrderProcessingService>();
        private readonly IPermissionService _permissionService = EngineContext.Current.Resolve<IPermissionService>();
        private readonly IStoreMappingService _storeMappingService = EngineContext.Current.Resolve<IStoreMappingService>();
        private readonly ICurrencyService _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
        private readonly IPriceFormatter _priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
        private readonly IPaymentService _paymentService = EngineContext.Current.Resolve<IPaymentService>();
        private readonly ILanguageService _languageService = EngineContext.Current.Resolve<ILanguageService>();
        private readonly OrderSettings _orderSettings = EngineContext.Current.Resolve<OrderSettings>();
        private readonly CatalogSettings _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
        private readonly IProductAttributeParser _productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
        private readonly IDownloadService _downloadService = EngineContext.Current.Resolve<IDownloadService>();
        private readonly IPluginFinder _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
        private readonly ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>();
        private readonly IPriceCalculationService _priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
        private readonly ITaxService _taxService = EngineContext.Current.Resolve<ITaxService>();
        private readonly MediaSettings _mediaSettings = EngineContext.Current.Resolve<MediaSettings>();
        private readonly IPictureService _pictureService = EngineContext.Current.Resolve<IPictureService>();
        private readonly IWebHelper _webHelper = EngineContext.Current.Resolve<IWebHelper>();
        private readonly ShoppingCartSettings _shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
        private readonly IShippingService _shippingService = EngineContext.Current.Resolve<IShippingService>();
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter = EngineContext.Current.Resolve<ICheckoutAttributeFormatter>();
        private readonly IDiscountService _discountService = EngineContext.Current.Resolve<IDiscountService>();
        private readonly ICheckoutAttributeService _checkoutAttributeService = EngineContext.Current.Resolve<ICheckoutAttributeService>();
        private readonly ICheckoutAttributeParser _checkoutAttributeParser = EngineContext.Current.Resolve<ICheckoutAttributeParser>();
        private readonly AddressSettings _addressSettings = EngineContext.Current.Resolve<AddressSettings>();
        private readonly IProductAttributeFormatter _productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
        private readonly IAddressAttributeFormatter _addressAttributeFormatter = EngineContext.Current.Resolve<IAddressAttributeFormatter>();
        private readonly ShippingSettings _shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
        private readonly IOrderTotalCalculationService _orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
        private readonly RewardPointsSettings _rewardPointsSettings = EngineContext.Current.Resolve<RewardPointsSettings>();
        private readonly PaymentSettings _paymentSettings = EngineContext.Current.Resolve<PaymentSettings>();
        private readonly IRewardPointService _rewardPointService = EngineContext.Current.Resolve<IRewardPointService>();
        private readonly IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
        private readonly IAddressAttributeService _addressAttributeService = EngineContext.Current.Resolve<IAddressAttributeService>();
        private readonly IAddressAttributeParser _addressAttributeParser = EngineContext.Current.Resolve<IAddressAttributeParser>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        private readonly IUrlRecordService _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
        private readonly IGiftCardService _giftCardService = EngineContext.Current.Resolve<IGiftCardService>();
        private readonly IAddressService _addressService = EngineContext.Current.Resolve<IAddressService>();
        private readonly IRepository<Product> _productRepository = EngineContext.Current.Resolve<IRepository<Product>>();
        private readonly ICategoryService _categoryService = EngineContext.Current.Resolve<ICategoryService>();
        private readonly IPOSOrderService _POSOrderService = EngineContext.Current.Resolve<IPOSOrderService>();

        #endregion

        #region Ctor

        public POSCheckoutController()
        {
            
        }

        #endregion

        #region methods

        [HttpPost]
        public virtual IActionResult POSConfirmOrder([FromBody]POSConfirmOrderRequest confirmOrderRequest)
        {
            
            Guid tempCustomerGuid = new Guid();
                tempCustomerGuid = confirmOrderRequest.customerGuids.POSUserGuid;
            var currentCustomer = _customerService.GetCustomerByGuid(tempCustomerGuid);
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
            // code changed by phani 19-04-2019
            processPaymentRequest.CreditCardName = "";// "Hardik";
            var customValues = new Dictionary<string, object>();
            customValues.Add("PaymentMethodType", confirmOrderRequest.PaymentMethodType);
            if (!!confirmOrderRequest.IsRegisteredUser)
            {
                customValues.Add("CustomerGuid", confirmOrderRequest.customerGuids.CustomerGuid);
            }
            customValues.Add("IsRegisteredUser", confirmOrderRequest.IsRegisteredUser);
            processPaymentRequest.CustomValues = customValues;

            processPaymentRequest.CheckoutAddressDetails = confirmOrderRequest.CheckoutAddressDetails.FirstOrDefault();
            //Commented By Surakshith to display memberId in mail for register user strat on 15-06-2020
            //  var placeOrderResult = _POSOrderService.PlaceOrder(processPaymentRequest);
            //Commented By Surakshith to display memberId in mail for register user end on 15-06-2020

            //Added By Surakshith to display memberId in mail for register user strat on 15-06-2020
            bool isMember = confirmOrderRequest.IsRegisteredUser;
            string memberId = confirmOrderRequest.MemberId;
            var placeOrderResult = _POSOrderService.POSPlaceOrder(processPaymentRequest,memberId,isMember, confirmOrderRequest.OrderTax);
            //Added By Surakshith to display memberId in mail for register user end on 15-06-2020

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
        
        #endregion
    }
}
