using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public partial class FreshOrderDetailResponse: BaseNopEntityModel
    {
        public FreshOrderDetailResponse()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            //Items = new List<OrderItemModel>();
            OrderNotes = new List<OrderNote>();
            Shipments = new List<ShipmentBriefModel>();
            CustomValues = new Dictionary<string, object>();
        }

        public bool PrintMode { get; set; }
        public bool PdfInvoiceDisabled { get; set; }

        public string CustomOrderNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public string OrderStatus { get; set; }

        public bool IsReOrderAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }

        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        public string ShippingStatus { get; set; }
        public string ShippingMethod { get; set; }
        public IList<ShipmentBriefModel> Shipments { get; set; }

        public string CustomerEmail { get; set; }
        public string CustomerMobileNumber { get; set; }

        public string VatNumber { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentMethodStatus { get; set; }
        public bool CanRePostProcessPayment { get; set; }
        public Dictionary<string, object> CustomValues { get; set; }

        public string OrderSubtotal { get; set; }
        public string OrderSubTotalDiscount { get; set; }
        public string OrderShipping { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }
        public string CheckoutAttributeInfo { get; set; }

        public bool IsTinCartSelected { get; set; }
        public decimal PackagingCharges { get; set; }

        public bool PricesIncludeTax { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }
        public string OrderTotal { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public bool ShowSku { get; set; }
        public List<OrderMealPlanModel> orderMealPlanModels { get; set; }

        public OrderMealPlanModel orderMealPlan { get; set; }

        public IList<OrderNote> OrderNotes { get; set; }

        public bool ShowVendorName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string PaymentMethodType { get; set; }


        #region Nested Classes

        public partial class OrderItemModel : BaseNopEntityModel
        {
            public Guid OrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string UnitPrice { get; set; }
            public string SubTotal { get; set; }
            public string AttributeInfo { get; set; }
            public int ParentCategoryId { get; set; }
            public int MealNumber { get; set; }
            public DateTime MealDate { get; set; }
            public string MealTime { get; set; }
        }

        public partial class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class OrderNote : BaseNopEntityModel
        {
            public bool HasDownload { get; set; }
            public string Note { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public partial class ShipmentBriefModel : BaseNopEntityModel
        {
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }

        public partial class OrderMealPlanModel
        {
            public Guid MealOrderId { get; set; }
            public string MealPlanName { get; set; }
            public int ProductId { get; set; }
            public int MealPlanPrice { get; set; }
            public List<OrderItemModel> Items { get; set; }
        }

        #endregion

    }
}
