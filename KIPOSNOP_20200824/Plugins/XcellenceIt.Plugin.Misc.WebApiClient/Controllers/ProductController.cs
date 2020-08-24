using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using System.Reflection;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class ProductController : Controller
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IOrderReportService _orderReportService;
        private readonly IAclService _aclService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly SeoSettings _seoSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IStoreContext _storeContext;
        private readonly IDateRangeService _dateRangeService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public ProductController(
        CustomerSettings customerSettings,
        ICustomerService customerService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IGenericAttributeService genericAttributeService,
        IDateTimeHelper dateTimeHelper,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        IOrderService orderService,
        IPermissionService permissionService,
        IProductService productService,
        IStoreMappingService storeMappingService,
        ICurrencyService currencyService,
        IPriceFormatter priceFormatter,
        CatalogSettings catalogSettings,
        IProductAttributeParser productAttributeParser,
        IDownloadService downloadService,
        ICacheManager cacheManager,
        IOrderReportService orderReportService,
        IAclService aclService,
        IPriceCalculationService priceCalculationService,
        ITaxService taxService,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IWebHelper webHelper,
        ShoppingCartSettings shoppingCartSettings,
        SeoSettings seoSettings,
        VendorSettings vendorSettings,
        IVendorService vendorService,
        ICategoryService categoryService,
        IProductTagService productTagService,
        IProductTemplateService productTemplateService,
        IProductAttributeService productAttributeService,
        IManufacturerService manufacturerService,
        IRecentlyViewedProductsService recentlyViewedProductsService,
        ISpecificationAttributeService specificationAttributeService,
        IEventPublisher eventPublisher,
        ILogger logger,
        CaptchaSettings captchaSettings,
        IStoreContext storeContext,
        IDateRangeService dateRangeService,
        IUrlRecordService urlRecordService
        )
        {
            this._customerSettings = customerSettings;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._dateTimeHelper = dateTimeHelper;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._orderService = orderService;
            this._permissionService = permissionService;
            this._productService = productService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._catalogSettings = catalogSettings;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._orderReportService = orderReportService;
            this._aclService = aclService;
            this._priceCalculationService = priceCalculationService;
            this._mediaSettings = mediaSettings;
            this._webHelper = webHelper;
            this._pictureService = pictureService;
            this._taxService = taxService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._seoSettings = seoSettings;
            this._vendorSettings = vendorSettings;
            this._vendorService = vendorService;
            this._categoryService = categoryService;
            this._productTagService = productTagService;
            this._productTemplateService = productTemplateService;
            this._productAttributeService = productAttributeService;
            this._manufacturerService = manufacturerService;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            this._specificationAttributeService = specificationAttributeService;
            this._logger = logger;
            this._captchaSettings = captchaSettings;
            this._storeContext = storeContext;
            this._dateRangeService = dateRangeService;
            this._eventPublisher = eventPublisher;
            this._urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities 

        [NonAction]
        public IList<ProductListingResponse> PrepareProductOverviewModels(int storeId, int currencyId, int languageId, Customer currentCustomer,
        IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true,
        int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);

            if (products == null)
                return null;

            var models = new List<ProductListingResponse>();

            foreach (var product in products)
            {
                var model = new ProductListingResponse
                {
                    Id = product.Id,
                    Name = _localizationService.GetLocalized(product, x => x.Name, languageId: languageId),
                    ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, languageId: languageId),
                    FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription, languageId: languageId),
                    SeName = _urlRecordService.GetSeName(product, languageId: languageId),
                };

                //CustomerId
                model.CustomerGuid = currentCustomer.CustomerGuid;

                //price
                if (preparePriceModel)
                {
                    #region Prepare product price

                    var priceModel = new ProductListingResponse.ProductPriceModel
                    {
                        ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
                    };

                    switch (product.ProductType)
                    {
                        case ProductType.GroupedProduct:
                            {
                                #region Grouped product

                                var associatedProducts = _productService.GetAssociatedProducts(product.Id, storeId);

                                switch (associatedProducts.Count)
                                {
                                    case 0:
                                        {
                                            //no associated products
                                            priceModel.OldPrice = null;
                                            priceModel.Price = null;
                                            priceModel.DisableBuyButton = true;
                                            priceModel.DisableWishlistButton = true;
                                            priceModel.AvailableForPreOrder = false;
                                        }
                                        break;
                                    default:
                                        {
                                            //we have at least one associated product
                                            priceModel.DisableBuyButton = true;
                                            priceModel.DisableWishlistButton = true;
                                            priceModel.AvailableForPreOrder = false;

                                            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
                                            {
                                                //find a minimum possible price
                                                decimal? minPossiblePrice = null;
                                                Product minPriceProduct = null;
                                                foreach (var associatedProduct in associatedProducts)
                                                {
                                                    //calculate for the maximum quantity (in case if we have tier prices)
                                                    var tmpPrice = _priceCalculationService.GetFinalPrice(associatedProduct,
                                                        currentCustomer, decimal.Zero, true, int.MaxValue);
                                                    if (!minPossiblePrice.HasValue || tmpPrice < minPossiblePrice.Value)
                                                    {
                                                        minPriceProduct = associatedProduct;
                                                        minPossiblePrice = tmpPrice;
                                                    }
                                                }
                                                if (minPriceProduct != null && !minPriceProduct.CustomerEntersPrice)
                                                {
                                                    if (minPriceProduct.CallForPrice)
                                                    {
                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                                    }
                                                    else if (minPossiblePrice.HasValue)
                                                    {
                                                        //calculate prices
                                                        decimal finalPriceBase = _taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out decimal taxRate);
                                                        decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workingCurrency);

                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = string.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));

                                                    }
                                                    else
                                                    {
                                                        //Actually it's not possible (we presume that minimalPrice always has a value)
                                                        //We never should get here
                                                        Debug.WriteLine("Cannot calculate minPrice for product #{0}", product.Id);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //hide prices
                                                priceModel.OldPrice = null;
                                                priceModel.Price = null;
                                            }
                                        }
                                        break;
                                }

                                #endregion
                            }
                            break;
                        case ProductType.SimpleProduct:
                        default:
                            {
                                #region Simple product

                                //add to cart button
                                priceModel.DisableBuyButton = product.DisableBuyButton ||
                                    !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart.SystemName, currentCustomer) ||
                                    !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer);

                                //add to wishlist button
                                priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                    !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist.SystemName, currentCustomer) ||
                                    !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer);

                                //rental
                                priceModel.IsRental = product.IsRental;

                                //pre-order
                                if (product.AvailableForPreOrder)
                                {
                                    priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                        product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                                    priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
                                }

                                //prices
                                if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
                                {
                                    //calculate for the maximum quantity (in case if we have tier prices)
                                    decimal minPossiblePrice = _priceCalculationService.GetFinalPrice(product,
                                        currentCustomer, decimal.Zero, true, int.MaxValue);
                                    if (!product.CustomerEntersPrice)
                                    {
                                        if (product.CallForPrice)
                                        {
                                            //call for price
                                            priceModel.OldPrice = null;
                                            priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                        }
                                        else
                                        {
                                            //calculate prices
                                            decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out decimal taxRate);
                                            decimal finalPriceBase = _taxService.GetProductPrice(product, minPossiblePrice, out taxRate);

                                            decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, workingCurrency);
                                            decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, workingCurrency);

                                            //do we have tier prices configured?
                                            var tierPrices = new List<TierPrice>();
                                            if (product.HasTierPrices)
                                            {
                                                tierPrices.AddRange(product.TierPrices
                                                    .OrderBy(tp => tp.Quantity)
                                                    .ToList()
                                                    .FilterByStore(storeId)
                                                    .FilterForCustomer(currentCustomer)
                                                    .RemoveDuplicatedQuantities());
                                            }
                                            //When there is just one tier (with  qty 1), 
                                            //there are no actual savings in the list.
                                            bool displayFromMessage = tierPrices.Count > 0 &&
                                                !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                                            if (displayFromMessage)
                                            {
                                                priceModel.OldPrice = null;
                                                priceModel.Price = string.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                            }
                                            else
                                            {
                                                if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                                {
                                                    priceModel.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                                                    priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                                }
                                                else
                                                {
                                                    priceModel.OldPrice = null;
                                                    priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                                }
                                            }
                                            if (product.IsRental)
                                            {
                                                //rental product
                                                priceModel.OldPrice = _priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
                                                priceModel.Price = _priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
                                            }


                                            //property for German market
                                            //we display tax/shipping info only with "shipping enabled" for this product
                                            //we also ensure this it's not free shipping
                                            priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes
                                                && product.IsShipEnabled &&
                                                !product.IsFreeShipping;
                                        }
                                    }
                                }
                                else
                                {
                                    //hide prices
                                    priceModel.OldPrice = null;
                                    priceModel.Price = null;
                                }

                                #endregion
                            }
                            break;
                    }
                    model.ProductPrice = new ProductListingResponse.ProductPriceModel();
                    model.ProductPrice = priceModel;

                    #endregion
                }

                //picture
                if (preparePictureModel)
                {
                    #region Prepare product picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;
                    //prepare picture model
                    var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.Id, pictureSize, true, languageId, _webHelper.IsCurrentConnectionSecured(), storeId);
                    model.DefaultPictureModel = new PictureModel();
                    model.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name)
                        };
                        return pictureModel;
                    });

                    #endregion
                }

                //reviews
                model.ReviewOverviewModel = new ProductReviewOverviewModel();
                {
                    model.ReviewOverviewModel.ProductId = product.Id;
                    model.ReviewOverviewModel.RatingSum = product.ApprovedRatingSum;
                    model.ReviewOverviewModel.TotalReviews = product.ApprovedTotalReviews;
                    model.ReviewOverviewModel.AllowCustomerReviews = product.AllowCustomerReviews;
                };

                models.Add(model);
            }
            return models;
        }

        [NonAction]
        protected virtual ProductDetailsResponse PrepareProductDetailsPageModel(Product product, Customer currentCustomer,
           int languageId, int storeId, int currencyId, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);

            if (product == null)
                return null;

            var customerRolesIds = currentCustomer.CustomerRoles
                .Where(cr => cr.Active)
                .Select(cr => cr.Id)
                .ToList();

            #region Standard properties

            var model = new ProductDetailsResponse
            {
                Id = product.Id,
                Name = _localizationService.GetLocalized(product, x => x.Name, languageId: languageId),
                ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, languageId: languageId),
                FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription, languageId: languageId),
                MetaKeywords = _localizationService.GetLocalized(product, x => x.MetaKeywords, languageId: languageId),
                MetaDescription = _localizationService.GetLocalized(product, x => x.MetaDescription, languageId: languageId),
                MetaTitle = _localizationService.GetLocalized(product, x => x.MetaTitle, languageId: languageId),
                SeName = _urlRecordService.GetSeName(product, languageId: languageId),
                ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
                Sku = _localizationService.GetLocalized(product, x => x.Sku, languageId: languageId),
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = _localizationService.GetLocalized(product, x => x.ManufacturerPartNumber, languageId: languageId),
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = _localizationService.GetLocalized(product, x => x.Gtin, languageId: languageId),
                StockAvailability = _productService.FormatStockMessage(product, ""),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
            };

            //automatically generate product description?
            if (_seoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = _dateRangeService.GetDeliveryDateById(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = _localizationService.GetLocalized(deliveryDate, x => x.Name, languageId: languageId);
                }
            }

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;
            //store name
            model.CurrentStoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name, languageId: languageId);

            #endregion

            #region Vendor details

            //vendor
            if (_vendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = _vendorService.GetVendorById(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = _localizationService.GetLocalized(vendor, x => x.Name, languageId: languageId),
                        SeName = _urlRecordService.GetSeName(vendor, languageId: languageId)
                    };
                }
            }

            #endregion

            #region Page sharing

            if (_catalogSettings.ShowShareButton && !string.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }
                model.PageShareCode = shareCode;
            }

            #endregion

            #region Back in stock subscriptions

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                model.DisplayBackInStockSubscription = true;
            }

            #endregion

            #region Breadcrumb

            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY, product.Id, languageId, string.Join(",", customerRolesIds), storeId);
                model.Breadcrumb = new ProductDetailsResponse.ProductBreadcrumbModel();
                model.Breadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                {
                    var breadcrumbModel = new ProductDetailsResponse.ProductBreadcrumbModel
                    {
                        Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                        ProductId = product.Id,
                        ProductName = _localizationService.GetLocalized(product, x => x.Name, languageId: languageId),
                        ProductSeName = _urlRecordService.GetSeName(product, languageId: languageId)
                    };
                    var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                    if (productCategories.Count > 0)
                    {
                        var category = productCategories[0].Category;
                        if (category != null)
                        {
                            breadcrumbModel.CategoryBreadcrumb = new List<CategorySimpleModel>();
                            foreach (var catBr in _categoryService.GetCategoryBreadCrumb(category))
                            {
                                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                                {
                                    Id = catBr.Id,
                                    Name = _localizationService.GetLocalized(catBr, x => x.Name, languageId: languageId),
                                    SeName = _urlRecordService.GetSeName(catBr, languageId: languageId),
                                });
                            }
                        }
                    }
                    return breadcrumbModel;
                });
            }

            #endregion

            #region Product tags

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                var productTagsCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_BY_PRODUCT_MODEL_KEY, product.Id, languageId, storeId);
                model.ProductTags = new List<ProductTagModel>();
                model.ProductTags = _cacheManager.Get(productTagsCacheKey, () =>
                _productTagService.GetAllProductTagsByProductId(product.Id)
                    //filter by store
                    .Where(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                    .Select(x => new ProductTagModel
                    {
                        Id = x.Id,
                        Name = _localizationService.GetLocalized(x, y => y.Name),
                        SeName = _urlRecordService.GetSeName(x),
                        ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                    })
                    .ToList());
            }

            #endregion

            #region Templates

            var templateCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_TEMPLATE_MODEL_KEY, product.ProductTemplateId);
            model.ProductTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId);
                if (template == null)
                    template = _productTemplateService.GetAllProductTemplates().FirstOrDefault();
                if (template == null)
                    return null;
                return template.ViewPath;
            });

            #endregion

            #region Pictures

            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            //default picture
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;
            //prepare picture models
            var productPicturesCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DETAILS_PICTURES_MODEL_KEY, product.Id, defaultPictureSize, isAssociatedProduct, languageId, _webHelper.IsCurrentConnectionSecured(), storeId);
            var cachedPictures = _cacheManager.Get(productPicturesCacheKey, () =>
            {
                var pictures = _pictureService.GetPicturesByProductId(product.Id);

                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(pictures.FirstOrDefault(), defaultPictureSize, !isAssociatedProduct),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(pictures.FirstOrDefault(), 0, !isAssociatedProduct),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), model.Name)
                };
                //all pictures
                var pictureModels = new List<PictureModel>();
                foreach (var picture in pictures)
                {
                    pictureModels.Add(new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                        ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), model.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), model.Name),
                    });
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });
            model.DefaultPictureModel = cachedPictures.DefaultPictureModel;
            model.PictureModels = cachedPictures.PictureModels;

            #endregion

            #region Product price

            model.ProductPrice = new ProductDetailsResponse.ProductPriceModel
            {
                ProductId = product.Id
            };
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
            {
                model.ProductPrice.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.ProductPrice.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice)
                    {
                        model.ProductPrice.CallForPrice = true;
                    }
                    else
                    {
                        decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out decimal taxRate);
                        decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, currentCustomer, includeDiscounts: false), out taxRate);
                        decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, currentCustomer, includeDiscounts: true), out taxRate);

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _currencyService.GetCurrencyById(currencyId));
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _currencyService.GetCurrencyById(currencyId));
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _currencyService.GetCurrencyById(currencyId));

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.ProductPrice.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.ProductPrice.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.ProductPrice.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.ProductPrice.PriceValue = finalPriceWithoutDiscount;

                        if (finalPriceWithDiscount != 0)
                            model.ProductPrice.PriceWithDiscountValue = finalPriceWithDiscount;
                        else
                            model.ProductPrice.PriceWithDiscountValue = finalPriceWithoutDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.ProductPrice.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //currency code
                        model.ProductPrice.CurrencyCode = _currencyService.GetCurrencyById(currencyId).CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.ProductPrice.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.ProductPrice.RentalPrice = _priceFormatter.FormatRentalProductPeriod(product, priceStr);
                        }
                    }
                }
            }
            else
            {
                model.ProductPrice.HidePrices = true;
                model.ProductPrice.OldPrice = null;
                model.ProductPrice.Price = null;
            }
            #endregion

            #region 'Add to cart' model
            model.AddToCart = new ProductDetailsResponse.AddToCartModel
            {
                ProductId = product.Id,
                UpdatedShoppingCartItemId = updatecartitem != null ? updatecartitem.Id : 0,

                //quantity
                EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity
            };

            //minimum quantity notification
            if (product.OrderMinimumQuantity > 1)
            {
                model.AddToCart.MinimumQuantityNotification = string.Format(_localizationService.GetResource("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
            }

            //'add to cart', 'add to wishlist' buttons
            model.AddToCart.DisableBuyButton = product.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart.SystemName, currentCustomer);
            model.AddToCart.DisableWishlistButton = product.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist.SystemName, currentCustomer);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
            {
                model.AddToCart.DisableBuyButton = true;
                model.AddToCart.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AddToCart.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.AddToCart.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }
            //rental
            model.AddToCart.IsRental = product.IsRental;

            //customer entered price
            model.AddToCart.CustomerEntersPrice = product.CustomerEntersPrice;
            if (model.AddToCart.CustomerEntersPrice)
            {
                decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _currencyService.GetCurrencyById(currencyId));
                decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _currencyService.GetCurrencyById(currencyId));

                model.AddToCart.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
                model.AddToCart.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Products.EnterProductPrice.Range"),
                    _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                    _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));
            }
            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            model.AddToCart.AllowedQuantities = new List<SelectListItem>();

            foreach (var qty in allowedQuantities)
            {
                model.AddToCart.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }

            #endregion

            #region Gift card
            model.GiftCard = new ProductDetailsResponse.GiftCardModel
            {
                IsGiftCard = product.IsGiftCard
            };
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = _customerService.GetCustomerFullName(currentCustomer);
                    model.GiftCard.SenderEmail = currentCustomer.Email;
                }
                else
                {
                    _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out string giftCardRecipientName, out string giftCardRecipientEmail,
                        out string giftCardSenderName, out string giftCardSenderEmail, out string giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            #endregion

            #region Product attributes

            //performance optimization
            //We cache a value indicating whether a product has attributes
            IList<ProductAttributeMapping> productAttributeMapping = null;
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY, product.Id);
            var hasProductAttributesCache = _cacheManager.Get(cacheKey, () =>
            {
                //no value in the cache yet
                //let's load attributes and cache the result (true/false)
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                return productAttributeMapping.Any();
            });
            if (hasProductAttributesCache && productAttributeMapping == null)
            {
                //cache indicates that the product has attributes
                //let's load them
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            }
            if (productAttributeMapping == null)
            {
                productAttributeMapping = new List<ProductAttributeMapping>();
            }
            model.ProductAttributes = new List<ProductDetailsResponse.ProductAttributeModel>();
            foreach (var attribute in productAttributeMapping)
            {
                var attributeModel = new ProductDetailsResponse.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = _localizationService.GetLocalized(attribute.ProductAttribute, x => x.Name, languageId: languageId),
                    Description = _localizationService.GetLocalized(attribute.ProductAttribute, x => x.Description, languageId: languageId),
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = new List<string>();
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    attributeModel.Values = new List<ProductDetailsResponse.ProductAttributeValueModel>();
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsResponse.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            ProductAttributeMappingId=attributeValue.ProductAttributeMappingId,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name, languageId: languageId),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected,
                            WeightAdjustment = attributeValue.WeightAdjustment
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
                        {
                            decimal attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue, updatecartitem?.Customer ?? currentCustomer);
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, out decimal taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _currencyService.GetCurrencyById(currencyId));
                            if (priceAdjustmentBase > decimal.Zero)
                                valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //picture
                        var valuePicture = _pictureService.GetPictureById(attributeValue.PictureId);
                        if (valuePicture != null)
                        {
                            valueModel.PictureUrl = _pictureService.GetPictureUrl(valuePicture, defaultPictureSize);
                            valueModel.FullSizePictureUrl = _pictureService.GetPictureUrl(valuePicture);
                            valueModel.PictureId = valuePicture.Id;
                        }
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Count > 0)
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Count > 0)
                                {
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                           DateTimeStyles.None, out DateTime selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }

                            }
                            break;
                        case AttributeControlType.FileUpload:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var downloadGuidStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
                                    Guid.TryParse(downloadGuidStr, out Guid downloadGuid);
                                    var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                    if (download != null)
                                        attributeModel.DefaultValue = download.DownloadGuid.ToString();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                model.ProductAttributes.Add(attributeModel);
            }

            #endregion

            #region Product specifications

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                model.ProductSpecifications = new List<ProductSpecificationModel>();
                model.ProductSpecifications = PrepareProductSpecificationModel(languageId, product);
            }

            #endregion

            #region Product review overview
            model.ProductReviewOverview = new ProductReviewOverviewModel
            {
                ProductId = product.Id,
                RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews,
                AllowCustomerReviews = product.AllowCustomerReviews
            };

            #endregion

            #region Tier prices

            if (product.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices.SystemName, currentCustomer))
            {
                model.TierPrices = new List<ProductDetailsResponse.TierPriceModel>();
                model.TierPrices = product.TierPrices
                    .OrderBy(x => x.Quantity)
                    .ToList()
                    .FilterByStore(storeId)
                    .FilterForCustomer(currentCustomer)
                    .RemoveDuplicatedQuantities()
                    .Select(tierPrice =>
                    {
                        var m = new ProductDetailsResponse.TierPriceModel
                        {
                            Quantity = tierPrice.Quantity,
                        };
                        decimal priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, currentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out decimal taxRate);
                        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _currencyService.GetCurrencyById(currencyId));
                        m.Price = _priceFormatter.FormatPrice(price, false, false);
                        return m;
                    })
                    .ToList();
            }

            #endregion

            #region Manufacturers

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                string manufacturersCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_MANUFACTURERS_MODEL_KEY, product.Id, languageId, string.Join(",", customerRolesIds), storeId);
                model.ProductManufacturers = new List<ProductDetailsResponse.ManufacturerModel>();
                var manufacturerProducts = _cacheManager.Get(manufacturersCacheKey, () =>
                    _manufacturerService.GetProductManufacturersByProductId(product.Id).ToList());
                var productManufactureModel = new ProductDetailsResponse.ManufacturerModel();
                foreach (var item in manufacturerProducts)
                {
                    var manufacturer = _manufacturerService.GetManufacturerById(item.Manufacturer.Id);
                    productManufactureModel.Id = manufacturer.Id;
                    productManufactureModel.Name = _localizationService.GetLocalized(manufacturer, x => x.Name, languageId: languageId);
                    productManufactureModel.Description = _localizationService.GetLocalized(manufacturer, x => x.Description, languageId: languageId);
                    productManufactureModel.MetaDescription = _localizationService.GetLocalized(manufacturer, x => x.MetaDescription, languageId: languageId);
                    productManufactureModel.MetaKeywords = _localizationService.GetLocalized(manufacturer, x => x.MetaKeywords, languageId: languageId);
                    productManufactureModel.MetaTitle = _localizationService.GetLocalized(manufacturer, x => x.MetaTitle, languageId: languageId);
                    model.ProductManufacturers.Add(productManufactureModel);
                }
            }
            #endregion

            #region Rental products

            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            #endregion

            #region Associated products

            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = _productService.GetAssociatedProducts(product.Id, storeId);
                    model.AssociatedProducts = new List<ProductDetailsResponse>();
                    foreach (var associatedProduct in associatedProducts)
                        model.AssociatedProducts.Add(PrepareProductDetailsPageModel(associatedProduct, currentCustomer, languageId, storeId, currencyId, null, true));
                }
            }

            #endregion

            return model;
        }

        [NonAction]
        public virtual IList<ProductSpecificationModel> PrepareProductSpecificationModel(int languageId, Product product)
        {
            if (product == null)
                return null;

            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_SPECS_MODEL_KEY, product.Id, languageId);
            return _cacheManager.Get(cacheKey, () =>
                 _specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
                 .Select(psa =>
                 {
                     var m = new ProductSpecificationModel
                     {
                         SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                         SpecificationAttributeName = _localizationService.GetLocalized(psa.SpecificationAttributeOption.SpecificationAttribute, x => x.Name),
                         ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb,
                         AttributeTypeId = psa.AttributeTypeId
                     };

                     switch (psa.AttributeType)
                     {
                         case SpecificationAttributeType.Option:
                             m.ValueRaw = WebUtility.HtmlEncode(_localizationService.GetLocalized(psa.SpecificationAttributeOption, x => x.Name));
                             break;
                         case SpecificationAttributeType.CustomText:
                             m.ValueRaw = WebUtility.HtmlEncode(psa.CustomValue);
                             break;
                         case SpecificationAttributeType.CustomHtmlText:
                             m.ValueRaw = psa.CustomValue;
                             break;
                         case SpecificationAttributeType.Hyperlink:
                             m.ValueRaw = $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>";
                             break;
                         default:
                             break;
                     }
                     return m;
                 }).ToList()
             );
        }

        [NonAction]
        protected virtual string PrepareProductReviewsModel(ProductReviewsModelResponse model, Product product, Customer currentCustomer, int storeId, int languageId)
        {
            if (product == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.ProductNotFound";

            if (model == null)
                return "Plugins.XcellenceIT.WebApiClient.Message.ReviewModelEmpty";

            model.ProductId = product.Id;
            model.ProductName = _localizationService.GetLocalized(product, x => x.Name, languageId: languageId);
            model.ProductSeName = _urlRecordService.GetSeName(product, languageId: languageId);

            var productReviews = _catalogSettings.ShowProductReviewsPerStore
                ? product.ProductReviews.Where(pr => pr.IsApproved && pr.StoreId == storeId).OrderBy(pr => pr.CreatedOnUtc)
                : product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                var customer = pr.Customer;
                model.Items.Add(new ProductReviewCustomModel
                {
                    Id = pr.Id,
                    CustomerGuid=pr.Customer.CustomerGuid,
                    CustomerName = _customerService.FormatUserName(customer),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                    
                });
            }

            return "";
        }

        #endregion

        #region Method

        [HttpPost]
        public IActionResult BestSellers([FromBody]BestSellerRequest bestSellerRequest)
        {
            if (bestSellerRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(bestSellerRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ShowBestsellersOnHomepage")).BadRequest();
            }

            //load and cache report
            var report = _cacheManager.Get(string.Format(ModelCacheEventConsumer.HOMEPAGE_BESTSELLERS_IDS_KEY, bestSellerRequest.StoreId),
                () =>
                    _orderReportService.BestSellersReport(storeId: bestSellerRequest.StoreId,
                    pageSize: _catalogSettings.NumberOfBestsellersOnHomepage));

            //load products
            var products = _productService.GetProductsByIds(report.Select(x => x.ProductId).ToArray());
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p,currentCustomer) && _storeMappingService.Authorize(p, bestSellerRequest.StoreId)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            if (products.Count == 0)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFoundInStore")).BadRequest();

            //prepare model
            var model = PrepareProductOverviewModels(bestSellerRequest.StoreId, bestSellerRequest.CurrencyId, bestSellerRequest.LanguageId, currentCustomer, products, true, true, bestSellerRequest.ProductThumbPictureSize).ToList();

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult ProductDetail([FromBody] ProductDetailsRequest productDetailsRequest)
        {
            if (productDetailsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            int updatecartitemid = 0;
            var currentCustomer = _customerService.GetCustomerByGuid(productDetailsRequest.CustomerGUID);

            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            var product = _productService.GetProductById(productDetailsRequest.ProductId);

            if (product == null || product.Deleted)
                return new ResponseObject(_localizationService.GetResource("Product deleted or not found.")).BadRequest();

            //Is published?
            //published?
            if (!_catalogSettings.AllowViewUnpublishedProductPage)
            {
                //Check whether the current user has a "Manage catalog" permission
                //It allows him to preview a product before publishing
                if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts.SystemName, currentCustomer))
                    return new ResponseObject(_localizationService.GetResource("Product not published.")).BadRequest();
            }

            if (!_storeMappingService.Authorize(product, productDetailsRequest.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFoundInStore")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(product,currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotAuthorised")).BadRequest();

            //availability dates
            if (!_productService.ProductIsAvailable(product))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotAvailableForRent")).BadRequest();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NotAssociatedProduct")).BadRequest();
            }

            //update existing shopping cart item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = currentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(productDetailsRequest.StoreId)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return null;
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return null;
                }
            }

            //prepare the model
            var model = PrepareProductDetailsPageModel(product, currentCustomer, productDetailsRequest.LanguageId, productDetailsRequest.StoreId, productDetailsRequest.CurrencyId, updatecartitem, false);
            model.CustomerGuid = currentCustomer.CustomerGuid;

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",string.Format( _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name));

            return Ok(model);

        }

        [HttpPost]
        public virtual IActionResult HomePageProducts([FromBody]HomePageProductsRequest homePageProductsRequest)
        {

            if (homePageProductsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var models = new List<ProductListingResponse>();

            var currentCustomer = _customerService.GetCustomerByGuid(homePageProductsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //get all products that is enabled to show on home page
            var products = _productService.GetAllProductsDisplayedOnHomePage();

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p,currentCustomer) && _storeMappingService.Authorize(p, homePageProductsRequest.StoreId)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            if (products.Count == 0)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoProductFoundOnHomePage")).BadRequest();
            }

            var model = PrepareProductOverviewModels(homePageProductsRequest.StoreId, homePageProductsRequest.CurrencyId, homePageProductsRequest.LanguageId, currentCustomer, products, true, true, homePageProductsRequest.ProductThumbPictureSize).ToList();

            return Ok(model);
        }

        [HttpPost]
        public IActionResult RecentlyAddedProducts([FromBody]RecentlyAddedProductsRequest recentlyAddedProductsRequest)
        {
            if (recentlyAddedProductsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var models = new List<ProductListingResponse>();

            var currentCustomer = _customerService.GetCustomerByGuid(recentlyAddedProductsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            if (!_catalogSettings.NewProductsEnabled)
            {
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.RecentlyAddedProduct")).BadRequest();
            }

            var products = _productService.SearchProducts(
                storeId: recentlyAddedProductsRequest.StoreId,
                visibleIndividuallyOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);

            var model = new List<ProductListingResponse>();
            model.AddRange(PrepareProductOverviewModels(recentlyAddedProductsRequest.StoreId, recentlyAddedProductsRequest.CurrencyId, recentlyAddedProductsRequest.LanguageId, currentCustomer, products, true, true, recentlyAddedProductsRequest.ProductThumbPictureSize));

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult RelatedProducts([FromBody]RelatedProductsRequest relatedProductsRequest)
        {
            if (relatedProductsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(relatedProductsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, relatedProductsRequest.ProductId, relatedProductsRequest.StoreId),
                () =>
                    _productService.GetRelatedProductsByProductId1(relatedProductsRequest.ProductId).Select(x => x.ProductId2).ToArray()
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p,currentCustomer) && _storeMappingService.Authorize(p, relatedProductsRequest.StoreId)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            if (products.Count == 0)
            {
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoRelatedProductFound"), relatedProductsRequest.ProductId)).BadRequest();
            }

            var model = PrepareProductOverviewModels(relatedProductsRequest.StoreId, relatedProductsRequest.CurrencyId, relatedProductsRequest.LanguageId, currentCustomer, products, true, true, relatedProductsRequest.ProductThumbPictureSize).ToList();
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult ManufacturerProduct([FromBody]ManufacturerProductRequest manufacturerProduct)
        {
            if (manufacturerProduct == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(manufacturerProduct.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(manufacturerProduct.CurrencyId);

            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerProduct.ManufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ManufacturerEmptyOrDeleted")).BadRequest();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a manufacturer before publishing
            if (!manufacturer.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ManufacturerNotPublishedOrUnauthorized")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(manufacturer,currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoAccessToManufacturer")).BadRequest();

            //Store mapping
            if (!_storeMappingService.Authorize(manufacturer, manufacturerProduct.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ManufacturerStoreMappinUnauthorized")).BadRequest();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(currentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                manufacturerProduct.StoreId);

            var model = new ManufactureResponse();
            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                IPagedList<Product> featuredProducts = null;

                //We cache a value indicating whether we have featured products
                var customerRolesIds = currentCustomer.CustomerRoles
                    .Where(cr => cr.Active).Select(cr => cr.Id).ToList();
                string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY, manufacturerProduct.ManufacturerId,
                    string.Join(",", customerRolesIds), manufacturerProduct.StoreId);
                var hasFeaturedProductsCache = _cacheManager.Get(cacheKey, () =>
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                        manufacturerId: manufacturer.Id,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        featuredProducts: true);
                    return featuredProducts.TotalCount > 0;
                });
                if (hasFeaturedProductsCache && featuredProducts == null)
                {
                    //cache indicates that the manufacturer has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       manufacturerId: manufacturer.Id,
                       storeId: manufacturerProduct.StoreId,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = PrepareProductOverviewModels(manufacturerProduct.StoreId, manufacturerProduct.CurrencyId, manufacturerProduct.LanguageId, currentCustomer, featuredProducts).ToList();
                }
            }

            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds, true,
                manufacturerId: manufacturer.Id,
                storeId: manufacturerProduct.StoreId,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false
                );
            model.Products = PrepareProductOverviewModels(manufacturerProduct.StoreId, manufacturerProduct.CurrencyId, manufacturerProduct.LanguageId, currentCustomer, products).ToList();
            int languageId = manufacturerProduct.LanguageId;
            var manufacturerData = _manufacturerService.GetManufacturerById(manufacturerProduct.ManufacturerId);

            model.Id = manufacturerProduct.ManufacturerId;
            model.Name = _localizationService.GetLocalized(manufacturerData, x => x.Name, languageId: languageId);
            model.Description = _localizationService.GetLocalized(manufacturerData, x => x.Description, languageId: languageId);
            model.MetaKeywords = _localizationService.GetLocalized(manufacturerData, x => x.MetaKeywords, languageId: languageId);
            model.MetaDescription = _localizationService.GetLocalized(manufacturerData, x => x.MetaDescription, languageId: languageId);
            model.MetaTitle = _localizationService.GetLocalized(manufacturerData, x => x.MetaTitle, languageId: languageId);
            model.SeName = _urlRecordService.GetSeName(manufacturerData, languageId: languageId);
            model.CustomerGuid = currentCustomer.CustomerGuid;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewManufacturer", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name));

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult ProductReviews([FromBody]ProductReviewsRequest productReviewsRequest)
        {
            if (productReviewsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            ProductReviewsModelResponse model = new ProductReviewsModelResponse();

            var currentCustomer = _customerService.GetCustomerByGuid(productReviewsRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            model.CustomerGuid = currentCustomer.CustomerGuid;

            var product = _productService.GetProductById(productReviewsRequest.ProductId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFound")).BadRequest();

            PrepareProductReviewsModel(model, product, currentCustomer, productReviewsRequest.StoreId, productReviewsRequest.LanguageId);

            //only registered users can leave reviews by phnai(on product reviews view)
            //if (currentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            //    return new ResponseObject(_localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews")).BadRequest();

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing &&
                !_orderService.SearchOrders(customerId: currentCustomer.Id, productId: productReviewsRequest.ProductId, osIds: new List<int> { (int)OrderStatus.Complete }).Any())
                return new ResponseObject(_localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing")).BadRequest();

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult AddProductReview([FromBody]AddProductReviewRequest addProductReviewRequest)
        {
            if (addProductReviewRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

           

            ProductReviewAdd productReviewAdd = new ProductReviewAdd();

            if (productReviewAdd == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NullReview")).BadRequest();

            var currentCustomer = _customerService.GetCustomerByGuid(addProductReviewRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage && !addProductReviewRequest.CaptchaValid)
            {
                return new ResponseObject(_captchaSettings.GetWrongCaptchaMessage(_localizationService)).BadRequest();
            }

            if (currentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                return new ResponseObject(_localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews")).BadRequest();
            }

            // Get Product
            Product product = _productService.GetProductById(addProductReviewRequest.ProductId);
            if (product == null)
                return new ResponseObject(string.Format(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.ProductNotFound"), addProductReviewRequest.ProductId)).BadRequest();

            //save review
            int rating = addProductReviewRequest.ProductReviewRequest.Rating;
            if (rating < 1 || rating > 5)
                rating = _catalogSettings.DefaultProductRatingValue;
            bool isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

            var productReview = new ProductReview
            {
                ProductId = product.Id,
                CustomerId = currentCustomer.Id,
                Title = addProductReviewRequest.ProductReviewRequest.Title,
                ReviewText = addProductReviewRequest.ProductReviewRequest.ReviewText,
                Rating = rating,
                HelpfulYesTotal = 0,
                HelpfulNoTotal = 0,
                IsApproved = isApproved,
                CreatedOnUtc = DateTime.UtcNow,
                StoreId = addProductReviewRequest.StoreId,
            };
            product.ProductReviews.Add(productReview);
            _productService.UpdateProduct(product);

            //update product totals
            _productService.UpdateProductReviewTotals(product);

            //notify store owner
            if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddProductReview",string.Format( _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name));

            //raise event
            if (productReview.IsApproved)
                _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

            AddProductReviewsResponse model = new AddProductReviewsResponse
            {
                ProductReview = new ProductReviewAdd()
                {
                    SuccessfullyAdded = true,
                    DisplayCaptcha = addProductReviewRequest.ProductReviewRequest.DisplayCaptcha,

                    Rating = addProductReviewRequest.ProductReviewRequest.Rating,
                    Result = _localizationService.GetResource("Reviews.SuccessfullyAdded"),
                    ReviewText = addProductReviewRequest.ProductReviewRequest.ReviewText,
                    Title = addProductReviewRequest.ProductReviewRequest.Title,
                    CanCurrentCustomerLeaveReview = addProductReviewRequest.ProductReviewRequest.CanCurrentCustomerLeaveReview,
                },
                Id = productReview.Id,
                Message = _localizationService.GetResource("Reviews.SuccessfullyAdded"),
                ProductId = addProductReviewRequest.ProductId,
                CustomerGuid = currentCustomer.CustomerGuid,
                CustomerName = _customerService.GetCustomerFullName(currentCustomer)
            };
            model.CustomerGuid = currentCustomer.CustomerGuid;

            return Ok(model);
        }

        #endregion
    }
}
