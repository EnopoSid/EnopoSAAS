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

using System.Collections.Generic;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{

    public class OrderTotalsResponse : CustomerModel
    {
        public bool IsEditable { get; set; }

        public string SubTotal { get; set; }

        public string SubTotalDiscount { get; set; }
        
        public bool AllowRemovingSubTotalDiscount { get; set; }

        public string Shipping { get; set; }
        
        public bool RequiresShipping { get; set; }
        
        public string SelectedShippingMethod { get; set; }
        
        public string PaymentMethodAdditionalFee { get; set; }
        
        public string Tax { get; set; }
        
        public IList<TaxRate> TaxRates { get; set; }
        
        public bool DisplayTax { get; set; }
        
        public bool DisplayTaxRates { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public string OrderTotalDiscount { get; set; }
        
        public bool AllowRemovingOrderTotalDiscount { get; set; }
        
        public int RedeemedRewardPoints { get; set; }
        
        public string RedeemedRewardPointsAmount { get; set; }
        
        public int WillEarnRewardPoints { get; set; }
        
        public string OrderTotal { get; set; }

        #region Nested classes

        public partial class TaxRate
        {
            public string Rate { get; set; }
            
            public string Value { get; set; }
        }

        public partial class GiftCard
        {
            public int Id { get; set; }
            
            public string CouponCode { get; set; }
            
            public string Amount { get; set; }
            
            public string Remaining { get; set; }
        }
        #endregion

    }

}
