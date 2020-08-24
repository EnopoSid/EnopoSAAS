using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh
{
    public partial class FreshShoppingCartModelResponse:CustomerModel
    {
        public bool OnePageCheckoutEnabled { get; set; }

        public bool ShowSku { get; set; }
        public bool ShowProductImages { get; set; }
        public bool IsEditable { get; set; }
        //public IList<FreshShoppingCartItemModel> Items { get; set; }
        public string CheckoutAttributeInfo { get; set; }
        

        //public IList<CheckoutAttributeModel> CheckoutAttributes { get; set; }
        public IList<MealPlanModel> mealPlanModels { get; set; }
        public MealPlanModel mealPlanModel { get; set; }
        public IList<string> Warnings { get; set; }
        public string MinOrderSubtotalWarning { get; set; }
        public bool TermsOfServiceOnShoppingCartPage { get; set; }
        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public DiscountBoxModel DiscountBox { get; set; }
        public GiftCardBoxModel GiftCardBox { get; set; }
        public OrderReviewDataModel OrderReviewData { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        public string Tax { get; set; }
        public bool IsApplied { get; set; }
        


        #region Nested Classes

        public partial class FreshShoppingCartItemModel
        {
            public FreshShoppingCartItemModel()
            {
                Picture = new PictureModel();
            }
            public string Sku { get; set; }

            public PictureModel Picture { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public string Discount { get; set; }

            public int Quantity { get; set; }

            public List<SelectListItem> AllowedQuantities { get; set; }

            public string AttributeInfo { get; set; }

            public List<string> AttributeInfoAsArrayList { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public bool AllowItemEditing { get; set; }

            public IList<string> Warnings { get; set; }

            public int Id { get; set; }

            public string Message { get; set; }

            public int ParentCategoryId { get; set; }

            public DateTime MealDate { get; set; }

            public string MealTime { get; set; }

            public int MealNumber { get; set; }
        }


        public partial class MealPlanModel
        {
            public Guid MealOrderId { get; set; }
            public string MealPlanName { get; set; }
            public int ProductId { get; set; }
            public int MealPlanPrice { get; set; }
            public int MealPlanId { get; set; }
            public string PackageTypeName { get; set; }
            public int PackageTypeId { get; set; }
            public IList<FreshShoppingCartItemModel> Items { get; set; }
        }

        public partial class CheckoutAttributeModel
        {

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public int? SelectedDay { get; set; }

            public int? SelectedMonth { get; set; }

            public int? SelectedYear { get; set; }

            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CheckoutAttributeValueModel> Values { get; set; }

            public string Message { get; set; }

            public int Id { get; set; }
        }

        public partial class CheckoutAttributeValueModel
        {
            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }

            public string Message { get; set; }

            public int Id { get; set; }
        }

        public partial class DiscountBoxModel
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public string CurrentCode { get; set; }
        }

        public partial class GiftCardBoxModel
        {
            public bool Display { get; set; }
            public string Message { get; set; }
        }

        public partial class OrderReviewDataModel
        {
            public OrderReviewDataModel()
            {
                this.BillingAddress = new AddressModel();
                this.ShippingAddress = new AddressModel();
            }
            public bool Display { get; set; }

            public AddressModel BillingAddress { get; set; }

            public bool IsShippable { get; set; }
            public AddressModel ShippingAddress { get; set; }
            public bool SelectedPickUpInStore { get; set; }
            public string ShippingMethod { get; set; }

            public string PaymentMethod { get; set; }
        }

        public partial class PaymentInfoModel
        {
            public string CreditCardType { get; set; }
            public string CardholderName { get; set; }
            public string CardNumber { get; set; }
            public string ExpireMonth { get; set; }
            public string ExpireYear { get; set; }
            public string CardCode { get; set; }
        }

        public partial class TaxRate
        {
            [DataMember]
            public string Rate { get; set; }
            [DataMember]
            public string Value { get; set; }
        }

        #endregion
    }

}
