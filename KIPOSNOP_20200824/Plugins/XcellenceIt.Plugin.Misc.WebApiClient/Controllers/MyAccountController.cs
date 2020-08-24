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
using Nop.Services.Authentication;
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
using Nop.Web.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using System.Reflection;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom;
using Nop.Core.Infrastructure;
using Nop.Web.Models.Order;
using Nop.Core.Domain.Shipping;
using Nop.Web.Models.Common;
using Nop.Core.Domain.Vendors;
using Nop.Services.Vendors;
using Nop.Services.Discounts;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class MyAccountController : Controller
    {
        #region Fields
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly TaxSettings _taxSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAddressService _addressService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPaymentService _paymentService;
        private readonly ILanguageService _languageService;
        private readonly OrderSettings _orderSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ITaxService _taxService;
        private readonly IGdprService _gdprService;
        private readonly GdprSettings _gdprSettings;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrder_Pickup_CustDetailsService _order_Pickup_CustDetailsService = EngineContext.Current.Resolve<IOrder_Pickup_CustDetailsService>();
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly AddressSettings _addressSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IDiscountService _discountService;

        #endregion

        #region Ctor

        public MyAccountController(
        CustomerSettings customerSettings,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IShoppingCartService shoppingCartService,
        IAuthenticationService authenticationService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IGenericAttributeService genericAttributeService,
        IDateTimeHelper dateTimeHelper,
        TaxSettings taxSettings,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IStoreMappingService storeMappingService,
        IAddressService addressService,
        ICurrencyService currencyService,
        IPriceFormatter priceFormatter,
        IPaymentService paymentService,
        ILanguageService languageService,
        OrderSettings orderSettings,
        CatalogSettings catalogSettings,
        IProductAttributeParser productAttributeParser,
        IDownloadService downloadService,
        IPictureService pictureService,
        IEventPublisher eventPublisher,
        ILogger logger,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        MediaSettings mediaSettings,
        IWorkContext workContext,
        IStoreContext storeContext,
        DateTimeSettings dateTimeSettings,
        ITaxService taxService,
        IGdprService gdprService,
        GdprSettings gdprSettings,
        ICustomerModelFactory customerModelFactory,
        IProductService productService,
        IUrlRecordService urlRecordService,
        IOrderModelFactory orderModelFactory,
        IAddressModelFactory addressModelFactory,
        AddressSettings addressSettings,
        VendorSettings vendorSettings,
        IVendorService vendorService,
        IDiscountService discountService)
        {
            _customerSettings = customerSettings;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _authenticationService = authenticationService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _dateTimeHelper = dateTimeHelper;
            _taxSettings = taxSettings;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _storeMappingService = storeMappingService;
            _addressService = addressService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _paymentService = paymentService;
            _languageService = languageService;
            _orderSettings = orderSettings;
            _catalogSettings = catalogSettings;
            _productAttributeParser = productAttributeParser;
            _downloadService = downloadService;
            _pictureService = pictureService;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _mediaSettings = mediaSettings;
            _workContext = workContext;
            _storeContext = storeContext;
            _dateTimeSettings = dateTimeSettings;
            _taxService = taxService;
            _gdprService = gdprService;
            _gdprSettings = gdprSettings;
            _customerModelFactory = customerModelFactory;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _orderModelFactory = orderModelFactory;
            _addressModelFactory = addressModelFactory;
            _addressSettings = addressSettings;
            _vendorSettings = vendorSettings;
            _vendorService = vendorService;
            _discountService = discountService;
        }

        #endregion

        #region Utilities
        #endregion

        #region Method

        #region CustomerController

        [HttpPost]
        public virtual IActionResult CheckUserAvailability([FromBody]UserAvailabilityRequest userAvailabilityRequest)
        {
            if (userAvailabilityRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var customer = _customerService.GetCustomerByUsername(userAvailabilityRequest.UserName);

            if (customer?.Id > 0)
            {
                return Ok(new ResponseSuccess
                {
                    Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerExist")
                });
            }
            return Ok(new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerNotExist")
            });
        }

        [HttpPost]
        public virtual IActionResult CheckEmailIdAvailability([FromBody]EmailAvailabilityRequest emailAvailabilityRequest)
        {
            if (emailAvailabilityRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var customer = _customerService.GetCustomerByEmail(emailAvailabilityRequest.EmailId);

            if (customer?.Id > 0)
            {
                return Ok(new ResponseSuccess
                {
                    Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerExist")
                });
            }
            return Ok(new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerNotExist")
            });
        }

        [HttpPost]
        public virtual IActionResult Login([FromBody]LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            if (string.IsNullOrEmpty(loginRequest.Password) || string.IsNullOrEmpty(loginRequest.UserName))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.Login.Invalid")).BadRequest();

            if (_customerSettings.UsernamesEnabled)
            {
                loginRequest.UserName = loginRequest.UserName.Trim();
            }
            LoginResponse loginResponse = new LoginResponse();
            var loginResult = _customerRegistrationService.ValidateCustomer(loginRequest.UserName, loginRequest.Password);

            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(loginRequest.UserName) :
                            _customerService.GetCustomerByEmail(loginRequest.UserName);

                        if (customer == null)
                            return new ResponseObject(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist")).BadRequest();

                        if (loginRequest.GuestCustomerId.HasValue)
                        {
                            //migrate shopping cart
                            var currentCustomer = _customerService.GetCustomerById(loginRequest.GuestCustomerId.Value);
                            _shoppingCartService.MigrateShoppingCart(currentCustomer, customer, true);
                        }

                        //sign in new customer
                        _authenticationService.SignIn(customer, loginResponse.RememberMe);

                        //raise event       
                        _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                        loginResponse.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.Login.Success");
                        loginResponse.CustomerGuid = customer.CustomerGuid;
                        loginResponse.EmailId = customer.Email;
                        loginResponse.UserName = customer.Username;
                        loginResponse.IsValid = true;
                        break;
                    }
                case CustomerLoginResults.CustomerNotExist:
                    return new ResponseObject(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist")).BadRequest();
                case CustomerLoginResults.Deleted:
                    return new ResponseObject(_localizationService.GetResource("Account.Login.WrongCredentials.Deleted")).BadRequest();
                case CustomerLoginResults.NotActive:
                    return new ResponseObject(_localizationService.GetResource("Account.Login.WrongCredentials.NotActive")).BadRequest();
                case CustomerLoginResults.NotRegistered:
                    return new ResponseObject(_localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered")).BadRequest();
                case CustomerLoginResults.WrongPassword:
                default:
                    return new ResponseObject(_localizationService.GetResource("Account.Login.WrongCredentials")).BadRequest();
            }
            return Ok(loginResponse);
        }

        [HttpPost]
        public IActionResult Register([FromBody]RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
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
                    //_customerSettings.UsernamesEnabled ? registerRequest.UserName : registerRequest.EmailId,
                    registerRequest.UserName,
                    /*username*/
                    registerRequest.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    /*mobilenumber*/
                    registerRequest.MobileNumber,
                    /*mobilenumber*/
                    /*Added by surakshith for IsGuestUser Start*/
                    /*IsGuestUser*/
                    registerRequest.IsGuestUser,
                    /*IsGuestUser*/
                    /*Added by surakshith for IsGuestUser End*/
                    isApproved);
            var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
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
                MobileNumber = customer.MobileNumber,
                /*For MobileNumber*/
                /*Added by surakshith for IsGuestUser Start*/
                IsGuestUser = customer.IsGuestUser
                /*Added by surakshith for IsGuestUser End*/
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

                //login customer now
                if (isApproved)
                    _authenticationService.SignIn(customer, true);

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
                if (registerRequest.IsGuestUser == false)
                {
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                            _localizationSettings.DefaultAdminLanguageId);
                }
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
                            if (registerRequest.IsGuestUser == false)
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                            }
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
                    return new ResponseObject(registrationResult.Errors.FirstOrDefault()).BadRequest();
                }
            }
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult ChangePassword([FromBody]ChangePwdRequest changePwdRequest)
        {
            if (changePwdRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var changePasswordRequest = new ChangePasswordRequest(changePwdRequest.EmailId,
                true, _customerSettings.DefaultPasswordFormat, changePwdRequest.NewPassword, changePwdRequest.OldPassword);
            var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);

            string changePwdResponse = "";
            if (changePasswordResult.Success)
            {
                changePwdResponse = _localizationService.GetResource("Account.ChangePassword.Success");
            }

            //errors
            if (changePasswordResult != null && changePasswordResult.Errors.Count > 0)
            {
                return new ResponseObject(changePasswordResult.Errors.FirstOrDefault()).BadRequest();
            }

            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = changePwdResponse
            };
            return Ok(responseSuccess);
        }

        [HttpPost]
        public virtual IActionResult Info([FromBody]InfoRequest infoRequest)
        {
            if (infoRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var personalInfoResponse = new PersonalInfoResponse();
            var currentCustomer = _customerService.GetCustomerByGuid(infoRequest.CustomerGUID);

            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), infoRequest.CustomerGUID)).BadRequest();

            if (!currentCustomer.IsRegistered())
                return Ok(personalInfoResponse);

            personalInfoResponse.Gender = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.GenderAttribute);
            personalInfoResponse.FirstName = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.FirstNameAttribute);
            personalInfoResponse.LastName = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.LastNameAttribute);
            personalInfoResponse.DateOfBirth = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.DateOfBirthAttribute);
            personalInfoResponse.Email = currentCustomer.Email;
            personalInfoResponse.CompanyName = _genericAttributeService.GetAttribute<string>(currentCustomer, NopCustomerDefaults.CompanyAttribute);
            personalInfoResponse.Phone = currentCustomer.MobileNumber;
            personalInfoResponse.AvatarPictureUrl = _pictureService
                                            .GetPictureUrl(_genericAttributeService
                                            .GetAttribute<int>(currentCustomer, NopCustomerDefaults.AvatarPictureIdAttribute));

            return Ok(personalInfoResponse);
        }

        #endregion

        #region OrderController

        [HttpPost]
        public virtual IActionResult ReOrder([FromBody]ReorderRequest reorderRequest)
        {
            if (reorderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            string reOrderResponse = "";
            var order = _orderService.GetOrderById(reorderRequest.OrderId);
            var currentCustomer = _customerService.GetCustomerByGuid(reorderRequest.CustomerGUID);

            if (order == null || order.Deleted || currentCustomer.Id != order.CustomerId)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ReOrder.NullOrDeleted")).BadRequest();
            }

            _orderProcessingService.ReOrder(order);

            reOrderResponse = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ReOrder.Success");
            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = reOrderResponse
            };
            return Ok(responseSuccess);
        }
        #endregion

        //Added By Surakshith for POS ReOrder Functioanlity to achieve amended order functioanlity start on 06-07-2020
        [HttpPost]
        public virtual IActionResult PosReOrder([FromBody]ReorderRequest reorderRequest)
        {
            if (reorderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            string reOrderResponse = "";
            var order = _orderService.GetOrderById(reorderRequest.OrderId);
            var currentCustomer = _customerService.GetCustomerByGuid(reorderRequest.CustomerGUID);

            if (order == null || order.Deleted)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ReOrder.NullOrDeleted")).BadRequest();
            }

            _orderProcessingService.ReOrderForPOS(order, currentCustomer);

            reOrderResponse = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ReOrder.Success");
            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = reOrderResponse
            };
            return Ok(responseSuccess);
        }

        //Added By Surakshith for POS ReOrder Functioanlity to achieve amended order functioanlity end on 06-07-2020

        #region OrderModelFactory

        /// <summary>
        /// MethodName:PrepareOrderDetailsModel
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult GetOrderDetail([FromBody]OrderRequest orderRequest)
        {
            if (orderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();
            //Set working language
            _workContext.WorkingLanguage = _languageService.GetLanguageById(orderRequest.LanguageId);

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(orderRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), orderRequest.CustomerGUID)).BadRequest();

            var order = _orderService.GetOrderById(orderRequest.OrderId);
            if (order == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.OrderNotFound")).NotFound();
            Order_Pickup_CustDetails _Pickup_CustDetails = new Order_Pickup_CustDetails();

            //Added for category discount by sree 16_06_2020 start
            var orderDetail = PrepareOrderDetailsModel(order);
            //Added for category discount by sree 16_06_2020 end

            if (!!order.PickUpInStore)
            {
                _Pickup_CustDetails = _order_Pickup_CustDetailsService.GetCustomerDetails(order.Id);
                if (_Pickup_CustDetails != null)
                {
                    orderDetail.CustomerEmail = _Pickup_CustDetails.EmailId;
                    orderDetail.CustomerMobileNumber = _Pickup_CustDetails.MobileNumber;
                }
                else
                {
                    orderDetail.CustomerEmail = currentCustomer.Email;
                    orderDetail.CustomerMobileNumber = currentCustomer.MobileNumber;
                }
            }
            else
            {
                orderDetail.CustomerEmail = order.BillingAddress.Email;
                orderDetail.CustomerMobileNumber = order.BillingAddress.PhoneNumber;
            }


            return Ok(orderDetail);
        }
        #endregion

        //Added for category discount by sree 16_06_2020 start
        [NonAction]
        public virtual OrderDetailsModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            var model = new OrderDetailsModel
            {
                Id = order.Id,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
                IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                CustomOrderNumber = order.CustomOrderNumber,
                PaymentMethodType = order.PaymentMethodType,
                //shipping info
                ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                DeliveryDate = order.DeliveryDate,
                PickUpInStore = order.PickUpInStore
            };
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickUpInStore = order.PickUpInStore;
                if (!order.PickUpInStore)
                {
                    _addressModelFactory.PrepareAddressModel(model.ShippingAddress,
                        address: order.ShippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
                }
                else
                    if (order.PickupAddress != null)
                    model.PickupAddress = new AddressModel
                    {
                        Address1 = order.PickupAddress.Address1,
                        City = order.PickupAddress.City,
                        County = order.PickupAddress.County,
                        CountryName = order.PickupAddress.Country != null ? order.PickupAddress.Country.Name : string.Empty,
                        ZipPostalCode = order.PickupAddress.ZipPostalCode
                    };
                model.ShippingMethod = order.ShippingMethod;

                //shipments (only already shipped)
                var shipments = order.Shipments.Where(x => x.ShippedDateUtc.HasValue).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new OrderDetailsModel.ShipmentBriefModel
                    {
                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };
                    if (shipment.ShippedDateUtc.HasValue)
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                    if (shipment.DeliveryDateUtc.HasValue)
                        shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                    model.Shipments.Add(shipmentModel);
                }
            }

            //billing info
            _addressModelFactory.PrepareAddressModel(model.BillingAddress,
                address: order.BillingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = order.PaymentMethodType; 
            model.PaymentMethod = order.PaymentMethodType;
            model.PaymentMethodStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //Added by surakshith to get priceinclusion of tax start on 16-07-2020
            model.OrderSubTotalExclTax = order.OrderSubtotalExclTax;
            model.OrderSubTotalInclTax = order.OrderSubtotalInclTax;
            model.OrderSubTotalDiscountExclTax = order.OrderSubTotalDiscountExclTax;
            model.OrderSubTotalDiscountInclTax = order.OrderSubTotalDiscountInclTax;
            //Added by surakshith to get priceinclusion of tax end on 16-07-2020

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //Added by surakshith to get priceinclusion of tax start on 16-07-2020
            model.OrderShippingExclTax = order.OrderShippingExclTax;
            model.OrderShippingInclTax = order.OrderShippingInclTax;
            //Added by surakshith to get priceinclusion of tax end on 16-07-2020

            //tax
            var displayTax = true;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new OrderDetailsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new OrderDetailsModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in order.OrderNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsModel.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = _orderService.FormatOrderNoteText(orderNote),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            //purchased products
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;

            var orderItems = order.OrderItems;

            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? _vendorService.GetVendorsByIds(orderItems.Select(item => item.Product.VendorId).ToArray()) : new List<Vendor>();

            foreach (var orderItem in orderItems)
            {
                var orderItemModel = new OrderDetailsModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    OrderItemGuid = orderItem.OrderItemGuid,
                    Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml),
                    VendorName = vendors.FirstOrDefault(v => v.Id == orderItem.Product.VendorId)?.Name ?? string.Empty,
                    ProductId = orderItem.Product.Id,
                    ProductName = _localizationService.GetLocalized(orderItem.Product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(orderItem.Product),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                    ParentCategoryId = orderItem.ParentCategoryId,
                };
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                var basePrice = _currencyService.ConvertCurrency(orderItem.Product.Price, order.CurrencyRate);
                orderItemModel.BasePrice = _priceFormatter.FormatPrice(basePrice, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                model.Items.Add(orderItemModel);

                //unit price, subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    //Added By sree for category discount to be excluded in the subtotal 18-07-2020 start
                    if (orderItem.DiscountAmountInclTax > 0)
                    {
                        priceInclTaxInCustomerCurrency = priceInclTaxInCustomerCurrency + orderItem.DiscountAmountInclTax;
                    }
                    //Added By sree for category discount to be excluded in the subtotal 18-07-2020 end

                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    //Added By sree for category discount to be excluded in the subtotal 18-07-2020 start
                    if (orderItem.DiscountAmountExclTax > 0)
                    {
                        priceExclTaxInCustomerCurrency = priceExclTaxInCustomerCurrency + orderItem.DiscountAmountExclTax;
                    }
                    //Added By sree for category discount to be excluded in the subtotal 18-07-2020 end
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }

                //downloadable products
                if (_downloadService.IsDownloadAllowed(orderItem))
                    orderItemModel.DownloadId = orderItem.Product.DownloadId;
                if (_downloadService.IsLicenseDownloadAllowed(orderItem))
                    orderItemModel.LicenseId = orderItem.LicenseDownloadId.HasValue ? orderItem.LicenseDownloadId.Value : 0;
            }

            return model;
        }
        //Added for category discount by sree 16_06_2020 end

        /// <summary>
        /// Update customer information. it is not for update all details of customer.
        /// </summary>
        /// <param name="personalInfoRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult InfoEdit([FromBody] PersonalInfoRequest personalInfoRequest)
        {
            if (personalInfoRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(personalInfoRequest.CustomerGUID);

            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), personalInfoRequest.CustomerGUID)).BadRequest();

            if (!currentCustomer.IsRegistered())
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotRegister")).BadRequest();

            //Email
            if (!currentCustomer.Email.Equals(personalInfoRequest.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                //Change Email
                _customerRegistrationService.SetUsername(currentCustomer, personalInfoRequest.Email.Trim());

                //Re-Authenticate (if usernames are disabled)
                if (!_customerSettings.UsernamesEnabled)
                {
                    _authenticationService.SignIn(currentCustomer, true);
                }
            }

            if (_customerSettings.GenderEnabled)
                _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.GenderAttribute, personalInfoRequest.Gender);
            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.FirstNameAttribute, personalInfoRequest.FirstName);
            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.LastNameAttribute, personalInfoRequest.LastName);
            if (_customerSettings.DateOfBirthEnabled)
            {
                DateTime? dateOfBirth = null;
                try
                {
                    dateOfBirth = Convert.ToDateTime(personalInfoRequest.DateOfBirth);
                }
                catch { }
                _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
            }
            if (_customerSettings.CompanyEnabled)
                _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.CompanyAttribute, personalInfoRequest.CompanyName);

            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.InfoEdit.Success")
            };
            return Ok(responseSuccess);
        }

        [HttpPost]
        public virtual IActionResult GetAddress([FromBody]GetAddressRequest getAddressRequest)
        {
            if (getAddressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var model = new AddressListRespone();

            var currentCustomer = _customerService.GetCustomerByGuid(getAddressRequest.CustomerGUID);

            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), getAddressRequest.CustomerGUID)).BadRequest();

            if (!currentCustomer.IsRegistered())
            {
                model.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotRegister");
                return Ok(model);
            }

            var addresses = currentCustomer.Addresses
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country, getAddressRequest.StoreId))
                .ToList();
            foreach (var address in addresses)
            {
                var addressResponse = new AddressResponse
                {
                    AddressId = address.Id,
                    FirstName = address.FirstName,
                    Email = address.Email,
                    LastName = address.LastName,
                    StateProvinceId = address.StateProvinceId,
                    Company = address.Company,
                    CountryId = address.CountryId,
                    PhoneNumber = address.PhoneNumber,
                    FaxNumber = address.FaxNumber,
                    Address1 = address.Address1,
                    Address2 = address.Address2,
                    City = address.City,
                    ZipPostalCode = address.ZipPostalCode
                };
                model.Addresses.Add(addressResponse);
            }
            if (model.Addresses.Count == 0)
            {
                model.Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressNotFound");
            }
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult AddAddress([FromBody]AddressRequest addressRequest)
        {
            if (addressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(addressRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), addressRequest.CustomerGUID)).BadRequest();

            if (!currentCustomer.IsRegistered())
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotRegister")).BadRequest();

            var address = new Address
            {
                FirstName = addressRequest.FirstName,
                LastName = addressRequest.LastName,
                Email = addressRequest.Email,
                PhoneNumber = addressRequest.PhoneNumber,
                FaxNumber = addressRequest.FaxNumber,
                Address1 = addressRequest.Address1,
                CountryId = addressRequest.CountryId,
                StateProvinceId = addressRequest.StateProvinceId,
                City = addressRequest.City,
                Company = addressRequest.Company,
                Address2 = addressRequest.Address2,
                ZipPostalCode = addressRequest.ZipPostalCode,
                CreatedOnUtc = DateTime.UtcNow
            };

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            currentCustomer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = address });
            _customerService.UpdateCustomer(currentCustomer);

            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressAdd.Success")
            };
            return Ok(responseSuccess);
        }

        [HttpPost]
        public virtual IActionResult UpdateAddress([FromBody]AddressRequest addressRequest)
        {
            if (addressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(addressRequest.CustomerGUID);

            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), addressRequest.CustomerGUID)).BadRequest();

            if (!currentCustomer.IsRegistered())
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotRegister")).BadRequest();

            //find address (ensure that it belongs to the current customer)
            var address = currentCustomer.Addresses.FirstOrDefault(a => a.Id == addressRequest.AddressId);
            if (address == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressEdit.NotFound")).BadRequest();

            address.FirstName = addressRequest.FirstName;
            address.LastName = addressRequest.LastName;
            address.Email = addressRequest.Email;
            address.PhoneNumber = addressRequest.PhoneNumber;
            address.FaxNumber = addressRequest.FaxNumber;
            address.Address1 = addressRequest.Address1;
            address.CountryId = addressRequest.CountryId;
            address.StateProvinceId = addressRequest.StateProvinceId;
            address.City = addressRequest.City;
            address.Company = addressRequest.Company;
            address.Address2 = addressRequest.Address2;
            address.ZipPostalCode = addressRequest.ZipPostalCode;
            address.CreatedOnUtc = DateTime.UtcNow;

            _addressService.UpdateAddress(address);

            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressEdit.Update")
            };
            return Ok(responseSuccess);
        }

        [HttpPost]
        public virtual IActionResult DeleteAddress([FromBody]DeleteAddressRequest deleteAddressRequest)
        {
            if (deleteAddressRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(deleteAddressRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), deleteAddressRequest.CustomerGUID)).BadRequest();

            if (!currentCustomer.IsRegistered())
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotRegister")).BadRequest();

            //find address (ensure that it belongs to the current customer)
            var address = currentCustomer.Addresses.FirstOrDefault(a => a.Id == deleteAddressRequest.AddressId);
            if (address != null)
            {
                _customerService.RemoveCustomerAddress(currentCustomer, address);
                _customerService.UpdateCustomer(currentCustomer);

                //Delete address
                _addressService.DeleteAddress(address);

                ResponseSuccess responseSuccess = new ResponseSuccess
                {
                    Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressDelete.Delete")
                };
                return Ok(responseSuccess);
            }
            return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.AddressDelete.NotDeleted")).BadRequest();
        }

        [HttpPost]
        public virtual IActionResult PasswordRecovery([FromBody]PasswordRecoveryRequest passwordRecoveryRequest)
        {
            if (passwordRecoveryRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var customer = _customerService.GetCustomerByEmail(passwordRecoveryRequest.EmailId);

            if (customer != null && customer.Active && !customer.Deleted)
            {
                var passwordRecoveryToken = Guid.NewGuid();
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);
                _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer, passwordRecoveryRequest.LanguageId);

                ResponseSuccess responseSuccess = new ResponseSuccess
                {
                    Message = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent")
                };
                return Ok(responseSuccess);
            }
            else
                return new ResponseObject(_localizationService.GetResource("Account.PasswordRecovery.EmailNotFound")).NotFound();
        }

        [HttpPost]
        public virtual IActionResult PasswordRecoveryConfirm([FromBody]PasswordRecoveryRequest passwordRecoveryRequest)
        {
            if (passwordRecoveryRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var customer = _customerService.GetCustomerByEmail(passwordRecoveryRequest.EmailId);
            if (customer == null)
                return new ResponseObject("User Doesn't Exists").NotFound();

            //validate token
            if (!_customerService.IsPasswordRecoveryTokenValid(customer, passwordRecoveryRequest.Token))
            {
                return new ResponseObject(_localizationService.GetResource("Account.PasswordRecovery.WrongToken")).BadRequest();
            }

            //validate token expiration date
            if (_customerService.IsPasswordRecoveryLinkExpired(customer))
            {
                return new ResponseObject(_localizationService.GetResource("Account.PasswordRecovery.LinkExpired")).BadRequest();
            }

            if (ModelState.IsValid)
            {
                var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(passwordRecoveryRequest.EmailId,
                    false, _customerSettings.DefaultPasswordFormat, passwordRecoveryRequest.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, "");


                    ResponseSuccess responseSuccess = new ResponseSuccess
                    {
                        Message = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged")
                    };
                    return Ok(responseSuccess);
                }
                else
                {
                    return new ResponseObject(response.Errors.FirstOrDefault()).BadRequest();
                }

            }

            //If we got this far, something failed, redisplay form
            return new ResponseObject("Something went wrong. Please try again Later.").BadRequest();
        }

        [HttpPost]
        public virtual IActionResult GetOrder([FromBody]GetOrderRequest getOrderRequest)
        {
            if (getOrderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(getOrderRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), getOrderRequest.CustomerGUID)).BadRequest();

            var orderResponse = new List<OrderReponse>();

            if (!currentCustomer.IsRegistered())
                return Ok(orderResponse);

            var orders = _orderService.SearchOrders(storeId: getOrderRequest.StoreId,
                customerId: currentCustomer.Id);

            if (orders == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.OrderNotFound")).NotFound();

            var language = _languageService.GetLanguageById(getOrderRequest.LanguageId);

            orderResponse = orders.Select(s => new OrderReponse
            {
                OrderId = s.Id,
                OrderDate = s.CreatedOnUtc.ToShortDateString(),
                OrderStatus = _localizationService.GetLocalizedEnum(s.OrderStatus, language.Id),
                OrderTotal = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(s.OrderTotal, s.CurrencyRate), true, s.CustomerCurrencyCode, false, language),
                DeliveryDate = (s.DeliveryDate == null) ? (DateTime?)null : Convert.ToDateTime(s.DeliveryDate),
                PickUpInStore = s.PickUpInStore,
                ParentCategoryId = s.OrderItems.ToArray()[0].ParentCategoryId
            }).ToList();

            if (orderResponse.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoOrderFound")).NotFound();

            return Ok(orderResponse);
        }

        [HttpPost]
        public virtual IActionResult GetGuestCustomerGuid()
        {
            var customer = _customerService.InsertGuestCustomer();

            return Json(new { customer.CustomerGuid });
        }


        /*Added By sree for memberunsubscription start 18_01_2019*/
        [HttpPost]
        public virtual IActionResult UnSubscribeMember([FromBody]MemberUnsubscribeRequest memberUnsubscribeRequest)
        {
            var currentCustomer = GetCustomerIdByGUID(memberUnsubscribeRequest.CustomerGUID);

            var allGenericeAttributes = _genericAttributeService.GetAttributesForEntity(currentCustomer, "customer");

            foreach (var attribute in allGenericeAttributes)
            {
                if (attribute.Key == "DiscountCouponCode")
                {
                    _genericAttributeService.DeleteAttribute(attribute);
                }
            }

            return Ok();
        }

        [NonAction]
        public int GetCustomerIdByGUID(Guid customerGuid)
        {
            var customerInfo = _customerService.GetCustomerByGuid(customerGuid);
            //Modified by surakshith with exception case start on 30-06-2020
            if (customerInfo != null)
                return customerInfo.Id;
            else
                return 0;
            //Modified by surakshith with exception case end on 30-06-2020
        }

        /*Added By sree for memberunsubscription End 18_01_2019*/

        //Added By sree for validating discount coupon code before placing order 18_06_2020 start
        [HttpPost]
        public virtual IActionResult ValidateCouponCode([FromBody]DiscountRequest discountRequest)
        {
            if (discountRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

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
                    if (!anyValidDiscount)
                    {
                        if (userErrors.Any())
                        {
                            return new ResponseObject(userErrors[0].ToString()).BadRequest();
                        }
                    }
                    CustomerModel customerModel = new CustomerModel
                    {
                        CustomerGuid = currentCustomer.CustomerGuid,
                        Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied")
                    };

                    return Ok(customerModel);
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
        //Added By sree for validating discount coupon code before placing order 18_06_2020 end
        #endregion
    }
}
