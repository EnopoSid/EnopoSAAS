using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Text;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using Nop.Web.Factories;
using System.Linq;
using Nop.Services.POS;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class POSUserAccountController: Controller
    {
        #region fields
        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        private readonly ICurrencyService _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
        private readonly CustomerSettings _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
        private readonly ICustomerRegistrationService _customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
        private readonly IShoppingCartService _shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
        private readonly IAuthenticationService _authenticationService = EngineContext.Current.Resolve<IAuthenticationService>();
        private readonly ICustomerActivityService _customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
        private readonly ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        private readonly IGenericAttributeService _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        private readonly IDateTimeHelper _dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
        private readonly TaxSettings _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
        private readonly IWorkflowMessageService _workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
        private readonly LocalizationSettings _localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
        private readonly IOrderService _orderService = EngineContext.Current.Resolve<IOrderService>();
        private readonly IOrderProcessingService _orderProcessingService = EngineContext.Current.Resolve<IOrderProcessingService>();
        private readonly IStoreMappingService _storeMappingService = EngineContext.Current.Resolve<IStoreMappingService>();
        private readonly IAddressService _addressService = EngineContext.Current.Resolve<IAddressService>();
        private readonly IPriceFormatter _priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
        private readonly IPaymentService _paymentService = EngineContext.Current.Resolve<IPaymentService>();
        private readonly ILanguageService _languageService = EngineContext.Current.Resolve<ILanguageService>();
        private readonly OrderSettings _orderSettings = EngineContext.Current.Resolve<OrderSettings>();
        private readonly CatalogSettings _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
        private readonly IProductAttributeParser _productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
        private readonly IDownloadService _downloadService = EngineContext.Current.Resolve<IDownloadService>();
        private readonly MediaSettings _mediaSettings = EngineContext.Current.Resolve<MediaSettings>();
        private readonly IPictureService _pictureService = EngineContext.Current.Resolve<IPictureService>();
        private readonly IEventPublisher _eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService = EngineContext.Current.Resolve<INewsLetterSubscriptionService>();
        private readonly IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
        private readonly IStoreContext _storeContext = EngineContext.Current.Resolve<IStoreContext>();
        private readonly DateTimeSettings _dateTimeSettings = EngineContext.Current.Resolve<DateTimeSettings>();
        private readonly ITaxService _taxService = EngineContext.Current.Resolve<ITaxService>();
        private readonly IGdprService _gdprService = EngineContext.Current.Resolve<IGdprService>();
        private readonly GdprSettings _gdprSettings = EngineContext.Current.Resolve<GdprSettings>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        private readonly IUrlRecordService _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
        private readonly IOrderModelFactory _orderModelFactory = EngineContext.Current.Resolve<IOrderModelFactory>();
        private readonly ICustomerModelFactory _customerModelFactory = EngineContext.Current.Resolve<ICustomerModelFactory>();
        private readonly IPOSUserRegistrationService _POSUserRegistrationService = EngineContext.Current.Resolve<IPOSUserRegistrationService>();
        #endregion


        #region cTor

        public POSUserAccountController()
        {

        }

        #endregion

        #region methods
        [HttpPost]
        public IActionResult POSRegister([FromBody]RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
               // _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            //check whether registration is allowed 
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return new ResponseObject(_localizationService.GetResource("Account.Register.Result.Disabled")).BadRequest();

            var customer = _customerService.InsertGuestCustomer();
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;
            if (_customerSettings.UsernamesEnabled && registerRequest.UserName != null)
            {
                registerRequest.UserName = registerRequest.UserName.Trim();
            }

            var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                    registerRequest.EmailId,
                    /*username*/
                    registerRequest.UserName,
                    /*username*/
                    registerRequest.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    /*mobilenumber*/
                    registerRequest.MobileNumber,
                    /*mobilenumber*/
                    isApproved);
            var registrationResult = _POSUserRegistrationService.RegisterPOSCustomer(registrationRequest);
            //Set model to return response
            var model = new RegisterResponse()
            {
                Active = customer.Active,
                AdminComment = customer.AdminComment,
                AffiliateId = customer.AffiliateId,
                BillingAddress = customer.BillingAddress,
                CannotLoginUntilDateUtc = customer.CannotLoginUntilDateUtc,
                CreatedOnUtc = customer.CreatedOnUtc,
                CustomerGuid = customer.CustomerGuid,
                Deleted = customer.Deleted,
                Email = customer.Email,
                EmailToRevalidate = customer.EmailToRevalidate,
                FailedLoginAttempts = customer.FailedLoginAttempts,
                HasShoppingCartItems = customer.HasShoppingCartItems,
                IsSystemAccount = customer.IsSystemAccount,
                IsTaxExempt = customer.IsTaxExempt,
                LastActivityDateUtc = customer.LastActivityDateUtc,
                LastIpAddress = customer.LastIpAddress,
                LastLoginDateUtc = customer.LastLoginDateUtc,
                RegisteredInStoreId = customer.RegisteredInStoreId,
                RequireReLogin = customer.RequireReLogin,
                ShippingAddress = customer.ShippingAddress,
                SystemName = customer.SystemName,
                Username = customer.Username,
                VendorId = customer.VendorId,
                CustomerId = customer.Id,
                /*For MobileNumber*/
                MobileNumber = customer.MobileNumber
                /*For MobileNumber*/
            };

            if (registrationResult.Success)
            {
                if (model.CustomerId > 0)
                {
                    model.Message = _localizationService.GetResource("account.register.result.standard");
                }

                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, registerRequest.TimeZoneId);
                }

                //VAT number
                if (_taxSettings.EuVatEnabled)
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, registerRequest.VatNumber);

                    var vatNumberStatus = _taxService.GetVatNumberStatus(registerRequest.VatNumber, out string _, out string vatAddress);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                    //send VAT number admin notification
                    if (!string.IsNullOrEmpty(registerRequest.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                        _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, registerRequest.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                }

                //form fields
                if (_customerSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, registerRequest.Gender);
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, registerRequest.FirstName);
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, registerRequest.LastName);
                if (_customerSettings.DateOfBirthEnabled)
                {
                    var dateOfBirth = registerRequest.ParseDateOfBirth();
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                }
                if (_customerSettings.CompanyEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, registerRequest.CompanyName);
                if (_customerSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, registerRequest.PhoneNumber);

                //newsletter
                if (_customerSettings.NewsletterEnabled)
                {
                    //save newsletter value
                    var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(registerRequest.EmailId, _storeContext.CurrentStore.Id);
                    if (newsletter != null)
                    {
                        if (registerRequest.NewsLetter)
                        {
                            newsletter.Active = true;
                            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);

                            //GDPR
                            if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                            {
                                _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                            }
                        }
                    }
                    else
                    {
                        if (registerRequest.NewsLetter)
                        {
                            _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                            {
                                NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                Email = registrationRequest.Email,
                                Active = true,
                                StoreId = _storeContext.CurrentStore.Id,
                                CreatedOnUtc = DateTime.UtcNow
                            });

                            //GDPR
                            if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                            {
                                _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                            }
                        }
                    }
                }

                if (_customerSettings.AcceptPrivacyPolicyEnabled)
                {
                    //privacy policy is required
                    //GDPR
                    if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                    {
                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                    }
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                    LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                    Email = customer.Email,
                    Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                    CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0
                        ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute)
                        : null,
                    StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0
                        ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)
                        : null,
                    County = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute),
                    City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute),
                    Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                    Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                    ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                    PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                    FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute),
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (_addressService.IsAddressValid(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);
                    customer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = defaultAddress });
                    customer.BillingAddress = defaultAddress;
                    customer.ShippingAddress = defaultAddress;
                    _customerService.UpdateCustomer(customer);
                }

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                _eventPublisher.Publish(new CustomerRegisteredEvent(customer));
                switch (_customerSettings.UserRegistrationType)
                {
                    case UserRegistrationType.EmailValidation:
                        {
                            //email validation message
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                            _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                            var modelErr = _customerModelFactory.PrepareRegisterResultModel((int)UserRegistrationType.EmailValidation);
                            //result
                            return new ResponseObject(modelErr?.Result).BadRequest();
                        }
                    case UserRegistrationType.AdminApproval:
                        {
                            var modelErr = _customerModelFactory.PrepareRegisterResultModel((int)UserRegistrationType.AdminApproval);
                            //result
                            return new ResponseObject(modelErr?.Result).BadRequest();
                        }
                    case UserRegistrationType.Standard:
                        {
                            //send customer welcome message
                            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                            return Ok(model);
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                if (registrationResult != null && registrationResult.Errors.Count > 0)
                {
                    return new ResponseObject(registrationResult.Errors.FirstOrDefault() + " IsValidRegistration = false ").BadRequest();
                }
            }
            return Ok(model);
        }

        [HttpPost]
        public IActionResult DeletePosUser([FromBody]CartRequest cartRequest)
        {
            Customer customer = _customerService.GetCustomerByGuid(cartRequest.CustomerGUID);
            if (customer == null)
            {
                return BadRequest();
            }
                _customerService.DeleteCustomer(customer);
                return Ok(customer.Id);
        }

        #endregion

    }
}
