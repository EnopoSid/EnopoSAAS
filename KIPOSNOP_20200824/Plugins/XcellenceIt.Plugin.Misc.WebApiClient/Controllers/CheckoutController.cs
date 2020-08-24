using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Http.Extensions;
using Nop.Core.Plugins;
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
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class CheckoutController : Controller
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly TaxSettings _taxSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPaymentService _paymentService;
        private readonly ILanguageService _languageService;
        private readonly OrderSettings _orderSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ICacheManager _cacheManager;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IShippingService _shippingService;
        private readonly ILogger _logger;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IDiscountService _discountService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly AddressSettings _addressSettings;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IRewardPointService _rewardPointService;
        private readonly IWorkContext _workContext;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IGiftCardService _giftCardService;
        private readonly IAddressService _addressService;
        private readonly IRepository<Product> _productRepository;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        public CheckoutController(
        ICustomerService customerService,
        IShoppingCartService shoppingCartService,
        ILocalizationService localizationService,
        IGenericAttributeService genericAttributeService,
        TaxSettings taxSettings,
        ICountryService countryService,
        IStateProvinceService stateProvinceService,
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IPermissionService permissionService,
        IStoreMappingService storeMappingService,
        ICurrencyService currencyService,
        IPriceFormatter priceFormatter,
        IPaymentService paymentService,
        ILanguageService languageService,
        OrderSettings orderSettings,
        CatalogSettings catalogSettings,
        IProductAttributeParser productAttributeParser,
        IDownloadService downloadService,
        IPluginFinder pluginFinder,
        ICacheManager cacheManager,
        IPriceCalculationService priceCalculationService,
        ITaxService taxService,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IWebHelper webHelper,
        ShoppingCartSettings shoppingCartSettings,
        IShippingService shippingService,
        ILogger logger,
        ICheckoutAttributeFormatter checkoutAttributeFormatter,
        IDiscountService discountService,
        ICheckoutAttributeService checkoutAttributeService,
        ICheckoutAttributeParser checkoutAttributeParser,
        AddressSettings addressSettings,
        IProductAttributeFormatter productAttributeFormatter,
        IAddressAttributeFormatter addressAttributeFormatter,
        ShippingSettings shippingSettings,
        IOrderTotalCalculationService orderTotalCalculationService,
        RewardPointsSettings rewardPointsSettings,
        PaymentSettings paymentSettings,
        IRewardPointService rewardPointService,
        IWorkContext workContext,
        IAddressAttributeService addressAttributeService,
        IAddressAttributeParser addressAttributeParser,
        IProductService productService,
        IUrlRecordService urlRecordService,
        IGiftCardService giftCardService,
        IAddressService addressService,
         IRepository<Product> productRepository,
         ICategoryService categoryService)
        {
            this._customerService = customerService;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._taxSettings = taxSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._paymentService = paymentService;
            this._languageService = languageService;
            this._orderSettings = orderSettings;
            this._catalogSettings = catalogSettings;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._pluginFinder = pluginFinder;
            this._cacheManager = cacheManager;
            this._priceCalculationService = priceCalculationService;
            this._mediaSettings = mediaSettings;
            this._webHelper = webHelper;
            this._pictureService = pictureService;
            this._taxService = taxService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._shippingService = shippingService;
            this._logger = logger;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._discountService = discountService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._addressSettings = addressSettings;
            this._productAttributeFormatter = productAttributeFormatter;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._shippingSettings = shippingSettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._workContext = workContext;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._rewardPointService = rewardPointService;
            this._productService = productService;
            this._urlRecordService = urlRecordService;
            this._giftCardService = giftCardService;
            this._addressService = addressService;
            this._productRepository = productRepository;
            this._categoryService = categoryService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual CheckoutBillingAddressResponse PrepareBillingAddressModel(Customer currentCustomer, int languageId, int storeId,
            int? selectedCountryId = null, bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressResponse();
            //existing addresses
            //existing addresses
            var addresses = currentCustomer.Addresses
                .Where(a => a.Country == null ||
                    (//published
                    a.Country.Published &&
                    //allow billing
                    a.Country.AllowsBilling &&
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country, storeId)))
                .ToList();
            model.ExistingAddresses = new List<AddressModel>();
            model.NewAddress = new AddressModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                PrepareAddressModel(addressModel,
                     address: address,
                     languageId: languageId,
                     excludeProperties: false,
                     addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            PrepareAddressModel(model.NewAddress,
                 address: null,
                 excludeProperties: false,
                 languageId: languageId,
                 addressSettings: _addressSettings,
                 prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                 customer: currentCustomer,
                 overrideAttributesXml: overrideAttributesXml);
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
        protected virtual CheckoutShippingMethodResponse PrepareShippingMethodModel(Customer currentCustomer, Currency workingCurrency, int storeId, IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutShippingMethodResponse
            {
                ShippingMethods = new List<CheckoutShippingMethodResponse.ShippingMethodModel>()
            };

            var getShippingOptionResponse = _shippingService.GetShippingOptions(cart, currentCustomer.ShippingAddress, currentCustomer, storeId: storeId);

            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                _genericAttributeService.SaveAttribute(currentCustomer,
                                                       NopCustomerDefaults.OfferedShippingOptionsAttribute,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       storeId);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodResponse.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = shippingOption,
                    };

                    //adjust rate
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out List<DiscountForCaching> appliedDiscounts);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, currentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase,
                                                                                    workingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = _genericAttributeService.GetAttribute<ShippingOption>(currentCustomer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute, storeId);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                           !string.IsNullOrEmpty(so.Name) &&
                           so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                           !string.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                           so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
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

        [NonAction]
        protected virtual CheckoutShippingAddressResponse PrepareShippingAddressModel(Customer currentCustomer, int currencyId, int languageId, int? selectedCountryId = null,
           bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "", int storeId = 0)
        {
            var model = new CheckoutShippingAddressResponse();
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);

            //allow pickup in store?
            model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;
            if (model.AllowPickUpInStore)
            {
                model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
                model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
                var pickupPointProviders = _shippingService.LoadActivePickupPointProviders(currentCustomer, storeId);
                if (pickupPointProviders.Any())
                {
                    var pickupPointsResponse = _shippingService.GetPickupPoints(currentCustomer.BillingAddress,
                        currentCustomer, storeId: storeId);
                    if (pickupPointsResponse.Success)
                        model.PickupPoints = pickupPointsResponse.PickupPoints.Select(x =>
                        {
                            var country = _countryService.GetCountryByTwoLetterIsoCode(x.CountryCode);
                            var pickupPointModel = new CheckoutPickupPointModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProviderSystemName = x.ProviderSystemName,
                                Address = x.Address,
                                City = x.City,
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = x.ZipPostalCode,
                                Latitude = x.Latitude,
                                Longitude = x.Longitude,
                                OpeningHours = x.OpeningHours
                            };
                            if (x.PickupFee > 0)
                            {
                                var amount = _taxService.GetShippingPrice(x.PickupFee, currentCustomer);
                                amount = _currencyService.ConvertFromPrimaryStoreCurrency(amount, workingCurrency);
                                pickupPointModel.PickupFee = _priceFormatter.FormatShippingPrice(amount, true);
                            }

                            return pickupPointModel;
                        }).ToList();
                    else
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Warnings.Add(error);
                }

                //only available pickup points
                if (!_shippingService.LoadActiveShippingRateComputationMethods(currentCustomer, storeId).Any())
                {
                    if (!pickupPointProviders.Any())
                    {
                        model.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                        model.Warnings.Add(_localizationService.GetResource("Checkout.PickupPoints.NotAvailable"));
                    }
                    model.PickUpInStoreOnly = true;
                    model.PickUpInStore = true;
                    return model;
                }
            }

            //existing addresses
            var addresses = currentCustomer.Addresses
                .Where(a => a.Country == null ||
                    (//published
                    a.Country.Published &&
                    //allow billing
                    a.Country.AllowsBilling &&
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country, storeId)))
                .ToList();
            model.ExistingAddresses = new List<AddressModel>();
            model.NewAddress = new AddressModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                PrepareAddressModel(addressModel,
                    address: address,
                    languageId: languageId,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            PrepareAddressModel(model.NewAddress,
                 address: null,
                 languageId: languageId,
                 excludeProperties: false,
                 addressSettings: _addressSettings,
                 loadCountries: () => _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id),
                 prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                 customer: currentCustomer,
                 overrideAttributesXml: overrideAttributesXml);
            return model;
        }

        [NonAction]
        protected virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool ignoreRewardPoints = false)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, ignoreRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        [NonAction]
        protected virtual CheckoutPaymentMethodResponse PreparePaymentMethodModel(Customer currentCustomer, Currency workingCurrency, int storeId, int languageId, IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodResponse
            {
                PaymentMethods = new List<CheckoutPaymentMethodResponse.PaymentMethodModel>()
            };

            //reward points
            if (_rewardPointsSettings.Enabled && !_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(currentCustomer.Id, storeId);
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, workingCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    _orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(currentCustomer, storeId, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (_shoppingCartService.ShoppingCartIsRecurring(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodResponse.PaymentMethodModel
                {
                    Name = _localizationService.GetLocalizedFriendlyName(pm, languageId),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = PluginManager.GetLogoUrl(pm.PluginDescriptor)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, currentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, workingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(currentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, storeId);
            if (!string.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }

        [NonAction]
        protected virtual string PrepareShoppingCartModel(ShoppingCartModelResponse model,
           IList<ShoppingCartItem> cart, Customer currentCustomer, int languageId, int currencyId, int storeId, bool isEditable = true,
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
            var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(currentCustomer,
                NopCustomerDefaults.CheckoutAttributes, storeId);
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

            var requiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(storeId, !requiresShipping);
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
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name, languageId: languageId),
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
                model.OrderReviewData.PaymentMethod = paymentMethod != null
                        ? _localizationService.GetLocalizedFriendlyName(paymentMethod, languageId: languageId)
                        : "";

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
        protected virtual bool IsMinimumOrderPlacementIntervalValid(int storeId, Customer currentCustomer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: storeId,
                customerId: currentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
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
        protected virtual string PrepareOrderSummaryModel(OrderSummaryResponse model,
           IList<ShoppingCartItem> cart, Customer currentCustomer, int currencyId, int storeId, int languageId, bool isEditable = true,
           bool validateCheckoutAttributes = false, bool prepareAndDisplayOrderReviewData = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);
            if (cart == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCart";

            if (model == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.EmptyCartModel";

            if (cart.Count == 0)
                return "Plugins.XcellenceIT.WebApiClient.Message.E" +
                    "mptyCart";

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
            model.DiscountBox = new OrderSummaryResponse.DiscountBoxModel
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
            model.GiftCardBox = new OrderSummaryResponse.GiftCardBoxModel
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
            model.CheckoutAttributes = new List<OrderSummaryResponse.CheckoutAttributeModel>();

            foreach (var attribute in checkoutAttributes)
            {
                var caModel = new OrderSummaryResponse.CheckoutAttributeModel()
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
                    caModel.Values = new List<OrderSummaryResponse.CheckoutAttributeValueModel>();
                    foreach (var caValue in caValues)
                    {
                        var pvaValueModel = new OrderSummaryResponse.CheckoutAttributeValueModel()
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

            model.Items = new List<OrderSummaryResponse.ShoppingCartItemModel>();

            foreach (var sci in cart)
            {
                var cartItemModel = new OrderSummaryResponse.ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                    ProductId = sci.Product.Id,
                    ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name, languageId: languageId),
                    ProductSeName = _urlRecordService.GetSeName(sci.Product, languageId: languageId),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml)
                    
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
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
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
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, workingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    decimal shoppingCartItemSubTotalWithoutDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, false), out taxRate);
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
                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Order review data

            model.OrderReviewData = new OrderSummaryResponse.OrderReviewDataModel();
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = currentCustomer.BillingAddress;
                if (billingAddress != null)
                    PrepareAddressModel(model.OrderReviewData.BillingAddress,
                        address: billingAddress,
                        languageId: languageId,
                        excludeProperties: false,
                        addressSettings: _addressSettings);

                //shipping info
                if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
                {
                    model.OrderReviewData.IsShippable = true;

                    if (_shippingSettings.AllowPickUpInStore)
                    {
                        var pickupPoint = _genericAttributeService
                            .GetAttribute<PickupPoint>(currentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, storeId);
                        model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                    }

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = currentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            PrepareAddressModel(model.OrderReviewData.ShippingAddress,
                            address: currentCustomer.ShippingAddress,
                            languageId: languageId,
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
                model.OrderReviewData.PaymentMethod = paymentMethod != null
                        ? _localizationService.GetLocalizedFriendlyName(paymentMethod, languageId: languageId)
                        : "";
            }

            #endregion

            #region Order Total

            //order total
            model.IsEditable = isEditable;
            model.TaxRates = new List<OrderSummaryResponse.TaxRate>();

            if (cart.Count > 0)
            {
                //subtotal
                decimal subtotalBase = decimal.Zero;
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                var subTotalIncludingTax = _taxSettings.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                //_orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                //    out orderSubTotalDiscountAmountBase, out List<DiscountForCaching> orderSubTotalAppliedDiscounts,
                //    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                   out orderSubTotalDiscountAmountBase, out List<DiscountForCaching> orderSubTotalAppliedDiscounts,
                 out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, workingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);

                decimal tempCategoryDiscount = 0;
                for (int i=0;i<model.Items.Count;i++)
                {
                    if (model.Items[i].Discount!=null)
                    {
                        if (Convert.ToDecimal(model.Items[i].Discount.Split("$")[1]) > 0)
                        {
                            //if (Convert.ToDecimal(model.Items[i].Discount.Split("$")[1])>subtotal)
                            //{
                            //    tempCategoryDiscount = subtotal;
                            //}
                            //else
                            //{
                                tempCategoryDiscount = tempCategoryDiscount + Convert.ToDecimal(model.Items[i].Discount.Split("$")[1]);
                            //}
                        }
                    }
                }

                orderSubTotalDiscountAmountBase = orderSubTotalDiscountAmountBase + tempCategoryDiscount;
                subtotal = subtotal + tempCategoryDiscount;
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);


                //Added By Surakshith to get excluding of tax subtotal to show Tax amount in subtotal started on 28-07-2020
                decimal subtotalBasecc = decimal.Zero;
                decimal orderSubTotalDiscountAmountBasecc = decimal.Zero;
                decimal subTotalWithoutDiscountBasecc = decimal.Zero;
                decimal subTotalWithDiscountBasecc = decimal.Zero;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, false,
                   out orderSubTotalDiscountAmountBasecc, out List<DiscountForCaching> orderSubTotalAppliedDiscountscc,
                 out subTotalWithoutDiscountBasecc, out subTotalWithDiscountBasecc);
                subtotalBasecc = subTotalWithoutDiscountBasecc;
                model.SubTotalExcludingofTax = subtotalBasecc;
                //Added By Surakshith to get excluding of tax subtotal to show Tax amount in subtotal started on 28-07-2020

                //var appliedDiscountCount = 0;
                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    //Added By sree for inclusion of tax 07-17-2020 start
                    decimal temporaryDiscountAmount = 0;
                    foreach (var tempDiscount in orderSubTotalAppliedDiscounts)
                        //Added By sree to for fixed amount discount requirement 16_08_2020 start
                        if (!tempDiscount.UsePercentage)
                        {
                            if (tempDiscount.DiscountAmount>=Convert.ToDecimal(model.SubTotal.Split('$')[1]))
                            {
                                temporaryDiscountAmount = Convert.ToDecimal(model.SubTotal.Split('$')[1]);
                            }
                            else
                            {
                                temporaryDiscountAmount = temporaryDiscountAmount + tempDiscount.DiscountAmount;
                            }
                        }
                        //Added By sree for fixed amount discount requirement 16_08_2020 end
                        else
                        {
                            decimal percentagediscount = (subtotal * tempDiscount.DiscountPercentage / 100);
                            //Added By sree for maxdisocunt amount validation 19_08_2020 start
                            if (tempDiscount.MaximumDiscountAmount > 0)
                            {
                                if(percentagediscount>= tempDiscount.MaximumDiscountAmount)
                                {
                                    percentagediscount =Convert.ToDecimal(tempDiscount.MaximumDiscountAmount);
                                }
                            }
                            //Added By sree for maxdisocunt amount validation 19_08_2020 end
                            if (percentagediscount >= Convert.ToDecimal(model.SubTotal.Split('$')[1]))
                            {
                                percentagediscount = Convert.ToDecimal(model.SubTotal.Split('$')[1]);
                            }
                            temporaryDiscountAmount = temporaryDiscountAmount + percentagediscount;
                        }
                    
                    if (temporaryDiscountAmount!=0)
                    {
                        orderSubTotalDiscountAmountBase = temporaryDiscountAmount;
                    }
                    //Added By sree for inclusion of tax 07-17-2020 end

                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, workingCurrency);
                    //model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);
                    model.SubTotalDiscount = orderSubTotalDiscountAmount;


                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                        orderSubTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !string.IsNullOrEmpty(d.CouponCode));
                    string SpecailDiscount50CentAmountobj = orderSubTotalAppliedDiscounts.Any(d => d.CouponCode == "TakeAwayDiscount_50CEN").ToString();
                    var obj = orderSubTotalAppliedDiscounts.Where(x => x.CouponCode == "TakeAwayDiscount_50CEN").SingleOrDefault();
                    if(obj!=null)
                    {
                        decimal value = obj.DiscountAmount;
                        if (!(string.IsNullOrEmpty(SpecailDiscount50CentAmountobj)))
                        {
                            model.SpecailDiscount50CentAmount = value;
                        }
                        else
                        {
                            model.SpecailDiscount50CentAmount = 0;
                        }
                    }
                    else
                    {
                        model.SpecailDiscount50CentAmount = 0;
                    }
                    
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
                        model.TaxRates = new List<OrderSummaryResponse.TaxRate>();
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderSummaryResponse.TaxRate()
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
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
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
                var gcModel = new OrderSummaryResponse.GiftCard();
                model.GiftCards = new List<OrderSummaryResponse.GiftCard>();
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

        //Added By Surakshith to differentiate POS and Online OrderSummary Method 16_08_2020 start
        [NonAction]
        protected virtual string POSPrepareOrderSummaryModel(OrderSummaryResponse model,
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
            model.DiscountBox = new OrderSummaryResponse.DiscountBoxModel
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
            model.GiftCardBox = new OrderSummaryResponse.GiftCardBoxModel
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
            model.CheckoutAttributes = new List<OrderSummaryResponse.CheckoutAttributeModel>();

            foreach (var attribute in checkoutAttributes)
            {
                var caModel = new OrderSummaryResponse.CheckoutAttributeModel()
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
                    caModel.Values = new List<OrderSummaryResponse.CheckoutAttributeValueModel>();
                    foreach (var caValue in caValues)
                    {
                        var pvaValueModel = new OrderSummaryResponse.CheckoutAttributeValueModel()
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

            model.Items = new List<OrderSummaryResponse.ShoppingCartItemModel>();

            foreach (var sci in cart)
            {
                var cartItemModel = new OrderSummaryResponse.ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                    ProductId = sci.Product.Id,
                    ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name, languageId: languageId),
                    ProductSeName = _urlRecordService.GetSeName(sci.Product, languageId: languageId),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml)

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
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
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
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, workingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    decimal shoppingCartItemSubTotalWithoutDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, false), out taxRate);
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
                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Order review data

            model.OrderReviewData = new OrderSummaryResponse.OrderReviewDataModel();
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = currentCustomer.BillingAddress;
                if (billingAddress != null)
                    PrepareAddressModel(model.OrderReviewData.BillingAddress,
                        address: billingAddress,
                        languageId: languageId,
                        excludeProperties: false,
                        addressSettings: _addressSettings);

                //shipping info
                if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
                {
                    model.OrderReviewData.IsShippable = true;

                    if (_shippingSettings.AllowPickUpInStore)
                    {
                        var pickupPoint = _genericAttributeService
                            .GetAttribute<PickupPoint>(currentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, storeId);
                        model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                    }

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = currentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            PrepareAddressModel(model.OrderReviewData.ShippingAddress,
                            address: currentCustomer.ShippingAddress,
                            languageId: languageId,
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
                model.OrderReviewData.PaymentMethod = paymentMethod != null
                        ? _localizationService.GetLocalizedFriendlyName(paymentMethod, languageId: languageId)
                        : "";
            }

            #endregion

            #region Order Total

            //order total
            model.IsEditable = isEditable;
            model.TaxRates = new List<OrderSummaryResponse.TaxRate>();

            if (cart.Count > 0)
            {
                //subtotal
                decimal subtotalBase = decimal.Zero;
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                var subTotalIncludingTax = _taxSettings.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                //_orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                //    out orderSubTotalDiscountAmountBase, out List<DiscountForCaching> orderSubTotalAppliedDiscounts,
                //    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                   out orderSubTotalDiscountAmountBase, out List<DiscountForCaching> orderSubTotalAppliedDiscounts,
                 out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, workingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);

                decimal tempCategoryDiscount = 0;
                for (int i = 0; i < model.Items.Count; i++)
                {
                    if (model.Items[i].Discount != null)
                    {
                        if (Convert.ToDecimal(model.Items[i].Discount.Split("$")[1]) > 0)
                        {
                            tempCategoryDiscount = tempCategoryDiscount + Convert.ToDecimal(model.Items[i].Discount.Split("$")[1]);
                        }
                    }
                }

                orderSubTotalDiscountAmountBase = orderSubTotalDiscountAmountBase + tempCategoryDiscount;
                subtotal = subtotal + tempCategoryDiscount;
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);


                //Added By Surakshith to get excluding of tax subtotal to show Tax amount in subtotal started on 28-07-2020
                decimal subtotalBasecc = decimal.Zero;
                decimal orderSubTotalDiscountAmountBasecc = decimal.Zero;
                decimal subTotalWithoutDiscountBasecc = decimal.Zero;
                decimal subTotalWithDiscountBasecc = decimal.Zero;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, false,
                   out orderSubTotalDiscountAmountBasecc, out List<DiscountForCaching> orderSubTotalAppliedDiscountscc,
                 out subTotalWithoutDiscountBasecc, out subTotalWithDiscountBasecc);
                subtotalBasecc = subTotalWithoutDiscountBasecc;
                model.SubTotalExcludingofTax = subtotalBasecc;
                //Added By Surakshith to get excluding of tax subtotal to show Tax amount in subtotal started on 28-07-2020

                //var appliedDiscountCount = 0;
                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    //Added By sree for inclusion of tax 07-17-2020 start
                    decimal temporaryDiscountAmount = 0;
                    foreach (var tempDiscount in orderSubTotalAppliedDiscounts)
                        if (!tempDiscount.UsePercentage)
                        {
                            temporaryDiscountAmount = temporaryDiscountAmount + tempDiscount.DiscountAmount;
                        }
                        else
                        {
                            decimal percentagediscount = (subtotal * tempDiscount.DiscountPercentage / 100);
                            //Added  By sree for maxdiscount amount condition 19_08_2020 start
                            if (tempDiscount.MaximumDiscountAmount > 0)
                            {
                                if(percentagediscount>= Convert.ToDecimal(tempDiscount.MaximumDiscountAmount))
                                {
                                    percentagediscount = Convert.ToDecimal(tempDiscount.MaximumDiscountAmount);
                                }
                            }
                            //Added  By sree for maxdiscount amount condition 19_08_2020 start
                            temporaryDiscountAmount = temporaryDiscountAmount + percentagediscount;
                        }

                    if (temporaryDiscountAmount != 0)
                    {
                        orderSubTotalDiscountAmountBase = temporaryDiscountAmount;
                    }
                    //Added By sree for inclusion of tax 07-17-2020 end

                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, workingCurrency);
                    //model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, workingCurrency, _languageService.GetLanguageById(languageId), subTotalIncludingTax);
                    model.SubTotalDiscount = orderSubTotalDiscountAmount;


                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                        orderSubTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !string.IsNullOrEmpty(d.CouponCode));
                    string SpecailDiscount50CentAmountobj = orderSubTotalAppliedDiscounts.Any(d => d.CouponCode == "TakeAwayDiscount_50CEN").ToString();
                    var obj = orderSubTotalAppliedDiscounts.Where(x => x.CouponCode == "TakeAwayDiscount_50CEN").SingleOrDefault();
                    if (obj != null)
                    {
                        decimal value = obj.DiscountAmount;
                        if (!(string.IsNullOrEmpty(SpecailDiscount50CentAmountobj)))
                        {
                            model.SpecailDiscount50CentAmount = value;
                        }
                        else
                        {
                            model.SpecailDiscount50CentAmount = 0;
                        }
                    }
                    else
                    {
                        model.SpecailDiscount50CentAmount = 0;
                    }

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
                        model.TaxRates = new List<OrderSummaryResponse.TaxRate>();
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderSummaryResponse.TaxRate()
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
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
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
                var gcModel = new OrderSummaryResponse.GiftCard();
                model.GiftCards = new List<OrderSummaryResponse.GiftCard>();
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
        //Added By Surakshith to differentiate POS and Online OrderSummary Method 16_08_2020 end

        public static string ParseCustomAddressAttributes(List<string> attributeControlIds,
      IAddressAttributeParser addressAttributeParser,
      IAddressAttributeService addressAttributeService)
        {

            var attributesXml = "";
            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("address_attribute_{0}", attribute.Id);
                foreach (var attributeId in attributeControlIds)
                {
                    var customProductAttributeControlId = attributeId.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    string customControlId = customProductAttributeControlId[0] + "_" + customProductAttributeControlId[1] + "_" + customProductAttributeControlId[2];

                    string multipleAttributeValues = customProductAttributeControlId[3];
                    string[] attributesValues = multipleAttributeValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var attributeValue in attributesValues)
                    {
                        if (controlId == customControlId)
                        {
                            switch (attribute.AttributeControlType)
                            {
                                case AttributeControlType.DropdownList:
                                case AttributeControlType.RadioList:
                                    {
                                        var ctrlAttributes = attributeValue;
                                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                                        {
                                            var selectedAttributeId = int.Parse(ctrlAttributes);
                                            if (selectedAttributeId > 0)
                                                attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                                    attribute, selectedAttributeId.ToString());
                                        }
                                    }
                                    break;
                                case AttributeControlType.Checkboxes:
                                    {
                                        var cblAttributes = attributeValue;
                                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                                        {
                                            foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                            {
                                                var selectedAttributeId = int.Parse(item);
                                                if (selectedAttributeId > 0)
                                                    attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                                        attribute, selectedAttributeId.ToString());
                                            }
                                        }
                                    }
                                    break;
                                case AttributeControlType.ReadonlyCheckboxes:
                                    {
                                        //load read-only (already server-side selected) values
                                        var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                                        foreach (var selectedAttributeId in attributeValues
                                            .Where(v => v.IsPreSelected)
                                            .Select(v => v.Id)
                                            .ToList())
                                        {
                                            attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                                        attribute, selectedAttributeId.ToString());
                                        }
                                    }
                                    break;
                                case AttributeControlType.TextBox:
                                case AttributeControlType.MultilineTextbox:
                                    {
                                        var ctrlAttributes = attributeValue;
                                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                                        {
                                            var enteredText = ctrlAttributes.ToString().Trim();
                                            attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                                attribute, enteredText);
                                        }
                                    }
                                    break;
                                case AttributeControlType.Datepicker:
                                case AttributeControlType.ColorSquares:
                                case AttributeControlType.ImageSquares:
                                case AttributeControlType.FileUpload:
                                //not supported address attributes
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return attributesXml;

        }
        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult GetBillingAddress([FromBody]ShoppingCartRequest shoppingCartRequest)
        {
            if (shoppingCartRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(shoppingCartRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(shoppingCartRequest.StoreId)
                .ToList();

            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //model
            var model = PrepareBillingAddressModel(currentCustomer, prePopulateNewAddressWithCustomerFields: true, languageId: shoppingCartRequest.LanguageId
                , storeId: shoppingCartRequest.StoreId);

            //check whether "billing address" step is enabled
            if (_orderSettings.DisableBillingAddressCheckoutStep)
            {
                if (model.ExistingAddresses.Any())
                {
                    //choose the first one
                    var addressId = model.ExistingAddresses.First().Id;
                    var address = currentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
                    if (address == null)
                        return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressEdit.NotFound")).BadRequest();

                    currentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(currentCustomer);

                }
                else
                {
                    var address = _addressService.FindAddress(currentCustomer.Addresses.ToList(),
                      model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                      model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                      model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                      model.NewAddress.County, model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                      model.NewAddress.CountryId, null);

                    CheckoutBillingAddressResponse billingAdddresResponse = new CheckoutBillingAddressResponse();
                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = billingAdddresResponse.NewAddress.ToEntity();
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;
                        currentCustomer.Addresses.Add(address);
                    }
                    currentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(currentCustomer);
                    model = PrepareBillingAddressModel(currentCustomer, prePopulateNewAddressWithCustomerFields: true, languageId: shoppingCartRequest.LanguageId
                        , storeId: shoppingCartRequest.StoreId);
                }
            }
            model.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult SelectBillingAddress([FromBody]SelectAddressRequest billingAddressRequest)
        {
            if (billingAddressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(billingAddressRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var address = currentCustomer.Addresses.FirstOrDefault(a => a.Id == billingAddressRequest.AddressId);
            if (address == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressNotFound")).BadRequest();

            currentCustomer.BillingAddress = address;
            _customerService.UpdateCustomer(currentCustomer);

            //shipping address
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(billingAddressRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                currentCustomer.ShippingAddress = null;
                _customerService.UpdateCustomer(currentCustomer);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingRequired")).BadRequest();
            }
            //model
            var model = PrepareShippingAddressModel(currentCustomer, billingAddressRequest.CurrencyId, languageId: billingAddressRequest.LanguageId, prePopulateNewAddressWithCustomerFields: true);
            model.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult AddNewBillingAddress([FromBody]AddNewAddressRequest addNewAddressRequest)
        {
            if (addNewAddressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(addNewAddressRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(addNewAddressRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            string customAttributes = string.Empty;
            //custom address attributes
            if (addNewAddressRequest.AttributeControlIds != null)
            {
                customAttributes = ParseCustomAddressAttributes(addNewAddressRequest.AttributeControlIds, _addressAttributeParser, _addressAttributeService);
            }
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //try to find an address with the same values (don't duplicate records)
            var address = _addressService.FindAddress(currentCustomer.Addresses.ToList(),
                      addNewAddressRequest.AddressModel.FirstName, addNewAddressRequest.AddressModel.LastName, addNewAddressRequest.AddressModel.PhoneNumber,
                      addNewAddressRequest.AddressModel.Email, addNewAddressRequest.AddressModel.FaxNumber, addNewAddressRequest.AddressModel.Company,
                      addNewAddressRequest.AddressModel.Address1, addNewAddressRequest.AddressModel.Address2, addNewAddressRequest.AddressModel.City,
                      addNewAddressRequest.AddressModel.County, addNewAddressRequest.AddressModel.StateProvinceId, addNewAddressRequest.AddressModel.ZipPostalCode,
                      addNewAddressRequest.AddressModel.CountryId, customAttributes);

            if (address == null)
            {
                //address is not found. let's create a new one
                address = addNewAddressRequest.AddressModel.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                currentCustomer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = address });
            }
            currentCustomer.BillingAddress = address;
            _customerService.UpdateCustomer(currentCustomer);

            //shipping address
            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                currentCustomer.ShippingAddress = null;
                _customerService.UpdateCustomer(currentCustomer);

                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingRequired")).BadRequest();
            }

            //model
            var model = PrepareShippingAddressModel(currentCustomer, addNewAddressRequest.CurrencyId, languageId: addNewAddressRequest.LanguageId, prePopulateNewAddressWithCustomerFields: true);
            model.CustomerGuid = currentCustomer.CustomerGuid;
            model.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressAdd.Success");
            return Ok(model);

        }

        [HttpPost]
        public virtual IActionResult SelectShippingAddress([FromBody]SelectAddressRequest selectAddressRequest)
        {
            if (selectAddressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(selectAddressRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(selectAddressRequest.CurrencyId);

            //set value indicating that "pick up in store" option has not been chosen                    
            var pickupPoint = _genericAttributeService
                .GetAttribute<PickupPoint>(currentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, selectAddressRequest.StoreId);
            var selectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;

            var address = currentCustomer.Addresses.FirstOrDefault(a => a.Id == selectAddressRequest.AddressId);
            if (address == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressNotFound")).BadRequest();

            currentCustomer.ShippingAddress = address;
            _customerService.UpdateCustomer(currentCustomer);

            //Pick up in store?
            if (_shippingSettings.AllowPickUpInStore)
            {
                _genericAttributeService.SaveAttribute(currentCustomer, selectedPickUpInStore.ToString(), false, selectAddressRequest.StoreId);
            }

            //shipping method
            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(selectAddressRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();


            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(currentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, selectAddressRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingRequired")).BadRequest();
            }

            if (_shippingSettings.AllowPickUpInStore)
            {
                //customer decided to pick up in store?
                var pickUpInStore = _genericAttributeService
                    .GetAttribute<bool>(currentCustomer, selectedPickUpInStore.ToString(), selectAddressRequest.StoreId);

                if (pickUpInStore)
                {
                    return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PickUpInStore")).BadRequest();
                }
            }
            //model
            var model = PrepareShippingMethodModel(currentCustomer, workingCurrency, selectAddressRequest.StoreId, cart);

            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                model.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                _genericAttributeService.SaveAttribute(currentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    model.ShippingMethods.First().ShippingOption,
                    selectAddressRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.OnlyOneShippingMethod")).BadRequest();
            }
            model.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult AddNewShippingAddress([FromBody]AddNewAddressRequest addNewAddressRequest)
        {
            if (addNewAddressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(addNewAddressRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(addNewAddressRequest.CurrencyId);

            //set value indicating that "pick up in store" option has been chosen
            var pickupPoint = _genericAttributeService
                .GetAttribute<PickupPoint>(currentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, addNewAddressRequest.StoreId);
            var SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(addNewAddressRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();


            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                currentCustomer.ShippingAddress = null;
                _customerService.UpdateCustomer(currentCustomer);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingRequired")).BadRequest();
            }
            CheckoutShippingAddressResponse checkoutShippingAddressResponse = new CheckoutShippingAddressResponse
            {
                PickUpInStore = Convert.ToBoolean(addNewAddressRequest.PickUpInStore)
            };
            //Pick up in store?
            if (_shippingSettings.AllowPickUpInStore)
            {
                if (checkoutShippingAddressResponse.PickUpInStore)
                {
                    //customer decided to pick up in store

                    //no shipping address selected
                    currentCustomer.ShippingAddress = null;
                    _customerService.UpdateCustomer(currentCustomer);

                    _genericAttributeService.SaveAttribute(currentCustomer, SelectedPickUpInStore.ToString(), true, addNewAddressRequest.StoreId);

                    //save "pick up in store" shipping method
                    var pickUpInStoreShippingOption = new ShippingOption()
                    {
                        Name = _localizationService.GetResource("Checkout.PickUpInStore.MethodName"),
                        Rate = decimal.Zero,
                        Description = null,
                        ShippingRateComputationMethodSystemName = null
                    };
                    _genericAttributeService.SaveAttribute(currentCustomer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute,
                        pickUpInStoreShippingOption,
                        addNewAddressRequest.StoreId);

                    //load next step
                    return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckShippingMethod")).BadRequest();
                }

                //set value indicating that "pick up in store" option has not been chosen                   
                _genericAttributeService.SaveAttribute(currentCustomer, SelectedPickUpInStore.ToString(), false, addNewAddressRequest.StoreId);
            }
            string customAttributes = string.Empty;
            //custom address attributes
            if (addNewAddressRequest.AttributeControlIds != null)
            {
                customAttributes = ParseCustomAddressAttributes(addNewAddressRequest.AttributeControlIds, _addressAttributeParser, _addressAttributeService);
            }
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //try to find an address with the same values (don't duplicate records)
            var address = _addressService.FindAddress(currentCustomer.Addresses.ToList(),
                     addNewAddressRequest.AddressModel.FirstName, addNewAddressRequest.AddressModel.LastName, addNewAddressRequest.AddressModel.PhoneNumber,
                     addNewAddressRequest.AddressModel.Email, addNewAddressRequest.AddressModel.FaxNumber, addNewAddressRequest.AddressModel.Company,
                     addNewAddressRequest.AddressModel.Address1, addNewAddressRequest.AddressModel.Address2, addNewAddressRequest.AddressModel.City,
                     addNewAddressRequest.AddressModel.County, addNewAddressRequest.AddressModel.StateProvinceId, addNewAddressRequest.AddressModel.ZipPostalCode,
                     addNewAddressRequest.AddressModel.CountryId, customAttributes);

            if (address == null)
            {
                address = addNewAddressRequest.AddressModel.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                currentCustomer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = address });
            }
            currentCustomer.ShippingAddress = address;
            _customerService.UpdateCustomer(currentCustomer);

            var model = PrepareShippingMethodModel(currentCustomer, workingCurrency, addNewAddressRequest.StoreId, cart);
            model.CustomerGuid = currentCustomer.CustomerGuid;
            model.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressAdd.Success");

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult SelectShippingMethod([FromBody]SelectShippingRequest selectShippingRequest)
        {
            if (selectShippingRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(selectShippingRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(selectShippingRequest.CurrencyId);

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(selectShippingRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(currentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, null, selectShippingRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingRequired")).BadRequest();
            }

            //parse selected method 
            if (string.IsNullOrEmpty(selectShippingRequest.ShippingOption))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingOption")).BadRequest();
            var splittedOption = selectShippingRequest.ShippingOption.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingOption")).BadRequest();
            string selectedName = splittedOption[0];
            string shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = _genericAttributeService
                .GetAttribute<List<ShippingOption>>(currentCustomer, NopCustomerDefaults.OfferedShippingOptionsAttribute, selectShippingRequest.StoreId);

            if (shippingOptions == null || shippingOptions.Count == 0)
            {
                //not found? let's load them using shipping service
                shippingOptions = _shippingService
                    .GetShippingOptions(cart, currentCustomer.ShippingAddress, currentCustomer, shippingRateComputationMethodSystemName, selectShippingRequest.StoreId)
                    .ShippingOptions
                    .ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions
                .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingOption")).BadRequest();

            //save
            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, selectShippingRequest.StoreId);

            //payment method

            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart, true);
            if (!isPaymentWorkflowRequired)
            {
                _genericAttributeService.SaveAttribute<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, selectShippingRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest(); ;
            }

            //filter by country
            int filterByCountryId = 0;
            if (_addressSettings.CountryEnabled &&
                currentCustomer.BillingAddress != null &&
                currentCustomer.BillingAddress.Country != null)
            {
                filterByCountryId = currentCustomer.BillingAddress.Country.Id;
            }

            //model
            var paymentMethodModel = PreparePaymentMethodModel(currentCustomer, workingCurrency, selectShippingRequest.StoreId, selectShippingRequest.languageId, cart, filterByCountryId);

            paymentMethodModel.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(paymentMethodModel);
        }

        //Added method for POS Shipping Method Checkout- by Phani on 02_12_2019 START
        [HttpPost]
        public virtual IActionResult SelectPOSShippingMethod([FromBody]SelectShippingRequest selectShippingRequest,string POSOption)
        {
            if (selectShippingRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(selectShippingRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(selectShippingRequest.CurrencyId);

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(selectShippingRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(currentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, null, selectShippingRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingRequired")).BadRequest();
            }

            //parse selected method 
            if (string.IsNullOrEmpty(selectShippingRequest.ShippingOption))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingOption")).BadRequest();
            var splittedOption = selectShippingRequest.ShippingOption.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingOption")).BadRequest();
            string selectedName = splittedOption[0];
            string shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = _genericAttributeService
                .GetAttribute<List<ShippingOption>>(currentCustomer, NopCustomerDefaults.OfferedShippingOptionsAttribute, selectShippingRequest.StoreId);

            if (shippingOptions == null || shippingOptions.Count == 0)
            {
                //not found? let's load them using shipping service
                shippingOptions = _shippingService
                    .GetShippingOptions(cart, currentCustomer.ShippingAddress, currentCustomer, shippingRateComputationMethodSystemName, selectShippingRequest.StoreId)
                    .ShippingOptions
                    .ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions
                .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoShippingOption")).BadRequest();

            //save
            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, selectShippingRequest.StoreId);

            //payment method

            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart, true);

            //Added for No Payment method for POS Checkout -by Phani on 02_12_2019 START
            if (POSOption == "No Payment")
            {
                isPaymentWorkflowRequired = true;
            }
            //Added for No Payment method for POS Checkout -by Phani on 02_12_2019 END

            if (!isPaymentWorkflowRequired)
            {
                _genericAttributeService.SaveAttribute<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, selectShippingRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest(); ;
            }

            //filter by country
            int filterByCountryId = 0;
            if (_addressSettings.CountryEnabled &&
                currentCustomer.BillingAddress != null &&
                currentCustomer.BillingAddress.Country != null)
            {
                filterByCountryId = currentCustomer.BillingAddress.Country.Id;
            }

            //model
            var paymentMethodModel = PreparePaymentMethodModel(currentCustomer, workingCurrency, selectShippingRequest.StoreId, selectShippingRequest.languageId, cart, filterByCountryId);

            paymentMethodModel.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(paymentMethodModel);
        }
        //Added method for POS Shipping Method Checkout- by Phani on 02_12_2019 END

        [HttpPost]
        public virtual IActionResult GetPaymentMethod([FromBody]GetPaymentMethodRequest getPaymentMethodRequest)
        {
            if (getPaymentMethodRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(getPaymentMethodRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(getPaymentMethodRequest.CurrencyId);

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(getPaymentMethodRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //payment method

            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart, true);
            if (!isPaymentWorkflowRequired)
            {
                _genericAttributeService.SaveAttribute<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, getPaymentMethodRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest();
            }

            //filter by country
            int filterByCountryId = 0;
            if (_addressSettings.CountryEnabled &&
                currentCustomer.BillingAddress != null &&
                currentCustomer.BillingAddress.Country != null)
            {
                filterByCountryId = currentCustomer.BillingAddress.Country.Id;
            }

            //model
            var paymentMethodModel = PreparePaymentMethodModel(currentCustomer, workingCurrency, getPaymentMethodRequest.StoreId, getPaymentMethodRequest.LanguageId, cart, filterByCountryId);

            if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
            {
                //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                //so customer doesn't have to choose a payment method

                _genericAttributeService.SaveAttribute<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute,
                    paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
                    getPaymentMethodRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.OnlyOnePaymentMethod")).BadRequest();
            }
            paymentMethodModel.CustomerGuid = currentCustomer.CustomerGuid;
            return Ok(paymentMethodModel);
        }

        [HttpPost]
        public virtual IActionResult SelectPaymentMethod([FromBody]SelectPaymentMethodRequest selectPaymentMethodRequest)
        {

            if (selectPaymentMethodRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var currentCustomer = _customerService.GetCustomerByGuid(selectPaymentMethodRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(selectPaymentMethodRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //reward points
            if (_rewardPointsSettings.Enabled)
            {
                _genericAttributeService.SaveAttribute(currentCustomer,
                    NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, selectPaymentMethodRequest.UseRewardPoints,
                    selectPaymentMethodRequest.StoreId);
            }

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);

            // added by Phani on 15-08-2020 for making zero payable amount start
            if(selectPaymentMethodRequest.PaymentMethod == "Payments.Manual")
            {
                isPaymentWorkflowRequired = true;
            }
            // added by Phani on 15-08-2020 for making zero payable amount end

            if (!isPaymentWorkflowRequired)
            {
                _genericAttributeService.SaveAttribute<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, selectPaymentMethodRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest();
            }
            //payment method 
            if (string.IsNullOrEmpty(selectPaymentMethodRequest.PaymentMethod))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoPaymentMethod")).BadRequest();

            var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(selectPaymentMethodRequest.PaymentMethod);
            if (paymentMethodInst == null ||
               !_paymentService.IsPaymentMethodActive(paymentMethodInst) ||
               !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, selectPaymentMethodRequest.StoreId) ||
               !_pluginFinder.AuthorizedForUser(paymentMethodInst.PluginDescriptor, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentMethodNotActive")).BadRequest();

            //save
            _genericAttributeService.SaveAttribute<string>(currentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, selectPaymentMethodRequest.PaymentMethod, selectPaymentMethodRequest.StoreId);

            //save paymentInfo into cache and use at confirm order
            var paymentInfo = new ProcessPaymentRequest
            {
                PaymentMethodSystemName = selectPaymentMethodRequest.PaymentMethod
            };
            string cacheKey = string.Format("customerPaymentInfo-{0}", selectPaymentMethodRequest.CustomerGUID);

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentMethodSelect")
            };
            return Ok(customerModel);

        }

        //Added method for POS Payment Method Checkout- by Phani on 02_12_2019 START
        [HttpPost]
        public virtual IActionResult SelectPOSPaymentMethod([FromBody]SelectPaymentMethodRequest selectPaymentMethodRequest, string POSOption)
        {

            if (selectPaymentMethodRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var currentCustomer = _customerService.GetCustomerByGuid(selectPaymentMethodRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(selectPaymentMethodRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //reward points
            if (_rewardPointsSettings.Enabled)
            {
                _genericAttributeService.SaveAttribute(currentCustomer,
                    NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, selectPaymentMethodRequest.UseRewardPoints,
                    selectPaymentMethodRequest.StoreId);
            }

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);

            //Added for No Payment method for POS Checkout -by Phani on 02_12_2019 START
            if (POSOption == "No Payment")
            {
                isPaymentWorkflowRequired = true;
            }
            //Added for No Payment method for POS Checkout -by Phani on 02_12_2019 END

            if (!isPaymentWorkflowRequired)
            {
                _genericAttributeService.SaveAttribute<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, selectPaymentMethodRequest.StoreId);
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest();
            }
            //payment method 
            if (string.IsNullOrEmpty(selectPaymentMethodRequest.PaymentMethod))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoPaymentMethod")).BadRequest();

            var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(selectPaymentMethodRequest.PaymentMethod);
            if (paymentMethodInst == null ||
               !_paymentService.IsPaymentMethodActive(paymentMethodInst) ||
               !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, selectPaymentMethodRequest.StoreId) ||
               !_pluginFinder.AuthorizedForUser(paymentMethodInst.PluginDescriptor, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentMethodNotActive")).BadRequest();

            //save
            _genericAttributeService.SaveAttribute<string>(currentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, selectPaymentMethodRequest.PaymentMethod, selectPaymentMethodRequest.StoreId);

            //save paymentInfo into cache and use at confirm order
            var paymentInfo = new ProcessPaymentRequest
            {
                PaymentMethodSystemName = selectPaymentMethodRequest.PaymentMethod
            };
            string cacheKey = string.Format("customerPaymentInfo-{0}", selectPaymentMethodRequest.CustomerGUID);

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentMethodSelect")
            };
            return Ok(customerModel);

        }
        //Added method for POS Payment Method Checkout- by Phani on 02_12_2019 END

        [HttpPost]
        public virtual IActionResult EnterCreditCardInfo([FromBody]EnterCreditCardInfoRequest enterCreditCardInfoRequest)
        {
            if (enterCreditCardInfoRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(enterCreditCardInfoRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(enterCreditCardInfoRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest();
            }

            //load payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(enterCreditCardInfoRequest.PaymentMethodSystemName);
            if (paymentMethod == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoPaymentMethod")).BadRequest();

            var workingCurrency = _currencyService.GetCurrencyById(enterCreditCardInfoRequest.CurrencyId);
            var paymentInfo = new ProcessPaymentRequest();
            if (string.IsNullOrEmpty(enterCreditCardInfoRequest.PaymentToken))
            {
                paymentInfo.PaymentMethodSystemName = enterCreditCardInfoRequest.PaymentMethodSystemName;
                paymentInfo.CreditCardType = enterCreditCardInfoRequest.CreditCardType;
                paymentInfo.CreditCardName = enterCreditCardInfoRequest.CardholderName;
                paymentInfo.CreditCardNumber = enterCreditCardInfoRequest.CardNumber;
                paymentInfo.CreditCardExpireMonth = int.Parse(enterCreditCardInfoRequest.ExpireMonth);
                paymentInfo.CreditCardExpireYear = int.Parse(enterCreditCardInfoRequest.ExpireYear);
                paymentInfo.CreditCardCvv2 = enterCreditCardInfoRequest.CardCode;
            }
            else
            {
                paymentInfo.PaymentMethodSystemName = enterCreditCardInfoRequest.PaymentMethodSystemName;
                paymentInfo.CreditCardName = enterCreditCardInfoRequest.CardholderName;
                var customValues = new Dictionary<string, object>();
                customValues.Add("PaymentToken", enterCreditCardInfoRequest.PaymentToken);
                paymentInfo.CustomValues = customValues;
            }

            //session save
            HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

            //If we got this far, something failed, redisplay form
            //model
            ShoppingCartModelResponse model = new ShoppingCartModelResponse();
            var orderSummary = PrepareShoppingCartModel(model, cart, currentCustomer, enterCreditCardInfoRequest.CurrencyId, enterCreditCardInfoRequest.StoreId, enterCreditCardInfoRequest.LanguageId, isEditable: false, prepareAndDisplayOrderReviewData: true);
            model.CustomerGuid = currentCustomer.CustomerGuid;
            model.TaxRates = new List<ShoppingCartModelResponse.TaxRate>();
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
                    model.TaxRates = new List<ShoppingCartModelResponse.TaxRate>();
                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new ShoppingCartModelResponse.TaxRate()
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, workingCurrency), true, false),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult EnterPurchaseOrderInfo([FromBody]EnterPurchaseOrderInfoRequest enterPurchaseOrderInfoRequest)
        {
            if (enterPurchaseOrderInfoRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(enterPurchaseOrderInfoRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validation
            var cart = currentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(enterPurchaseOrderInfoRequest.StoreId)
                .ToList();
            if (cart.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.EmptyCart")).BadRequest();

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.PaymentFlowNotrequired")).BadRequest();

            }

            //load payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(enterPurchaseOrderInfoRequest.PaymentMethodSystemName);
            if (paymentMethod == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoPaymentMethod")).BadRequest();

            var paymentInfo = new ProcessPaymentRequest
            {
                PaymentMethodSystemName = enterPurchaseOrderInfoRequest.PaymentMethodSystemName
            };
            paymentInfo.CustomValues.Add(_localizationService.GetResource("Plugins.Payment.PurchaseOrder.PurchaseOrderNumber"), enterPurchaseOrderInfoRequest.PurchaseOrderNumber);

            //If we got this far, something failed, redisplay form
            //model
            ShoppingCartModelResponse model = new ShoppingCartModelResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            var orderSummary = PrepareShoppingCartModel(model, cart, currentCustomer, enterPurchaseOrderInfoRequest.CurrencyId, enterPurchaseOrderInfoRequest.StoreId, enterPurchaseOrderInfoRequest.LanguageId, isEditable: false, prepareAndDisplayOrderReviewData: true);

            return Ok(model);
        }

     
        [HttpPost]
        public virtual IActionResult ConfirmOrder([FromBody]ConfirmOrderRequest confirmOrderRequest)
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


            var parentCategoryIds = cart.GroupBy(x => x.ParentCategoryId).ToArray();
            var numberOfCategories = 0;
            decimal currentOrderTotal = 0.0M;

            if (parentCategoryIds.Length == 1)
            {
                numberOfCategories = 1;
            }else if(parentCategoryIds.Length == 2)
            {
                numberOfCategories = 2;
            }
            var isPickUpFalseCount = 0;
            foreach(var checkoutaddress in confirmOrderRequest.CheckoutAddressDetails)
            {
                if (checkoutaddress.PickUpInStore == false && checkoutaddress.ParentCategoryId!=0)
                {
                    isPickUpFalseCount++;
                }
            }
            var discounts = _customerService.ParseAppliedDiscountCouponCodes(currentCustomer);

            var cartitemprice = 0.0M;

            var discountamount = 0.0M;
            var grocerTotal = 0.0M;
            var gourmetTotal = 0.0M;

            foreach (var cartitem in cart)
            {
                var cartitemUnitPrice = _priceCalculationService.GetUnitPrice(cartitem, true);
                cartitemprice = cartitemUnitPrice * cartitem.Quantity;
                if (cartitem.ParentCategoryId == 49)
                {
                    grocerTotal = grocerTotal+cartitemUnitPrice * cartitem.Quantity;
                }else if(cartitem.ParentCategoryId == 50)
                {
                    gourmetTotal= gourmetTotal+ cartitemUnitPrice * cartitem.Quantity;
                }
                currentOrderTotal = currentOrderTotal + cartitemprice;
            }

            if (discounts.Length > 0)
            {
                foreach (var discount in discounts)
                {
                    if (discount.Contains("discount_"))
                    {

                    }
                    else
                    {
                        var a= CalculateDiscountAmountByCouponCode(discount.ToLower(), currentOrderTotal);
                        currentOrderTotal = a.Where(x=>x.Key=="amount").FirstOrDefault().Value;
                        discountamount= discountamount+ a.Where(x => x.Key == "result").FirstOrDefault().Value;
                    }
                }
            }
            if (numberOfCategories == 2)
            {
                if (!!confirmOrderRequest.IsMember)
                {
                    if (currentOrderTotal < 60)
                    {
                        currentOrderTotal = currentOrderTotal + 10;
                    }
                }
                else
                {
                    currentOrderTotal = currentOrderTotal + 10;
                }
            }
            else
            {
                if (isPickUpFalseCount == 1)
                {
                    if (!!confirmOrderRequest.IsMember)
                    {
                        if (currentOrderTotal < 60)
                        {
                            currentOrderTotal = currentOrderTotal + 10;
                        }
                    }
                    else
                    {
                        currentOrderTotal = currentOrderTotal + 10;
                    }
                }
            }


            List<IActionResult> confirmResponses = new List<IActionResult>();
            int checkoutOrderCount = 1;
            var captureTransaction = confirmResponses.FirstOrDefault();
            var captureTransactionId = "";
            confirmOrderRequest.OrderTotal = currentOrderTotal;
            foreach (var parentCategoryId in parentCategoryIds)
            {
                List<ShoppingCartItem> shoppingCartItems = cart.Where(x => x.ParentCategoryId == parentCategoryId.Key).ToList();

                CheckoutParameters checkoutParametersObj = new CheckoutParameters
                {
                    CheckOutOrderCount = checkoutOrderCount,
                    IsPickUpInStoreFalseCount=isPickUpFalseCount,
                    NumberOfCheckoutCategories= numberOfCategories,
                    CaptureTransactionId= captureTransactionId,
                    GourmetOrderTotal=gourmetTotal,
                    GrocerOrderTotal=grocerTotal,
                    TotalDiscountAmount=discountamount,
                    OrderTotal= currentOrderTotal
                };

                confirmResponses.Add(ConfirmOrderFinal(cart: shoppingCartItems, workingCurrency: workingCurrency, confirmOrderRequest:confirmOrderRequest, currentCustomer:currentCustomer,checkoutParameters: checkoutParametersObj));
                checkoutOrderCount++;

                if (checkoutOrderCount == 2 && checkoutParametersObj.NumberOfCheckoutCategories == 2)
                {
                    captureTransaction = confirmResponses.FirstOrDefault();
                    checkoutParametersObj.CaptureTransactionId = ((XcellenceIt.Plugin.Misc.WebApiClient.DataClass.CheckoutConfirmResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)captureTransaction).Value).CheckoutCompleteResponses.CaptureTransactionId;
                    //captureTransactionId.GetValue
                }
            }
            //reset checkout data
            _customerService.ResetCheckoutData(currentCustomer, confirmOrderRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);

            return Ok(confirmResponses);
        }

        [NonAction]
        public IActionResult ConfirmOrderFinal(List<ShoppingCartItem> cart, Currency workingCurrency, ConfirmOrderRequest confirmOrderRequest, Customer currentCustomer, CheckoutParameters checkoutParameters)
        {
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
            customValues.Add("OrderTotal", checkoutParameters.OrderTotal);
            customValues.Add("DiscountAmount", checkoutParameters.TotalDiscountAmount);
            customValues.Add("CheckoutOrderCount", checkoutParameters.CheckOutOrderCount);
            customValues.Add("NumberOfCheckoutCategories", checkoutParameters.NumberOfCheckoutCategories);
            customValues.Add("IsPickUpInStoreFalseCount", checkoutParameters.IsPickUpInStoreFalseCount);
            customValues.Add("CaptureTransactionId", checkoutParameters.CaptureTransactionId);
            customValues.Add("GourmetOrderTotal", checkoutParameters.GourmetOrderTotal);
            customValues.Add("GrocerOrderTotal", checkoutParameters.GrocerOrderTotal);
            //Added By Sree for ZeroPayable Amount by KPOINTS 04-06-2020 start
            customValues.Add("PaymentMethodType", confirmOrderRequest.PaymentMethodType);
            //Added By Sree for ZeroPayable Amount by KPOINTS 04-06-2020 end
            // customValues.Add("DeliveryDate", confirmOrderRequest.DeliveryDate);

            processPaymentRequest.CustomValues = customValues;

            //Added By Surakshith to display memberId in mail for register user start on 13-06-2020
            string memberId = confirmOrderRequest.MemberId;
            bool isMember = confirmOrderRequest.IsMember;
            //Added By Surakshith to display memberId in mail for register user end on 13-06-2020

            #region Added by Praveen for Common Checkout
            processPaymentRequest.CheckoutAddressDetails = confirmOrderRequest.CheckoutAddressDetails.Where(x => x.ParentCategoryId == cart[0].ParentCategoryId).SingleOrDefault();
            #endregion

            //Added By Surakshith to display memberId in mail for register user start on 13-06-2020
            var placeOrderResult = _orderProcessingService.PlaceOrderForOnline(processPaymentRequest, cart,memberId,isMember,confirmOrderRequest.OrderTax);
            //Added By Surakshith to display memberId in mail for register user end on 13-06-2020

            if (placeOrderResult.Success)
            {
                model.CheckoutCompleteResponses.OrderId = placeOrderResult.PlacedOrder.Id;
                if(checkoutParameters.CheckOutOrderCount == 2 && checkoutParameters.NumberOfCheckoutCategories == 2)
                {
                    model.CheckoutCompleteResponses.CaptureTransactionId = checkoutParameters.CaptureTransactionId;
                }
                else
                {
                    model.CheckoutCompleteResponses.CaptureTransactionId = placeOrderResult.PlacedOrder.CaptureTransactionId;
                }
                
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


        /*Written by Saisree for calulating applied discount coupon codes*/
        [NonAction]
        public Dictionary<string, decimal> CalculateDiscountAmountByCouponCode(string discountCouponCode,decimal amount )
        {
            var discount = _discountService.GetAllDiscounts(null, discountCouponCode).FirstOrDefault();
            Dictionary<string, decimal> returnValues = new Dictionary<string, decimal>();
            decimal result;
            if (discount.UsePercentage)
            {
                result = (decimal)((float)amount * (float)discount.DiscountPercentage / 100f);
                amount = amount - result;
            }
            else
            {
                result= discount.DiscountAmount;
                amount = amount - discount.DiscountAmount;
            }
            returnValues.Add("amount",amount);
            returnValues.Add("result", result);
            return returnValues;
        }

        [NonAction]
        public string GetCategoryNameByCategoryId(int categoryId)
        {
            Category category = _categoryService.GetCategoryById(categoryId);
            return category.Name;
        }

        [HttpPost]
        public virtual IActionResult OrderSummary([FromBody]OrderSummaryRequest orderSummaryRequest)
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
            var model = new OrderSummaryResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            var shoppingCartModel = PrepareOrderSummaryModel(model, cart, currentCustomer, orderSummaryRequest.CurrencyId, orderSummaryRequest.StoreId, orderSummaryRequest.LanguageId, prepareAndDisplayOrderReviewData: true);

            //If we got this far, something failed, redisplay form
            return Ok(model);
        }

        //Added By Surakshith to differentiate POS and Online OrderSummary Method 16_08_2020 start
        [HttpPost]
        public virtual IActionResult POSOrderSummary([FromBody]OrderSummaryRequest orderSummaryRequest)
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
            var model = new OrderSummaryResponse
            {
                CustomerGuid = currentCustomer.CustomerGuid
            };

            var shoppingCartModel = POSPrepareOrderSummaryModel(model, cart, currentCustomer, orderSummaryRequest.CurrencyId, orderSummaryRequest.StoreId, orderSummaryRequest.LanguageId, prepareAndDisplayOrderReviewData: true);

            //If we got this far, something failed, redisplay form
            return Ok(model);
        }
        //Added By Surakshith to differentiate POS and Online OrderSummary Method 16_08_2020 end


        [HttpPost]
        public virtual IActionResult GetAllCountries([FromBody]CountryRequest countryRequest)
        {
            var query = _countryService.GetAllCountries(countryRequest.LanguageId).AsQueryable();
            var countries = query.ToList()
                .Select(x =>
                {
                    var countryModel = new CountryListResponse
                    {
                        CountryId = x.Id,
                        CountryName = _localizationService.GetLocalized(x, y => y.Name, languageId: countryRequest.LanguageId),
                        LimitedToStore = _localizationService.GetLocalized(x, y => y.LimitedToStores, languageId: countryRequest.LanguageId),
                        TwoLetterIsoCode = _localizationService.GetLocalized(x, y => y.TwoLetterIsoCode, languageId: countryRequest.LanguageId),
                        NumericIsoCode = _localizationService.GetLocalized(x, y => y.NumericIsoCode, languageId: countryRequest.LanguageId),
                    };
                    return countryModel;
                }).AsQueryable();

            if (countries == null)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.RestApi.Message.NoCountryFound")).BadRequest();
            }

            return Ok(countries.ToList());
        }

        public virtual IActionResult GetAllStateByCountryId([FromBody]CountryStateRequest countryStateRequest)
        {
            var query = _stateProvinceService.GetStateProvincesByCountryId(countryStateRequest.CountryId, countryStateRequest.LanguageId).AsQueryable();

            var Availablestates = query.ToList()
                .Select(x =>
                {
                    var stateListResponse = new CountryStateResponse
                    {
                        CountryName = _localizationService.GetLocalized(x, y => y.Country.Name, languageId: countryStateRequest.LanguageId),
                        StateId = x.Id,
                        CountryId = x.CountryId,
                        StateName = _localizationService.GetLocalized(x, y => y.Name, languageId: countryStateRequest.LanguageId),
                        Abbreviation = _localizationService.GetLocalized(x, y => y.Abbreviation, languageId: countryStateRequest.LanguageId),
                    };
                    return stateListResponse;
                }).AsQueryable();

            if (Availablestates == null)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.RestApi.Message.NoStateFound")).BadRequest();
            }

            return Ok(Availablestates.ToList());
        }

        public virtual IActionResult GetCountryByStateId([FromBody]CountryStateRequest countryStateRequest)
        {
            var state = _stateProvinceService.GetStateProvinceById(countryStateRequest.StateId);
            if (state != null)
            {
                var country = state.Country;
                var countryListResponse = new CountryListResponse
                {
                    CountryName = _localizationService.GetLocalized(country, y => y.Name, languageId: countryStateRequest.LanguageId),
                    CountryId = country.Id,
                    TwoLetterIsoCode = _localizationService.GetLocalized(country, y => y.TwoLetterIsoCode, languageId: countryStateRequest.LanguageId),
                    NumericIsoCode = _localizationService.GetLocalized(country, y => y.NumericIsoCode, languageId: countryStateRequest.LanguageId),
                };
                return Ok(countryListResponse);
            }
            else
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.RestApi.Message.NoCountryFound")).BadRequest();
            }
        }
        #endregion
    }
}
