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

using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XcellenceIt.Plugin.Misc.WebApiClient
{
    public class WebApiPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        public const string ResourceStringPath = "~/Plugins/XcellenceIt.WebApiClient/Localization/ResourceString";       

        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly ILanguageService _languageService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor
        public WebApiPlugin(IPermissionService permissionService,
            ILanguageService languageService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            ISettingService settingService)
        {
            _permissionService = permissionService;
            _languageService = languageService;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _settingService = settingService;
        }
        #endregion

        #region Utilities

        /// <summary>
        ///Import Resource string from xml and save
        /// </summary>
        protected virtual void InstallLocaleResources()
        {
            var allLanguages = _languageService.GetAllLanguages();
            string[] resourceStringfilePaths = Directory.GetFiles(_fileProvider.MapPath(ResourceStringPath), "*.xml");
            foreach (var filePath in resourceStringfilePaths)
            {
                string languageName = (from name in XDocument.Load(filePath).Document.Descendants("Language")
                                       select name.Attribute("Name").Value).FirstOrDefault();
                if (!string.IsNullOrEmpty(languageName))
                {
                    var language = allLanguages.Where(x => x.Name.ToLowerInvariant() == languageName.ToLowerInvariant()).FirstOrDefault();
                    if (language != null)
                    {
                        var localesXml = File.ReadAllText(filePath);
                        _localizationService.ImportResourcesFromXml(language, localesXml);
                    }
                }
            }            
        }

        ///<summry>
        ///Delete Resource String
        ///</summry>
        protected virtual void DeleteLocalResources()
        {
            string[] resourceStringfilePaths = Directory.GetFiles(_fileProvider.MapPath(ResourceStringPath), "*.xml");
            foreach (var filePath in resourceStringfilePaths)
            {
                var languageResourceNames = from name in XDocument.Load(filePath).Document.Descendants("LocaleResource")
                                            select name.Attribute("Name").Value;

                foreach (var item in languageResourceNames)
                {
                    _localizationService.DeletePluginLocaleResource(item);
                }
            }
        }

        #endregion

        #region Methods

        public override void Install()
        {
            InstallLocaleResources();
            base.Install();
        }

        public override void Uninstall()
        {
            DeleteLocalResources();
            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode bulidmenu)
        {
            // Add QuickView admin Menu to Plugin Menu
            var mainMenuItem = new SiteMapNode()
            {
                Title = "nopAccelerate",
                Visible = Authenticate(),
                IconClass = "fa fa-buysellads"
            };
            mainMenuItem.Visible = Authenticate();

            //Add the pluginmenuitem to plugin sub menu
            var pluginMenuItem = new SiteMapNode()
            {
                Title = "Client Web API",
                Visible = Authenticate(),
                IconClass = "fa-dot-circle-o"
            };
            mainMenuItem.ChildNodes.Add(pluginMenuItem);

            var Configure = new SiteMapNode()
            {
                SystemName = "XcellenceIt.WebApiClient.Configure",
                Title = "Configuration",
                ControllerName = "WebApiClient",
                ActionName = "Configure",
                Visible = Authenticate(),
                IconClass = "fa-genderless",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            pluginMenuItem.ChildNodes.Add(Configure);

            //Get url from setting
            var settings = _settingService.LoadSetting<NopRestApiClientSettings>();

            var HelpDocument = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Field.HelpDocument"),
                Url = "http://docs.nopaccelerate.com/files/webapi-nop-commerce-plug-in/",
                OpenUrlInNewTab = true,
                Visible = Authenticate(),
                IconClass = "fa-genderless",
            };
            pluginMenuItem.ChildNodes.Add(HelpDocument);

            var targetMenu = bulidmenu.ChildNodes.FirstOrDefault(x => x.Title == "nopAccelerate");
            if (targetMenu != null)
            {
                targetMenu.ChildNodes.Add(pluginMenuItem);
            }
            else
            {
                bulidmenu.ChildNodes.Add(mainMenuItem);
            }
        }

        public bool Authenticate()
        {
            return _permissionService.Authorize(StandardPermissionProvider.ManagePlugins);
        }


        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WebApiClient/Configure";
        }
        #endregion
    }
}
