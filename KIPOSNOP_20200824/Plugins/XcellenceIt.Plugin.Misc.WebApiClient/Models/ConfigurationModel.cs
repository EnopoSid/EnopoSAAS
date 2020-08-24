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

using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        /// <summary>
        ///  Gets or sets ActiveStoreScopeConfiguration
        /// </summary>
        public int ActiveStoreScopeConfiguration { get; set; }

        /// <summary>
        ///  Gets or sets RestApi
        /// </summary>
        [NopResourceDisplayName("Plugins.XcellenceIT.WebApiClient.Field.RestApi")]
        public bool RestApi { get; set; }

        /// <summary>
        ///  Gets or sets RestApi_OverrideForStore
        /// </summary>
        public bool RestApi_OverrideForStore { get; set; }

        /// <summary>
        ///  Gets or sets DebugMode
        /// </summary>
        [NopResourceDisplayName("Plugins.XcellenceIT.WebApiClient.Field.DebugMode")]
        public bool DebugMode { get; set; }

        /// <summary>
        ///  Gets or sets
        /// </summary>
        public bool DebugMode_OverrideForStore { get; set; }

        /// <summary>
        ///  Gets or sets APIKey
        /// </summary>
        [NopResourceDisplayName("Plugins.XcellenceIT.WebApiClient.Field.APIKey")]
        public string APIKey { get; set; }

        /// <summary>
        ///  Gets or sets APIKey_OverrideForStore
        /// </summary>
        public bool APIKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets License Information
        /// </summary>        
        public string LicenseInformation { get; set; }

        /// <summary>
        /// Gets or sets IsLicenseActive
        /// </summary>
        public bool IsLicenseActive { get; set; }

        /// <summary>
        /// Gets or sets License Registration Form
        /// </summary>
        public string RegistrationForm { get; set; }

        /// <summary>
        /// get or set Version number
        /// </summary>
        public string VersionNumber { get; set; }
    }
}