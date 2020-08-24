// *************************************************************************
// *                                                                       *
// * nopAccelerate - nopAccelerate REST Web Api Client Plugin           *
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
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductListingResponse : CustomerModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
      
        public string ShortDescription { get; set; }
        
        public string FullDescription { get; set; }
       
        public string SeName { get; set; }

        public ProductPriceModel ProductPrice { get; set; }

        public PictureModel DefaultPictureModel { get; set; }

        public ProductReviewOverviewModel ReviewOverviewModel { get; set; }

		#region Nested Classes

        public partial class ProductPriceModel : BaseNopModel
        {
      
            public string OldPrice { get; set; }
          
            public string Price {get;set;}

            public bool DisableBuyButton { get; set; }

            public bool DisableWishlistButton { get; set; }

            public bool AvailableForPreOrder { get; set; }
 
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

            public bool IsRental { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }

            public bool DisplayTaxShippingInfo { get; set; }
        }

		#endregion
    }
}
