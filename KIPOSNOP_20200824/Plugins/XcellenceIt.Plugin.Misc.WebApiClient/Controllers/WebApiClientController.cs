// *************************************************************************
// *                                                                       *
// * nopAccelerate - nopAccelerate Web Api Client Plugin           *
// * Copyright (c) Xcellence-IT. All Rights Reserved.                      *
// *                                                                       *
// *************************************************************************
// *                                                                       *
// * Email: info@nopaccelerate.com                                         *
// * Website: http://www.nopaccelerate.com                                 *
// *                                                                       *
// *************************************************************************
// *                                                                       *
// * This  software is furnished  under a license  and  may  be  used  and *
// * modified  only in  accordance with the terms of such license and with *
// * the  inclusion of the above  copyright notice.  This software or  any *
// * other copies thereof may not be provided or  otherwise made available *
// * to any  other  person.   No title to and ownership of the software is *
// * hereby transferred.                                                   *
// *                                                                       *
// * You may not reverse  engineer, decompile, defeat  license  encryption *
// * mechanisms  or  disassemble this software product or software product *
// * license.  Xcellence-IT may terminate this license if you don't comply *
// * with  any  of  the  terms and conditions set forth in  our  end  user *
// * license agreement (EULA).  In such event,  licensee  agrees to return *
// * licensor  or destroy  all copies of software  upon termination of the *
// * license.                                                              *
// *                                                                       *
// * Please see the  License file for the full End User License Agreement. *
// * The  complete license agreement is also available on  our  website at *
// * http://www.nopaccelerate.com/enterprise-license                       *
// *                                                                       *
// *************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using System;
using System.Text;
using XcellenceIt.Core;
using XcellenceIt.Core.Enums;
using XcellenceIt.Plugin.Misc.WebApiClient.Models;
using System.Reflection;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Area(AreaNames.Admin)]
    public class WebApiClientController : BaseAdminController
    {
        #region Object Cache

        //protected ObjectCache ResponseCache
        //{
        //    get
        //    {
        //        return MemoryCache.Default;
        //    }
        //}

        #endregion

        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly ILocalizationService _localizationService;      
        private readonly NopRestApiClientSettings _nopRestApiSettings;      
        private readonly IPluginFinder _pluginFinder;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public WebApiClientController(ISettingService settingService,
            IStoreService storeService, 
            IWorkContext workContext, 
            ILocalizationService localizationService,
            NopRestApiClientSettings nopRestApiSettings,
             IPluginFinder pluginFinder,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            IPermissionService permissionService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeService = storeService;
            _workContext = workContext;
            _localizationService = localizationService;           
            _nopRestApiSettings = nopRestApiSettings;
            _pluginFinder = pluginFinder;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        #region Configurations

        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            // Vendor should not access to use nop rest api plugins 
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();
            
            //Get Current URL
            string CurrentUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.ToString() + _httpContextAccessor.HttpContext.Request.Path.ToString();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var model = new ConfigurationModel();

            #region ++License implementation

            // get all store rest api setting
            var allStoresetting = _settingService.LoadSetting<NopRestApiClientSettings>(0);
            LicenseImplementer licenseImplementer = new LicenseImplementer();

            model.IsLicenseActive = licenseImplementer.IsLicenseActive(allStoresetting.LicenseKey, allStoresetting.OtherLicenseSettings);
            if (model.IsLicenseActive)
                model.LicenseInformation = licenseImplementer.GetLicenseInformation(allStoresetting.LicenseKey, allStoresetting.OtherLicenseSettings, ProductName.NopRestApi);
            else
            {
                string validationURL = string.Empty;
                validationURL = _webHelper.GetStoreLocation();
                if (!_webHelper.GetStoreLocation().EndsWith("/"))
                    validationURL = "/";

                validationURL += "Admin/WebApiClient/ValidateLicense";

                model.RegistrationForm = licenseImplementer.GetRegistrationForm(ProductName.NopRestApi, LicenseApiVersion.V1, NopVersion.CurrentVersion, "", validationURL, "http://shop.xcellence-it.com/nopaccelerate-web-api-plugin");
            } 

            #endregion --License implementation

            var settings = _settingService.LoadSetting<NopRestApiClientSettings>(storeScope);

            //ViewBag.Status = Convert.ToInt32(ResponseCache["Response-RestApi"]);
            model.RestApi = settings.RestApi;
            model.APIKey = settings.APIKey;
            model.ActiveStoreScopeConfiguration = storeScope;
            model.DebugMode = settings.DebugMode;
            model.VersionNumber = _pluginFinder.GetPluginDescriptorBySystemName("XcellenceIt.WebApiClient")?.Version;
            if (storeScope > 0)
            {
                model.RestApi_OverrideForStore = _settingService.SettingExists(settings, x => x.RestApi, storeScope);
                model.APIKey_OverrideForStore = _settingService.SettingExists(settings, x => x.APIKey, storeScope);
                model.DebugMode_OverrideForStore = _settingService.SettingExists(settings, x => x.DebugMode, storeScope);
            }           
            return View("~/Plugins/XcellenceIt.WebApiClient/Views/WebApiClient/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            _nopRestApiSettings.RestApi = model.RestApi;
            _nopRestApiSettings.DebugMode = model.DebugMode;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.RestApi_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(_nopRestApiSettings, x => x.RestApi, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(_nopRestApiSettings, x => x.RestApi, storeScope);

            if (model.RestApi_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(_nopRestApiSettings, x => x.DebugMode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(_nopRestApiSettings, x => x.DebugMode, storeScope);

            if (model.APIKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(_nopRestApiSettings, x => x.APIKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(_nopRestApiSettings, x => x.DebugMode, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        #endregion

        #region License Key

        [HttpPost]
        public JsonResult ValidateLicense(string licenseKey)
        {
            try
            {
                PluginDescriptor pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("XcellenceIt.WebApiClient");
                LicenseDetails licenseDetails = new LicenseDetails
                {
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    NopVersion = NopVersion.CurrentVersion,
                    SystemName = "XcellenceIt.Plugin.Misc.NopRestApi",
                    ProductName = ProductName.NopRestApi,
                    ProductVersion = pluginDescriptor.Version,
                    Hours = 24,
                    CurrentApiVersion = LicenseApiVersion.V1,
                    LicenseKey = licenseKey,
                    DomainName = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host
                };

                LicenseImplementer licenseImplementer = new LicenseImplementer();
                LicenseRegistrationStatus registrationStatus = licenseImplementer.RegisterLicenseKey(licenseDetails);
                if (registrationStatus.ActiveStatus)
                {
                    var settings = _settingService.LoadSetting<NopRestApiClientSettings>(0);

                    settings.LicenseKey = registrationStatus.LicenseKey;
                    settings.OtherLicenseSettings = registrationStatus.OtherLicenseSetting;

                    _settingService.SaveSetting(settings, x => x.LicenseKey, 0, true);
                    _settingService.SaveSetting(settings, x => x.OtherLicenseSettings, 0, true);

                    return Json(new { success = registrationStatus.StatusMessage + Environment.NewLine + "STATUS: " + registrationStatus.StatusDescription });
                }
                else
                    if (string.IsNullOrEmpty(registrationStatus.StatusDescription))
                    return Json(new { error = registrationStatus.StatusMessage });
                else
                    return Json(new { error = registrationStatus.StatusMessage + Environment.NewLine + "STATUS: " + registrationStatus.StatusDescription });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region API Key
        public IActionResult GenerateAPIKey(string btnId, string formId)
        {
            var model = new AppSecretModel();
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View("~/Plugins/XcellenceIt.WebApiClient/Views/WebApiClient/GenerateAPIKey.cshtml", model);
        }

        [HttpPost]
        public IActionResult GenerateAPIKey(AppSecretModel model, string btnId, string formId)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 97))));
                builder.Append(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 97)));
            }

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;

            _nopRestApiSettings.APIKey = builder.ToString();
            _nopRestApiSettings.StoreId = storeScope;
            _nopRestApiSettings.SecretKey = model.AppID + "|||" + model.SecretKey;

            _settingService.SaveSetting(_nopRestApiSettings, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return Configure();
        }

        #endregion API Key

        #endregion
    }
}
