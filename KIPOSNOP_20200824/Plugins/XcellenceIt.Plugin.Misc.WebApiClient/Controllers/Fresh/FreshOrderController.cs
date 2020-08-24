using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Fresh;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Fresh;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Fresh;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public partial class FreshOrderController : Controller
    {
        #region fields
        private readonly ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        private readonly IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
        private readonly ILanguageService _languageService = EngineContext.Current.Resolve<ILanguageService>();
        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        private readonly IFcartService _fCartService = EngineContext.Current.Resolve<IFcartService>();
        private readonly IFreshPriceCalculationService _freshPriceCalculationService = EngineContext.Current.Resolve<IFreshPriceCalculationService>();
        private readonly IFreshMealPlansService _freshMealPlansService = EngineContext.Current.Resolve<IFreshMealPlansService>();
        private readonly IFreshOrderTotalCalulationService _freshOrderTotalCalulationService = EngineContext.Current.Resolve<IFreshOrderTotalCalulationService>();
        private readonly IFreshOrderService _freshOrderService = EngineContext.Current.Resolve<IFreshOrderService>();
        private readonly IFOrderItemsService _fOrderItemsService = EngineContext.Current.Resolve<IFOrderItemsService>();
        private readonly IOrderService _orderService = EngineContext.Current.Resolve<IOrderService>();
        private readonly IDateTimeHelper _dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
        private readonly OrderSettings _orderSettings = EngineContext.Current.Resolve<OrderSettings>();
        private readonly PdfSettings _pdfSettings = EngineContext.Current.Resolve<PdfSettings>();
        private readonly IPaymentService _paymentService = EngineContext.Current.Resolve<IPaymentService>();
        private readonly TaxSettings _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
        private readonly ICountryService _countryService = EngineContext.Current.Resolve<ICountryService>();
        private readonly ICurrencyService _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
        private readonly IPriceFormatter _priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
        private readonly CatalogSettings _catalogSettings= EngineContext.Current.Resolve<CatalogSettings>();
        private readonly VendorSettings _vendorSettings = EngineContext.Current.Resolve<VendorSettings>();
        private readonly IVendorService _vendorService = EngineContext.Current.Resolve<IVendorService>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        private readonly IUrlRecordService _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
        private readonly IDownloadService _downloadService = EngineContext.Current.Resolve<IDownloadService>();
        private readonly IOrder_Pickup_CustDetailsService _order_Pickup_CustDetailsService = EngineContext.Current.Resolve<IOrder_Pickup_CustDetailsService>();
        #endregion

        #region cTor
        public FreshOrderController()
        {

        }
        #endregion

        #region methods
        [HttpPost]
        public virtual IActionResult GetFOrderDetail([FromBody]FOrderRequest fOrderRequest)
        {
            if (fOrderRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();
            //Set working language
            _workContext.WorkingLanguage = _languageService.GetLanguageById(fOrderRequest.LanguageId);

            if (!ModelState.IsValid)
            {
                //_logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(fOrderRequest.CustomerGUID);
            if (currentCustomer == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoCustomerFound"), fOrderRequest.CustomerGUID)).BadRequest();

            var order = _orderService.GetOrderById(fOrderRequest.OrderId);
            if (order == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.OrderNotFound")).NotFound();

            Order_Pickup_CustDetails _Pickup_CustDetails = new Order_Pickup_CustDetails();
            var orderDetail = PrepareFreshOrderDetailsModel(order);

            if (!!order.PickUpInStore)
            {
                _Pickup_CustDetails = _order_Pickup_CustDetailsService.GetCustomerDetails(order.Id);
                if (_Pickup_CustDetails != null)
                {
                    orderDetail.CustomerEmail = _Pickup_CustDetails.EmailId;
                    orderDetail.CustomerMobileNumber = _Pickup_CustDetails.MobileNumber;
                }
                else
                {
                    orderDetail.CustomerEmail = currentCustomer.Email;
                    orderDetail.CustomerMobileNumber = currentCustomer.MobileNumber;
                }
            }
            else
            {
                orderDetail.CustomerEmail = order.BillingAddress.Email;
                orderDetail.CustomerMobileNumber = order.BillingAddress.PhoneNumber;
            }

            return Ok(orderDetail);
        }


        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        protected virtual FreshOrderDetailResponse PrepareFreshOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            var model = new FreshOrderDetailResponse
            {
                Id = order.Id,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
                IsReturnRequestAllowed = _freshOrderService.IsReturnRequestAllowed(order),
                PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending,
                CustomOrderNumber = order.CustomOrderNumber,
                PaymentMethodType=order.PaymentMethodType,
                //shipping info
                ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                DeliveryDate = order.DeliveryDate,
                PickUpInStore = order.PickUpInStore
            };
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickUpInStore = order.PickUpInStore;
              
                model.ShippingMethod = order.ShippingMethod;

                //shipments (only already shipped)
                var shipments = order.Shipments.Where(x => x.ShippedDateUtc.HasValue).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new FreshOrderDetailResponse.ShipmentBriefModel
                    {
                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };
                    if (shipment.ShippedDateUtc.HasValue)
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                    if (shipment.DeliveryDateUtc.HasValue)
                        shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                    model.Shipments.Add(shipmentModel);
                }
            }

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            model.PaymentMethodStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //tax
            var displayTax = true;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new FreshOrderDetailResponse.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new FreshOrderDetailResponse.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in order.OrderNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new FreshOrderDetailResponse.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = _orderService.FormatOrderNoteText(orderNote),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            //purchased products
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;

            var orderItems = order.OrderItems;

            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? _vendorService.GetVendorsByIds(orderItems.Select(item => item.Product.VendorId).ToArray()) : new List<Vendor>();
            var fOrderItems = _fOrderItemsService.GetFOrderItemsByOrderId(order.Id).GroupBy(x => x.MealOrderId).ToArray();
            model.orderMealPlanModels = new List<FreshOrderDetailResponse.OrderMealPlanModel>();

            foreach (var fOrderItem in fOrderItems)
            {
               
                model.orderMealPlan = new FreshOrderDetailResponse.OrderMealPlanModel();
                model.orderMealPlan.MealOrderId = (Guid)fOrderItem.Key;
                model.orderMealPlan.MealPlanName = "" + fOrderItem.Count().ToString() + "-MealPlan";
                //model.orderMealPlan.ProductId = orderItems[]
                model.orderMealPlan.Items = new List<FreshOrderDetailResponse.OrderItemModel>();
                foreach (var orderItem in orderItems)
                {
                    var tempOrderItem = fOrderItem.Where(x => x.OrderItemId == orderItem.Id).FirstOrDefault();
                    if (tempOrderItem != null)
                    {
                        var orderItemModel = new FreshOrderDetailResponse.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml),
                            ProductId = orderItem.Product.Id,
                            ProductName = _localizationService.GetLocalized(orderItem.Product, x => x.Name),
                            ProductSeName = _urlRecordService.GetSeName(orderItem.Product),
                            AttributeInfo = orderItem.AttributeDescription,
                            ParentCategoryId = orderItem.ParentCategoryId,
                            MealDate = (DateTime)tempOrderItem.MealDate,
                            MealTime = tempOrderItem.MealTime,
                            MealNumber = tempOrderItem.MealNo
                        };
                        //rental info
                        if (orderItem.Product.IsRental)
                        {
                            var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                                ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : "";
                            var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                                ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : "";
                        }
                        model.orderMealPlan.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                        }
                    }

                }
                if (model.orderMealPlan.Items.Count != 0)
                {
                    model.orderMealPlanModels.Add(model.orderMealPlan);
                }
                model.orderMealPlan = null;
            }
            return model;
        }

        #endregion
    }

    public class packageTypeResponse
    {
       public int PackageTypeId { get; set; }
    }
}
