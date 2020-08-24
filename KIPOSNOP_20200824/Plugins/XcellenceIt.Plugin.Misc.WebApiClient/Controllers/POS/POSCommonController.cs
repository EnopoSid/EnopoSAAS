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
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.POS;
using Nop.Services.Security;
using Nop.Services.POS;
using Nop.Core.Infrastructure;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class POSCommonController : Controller
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
        private readonly IPOSOrderService _POSOrderService = EngineContext.Current.Resolve<IPOSOrderService>();
        private readonly IOrderService _orderService = EngineContext.Current.Resolve<IOrderService>();
        
        #endregion

        #region Ctor

        public POSCommonController(
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
        }

        #endregion

        #region methods
        [HttpPost]
        public virtual List<IActionResult> ApplyMemberDiscounts([FromBody]POSDefaultCouponDiscountRequest discountRequests)
        {
            List<IActionResult> returnResponse = new List<IActionResult>();
            foreach(var customerGuid in discountRequests.CustomerGUIDs)
            {
                foreach (DiscountRequest discountRequest in discountRequests.DiscountsInfo)
                {
                    discountRequest.ApiSecretKey = discountRequests.ApiSecretKey;
                    discountRequest.CustomerGUID = customerGuid;
                    discountRequest.StoreId = discountRequests.StoreId;
                    returnResponse.Add(ApplyDiscountLocal(discountRequest));
                }
            }

            return returnResponse;
        }

        [HttpPost]
        public virtual IActionResult ApplyPOSDiscount([FromBody]POSDiscountRequest discountRequest)
        {
            List<IActionResult> returnResponse = new List<IActionResult>();
            var userErrors = new List<string>();
            foreach (var customerGuid in discountRequest.CustomerGUIDs)
            {
                var currentCustomer = _customerService.GetCustomerByGuid(customerGuid);
                var discount = _discountService.GetAllDiscountsForCaching(couponCode: discountRequest.DiscountCouponCode, showHidden: true)
                .Where(d => d.RequiresCouponCode)
                .ToList();
                if (discount.FirstOrDefault() != null)
                {
                    var anyValidDiscount = discount.Any(d =>
                    {
                        var validationResult = _discountService.ValidateDiscount(d, currentCustomer, new[] { discountRequest.DiscountCouponCode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });
                }
                else
                {
                    userErrors.Add("Coupon Code doesn't exists");
                }
            };
            if (userErrors.Count != 0)
            {
                if (userErrors.Count == 2)
                {
                    return new ResponseObject(userErrors[0].ToString()).BadRequest();
                }
                else
                {
                    return new ResponseObject("Invalid CouponCode").BadRequest();
                }
                
            }
            else
            {
                foreach (var customerGuid in discountRequest.CustomerGUIDs)
                {
                    DiscountRequest tempDiscount = new DiscountRequest
                    {
                        ApiSecretKey = discountRequest.ApiSecretKey,
                        CustomerGUID = customerGuid,
                        DiscountCouponCode = discountRequest.DiscountCouponCode,
                        OrderTotal = discountRequest.OrderTotal,
                        StoreId = discountRequest.StoreId
                    };
                    ApplyDiscountLocal(tempDiscount);
                }
                return Ok("DiscountsApplied");
            }
        }

        [HttpPost]
        public virtual List<IActionResult> RemovePOSDiscount([FromBody]POSDiscountRequest discountRequestPOS)
        {
            List<IActionResult> returnResponse = new List<IActionResult>();
            foreach(var customerGuid in discountRequestPOS.CustomerGUIDs)
            {
                DiscountRequest discountRequest = new DiscountRequest()
                {
                    ApiSecretKey = discountRequestPOS.ApiSecretKey,
                    CustomerGUID = customerGuid,
                    DiscountCouponCode = discountRequestPOS.DiscountCouponCode,
                    OrderTotal = discountRequestPOS.OrderTotal,
                    StoreId = discountRequestPOS.StoreId
                };
                returnResponse.Add(RemoveDiscountLocal(discountRequest));
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
                            //valid
                            _customerService.ApplyDiscountCouponCode(currentCustomer, discountRequest.DiscountCouponCode);
                            CustomerModel customerModel = new CustomerModel
                            {
                                CustomerGuid = currentCustomer.CustomerGuid,
                                Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied")
                            };

                            return Ok(customerModel);
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
                        return new ResponseObject("Discount Code Not Applied as Discount Amount should be less than the Order Amount").BadRequest();
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

        [NonAction]
        protected virtual IActionResult RemoveDiscountLocal(DiscountRequest discountRequest)
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


        #endregion

    }
}
