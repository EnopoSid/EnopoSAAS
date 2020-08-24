using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Html;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute formatter
    /// </summary>
    public partial class ProductAttributeFormatter : IProductAttributeFormatter
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public ProductAttributeFormatter(ICurrencyService currencyService,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._currencyService = currencyService;
            this._downloadService = downloadService;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(Product product, string attributesXml)
        {
            var customer = _workContext.CurrentCustomer;
            FormatAttributesInArrayFormat(product, attributesXml, customer);
            return FormatAttributes(product, attributesXml, customer);
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customer">Customer</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(Product product, string attributesXml,
            Customer customer, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();
            string strProd = "", strProd1 = "", strProd2 = "";
            int prodCount = 0;
            decimal prodPrice = 0;
            //attributes
            if (renderProductAttributes)
            {
                foreach (var attribute in _productAttributeParser.ParseProductAttributeMappings(attributesXml))
                {
                    //attributes without values
                    if (!attribute.ShouldHaveValues())
                    {
                        foreach (var value in _productAttributeParser.ParseValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Empty;
                            if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //multiline textbox
                                var attributeName = _localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id);

                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);

                                //we never encode multiline textbox input
                                formattedAttribute = $"{attributeName}: {HtmlHelper.FormatText(value, false, true, false, false, false, false)}";
                            }
                            else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                Guid.TryParse(value, out var downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                {
                                    var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";

                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = WebUtility.HtmlEncode(fileName);

                                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                    var attributeText = allowHyperlinks ? $"<a href=\"{_webHelper.GetStoreLocation(false)}download/getfileupload/?downloadId={download.DownloadGuid}\" class=\"fileuploadattribute\">{fileName}</a>"
                                        : fileName;

                                    var attributeName = _localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id);

                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = WebUtility.HtmlEncode(attributeName);

                                    formattedAttribute = $"{attributeName}: {attributeText}";
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                formattedAttribute = $"{_localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id)}: {value}";

                                //encode (if required)
                                if (htmlEncode)
                                    formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                            }

                            if (string.IsNullOrEmpty(formattedAttribute)) 
                                continue;

                            if (result.Length > 0)
                                result.Append(separator);
                            result.Append(formattedAttribute);
                        }
                    }
                    //product attribute values
                    else

                    {
                        result.Append(separator);
                        result.Append($"{_localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id)}:");
                        foreach (var attributeValue in _productAttributeParser.ParseProductAttributeValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = $"{_localizationService.GetLocalized(attributeValue, a => a.Name, _workContext.WorkingLanguage.Id)}";

                            prodPrice = attributeValue.PriceAdjustment;
                            if (strProd.Contains(attributeValue.Name))
                            {
                                if (prodPrice > 0)
                                {
                                    prodPrice = attributeValue.PriceAdjustment * prodCount;
                                    strProd1 = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "] (" + prodCount + ")";
                                    prodCount++;
                                    prodPrice = attributeValue.PriceAdjustment * prodCount;
                                    strProd = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "] (" + prodCount + ")";
                                }
                                else
                                {
                                    strProd1 = attributeValue.Name + " (" + prodCount + ")";
                                    prodCount++;
                                    strProd = attributeValue.Name + " (" + prodCount + ")";
                                }
                            }
                            else
                            {
                                strProd = "";
                                prodCount = 1;
                                if (prodPrice > 0)
                                {
                                    strProd = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "] (" + prodCount + ")";
                                    strProd1 = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "]";
                                }
                                else
                                {
                                    strProd = attributeValue.Name + " (" + prodCount + ")";
                                    strProd1 = attributeValue.Name;
                                }
                                strProd2 = strProd1;
                            }
                            
                            if (renderPrices)
                            {
                                if (attributeValue.PriceAdjustmentUsePercentage)
                                {
                                    var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                                    if (attributeValue.PriceAdjustment > decimal.Zero)
                                        formattedAttribute += $" [+{priceAdjustmentStr}%]";
                                    else if (attributeValue.PriceAdjustment < decimal.Zero)
                                        formattedAttribute += $" [{priceAdjustmentStr}%]";
                                }
                                else
                                {
                                    var attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue, customer);
                                    var priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, customer, out var _);
                                    var priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                                    if (priceAdjustmentBase > decimal.Zero)
                                        formattedAttribute += $" [+{_priceFormatter.FormatPrice(priceAdjustment, false, false)}]";
                                    else if (priceAdjustmentBase < decimal.Zero)
                                        formattedAttribute += $" [-{_priceFormatter.FormatPrice(-priceAdjustment, false, false)}]";
                                }
                            }

                            //display quantity
                            if (_shoppingCartSettings.RenderAssociatedAttributeValueQuantity && attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                            {
                                //render only when more than 1
                                if (attributeValue.Quantity > 1)
                                    formattedAttribute += string.Format(_localizationService.GetResource("ProductAttributes.Quantity"), attributeValue.Quantity);
                            }

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);

                            if (string.IsNullOrEmpty(formattedAttribute)) 
                                continue;

                            if (result.Length > 0)
                            {
                                if (prodCount == 1)
                                {
                                    result.Append(separator);
                                    result.Append(formattedAttribute);
                                }
                            }

                            if (strProd.Contains(attributeValue.Name))
                            {
                                result.Replace(strProd1, strProd);
                            }
                        }
                    }
                }
            }

            //gift cards
            if (!renderGiftCardAttributes) 
                return result.ToString();

            if (!product.IsGiftCard) 
                return result.ToString();

            _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

            //sender
            var giftCardFrom = product.GiftCardType == GiftCardType.Virtual ?
                string.Format(_localizationService.GetResource("GiftCardAttribute.From.Virtual"), giftCardSenderName, giftCardSenderEmail) :
                string.Format(_localizationService.GetResource("GiftCardAttribute.From.Physical"), giftCardSenderName);
            //recipient
            var giftCardFor = product.GiftCardType == GiftCardType.Virtual ?
                string.Format(_localizationService.GetResource("GiftCardAttribute.For.Virtual"), giftCardRecipientName, giftCardRecipientEmail) :
                string.Format(_localizationService.GetResource("GiftCardAttribute.For.Physical"), giftCardRecipientName);

            //encode (if required)
            if (htmlEncode)
            {
                giftCardFrom = WebUtility.HtmlEncode(giftCardFrom);
                giftCardFor = WebUtility.HtmlEncode(giftCardFor);
            }

            if (!string.IsNullOrEmpty(result.ToString()))
            {
                result.Append(separator);
            }

            result.Append(giftCardFrom);
            result.Append(separator);
            result.Append(giftCardFor);

            return result.ToString();
        }

        // Updated by Phani for changing bowl ingredients for Admin on 24-09-2019 start 

        /// <summary>
        /// Formats attributes Email
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customer">Customer</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributesEmail(Product product, string attributesXml,
            Customer customer, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();
            string strProd = "", strProd1 = "", strProd2 = "";
            int prodCount = 0;
            decimal prodPrice = 0;
            int ingCount = 1;
            //attributes
            if (renderProductAttributes)
            {
                foreach (var attribute in _productAttributeParser.ParseProductAttributeMappings(attributesXml))
                {
                    //attributes without values
                    if (!attribute.ShouldHaveValues())
                    {
                        foreach (var value in _productAttributeParser.ParseValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Empty;
                            if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //multiline textbox
                                var attributeName = _localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id);

                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);

                                //we never encode multiline textbox input
                                formattedAttribute = $"{attributeName}: {HtmlHelper.FormatText(value, false, true, false, false, false, false)}";
                            }
                            else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                Guid.TryParse(value, out var downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                {
                                    var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";

                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = WebUtility.HtmlEncode(fileName);

                                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                    var attributeText = allowHyperlinks ? $"<a href=\"{_webHelper.GetStoreLocation(false)}download/getfileupload/?downloadId={download.DownloadGuid}\" class=\"fileuploadattribute\">{fileName}</a>"
                                        : fileName;

                                    var attributeName = _localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id);

                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = WebUtility.HtmlEncode(attributeName);

                                    formattedAttribute = $"{attributeName}: {attributeText}";
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                formattedAttribute = $"{_localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id)}: {value}";

                                //encode (if required)
                                if (htmlEncode)
                                    formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                            }

                            if (string.IsNullOrEmpty(formattedAttribute))
                                continue;

                            if (result.Length > 0)
                                result.Append(separator);
                            result.Append(formattedAttribute);
                        }
                    }
                    //product attribute values
                    else

                    {
                        result.Append("  <h2 style='margin-bottom:3px;'>" + Convert.ToString(ingCount) + "  <b>");
                        result.Append($"{_localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id)}:");
                        result.Append("  </h2></b>");
                        foreach (var attributeValue in _productAttributeParser.ParseProductAttributeValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = $"{_localizationService.GetLocalized(attributeValue, a => a.Name, _workContext.WorkingLanguage.Id)}";

                            prodPrice = attributeValue.PriceAdjustment;
                            if (strProd.Contains(attributeValue.Name))
                            {
                                if (prodPrice > 0)
                                {
                                    prodPrice = attributeValue.PriceAdjustment * prodCount;
                                    strProd1 = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "] (" + prodCount + ")";
                                    prodCount++;
                                    prodPrice = attributeValue.PriceAdjustment * prodCount;
                                    strProd = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "] (" + prodCount + ")";
                                }
                                else
                                {
                                    strProd1 = attributeValue.Name + " (" + prodCount + ")";
                                    prodCount++;
                                    strProd = attributeValue.Name + " (" + prodCount + ")";
                                }
                            }
                            else
                            {
                                strProd = "";
                                prodCount = 1;
                                if (prodPrice > 0)
                                {
                                    strProd = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "] (" + prodCount + ")";
                                    strProd1 = attributeValue.Name + " [+$" + prodPrice.ToString("0.00") + "]";
                                }
                                else
                                {
                                    strProd = attributeValue.Name + " (" + prodCount + ")";
                                    strProd1 = attributeValue.Name;
                                }
                                strProd2 = strProd1;
                            }

                            if (renderPrices)
                            {
                                if (attributeValue.PriceAdjustmentUsePercentage)
                                {
                                    var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                                    if (attributeValue.PriceAdjustment > decimal.Zero)
                                        formattedAttribute += $" [+{priceAdjustmentStr}%]";
                                    else if (attributeValue.PriceAdjustment < decimal.Zero)
                                        formattedAttribute += $" [{priceAdjustmentStr}%]";
                                }
                                else
                                {
                                    var attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue, customer);
                                    var priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, customer, out var _);
                                    var priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                                    if (priceAdjustmentBase > decimal.Zero)
                                        formattedAttribute += $" [+{_priceFormatter.FormatPrice(priceAdjustment, false, false)}]";
                                    else if (priceAdjustmentBase < decimal.Zero)
                                        formattedAttribute += $" [-{_priceFormatter.FormatPrice(-priceAdjustment, false, false)}]";
                                }
                            }

                            //display quantity
                            if (_shoppingCartSettings.RenderAssociatedAttributeValueQuantity && attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                            {
                                //render only when more than 1
                                if (attributeValue.Quantity > 1)
                                    formattedAttribute += string.Format(_localizationService.GetResource("ProductAttributes.Quantity"), attributeValue.Quantity);
                            }

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);

                            if (string.IsNullOrEmpty(formattedAttribute))
                                continue;

                            if (result.Length > 0)
                            {
                                if (prodCount == 1)
                                {
                                    result.Append(" <p style='margin-top:0px; margin-bottom:5px; text-indent:15px;'> ");
                                    result.Append(formattedAttribute);
                                    result.Append(" </p> ");
                                }
                            }

                            if (strProd.Contains(attributeValue.Name))
                            {
                                result.Replace(strProd1, strProd);
                            }

                        }
                        ingCount++;
                    }
                }
            }

            //gift cards
            if (!renderGiftCardAttributes)
                return result.ToString();

            if (!product.IsGiftCard)
                return result.ToString();

            _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

            //sender
            var giftCardFrom = product.GiftCardType == GiftCardType.Virtual ?
                string.Format(_localizationService.GetResource("GiftCardAttribute.From.Virtual"), giftCardSenderName, giftCardSenderEmail) :
                string.Format(_localizationService.GetResource("GiftCardAttribute.From.Physical"), giftCardSenderName);
            //recipient
            var giftCardFor = product.GiftCardType == GiftCardType.Virtual ?
                string.Format(_localizationService.GetResource("GiftCardAttribute.For.Virtual"), giftCardRecipientName, giftCardRecipientEmail) :
                string.Format(_localizationService.GetResource("GiftCardAttribute.For.Physical"), giftCardRecipientName);

            //encode (if required)
            if (htmlEncode)
            {
                giftCardFrom = WebUtility.HtmlEncode(giftCardFrom);
                giftCardFor = WebUtility.HtmlEncode(giftCardFor);
            }

            if (!string.IsNullOrEmpty(result.ToString()))
            {
                result.Append(separator);
            }

            result.Append(giftCardFrom);
            result.Append(separator);
            result.Append(giftCardFor);

            return result.ToString();
        }
        // Updated by Phani for changing bowl ingredients for Admin on 24-09-2019 end 


        public virtual List<string> FormatAttributesInArrayFormat(Product product, string attributesXml)
        {
            var customer = _workContext.CurrentCustomer;
            return FormatAttributesInArrayFormat(product, attributesXml, customer);
        }

        public virtual List<string> FormatAttributesInArrayFormat(Product product, string attributesXml,
           Customer customer, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();
            List<string> productAttributeResults = new List<string>();

            //attributes
            if (renderProductAttributes)
            {
                foreach (var attribute in _productAttributeParser.ParseProductAttributeMappings(attributesXml))
                {
                    //attributes without values
                    if (!attribute.ShouldHaveValues())
                    {
                        foreach (var value in _productAttributeParser.ParseValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Empty;
                            if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //multiline textbox
                                var attributeName = _localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id);

                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);

                                //we never encode multiline textbox input
                                formattedAttribute = $"{attributeName}: {HtmlHelper.FormatText(value, false, true, false, false, false, false)}";
                            }
                            else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                Guid.TryParse(value, out var downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                {
                                    var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";

                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = WebUtility.HtmlEncode(fileName);

                                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                    var attributeText = allowHyperlinks ? $"<a href=\"{_webHelper.GetStoreLocation(false)}download/getfileupload/?downloadId={download.DownloadGuid}\" class=\"fileuploadattribute\">{fileName}</a>"
                                        : fileName;

                                    var attributeName = _localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id);

                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = WebUtility.HtmlEncode(attributeName);

                                    formattedAttribute = $"{attributeName}: {attributeText}";
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                formattedAttribute = $"{_localizationService.GetLocalized(attribute.ProductAttribute, a => a.Name, _workContext.WorkingLanguage.Id)}: {value}";

                                //encode (if required)
                                if (htmlEncode)
                                    formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                            }

                            if (string.IsNullOrEmpty(formattedAttribute))
                                continue;

                            if (result.Length > 0)
                                result.Append(separator);
                            result.Append(formattedAttribute);
                        }
                    }
                    //product attribute values
                    else
                    {
                        foreach (var attributeValue in _productAttributeParser.ParseProductAttributeValues(attributesXml, attribute.Id))
                        {
                            string productAttribute =
                                "product_attribute_"+attribute.ProductId+"_"+attribute.ProductAttributeId+"_"+attribute.Id+"_"+attributeValue.Id;
                            productAttributeResults.Add(productAttribute);
                            //"product_attribute_attribute.productid_attribute.productatrributeid_attribute.id_attributevalue.id"
                        }
                    }
                }
            }

            return productAttributeResults;
        }

        #endregion
    }
}