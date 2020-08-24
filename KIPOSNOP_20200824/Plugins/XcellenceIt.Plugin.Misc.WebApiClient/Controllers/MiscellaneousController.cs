using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Widgets.NivoSlider;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using System.Reflection;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class MiscellaneousController : Controller
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
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ICacheManager _cacheManager;
        private readonly IAclService _aclService;
        private readonly IPictureService _pictureService;
        private readonly ITopicService _topicService;
        private readonly ILogger _logger;
        private readonly IPollService _pollService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStoreService _storeService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IWorkContext _workContext;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public MiscellaneousController(
         CustomerSettings customerSettings,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IShoppingCartService shoppingCartService,
        IAuthenticationService authenticationService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IGenericAttributeService genericAttributeService,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        IPermissionService permissionService,
        IStoreMappingService storeMappingService,
        ICurrencyService currencyService,
        ILanguageService languageService,
        ISettingService settingService,
        IPluginFinder pluginFinder,
        ICacheManager cacheManager,
        IAclService aclService,
        IPictureService pictureService,
        ITopicService topicService,
        ILogger logger,
        IPollService pollService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IStoreService storeService,
        IEmailAccountService emailAccountService,
        EmailAccountSettings emailAccountSettings,
        CommonSettings commonSettings,
        IQueuedEmailService queuedEmailService,
        IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
        ExternalAuthenticationSettings externalAuthenticationSettings,
        IExternalAuthenticationService externalAuthenticationService,
        IWorkContext workContext,
        IUrlRecordService urlRecordService
       )
        {
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._shoppingCartService = shoppingCartService;
            this._authenticationService = authenticationService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._permissionService = permissionService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._languageService = languageService;
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._cacheManager = cacheManager;
            this._aclService = aclService;
            this._pictureService = pictureService;
            this._logger = logger;
            this._pollService = pollService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._storeService = storeService;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._queuedEmailService = queuedEmailService;
            this._externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._externalAuthenticationService = externalAuthenticationService;
            this._topicService = topicService;
            this._workContext = workContext;
            this._urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual TopicModelResponse PrepareTopicModel(Topic topic, int languageId)
        {
            if (topic == null)
                return new TopicModelResponse { Message = _localizationService.GetResource("Topic not found") };

            var model = new TopicModelResponse()
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : _localizationService.GetLocalized(topic, x => x.Title, languageId: languageId),
                Body = topic.IsPasswordProtected ? "" : _localizationService.GetLocalized(topic, x => x.Body, languageId: languageId),
                MetaKeywords = _localizationService.GetLocalized(topic, x => x.MetaKeywords, languageId: languageId),
                MetaDescription = _localizationService.GetLocalized(topic, x => x.MetaDescription, languageId: languageId),
                MetaTitle = _localizationService.GetLocalized(topic, x => x.MetaTitle, languageId: languageId),
                SeName = _urlRecordService.GetSeName(topic, languageId: languageId),
            };
            return model;
        }

        [NonAction]
        protected virtual PollModelResponse PreparePollModel(int cusotmerId, Poll poll, bool setAlreadyVotedProperty)
        {
            var model = new PollModelResponse
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && _pollService.AlreadyVoted(poll.Id, cusotmerId),
                Name = poll.Name,
                Answers = new List<PollAnswerModel>()
            };
            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder);
            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;
            foreach (var pa in answers)
            {
                model.Answers.Add(new PollAnswerModel()
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0,
                });
            }
            return model;
        }

        [NonAction]
        public bool CheckExternalPluginInstall(string providerSystemName)
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(providerSystemName);
            if (pluginDescriptor == null)
                return false;
            if (pluginDescriptor.SystemName == "ExternalAuth.Facebook" || pluginDescriptor.SystemName == "ExternalAuth.Google")
                return true;
            else
                return false;
        }

        [NonAction]
        private bool AccountAlreadyExists(Customer userFound, Customer userLoggedIn)
        {
            return userFound != null && userLoggedIn != null;
        }

        [NonAction]
        private bool AccountIsAssignedToLoggedOnAccount(Customer userFound, Customer userLoggedIn)
        {
            return userFound.Id.Equals(userLoggedIn.Id);
        }

        [NonAction]
        private bool AccountDoesNotExistAndUserIsNotLoggedOn(Customer userFound, Customer userLoggedIn)
        {
            return userFound == null && userLoggedIn == null;
        }

        [NonAction]
        private bool AutoRegistrationIsEnabled()
        {
            return _customerSettings.UserRegistrationType != UserRegistrationType.Disabled;
        }

        [NonAction]
        private bool RegistrationIsEnabled()
        {
            return _customerSettings.UserRegistrationType != UserRegistrationType.Disabled;
        }

        [NonAction]
        protected string GetPictureUrl(int pictureId)
        {
            string cacheKey = string.Format("Nop.plugins.widgets.nivosrlider.pictureurl-{0}", pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
                //little hack here. nulls aren't cache able so set it to ""
                if (url == null)
                    url = "";

                return url;
            });
        }
        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult TopicDetails([FromBody]TopicDetailsRequest topicDetailsRequest)
        {
            if (topicDetailsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var currentCustomer = _customerService.GetCustomerByGuid(topicDetailsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_ID_KEY, topicDetailsRequest.TopicId, topicDetailsRequest.LanguageId,
                                topicDetailsRequest.StoreId, string.Join(",", currentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var topic = _topicService.GetTopicById(topicDetailsRequest.TopicId);
                if (topic == null)
                    return new TopicModelResponse { Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.TopicDetails.TopicNotFound") };
                //Store mapping
                if (!_storeMappingService.Authorize(topic, topicDetailsRequest.StoreId))
                    return new TopicModelResponse { Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.TopicDetails.NotAuthorized") };
                //ACL (access control list)
                if (!_aclService.Authorize(topic,currentCustomer))
                    return new TopicModelResponse { Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.TopicDetails.NotACL") };
                return PrepareTopicModel(topic, topicDetailsRequest.LanguageId);
            }
            );

            cacheModel.CustomerGuid = currentCustomer.CustomerGuid;

            if (cacheModel == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.TopicDetails.ModelNotFound")).NotFound();

            return Ok(cacheModel);
        }

        [HttpPost]
        public virtual IActionResult SubscribeNewsletter([FromBody]SubscribeNewsletterRequest subscribeNewsletterRequest)
        {
            if (subscribeNewsletterRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            string result;

            if (!CommonHelper.IsValidEmail(subscribeNewsletterRequest.Email))
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            else
            {
                //subscribe/unsubscripted
                subscribeNewsletterRequest.Email = subscribeNewsletterRequest.Email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(subscribeNewsletterRequest.Email, subscribeNewsletterRequest.StoreId);
                if (subscription != null)
                {
                    if (subscribeNewsletterRequest.IsSubscribed)
                    {
                        _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
                        _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessage(subscription, subscribeNewsletterRequest.LanguageId);
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        return new ResponseObject("You have already subscribed to newsletter").BadRequest();
                    }
                    
                }
                else if (!subscribeNewsletterRequest.IsSubscribed)
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = subscribeNewsletterRequest.Email,
                        Active = true,
                        StoreId = subscribeNewsletterRequest.StoreId,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, subscribeNewsletterRequest.LanguageId);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                }
            }
            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = result
            };
            return Ok(result);

        }

        [HttpPost]
        public virtual IActionResult Vote([FromBody]VoteRequest voteRequest)
        {
            if (voteRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(voteRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var pollAnswer = _pollService.GetPollAnswerById(voteRequest.PollAnswerId);
            if (pollAnswer == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.Vote.NotFound ")).BadRequest();

            var poll = pollAnswer.Poll;
            if (!poll.Published)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.Vote.NotAvailable")).BadRequest();

            if (currentCustomer.IsGuest() && !poll.AllowGuestsToVote)
                return new ResponseObject(_localizationService.GetResource("Polls.OnlyRegisteredUsersVote")).BadRequest();

            bool alreadyVoted = _pollService.AlreadyVoted(poll.Id, currentCustomer.Id);
            if (!alreadyVoted)
            {
                //vote
                pollAnswer.PollVotingRecords.Add(new PollVotingRecord()
                {
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = currentCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
                //update totals
                pollAnswer.NumberOfVotes = pollAnswer.PollVotingRecords.Count;
                _pollService.UpdatePoll(poll);
            }
            var model = PreparePollModel(currentCustomer.Id, poll, true);
            model.CustomerGuid = currentCustomer.CustomerGuid;
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult GetLanguage([FromBody]LanguageRequest languageRequest)
        {
            if (languageRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, languageRequest.StoreId), () =>
            {
                var result = _languageService
                    .GetAllLanguages(storeId: languageRequest.StoreId)
                    .Select(x => new LanguageModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorResponse
            {
                CurrentLanguageId = languageRequest.LanguageId,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            if (model.AvailableLanguages.Count == 1)
            {
                return Ok(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.GetLanguage"));
            }
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult SetLanguage([FromBody]LanguageRequest languageRequest)
        {
            if (languageRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            if (languageRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var language = _languageService.GetLanguageById(languageRequest.LanguageId);
            if (language == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.LanguageNotAvaialable")).BadRequest();

            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.LanguageSet")
            };
            return Ok(responseSuccess);

        }

        [HttpPost]
        public virtual IActionResult NivoSlider([FromBody]NivoSliderRequest nivoSliderRequest)
        {
            if (nivoSliderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            if (nivoSliderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(nivoSliderRequest.StoreId);
            var model = new NivoSliderResponse
            {
                Picture1Url = GetPictureUrl(nivoSliderSettings.Picture1Id),
                Text1 = nivoSliderSettings.Text1,
                Link1 = nivoSliderSettings.Link1,

                Picture2Url = GetPictureUrl(nivoSliderSettings.Picture2Id),
                Text2 = nivoSliderSettings.Text2,
                Link2 = nivoSliderSettings.Link2,

                Picture3Url = GetPictureUrl(nivoSliderSettings.Picture3Id),
                Text3 = nivoSliderSettings.Text3,
                Link3 = nivoSliderSettings.Link3,

                Picture4Url = GetPictureUrl(nivoSliderSettings.Picture4Id),
                Text4 = nivoSliderSettings.Text4,
                Link4 = nivoSliderSettings.Link4,

                Picture5Url = GetPictureUrl(nivoSliderSettings.Picture5Id),
                Text5 = nivoSliderSettings.Text5,
                Link5 = nivoSliderSettings.Link5
            };

            if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
            string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
            string.IsNullOrEmpty(model.Picture5Url))
                //no pictures uploaded
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.UrlNotFound")).NotFound();

            return Ok(model);

        }

        [HttpPost]
        public virtual IActionResult GetCurrency([FromBody]CurrencyRequest currencyRequest)
        {
            if (currencyRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            if (currencyRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, currencyRequest.LanguageId, currencyRequest.StoreId), () =>
            {
                var result = _currencyService
                    .GetAllCurrencies(storeId: currencyRequest.StoreId)
                    .Select(x =>
                    {
                        //currency char
                        var currencySymbol = "";
                        if (!string.IsNullOrEmpty(x.DisplayLocale))
                            currencySymbol = new RegionInfo(x.DisplayLocale).CurrencySymbol;
                        else
                            currencySymbol = x.CurrencyCode;
                        //model
                        var currencyModel = new CurrencyModel()
                        {
                            Id = x.Id,
                            Name = _localizationService.GetLocalized(x, y => y.Name, languageId: currencyRequest.LanguageId),
                            CurrencySymbol = currencySymbol
                        };
                        return currencyModel;
                    })
                    .ToList();
                return result;
            });

            var model = new CurrencySelectorResponse()
            {
                CurrentCurrencyId = currencyRequest.CurrencyId,
                AvailableCurrencies = availableCurrencies
            };

            if (model.AvailableCurrencies.Count == 1)
            {
                return new ResponseObject("Plugins.XcellenceIT.WebApiClient.Message.GetCurrency").BadRequest();
            }

            return Ok(model);

        }

        [HttpPost]
        public virtual IActionResult SetCurrency([FromBody]CurrencyRequest currencyRequest)
        {
            if (currencyRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            if (currencyRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currency = _currencyService.GetCurrencyById(currencyRequest.CurrencyId);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            if (currency == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CurrencyNotAvaialable")).BadRequest();

            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CurrencySet")
            };
            return Ok(responseSuccess);
        }

        [HttpPost]
        public virtual IActionResult ContactUsSend([FromBody]ContactUsSendRequest contactUsSend)
        {
            if (contactUsSend == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            if (!CommonHelper.IsValidEmail(contactUsSend.ContactUsRequest.Email))
                return new ResponseObject(_localizationService.GetResource("Newsletter.Email.Wrong")).BadRequest();

            var currentStore = _storeService.GetStoreById(contactUsSend.StoreId);
            string email = contactUsSend.ContactUsRequest.Email.Trim();
            string fullName = contactUsSend.ContactUsRequest.FullName;
            string subject = string.Format(_localizationService.GetResource("ContactUs.EmailSubject"), _localizationService.GetLocalized(currentStore, x => x.Name, languageId: contactUsSend.LanguageId));

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount != null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            if (emailAccount == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ContactUs.Email")).BadRequest();

            string from = null;
            string fromName = null;
            string body = Nop.Core.Html.HtmlHelper.FormatText(contactUsSend.ContactUsRequest.Enquiry, false, true, false, false, false, false);
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                from = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                    fullName,
                    contactUsSend.ContactUsRequest.Email, contactUsSend.ContactUsRequest.Enquiry);
            }
            else
            {
                from = email;
                fromName = fullName;
            }
            _queuedEmailService.InsertQueuedEmail(new QueuedEmail()
            {
                From = from,
                FromName = fromName,
                To = emailAccount.Email,
                ToName = emailAccount.DisplayName,
                ReplyTo = email,
                ReplyToName = fullName,
                Priority = QueuedEmailPriority.High,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            });

            contactUsSend.ContactUsRequest.SuccessfullySent = true;
            contactUsSend.ContactUsRequest.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ContactUs", _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));
            return Ok(contactUsSend.ContactUsRequest);
        }

        [HttpPost]
        public virtual IActionResult GetAllSetting([FromBody]CommonDataRequest commonDataRequest)
        {
            if (commonDataRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var query = _settingService.GetAllSettings().AsQueryable();

            // Filter by store wise if storeId 0 than return all setting                
            if (commonDataRequest.StoreId > 0)
            {
                query = query.Where(x => x.StoreId == commonDataRequest.StoreId);
            }
            if (query.Count() == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoStoreFound")).NotFound();

            }

            var settings = query.ToList()
                .Select(x =>
                {
                    string storeName;
                    if (x.StoreId == 0)
                    {
                        storeName = _localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
                    }
                    else
                    {
                        var store = _storeService.GetStoreById(x.StoreId);
                        storeName = store != null ? store.Name : "Unknown";
                    }

                    var settingModel = new SettingModelResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.Value,
                        Store = storeName,
                        StoreId = x.StoreId
                    };
                    return settingModel;
                })
                .AsQueryable();

            if (settings == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NoName")).BadRequest();

            return Ok(settings.ToList());
        }

        [HttpPost]
        public virtual IActionResult GetSetting([FromBody]GetSettingBySettingKeyRequest getSettingBySettingKeyRequest)
        {
            if (getSettingBySettingKeyRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            if (getSettingBySettingKeyRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var value = _settingService.GetSettingByKey<string>(getSettingBySettingKeyRequest.SettingKey);

            if (value == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NoName")).BadRequest();

            return Ok(value);

        }

        [HttpPost]
        public virtual IActionResult SettingUpdate([FromBody]SettingUpdateRequest settingUpdateRequest)
        {
            if (settingUpdateRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var customer = _customerService.GetCustomerByGuid(settingUpdateRequest.CustomerGUID);
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings.SystemName, customer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NotAuthorize")).BadRequest();

            if (settingUpdateRequest.SettingModelRequest.Name != null)
                settingUpdateRequest.SettingModelRequest.Name = settingUpdateRequest.SettingModelRequest.Name.Trim();
            if (settingUpdateRequest.SettingModelRequest.Value != null)
                settingUpdateRequest.SettingModelRequest.Value = settingUpdateRequest.SettingModelRequest.Value.Trim();

            var setting = _settingService.GetSettingById(settingUpdateRequest.SettingModelRequest.Id);
            if (setting == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NoSetting")).BadRequest();

            var storeIdModel = settingUpdateRequest.SettingModelRequest.StoreId;

            if (!setting.Name.Equals(settingUpdateRequest.SettingModelRequest.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                //setting name or store has been changed
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.NoName")).BadRequest();
            }

            if (!setting.Name.Equals(settingUpdateRequest.SettingModelRequest.Name, StringComparison.InvariantCultureIgnoreCase) ||
                setting.StoreId != storeIdModel)
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(settingUpdateRequest.SettingModelRequest.Name, settingUpdateRequest.SettingModelRequest.Value, storeIdModel);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            ResponseSuccess respSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SettingUpdate.Success")
            };
            return Ok(respSuccess);
        }

        [HttpPost]
        public virtual IActionResult GetAllExternalAuthentications()
        {
            List<ExternalAuthenticationMethodResponse> externalAuthenticationMethodResponse = new List<ExternalAuthenticationMethodResponse>();

            var methods = _externalAuthenticationService.LoadAllExternalAuthenticationMethods();
            foreach (var method in methods)
            {
                ExternalAuthenticationMethodResponse model = new ExternalAuthenticationMethodResponse()
                {
                    DisplayOrder = method.PluginDescriptor.DisplayOrder,
                    FriendlyName = method.PluginDescriptor.FriendlyName,
                    IsActive = _externalAuthenticationService.IsExternalAuthenticationMethodActive(method),
                    SystemName = method.PluginDescriptor.SystemName
                };
                externalAuthenticationMethodResponse.Add(model);
            }

            return Ok(externalAuthenticationMethodResponse);
        }

        [HttpPost]
        public virtual IActionResult ExternalAuthentication([FromBody]ExternalAuthenticationRequest externalAuthenticationRequest)
        {
            if (externalAuthenticationRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var externalAuthenticationResponse = new ExternalAuthenticationResponse();

            //check plugin is installed or not
            if (!CheckExternalPluginInstall(externalAuthenticationRequest.ExternalAuthenticationParameter.ProviderSystemName))
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.ExternalAuthentication.NotInstall")).BadRequest();
            }

            var currentCustomer = _customerService.GetCustomerByGuid(externalAuthenticationRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //authorization list
            var record = _externalAuthenticationRecordRepository.Table.
                FirstOrDefault(o => o.ExternalIdentifier == externalAuthenticationRequest.ExternalAuthenticationParameter.ExternalIdentifier &&
                o.ProviderSystemName == externalAuthenticationRequest.ExternalAuthenticationParameter.ProviderSystemName);
            Customer userFound;
            if (record != null)
                userFound = _customerService.GetCustomerById(record.CustomerId);
            else
                userFound = null;

            var userLoggedIn = currentCustomer.IsRegistered() ? currentCustomer : null;

            if (AccountAlreadyExists(userFound, userLoggedIn))
            {
                if (AccountIsAssignedToLoggedOnAccount(userFound, userLoggedIn))
                {
                    return new ResponseObject(_localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.UserAlreadyAuthenticated")).BadRequest();
                }
                return new ResponseObject(_localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.AccountAlreadyAssigned")).BadRequest();

            }
            if (AccountDoesNotExistAndUserIsNotLoggedOn(userFound, userLoggedIn))
            {
                if (AutoRegistrationIsEnabled())
                {
                    #region Register user
                    var randomPassword = CommonHelper.GenerateRandomDigitCode(20);

                    bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;

                    /*mobile number*/
                    var registrationRequest = new CustomerRegistrationRequest(currentCustomer, externalAuthenticationRequest.ResponseEmailAddress, _customerSettings.UsernamesEnabled ? externalAuthenticationRequest.ResponseUserName : externalAuthenticationRequest.ResponseEmailAddress, randomPassword, PasswordFormat.Clear, externalAuthenticationRequest.StoreId, currentCustomer.MobileNumber,isApproved);
                    /*mobile number*/
                    var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {
                        //store other parameters (form fields)
                        if (!string.IsNullOrEmpty(externalAuthenticationRequest.ResponseFirstName))
                            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.FirstNameAttribute, externalAuthenticationRequest.ResponseFirstName);
                        if (!string.IsNullOrEmpty(externalAuthenticationRequest.ResponseLastName))
                            _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.LastNameAttribute, externalAuthenticationRequest.ResponseLastName);

                        userFound = currentCustomer;
                        var externalAuthenticationRecord = new ExternalAuthenticationRecord()
                        {
                            CustomerId = currentCustomer.Id,
                            Email = externalAuthenticationRequest.ExternalAuthenticationParameter.Email,
                            ExternalIdentifier = externalAuthenticationRequest.ExternalAuthenticationParameter.ExternalIdentifier,
                            ExternalDisplayIdentifier = externalAuthenticationRequest.ExternalAuthenticationParameter.ExternalDisplayIdentifier,
                            OAuthToken = externalAuthenticationRequest.ExternalAuthenticationParameter.OAuthToken,
                            OAuthAccessToken = externalAuthenticationRequest.ExternalAuthenticationParameter.OAuthAccessToken,
                            ProviderSystemName = externalAuthenticationRequest.ExternalAuthenticationParameter.ProviderSystemName,
                        };

                        _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);

                        externalAuthenticationResponse.CustomerGuid = externalAuthenticationRecord.Customer.CustomerGuid;
                        externalAuthenticationResponse.CustomerEmail = externalAuthenticationRecord.Email;

                        //code below is copied from CustomerController.Register method

                        //authenticate
                        if (isApproved)
                            _authenticationService.SignIn(userFound ?? userLoggedIn, false);

                        //notifications
                        if (_customerSettings.NotifyNewCustomerRegistration)
                            _workflowMessageService.SendCustomerRegisteredNotificationMessage(currentCustomer, _localizationSettings.DefaultAdminLanguageId);

                        switch (_customerSettings.UserRegistrationType)
                        {
                            case UserRegistrationType.EmailValidation:
                                {
                                    //email validation message
                                    _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                    _workflowMessageService.SendCustomerEmailValidationMessage(currentCustomer, externalAuthenticationRequest.LanguageId);
                                    ResponseSuccess respSuccess = new ResponseSuccess
                                    {
                                        Message = _localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.AutoRegisteredEmailValidation")
                                    };
                                    return Ok(respSuccess);

                                }
                            case UserRegistrationType.AdminApproval:
                                {
                                    ResponseSuccess respSuccess = new ResponseSuccess
                                    {
                                        Message = _localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.AutoRegisteredAdminApproval")
                                    };
                                    return Ok(respSuccess);

                                }
                            case UserRegistrationType.Standard:
                                {
                                    //send customer welcome message
                                    _workflowMessageService.SendCustomerWelcomeMessage(currentCustomer, externalAuthenticationRequest.LanguageId);

                                    ResponseSuccess respSuccess = new ResponseSuccess
                                    {
                                        Message = _localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.AutoRegisteredStandard")
                                    };
                                    return Ok(respSuccess);
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        externalAuthenticationResponse.Errors = new List<string>();
                        foreach (var error in registrationResult.Errors)
                        {
                            externalAuthenticationResponse.Errors.Add(error);
                        }
                        externalAuthenticationResponse.IsValid = false;
                        return new ResponseObject(externalAuthenticationResponse.ToString()).BadRequest();
                    }

                    #endregion
                }
                else if (RegistrationIsEnabled())
                {
                    return new ResponseObject(_localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.AssociateOnLogon")).BadRequest();

                }
                else
                {
                    return new ResponseObject(_localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.RegistrationIsDisabled")).BadRequest();

                }
            }
            if (userFound == null)
            {
                if (userLoggedIn != null)
                {
                    externalAuthenticationResponse.IsValid = false;
                    externalAuthenticationResponse.CustomerGuid = currentCustomer.CustomerGuid;
                    externalAuthenticationResponse.CustomerEmail = currentCustomer.Email;
                    externalAuthenticationResponse.Name = _customerService.GetCustomerFullName(currentCustomer);
                    externalAuthenticationResponse.Username = currentCustomer.Username;
                    return new ResponseObject(_localizationService.GetResource(externalAuthenticationResponse.ToString())).BadRequest();
                }
            }

            //migrate shopping cart
            _shoppingCartService.MigrateShoppingCart(currentCustomer, userFound ?? userLoggedIn, true);
            //authenticate
            _authenticationService.SignIn(userFound ?? userLoggedIn, false);
            //activity log
            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"),
                userFound ?? userLoggedIn);

            CustomerModel customerModel = new CustomerModel
            {
                CustomerGuid = currentCustomer.CustomerGuid,
                Message = _localizationService.GetResource("Plugins.Xcellenceit.Webapiclient.Message.AlreadyAuthenticated")
            };

            return Ok(customerModel);
        }

        [HttpPost]
        public virtual IActionResult GetExternalAuthentication([FromBody]GetExternalAuthenticationRequest getExternalAuthenticationRequest)
        {
            if (getExternalAuthenticationRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var model = new List<ExternalAuthenticationParameter>();

            var currentCustomer = _customerService.GetCustomerByGuid(getExternalAuthenticationRequest.CustomerGUID);
            if (currentCustomer == null)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerNotAvailable")).BadRequest();
            }

            //authorization list
            var record = (from c in _externalAuthenticationRecordRepository.Table
                          where c.CustomerId.Equals(currentCustomer.Id)
                          select c).ToList();

            if (record.Count == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ExternalAuthenticationNotFound")).BadRequest();
            }

            foreach (var item in record)
            {
                var externalAuthenticationParameter = new ExternalAuthenticationParameter
                {
                    CustomerGuid = item.Customer.CustomerGuid,
                    Email = item.Email,
                    ProviderSystemName = item.ProviderSystemName,
                    ExternalIdentifier = item.ExternalIdentifier,
                    ExternalDisplayIdentifier = item.ExternalDisplayIdentifier,
                    OAuthToken = item.OAuthToken,
                    OAuthAccessToken = item.OAuthAccessToken
                };
                model.Add(externalAuthenticationParameter);
            }

            return Ok(model);

        }

        [HttpPost]
        public virtual IActionResult RemoveExternalAuthentication([FromBody]RemoveExternalAuthenticationRequest removeExternalAuthenticationRequest)
        {
            if (removeExternalAuthenticationRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            //check plugin is installed or not
            if (!CheckExternalPluginInstall(removeExternalAuthenticationRequest.ProviderSystemName))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.ExternalAuthentication.NotInstall")).BadRequest();


            var currentCustomer = _customerService.GetCustomerByGuid(removeExternalAuthenticationRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CustomerNotAvailable")).BadRequest();

            //authorization list
            var record = (from c in _externalAuthenticationRecordRepository.Table
                          where c.CustomerId.Equals(currentCustomer.Id) && c.ProviderSystemName.Equals(removeExternalAuthenticationRequest.ProviderSystemName)
                          select c).ToList();

            if (record.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.RemoveExternalAuthentication.NotRegisteredOrAvailable")).BadRequest();

            foreach (var item in record)
            {
                var externalAuthenticationParameter = new ExternalAuthenticationRecord
                {
                    CustomerId = item.CustomerId,
                    Email = item.Email,
                    ProviderSystemName = item.ProviderSystemName,
                    ExternalIdentifier = item.ExternalIdentifier,
                    ExternalDisplayIdentifier = item.ExternalDisplayIdentifier,
                    OAuthToken = item.OAuthToken,
                    OAuthAccessToken = item.OAuthAccessToken
                };
                _externalAuthenticationRecordRepository.Delete(item);
            }
            ResponseSuccess responseSuccess = new ResponseSuccess
            {
                Message = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.RemoveExternalAuthentication.Deleted")
            };
            return Ok(responseSuccess);
        }

        #endregion
    }
}
