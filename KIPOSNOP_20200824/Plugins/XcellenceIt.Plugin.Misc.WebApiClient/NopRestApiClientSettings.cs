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

using Nop.Core.Configuration;

namespace XcellenceIt.Plugin.Misc.WebApiClient
{
    public class NopRestApiClientSettings : ISettings
    {
        /// <summary>
        ///  Gets or sets LicenseKey
        /// </summary>
        public string LicenseKey { get; set; }

        /// <summary>
        ///  Gets or sets StoreId 
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        ///  Gets or sets RestApi
        /// </summary>
        public bool RestApi { get; set; }

        /// <summary>
        ///  Gets or sets APIKey 
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        ///  Gets or sets DebugMode
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        ///  Gets or sets SecretKey
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// get or set Other License Settings
        /// </summary>
        public string OtherLicenseSettings { get; set; }
    }
}