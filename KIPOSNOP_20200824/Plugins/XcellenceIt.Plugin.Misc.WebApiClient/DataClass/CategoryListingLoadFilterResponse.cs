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
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public partial class CategoryListingLoadFilterResponse :CustomerModel
    {

        public IList<CategorySimpleModel> Categories { get; set; }
      
        public IList<TopMenuTopicModel> Topics { get; set; }

      
        public bool BlogEnabled { get; set; }

        public bool RecentlyAddedProductsEnabled { get; set; }
 
        public bool ForumEnabled { get; set; }

        #region Nested classes
        public class TopMenuTopicModel : BaseNopEntityModel
        {
         
            public string Name { get; set; }
            
            public string SeName { get; set; }
        }
        #endregion

        //Get Product By Category
      
        public string Name { get; set; }
      
        public string Description { get; set; }
       
        public string MetaKeywords { get; set; }
       
        public string MetaDescription { get; set; }
    
        public string MetaTitle { get; set; }
       
        public string SeName { get; set; }

        
        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringResponse PagingFilteringContext { get; set; }
        
        public IList<ProductListingResponse> FeaturedProducts { get; set; }

        public IList<ProductListingResponse> Products { get; set; }

        public IList<SpecificationAttributeFilter> SpecificationFilter { get; set; }

        public IList<PriceRangeFilters> PriceRangeFilters { get; set; }
       
        public IList<SpecificationFilterItemResponse> AlreadyFilteredItems { get; set; }

        public IList<SpecificationFilterItemResponse> NotFilteredItems { get; set; }

    }

    public class SpecificationAttributeFilter {

        public SpecificationAttributeFilter()
        {
            this.SpecificationAttributeOptions = new List<SpecificationAttributeOptionLoadFilter>();
        }
        public int SpecificationAttributeId { get; set; }

        public string SpecificationAttributeName { get; set; }

        public int SpecificationAttributeDisplayOrder { get; set; }

        public List<SpecificationAttributeOptionLoadFilter> SpecificationAttributeOptions { get; set; }

    }

    public class SpecificationAttributeOptionLoadFilter
    {
        public int SpecificationAttributeOptionId { get; set; }

        public string SpecificationAttributeOptionName { get; set; }

        public string SpecificationAttributeColorRGB{ get; set; }

        public int SpecificationAttributeOptionDisplayOrder { get; set; }
    }
}
