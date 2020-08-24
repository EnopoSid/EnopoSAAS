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

using Nop.Core.Domain.Common;
using System;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class RegisterResponse : CustomerModel
    {
        public bool IsValidRegistration { get; set; }

        public int CustomerId { get; set; }

        public string AdminComment { get; set; }

        public bool Active { get; set; }

        public int AffiliateId { get; set; }

        public virtual Address BillingAddress { get; set; }

        public DateTime? CannotLoginUntilDateUtc { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public Guid CustomerGuid { get; set; }

        public bool Deleted { get; set; }

        public string Email { get; set; }

        public string EmailToRevalidate { get; set; }

        public int FailedLoginAttempts { get; set; }

        public bool HasShoppingCartItems { get; set; }

        public bool IsSystemAccount { get; set; }

        public bool IsTaxExempt { get; set; }

        public DateTime LastActivityDateUtc { get; set; }

        public string LastIpAddress { get; set; }

        public DateTime? LastLoginDateUtc { get; set; }

        public int RegisteredInStoreId { get; set; }

        public bool RequireReLogin { get; set; }

        public virtual Address ShippingAddress { get; set; }

        public string SystemName { get; set; }

        public string Username { get; set; }

        public int VendorId { get; set; }

        public string Message { get; set; }

        /*For MobileNumber*/
        public string MobileNumber { get; set; }
        /*For MobileNumber*/

        /*Added by surakshith for IsGuestUser Start*/
        public bool? IsGuestUser { get; set; }
        /*Added by surakshith for IsGuestUser End*/
    }

}
