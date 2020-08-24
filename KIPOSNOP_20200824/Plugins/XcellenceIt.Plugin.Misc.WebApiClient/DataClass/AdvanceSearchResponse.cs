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

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class AdvanceSearchResponse 
    {
        public AdvanceSearchResponse()
        {
            this.PagingFilteringContext = new CatalogPagingFilteringResponse();
            this.Products = new List<ProductListingResponse>();
            this.AvailableCategories = new List<SelectListItem>();
            this.AvailableManufacturers = new List<SelectListItem>();
            this.AvailableVendors = new List<SelectListItem>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        public string Q { get; set; }

        public int cid { get; set; }

        [DataMember]
        public bool isc { get; set; }

        public int mid { get; set; }

        public int vid { get; set; }

        public string pf { get; set; }

        public string pt { get; set; }

        public bool sid { get; set; }

        public bool adv { get; set; }

        public bool asv { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public CatalogPagingFilteringResponse PagingFilteringContext { get; set; }

        public IList<ProductListingResponse> Products { get; set; }
    }
}
