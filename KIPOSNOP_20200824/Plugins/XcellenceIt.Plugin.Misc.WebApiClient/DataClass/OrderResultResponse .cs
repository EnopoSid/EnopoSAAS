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

using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class OrderResultResponse 
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public IList<OrderItemReponse> OrderItems { get; set; }

        public AddressResponse BillingAddress { get; set; }

        public AddressResponse ShippingAddress { get; set; }

        public string OrderTotal { get; set; }     
        
        public IList<OrderReponse> Orders { get; set; }

        public string OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethods { get; set; }

        public string ShippingStatus { get; set; }

        public string ShippingMethods { get; set; }

        public bool IsReOrderAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }

        public bool IsShippable { get; set; }

        public bool PickUpInStore { get; set; }

        public string VatNumber { get; set; }

        public bool CanRePostProcessPayment { get; set; }

        public string OrderSubtotal { get; set; }

        public string OrderSubTotalDiscount { get; set; }

        public string OrderShipping { get; set; }

        public string PaymentMethodAdditionalFee { get; set; }

        public string Tax { get; set; }

        public IList<TaxRate> TaxRates { get; set; }

        public bool DisplayTax { get; set; }

        public bool DisplayTaxRates { get; set; }

        public bool PricesIncludeTax { get; set; }

        public bool DisplayTaxShippingInfo { get; set; }

        public string OrderTotalDiscount { get; set; }

        public int RedeemedRewardPoints { get; set; }

        public string RedeemedRewardPointsAmount { get; set; }

        /*For Redeem Amount by sree 09_02_2019*/
        public decimal RedeemAmount { get; set; }
        /*For Redeem Amount by sree 09_02_2019*/

        public string CheckoutAttributeInfo { get; set; }

        public bool ShowSku { get; set; }

    }

    public class OrderItemReponse
    {
        public int Id { get; set; }
        
        public Guid OrderItemGuid { get; set; }
        
        public string Sku { get; set; }
        
        public int ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public string ProductSeName { get; set; }
        
        public string UnitPrice { get; set; }
        
        public string SubTotal { get; set; }
        
        public int Quantity { get; set; }
        
        public string AttributeInfo { get; set; }
       
        public string RentalInfo { get; set; }
        
        public int DownloadId { get; set; }

        public int LicenseId { get; set; }

        public PictureModel Picture { get; set; }
    }

    public class OrderReponse
    {
        public int OrderId { get; set; }
  
        public string OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public string OrderTotal { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool PickUpInStore { get; set; }

        public int ParentCategoryId { get; set; }
    }

    public class TaxRate 
    {
        public string Rate { get; set; }

        public string Value { get; set; }
    }
}
