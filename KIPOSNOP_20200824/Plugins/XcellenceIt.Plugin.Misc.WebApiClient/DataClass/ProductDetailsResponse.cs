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
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ProductDetailsResponse : CustomerModel
    {
        public int Id { get; set; }

        public bool DefaultPictureZoomEnabled { get; set; }
        
        public PictureModel DefaultPictureModel { get; set; }
        
        public IList<PictureModel> PictureModels { get; set; }

        public string Name { get; set; }
        
        public string ShortDescription { get; set; }
        
        public string FullDescription { get; set; }
        
        public string ProductTemplateViewPath { get; set; }
        
        public string MetaKeywords { get; set; }
        
        public string MetaDescription { get; set; }
        
        public string MetaTitle { get; set; }
        
        public string SeName { get; set; }
        
        public bool ShowSku { get; set; }
        
        public string Sku { get; set; }

        public bool ShowManufacturerPartNumber { get; set; }
        
        public string ManufacturerPartNumber { get; set; }

        public bool ShowGtin { get; set; }
        
        public string Gtin { get; set; }

        public bool ShowVendor { get; set; }
        
        public VendorBriefInfoModel VendorModel { get; set; }

        public bool HasSampleDownload { get; set; }

        public GiftCardModel GiftCard { get; set; }

        public bool IsShipEnabled { get; set; }
        
        public bool IsFreeShipping { get; set; }
        
        public bool FreeShippingNotificationEnabled { get; set; }
        
        public string DeliveryDate { get; set; }

        public bool IsRental { get; set; }
        
        public DateTime? RentalStartDate { get; set; }
        
        public DateTime? RentalEndDate { get; set; }

        public string StockAvailability { get; set; }
        
        public bool DisplayBackInStockSubscription { get; set; }

        public bool EmailAFriendEnabled { get; set; }
        
        public bool CompareProductsEnabled { get; set; }
        
        public string PageShareCode { get; set; }

        public ProductPriceModel ProductPrice { get; set; }
        
        public AddToCartModel AddToCart { get; set; }
        
        public ProductBreadcrumbModel Breadcrumb { get; set; }
        
        public IList<ProductTagModel> ProductTags { get; set; }

        public IList<ProductAttributeModel> ProductAttributes { get; set; }
        
        public IList<ProductSpecificationModel> ProductSpecifications { get; set; }

        public IList<ManufacturerModel> ProductManufacturers { get; set; }

        public ProductReviewOverviewModel ProductReviewOverview { get; set; }
        
        public IList<TierPriceModel> TierPrices { get; set; }

        public IList<ProductDetailsResponse> AssociatedProducts { get; set; }
        
        public bool DisplayDiscontinuedMessage { get; set; }

		#region Nested Classes

        public partial class ProductBreadcrumbModel : BaseNopModel
        {
            
            public bool Enabled { get; set; }
            
            public int ProductId { get; set; }
            
            public string ProductName { get; set; }
            
            public string ProductSeName { get; set; }
            
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        public partial class AddToCartModel : BaseNopModel
        {
            
            public int ProductId { get; set; }

            
            public int EnteredQuantity { get; set; }

            
            public bool CustomerEntersPrice { get; set; }
            
            public decimal CustomerEnteredPrice { get; set; }
            
            public string CustomerEnteredPriceRange { get; set; }

            
            public bool DisableBuyButton { get; set; }
            
            public bool DisableWishlistButton { get; set; }
            
            public List<SelectListItem> AllowedQuantities { get; set; }
            
            public bool IsRental { get; set; }

            public bool AvailableForPreOrder { get; set; }
            
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

            public int UpdatedShoppingCartItemId { get; set; }

            public string MinimumQuantityNotification { get; set; }
        }

        public partial class ProductPriceModel : BaseNopModel
        {
            
            public string CurrencyCode { get; set; }

            public string OldPrice { get; set; }
            
            public string Price { get; set; }
            
            public string PriceWithDiscount { get; set; }

            public decimal PriceValue { get; set; }
            
            public decimal PriceWithDiscountValue { get; set; }
            
            public bool CustomerEntersPrice { get; set; }
            
            public bool CallForPrice { get; set; }
            
            public int ProductId { get; set; }

            public bool HidePrices { get; set; }
            
            public bool IsRental { get; set; }
            
            public string RentalPrice { get; set; }
            
            public bool DisplayTaxShippingInfo { get; set; }
        }

        public partial class GiftCardModel : BaseNopModel
        {
            
            public bool IsGiftCard { get; set; }

            
            public string RecipientName { get; set; }
            
            public string RecipientEmail { get; set; }
            
            public string SenderName { get; set; }
            
            public string SenderEmail { get; set; }
            
            public string Message { get; set; }

            
            public GiftCardType GiftCardType { get; set; }
        }

        public partial class TierPriceModel : BaseNopModel
        {
            
            public string Price { get; set; }

            
            public int Quantity { get; set; }
        }

        public partial class ProductAttributeModel : BaseNopEntityModel
        {
            
            public int ProductId { get; set; }

            
            public int ProductAttributeId { get; set; }

            
            public string Name { get; set; }

            
            public string Description { get; set; }

            
            public string TextPrompt { get; set; }

            
            public bool IsRequired { get; set; }
            
            public string DefaultValue { get; set; }
            
            public int? SelectedDay { get; set; }
            
            public int? SelectedMonth { get; set; }
            
            public int? SelectedYear { get; set; }
            
            public IList<string> AllowedFileExtensions { get; set; }
            
            public AttributeControlType AttributeControlType { get; set; }
            
            public IList<ProductAttributeValueModel> Values { get; set; }

            public bool HasCondition { get; set; }

        }

        public partial class ProductAttributeValueModel : BaseNopEntityModel
        {
            public int ProductAttributeMappingId { get; set; }

            public string Name { get; set; }
            
            public string ColorSquaresRgb { get; set; }
            
            public string PriceAdjustment { get; set; }
            
            public decimal PriceAdjustmentValue { get; set; }
            
            public bool IsPreSelected { get; set; }

            public int PictureId { get; set; }
            
            public string PictureUrl { get; set; }
            
            public string FullSizePictureUrl { get; set; }

            public decimal WeightAdjustment { get; set; }
        }

        public partial class ManufacturerModel : BaseNopEntityModel
        {
            
            public string Name { get; set; }
            
            public string Description { get; set; }
            
            public string MetaKeywords { get; set; }
            
            public string MetaDescription { get; set; }
            
            public string MetaTitle { get; set; }
            
            public string SeName { get; set; }

        }

        #endregion

        public string CurrentStoreName { get; set; }

    }
}
