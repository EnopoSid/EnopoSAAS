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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace XcellenceIt.Plugin.Misc.WebApiClient
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {

            routes.MapAreaRoute(name: "XcellenceIT.Plugin.WebApiClient.Configure",
            template: "Plugins/WebApiClient/Configure",
            areaName: "Admin",
            defaults: new { controller = "WebApiClient", action = "Configure" }
          );

            routes.MapAreaRoute(name: "XcellenceIT.Plugin.WebApiClient.GenerateAPIKey",
            template: "Plugins/WebApiClient/GenerateAPIKey",
            areaName: "Admin",
            defaults: new { controller = "WebApiClient", action = "GenerateAPIKey" }
           );

            routes.MapAreaRoute(name: "XcellenceIT.Plugin.WebApiClient.Error",
            template: "Plugins/WebApiClient/WebApiClient/Error",
            areaName: "Admin",
            defaults: new { controller = "WebApiClient", action = "Error" }
           );

        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }
    }
}


