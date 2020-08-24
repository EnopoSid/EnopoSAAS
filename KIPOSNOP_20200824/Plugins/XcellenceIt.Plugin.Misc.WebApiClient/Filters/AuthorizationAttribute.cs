using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.IO;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Filters
{
    public class AuthorizationAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public AuthorizationAttribute() : base(typeof(AuthorizationFilter))
        {
        }

        #endregion

        private class AuthorizationFilter : IActionFilter
        {
            #region Fields

            private readonly ISettingService _settingService;
            private readonly ILocalizationService _localizationService;
            private readonly IPluginFinder _pluginFinder;
            private readonly NopRestApiClientSettings _nopRestApiClientSettings;
            private readonly ICustomerService _customerService;
            private readonly ILogger _logger;

            #endregion

            #region Ctor
            public AuthorizationFilter(ISettingService settingService,
                ILocalizationService localizationService,
                IPluginFinder pluginFinder,
                ICustomerService customerService,
                ILogger logger,
                NopRestApiClientSettings nopRestApiClientSettings
                )
            {
                this._settingService = settingService;
                this._localizationService = localizationService;
                this._pluginFinder = pluginFinder;
                this._nopRestApiClientSettings = nopRestApiClientSettings;
                this._customerService = customerService;
                this._logger = logger;
            }
            #endregion

            public void OnActionExecuted(ActionExecutedContext context)
            {
                //throw new System.NotImplementedException();
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                //Get ApiSecretKey from body parameter of request
                string ApiSecretKey = GetApiSecretKeyFromBody(context.HttpContext.Request);

                if (ApiSecretKey == null)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "ApiSecretKey can not be empty."
                    };
                }

                //string apiSecretKey = headers["apiSecretKey"];
                bool isSecure = false;
                var headers = context.HttpContext.Request.Headers;
                if (!string.IsNullOrEmpty(headers["IsSecure"]) && headers["IsSecure"] == "IsSecure")
                    isSecure = true;

                if (!CheckAPIKey(ApiSecretKey, isSecure))
                {
                    context.Result = new ContentResult()
                    {
                        Content = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CheckApi")
                    };
                }
            }

            #region Utility

            private string GetApiSecretKeyFromBody(HttpRequest request)
            {
                try
                {
                    //https://gunnarpeipman.com/aspnet/aspnet-core-request-body/
                    string body = string.Empty;
                    request.EnableRewind();
                    using (var reader = new StreamReader(request.Body))
                    {
                        request.Body.Seek(0, SeekOrigin.Begin);
                        body = reader.ReadToEnd();
                    }

                    //Dictionary key checking case sensitive so for avoid that problem. we check key in lower so it get matched.
                    string apiKeyText = "apisecretkey";
                    string apiSecretKeyValue = string.Empty;
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                    foreach (string key in data.Keys)
                    {
                        if (key.ToLower() == apiKeyText)
                        {
                            //Key matched. get value.
                            apiSecretKeyValue = Convert.ToString(data[key]);
                            break;
                        }
                    }

                    return apiSecretKeyValue;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to get apiSecretKey from body.", ex);
                }
                return string.Empty;
            }

            #endregion

            #region Check API Key

            protected bool CheckAPIKey(string apiSecretKey, bool isSecure)
            {
                #region ++License implementation

                //// get all store rest api setting
                //var allStoresetting = _settingService.LoadSetting<NopRestApiClientSettings>(0);
                //LicenseImplementer licenseImplimenter = new LicenseImplementer();
                //Boolean isLicenseActive = licenseImplimenter.IsLicenseActive(allStoresetting.LicenseKey, allStoresetting.OtherLicenseSettings);
                //if (!isLicenseActive)
                //    throw new ApplicationException("License key not active");

                #endregion --License implementation

                //check whether web service plugin is installed
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("XcellenceIt.WebApiClient");
                var nopRestApiSettings = _settingService.LoadSetting<NopRestApiClientSettings>(0);

                if (pluginDescriptor == null || !nopRestApiSettings.RestApi)
                    throw new ApplicationException("XcellenceIT NopRestApi plugin can not be loaded or enabled");
                if (!_pluginFinder.AuthenticateStore(pluginDescriptor, 0))
                    throw new ApplicationException("XcellenceIT NopRestApi plugin is not available in this store");

                //Check debug mode is enable or not
                if (isSecure != true)
                {
                    if (_nopRestApiClientSettings.DebugMode)
                    {
                        if (apiSecretKey == _nopRestApiClientSettings.APIKey)
                            return true;
                        else
                            return false;
                    }
                    else
                        throw new ApplicationException("401 - The user is not authorized to make the request.");
                }
                else
                {
                    if (apiSecretKey == _nopRestApiClientSettings.APIKey)
                        return true;
                    else
                        return false;
                }
            }

            #endregion
        }
    }
}
