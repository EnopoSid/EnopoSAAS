using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Data.Extensions;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Events;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass.Custom;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using XcellenceIt.Plugin.Misc.WebApiClient.Models;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [Authorization]
    [ApiException]
    public class CatalogController : Controller
    {
        #region Fields       

        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IAclService _aclService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IProductTagService _productTagService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ITopicService _topicService;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISearchTermService _searchTermService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly string _entityName;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly CommonSettings _commonSettings;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Category> _categoryRepository;
        // added by Phanendra on 04-05-2020 to get only Gourmet related products 
        private readonly IKiposCategorySIteStatusService _Category_SIteStatusService = EngineContext.Current.Resolve<IKiposCategorySIteStatusService>();

        #endregion

        #region Ctor

        public CatalogController(
        ICustomerService customerService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IGenericAttributeService genericAttributeService,
        IPermissionService permissionService,
        IProductService productService,
        IStoreMappingService storeMappingService,
        ICurrencyService currencyService,
        IPriceFormatter priceFormatter,
        CatalogSettings catalogSettings,
        ICacheManager cacheManager,
        IAclService aclService,
        IPriceCalculationService priceCalculationService,
        ITaxService taxService,
        MediaSettings mediaSettings,
        IPictureService pictureService,
        IWebHelper webHelper,
        VendorSettings vendorSettings,
        IVendorService vendorService,
        ICategoryService categoryService,
        IProductTagService productTagService,
        IManufacturerService manufacturerService,
        ISpecificationAttributeService specificationAttributeService,
        ITopicService topicService,
        BlogSettings blogSettings,
        ForumSettings forumSettings,
        IEventPublisher eventPublisher,
        ISearchTermService searchTermService,
        ILogger logger,
        IWorkContext workContext,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IRepository<AclRecord> aclRepository,
        IRepository<Manufacturer> manufacturerRepository,
        IRepository<StoreMapping> storeMappingRepository,
        IStaticCacheManager staticCacheManager,
        CommonSettings commonSettings,
        IDataProvider dataProvider,
        IDbContext dbContext,
        IRepository<Category> categoryRepository
        )
        {
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._permissionService = permissionService;
            this._productService = productService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._catalogSettings = catalogSettings;
            this._cacheManager = cacheManager;
            this._aclService = aclService;
            this._priceCalculationService = priceCalculationService;
            this._mediaSettings = mediaSettings;
            this._webHelper = webHelper;
            this._pictureService = pictureService;
            this._taxService = taxService;
            this._vendorSettings = vendorSettings;
            this._vendorService = vendorService;
            this._categoryService = categoryService;
            this._productTagService = productTagService;
            this._manufacturerService = manufacturerService;
            this._specificationAttributeService = specificationAttributeService;
            this._topicService = topicService;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._eventPublisher = eventPublisher;
            this._searchTermService = searchTermService;
            this._logger = logger;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._urlRecordService = urlRecordService;
            this._aclRepository = aclRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._entityName = typeof(Manufacturer).Name;
            this._staticCacheManager = staticCacheManager;
            this._commonSettings = commonSettings;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._categoryRepository = categoryRepository;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual IList<CustomCategorySimpleModel> PrepareCategorySimpleModels(Customer currentCustomer, int languageId, int storeId, int rootCategoryId,
        bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CustomCategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemented it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once             
                allCategories = GetAllCategories(currentCustomer ,storeId: storeId);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            foreach (var category in categories)
            {

                //prepare picture model
                var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, category.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                PictureModel categoryPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(category.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), category.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), category.Name)
                    };
                    return pictureModel;
                });

                var categoryModel = new CustomCategorySimpleModel
                {
                    Id = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name, languageId: languageId),
                    SeName = _urlRecordService.GetSeName(category, languageId: languageId),
                    IncludeInTopMenu = category.IncludeInTopMenu,
                    PictureModel = categoryPictureModel
                };

                // nubmer of products in each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                        string.Join(",", currentCustomer.GetCustomerRoleIds()),
                        storeId,
                        category.Id);
                    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                    {
                        var categoryIds = new List<int>
                        {
                            category.Id
                        };
                        //include subcategories
                        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                            categoryIds.AddRange(GetChildCategoryIds(currentCustomer, storeId, category.Id));
                        return _productService.GetNumberOfProductsInCategory(categoryIds, storeId);
                    });
                }

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(currentCustomer, languageId, storeId, category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        // added by Phanendra on 04-05-2020 to get only Gourmet related products  start
        [NonAction]
        protected virtual IList<CustomCategorySimpleModel> PrepareCategorySimpleModelsBYSiteStatus(Customer currentCustomer, int languageId, int storeId, int rootCategoryId,
        bool loadSubCategories = true, IList<Category> allCategories = null, bool IsOnline =true)
        {
            var result = new List<CustomCategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemented it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once             
                allCategories = GetAllCategories(currentCustomer, storeId: storeId);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            var category_status = _Category_SIteStatusService.GetAllKiposCategorySiteStatus();

            foreach (var category in categories)
            {
                int mainCatStatus = 0;
                foreach(var varcategoryStatus in category_status)
                {
                    if (IsOnline == true)
                    {
                        if (varcategoryStatus.CategoryId == category.Id && varcategoryStatus.IsOnline == 1)
                        {
                            mainCatStatus = 1;
                        }
                    } else if(IsOnline== false)
                    {
                        if (varcategoryStatus.CategoryId == category.Id && varcategoryStatus.IsPOS == 1)
                        {
                            mainCatStatus = 1;
                        }
                    }
                }

                if (mainCatStatus == 1)
                {
                    //prepare picture model
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, category.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    PictureModel categoryPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(category.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), category.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), category.Name)
                        };
                        return pictureModel;
                    });

                    var categoryModel = new CustomCategorySimpleModel
                    {
                        Id = category.Id,
                        Name = _localizationService.GetLocalized(category, x => x.Name, languageId: languageId),
                        SeName = _urlRecordService.GetSeName(category, languageId: languageId),
                        IncludeInTopMenu = category.IncludeInTopMenu,
                        PictureModel = categoryPictureModel
                    };

                    // nubmer of products in each category
                    if (_catalogSettings.ShowCategoryProductNumber)
                    {
                        string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                            string.Join(",", currentCustomer.GetCustomerRoleIds()),
                            storeId,
                            category.Id);
                        categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                        {
                            var categoryIds = new List<int>
                            {
                            category.Id
                            };
                        //include subcategories
                        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                                categoryIds.AddRange(GetChildCategoryIds(currentCustomer, storeId, category.Id));
                            return _productService.GetNumberOfProductsInCategory(categoryIds, storeId);
                        });
                    }

                    if (loadSubCategories)
                    {
                        var subCategories = PrepareCategorySimpleModels(currentCustomer, languageId, storeId, category.Id, loadSubCategories, allCategories);
                        var subCategoriesList = subCategories;

                        if (subCategories.Count > 0)
                        {
                            for (int i = 0; i < subCategories.Count; i++)
                            {
                                mainCatStatus = 0;
                                foreach (var varcategoryStatus in category_status)
                                {

                                    if (IsOnline == true)
                                    {
                                        if (varcategoryStatus.ParentCategoryId == subCategories[i].Id && varcategoryStatus.IsOnline == 1)
                                        {
                                            mainCatStatus = 1;
                                        }
                                    }
                                    else if (IsOnline == false)
                                    {
                                        if (varcategoryStatus.ParentCategoryId == subCategories[i].Id && varcategoryStatus.IsPOS == 1)
                                        {
                                            mainCatStatus = 1;
                                        }
                                    }
                                }
                                if (mainCatStatus == 0)
                                {
                                    subCategoriesList.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                        categoryModel.SubCategories.AddRange(subCategoriesList);
                    }
                    result.Add(categoryModel);
                }
            }

            return result;
        }
        // added by Phanendra on 04-05-2020 to get only Gourmet related products end

        // added by Phanendra on 04-05-2020 to get only Gourmet related products  start
        [NonAction]
        protected virtual IList<CustomCategorySimpleModel> PrepareCategorySimpleModelsBYSiteStatusPOS(Customer currentCustomer, int languageId, int storeId, int rootCategoryId,
        bool loadSubCategories = true, IList<Category> allCategories = null, bool IsOnline = false)
        {
            var result = new List<CustomCategorySimpleModel>();
            loadSubCategories = true;
            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemented it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once             
                allCategories = GetAllCategories(currentCustomer, storeId: storeId);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            var category_status = _Category_SIteStatusService.GetAllKiposCategorySiteStatus();
            int categoryId = 0;

            foreach (var category in categories)
            {
                int mainCatStatus = 0;
                foreach (var varcategoryStatus in category_status)
                {
                    if (IsOnline == true)
                    {
                        if (varcategoryStatus.CategoryId == category.Id && varcategoryStatus.IsOnline == 1)
                        {
                            mainCatStatus = 1;
                        }
                    }
                    else if (IsOnline == false)
                    {
                        if (varcategoryStatus.CategoryId == category.Id && varcategoryStatus.IsPOS == 1)
                        {
                            categoryId = varcategoryStatus.CategoryId;
                            mainCatStatus = 1;
                            break;
                        }
                    }
                }

                if (mainCatStatus == 1)
                {
                    //prepare picture model
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, category.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    PictureModel categoryPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(category.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), category.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), category.Name)
                        };
                        return pictureModel;
                    });

                    var categoryModel = new CustomCategorySimpleModel
                    {
                        Id = category.Id,
                        Name = _localizationService.GetLocalized(category, x => x.Name, languageId: languageId),
                        SeName = _urlRecordService.GetSeName(category, languageId: languageId),
                        IncludeInTopMenu = category.IncludeInTopMenu,
                        PictureModel = categoryPictureModel
                    };

                    // nubmer of products in each category
                    if (_catalogSettings.ShowCategoryProductNumber)
                    {
                        string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                            string.Join(",", currentCustomer.GetCustomerRoleIds()),
                            storeId,
                            category.Id);
                        categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                        {
                            var categoryIds = new List<int>
                            {
                            category.Id
                            };
                            //include subcategories
                            if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                                categoryIds.AddRange(GetChildCategoryIds(currentCustomer, storeId, category.Id));
                            return _productService.GetNumberOfProductsInCategory(categoryIds, storeId);
                        });
                    }

                    if (loadSubCategories)
                    {
                        var subCategories = PrepareCategorySimpleModels(currentCustomer, languageId, storeId, categoryId, loadSubCategories, allCategories);
                        var subCategoriesList = subCategories;

                        if (subCategories.Count > 0) 
                        {
                            for (int i = 0; i < subCategories.Count; i++)
                            {
                                mainCatStatus = 0;
                                foreach (var varcategoryStatus in category_status)
                                {

                                    if (IsOnline == true)
                                    {
                                        if (varcategoryStatus.ParentCategoryId == subCategories[i].Id && varcategoryStatus.IsOnline == 1)
                                        {
                                            mainCatStatus = 1;
                                        }
                                    }
                                    else if (IsOnline == false)
                                    {
                                        if (varcategoryStatus.ParentCategoryId == subCategories[i].Id && varcategoryStatus.IsPOS == 1)
                                        {
                                            mainCatStatus = 1;
                                        }
                                    }
                                }
                                if (mainCatStatus == 0)
                                {
                                    subCategoriesList.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                        categoryModel.SubCategories.AddRange(subCategoriesList);
                    }
                    result.Add(categoryModel);
                }
            }

            return result;
        }
        // added by Phanendra on 04-05-2020 to get only Gourmet related products end

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Categories</returns>
        [NonAction]
        public virtual IList<Category> GetAllCategories(Customer customer, int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true)
        {
            IList<Category> LoadCategoriesFunc() => GetAllCategories(customer,string.Empty, storeId, showHidden: showHidden);

            IList<Category> categories;
            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(NopCatalogDefaults.CategoriesAllCacheKey,
                    storeId,
                    string.Join(",", customer.GetCustomerRoleIds()),
                    showHidden);
                categories = _staticCacheManager.Get(key, () =>
                {
                    var result = new List<Category>();
                    foreach (var category in LoadCategoriesFunc())
                        result.Add(new CategoryForCaching(category));
                    return result;
                });
            }
            else
            {
                categories = LoadCategoriesFunc();
            }

            return categories;
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        [NonAction]
        public virtual IPagedList<Category> GetAllCategories(Customer currentCustomer,string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (_commonSettings.UseStoredProcedureForLoadingCategories)
            {
                //stored procedures are enabled for loading categories and supported by the database. 
                //It's much faster with a large number of categories than the LINQ implementation below 

                //prepare parameters
                var showHiddenParameter = _dataProvider.GetBooleanParameter("ShowHidden", showHidden);
                var nameParameter = _dataProvider.GetStringParameter("Name", categoryName ?? string.Empty);
                var storeIdParameter = _dataProvider.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
                var pageIndexParameter = _dataProvider.GetInt32Parameter("PageIndex", pageIndex);
                var pageSizeParameter = _dataProvider.GetInt32Parameter("PageSize", pageSize);
                //pass allowed customer role identifiers as comma-delimited string
                var customerRoleIdsParameter = _dataProvider.GetStringParameter("CustomerRoleIds", !_catalogSettings.IgnoreAcl ? string.Join(",", currentCustomer.GetCustomerRoleIds()) : string.Empty);

                var totalRecordsParameter = _dataProvider.GetOutputInt32Parameter("TotalRecords");

                //invoke stored procedure
                var categories = _dbContext.EntityFromSql<Category>("CategoryLoadAllPaged",
                    showHiddenParameter, nameParameter, storeIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter).ToList();
                var totalRecords = totalRecordsParameter.Value != DBNull.Value ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<Category>(categories, pageIndex, pageSize, totalRecords);
            }

            //don't use a stored procedure. Use LINQ
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = currentCustomer.GetCustomerRoleIds();
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            var unsortedCategories = query.ToList();

            //sort categories
            var sortedCategories = _categoryService.SortCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId,Customer currentCustomer)
        {
            string cacheKey = string.Format(NopCatalogDefaults.CategoriesChildIdentifiersCacheKey,
               parentCategoryId,
               string.Join(",", currentCustomer.GetCustomerRoleIds()),
               _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id, currentCustomer));
                }
                return categoriesIds;
            });
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(Customer currentCustomer, int storeId, int parentCategoryId)
        {
            string cacheKey = string.Format(NopCatalogDefaults.CategoriesChildIdentifiersCacheKey,
               parentCategoryId,
               string.Join(",", currentCustomer.GetCustomerRoleIds()),
               storeId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id, currentCustomer));
                }
                return categoriesIds;
            });
        }

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
                                if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
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

                model.CustomerGuid = currentCustomer.CustomerGuid;
                models.Add(model);
            }
            return models;
        }

        [NonAction]
        protected virtual void PrepareSortingOptions(int languageId, CatalogPagingFilteringResponse pagingFilteringModel, CatalogPagingResponse command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            var allDisabled = _catalogSettings.ProductSortingEnumDisabled.Count == Enum.GetValues(typeof(ProductSortingEnum)).Length;
            pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting && !allDisabled;

            var activeOptions = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(_catalogSettings.ProductSortingEnumDisabled)
                .Select((idOption) =>
                {
                    return new KeyValuePair<int, int>(idOption, _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(idOption, out int order) ? order : idOption);
                })
                .OrderBy(x => x.Value);
            if (command.OrderBy == 0)
                command.OrderBy = allDisabled ? 0 : activeOptions.First().Key;

            if (pagingFilteringModel.AllowProductSorting)
            {
                pagingFilteringModel.AvailableSortOptions = new List<SelectListItem>();

                foreach (var option in activeOptions)
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + (option.Key).ToString(), null);

                    var sortValue = _localizationService.GetLocalizedEnum(((ProductSortingEnum)option.Key));

                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = option.Key == command.OrderBy
                    });
                }
            }
        }

        [NonAction]
        public virtual List<int> GetAlreadyFilteredSpecOptionIds(IWebHelper webHelper)
        {
            var result = new List<int>();

            var alreadyFilteredSpecsStr = webHelper.QueryString<string>("specs");
            if (string.IsNullOrWhiteSpace(alreadyFilteredSpecsStr))
                return result;

            foreach (var spec in alreadyFilteredSpecsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int.TryParse(spec.Trim(), out int specId);
                if (!result.Contains(specId))
                    result.Add(specId);
            }
            return result;
        }

        [NonAction]
        public virtual IList<SpecificationAttributeFilter> PrepareSpecsLoadFilters(IList<int> alreadyFilteredSpecOptionIds,
        IList<int> filterableSpecificationAttributeOptionIds, int languageId)
        {
            IList<SpecificationAttributeFilter> allFilters = new List<SpecificationAttributeFilter>();
            var specificationAttributeOptions = _specificationAttributeService
                .GetSpecificationAttributeOptionsByIds(filterableSpecificationAttributeOptionIds != null ?
                filterableSpecificationAttributeOptionIds.ToArray() : null);

            // Filter attributes and attributes wise options
            // Reference from _filterSpectBox.cshtml

            //Get Unique Attributes
            var notFilteredItemsGroups = specificationAttributeOptions.GroupBy(x => x.SpecificationAttributeId);

            if (notFilteredItemsGroups.Count() > 0)
            {
                foreach (var sao in notFilteredItemsGroups)
                {
                    SpecificationAttributeFilter saf = new SpecificationAttributeFilter();

                    var attribute = specificationAttributeOptions.Where(x => x.SpecificationAttributeId == sao.Key).FirstOrDefault().SpecificationAttribute;
                    var groupList = sao.ToList();

                    saf.SpecificationAttributeId = sao.Key;
                    saf.SpecificationAttributeDisplayOrder = attribute.DisplayOrder;
                    saf.SpecificationAttributeName = attribute.Name;

                    foreach (var spec in groupList)
                    {
                        SpecificationAttributeOptionLoadFilter attributeOption = new SpecificationAttributeOptionLoadFilter()
                        {
                            SpecificationAttributeColorRGB = spec.ColorSquaresRgb,
                            SpecificationAttributeOptionDisplayOrder = spec.DisplayOrder,
                            SpecificationAttributeOptionId = spec.Id,
                            SpecificationAttributeOptionName = spec.Name
                        };

                        // Add atributes option
                        saf.SpecificationAttributeOptions.Add(attributeOption);
                    }

                    // Add Attribute
                    allFilters.Add(saf);
                }
            }

            //sort loaded options
            allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeName).ToList();

            //get already filtered specification options
            var alreadyFilteredOptions = allFilters
                .Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeId))
                .Select(x => x)
                .ToList();

            //get not filtered specification options
            var notFilteredOptions = new List<SpecificationAttributeOptionFilter>();

            return allFilters;
        }

        [NonAction]
        public virtual IList<PriceRangeFilters> LoadPriceRangeFilters(string priceRangeStr, IWebHelper webHelper, IPriceFormatter priceFormatter)
        {
            var priceRangeList = GetPriceRangeList(priceRangeStr);
            if (priceRangeList.Count > 0)
            {
                IList<PriceRangeFilters> PriceRangeFilters = priceRangeList.ToList().Select(x =>
                {
                    //from&to
                    var item = new PriceRangeFilters();
                    if (x.From.HasValue)
                    {
                        item.FromCurrency = priceFormatter.FormatPrice(x.From.Value, true, false);
                        item.From = x.From.Value;
                    }
                    if (x.To.HasValue)
                    {
                        item.ToCurrency = priceFormatter.FormatPrice(x.To.Value, true, false);
                        item.To = x.To.Value;
                    }

                    return item;
                }).ToList();

                return PriceRangeFilters;
            }
            else
            {
                return new List<PriceRangeFilters>();
            }

        }

        [NonAction]
        protected virtual IList<PriceRange> GetPriceRangeList(string priceRangesStr)
        {
            var priceRanges = new List<PriceRange>();
            if (string.IsNullOrWhiteSpace(priceRangesStr))
                return priceRanges;
            string[] rangeArray = priceRangesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str1 in rangeArray)
            {
                string[] fromTo = str1.Trim().Split(new[] { '-' });

                decimal? from = null;
                if (!string.IsNullOrEmpty(fromTo[0]) && !string.IsNullOrEmpty(fromTo[0].Trim()))
                    from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));

                decimal? to = null;
                if (!string.IsNullOrEmpty(fromTo[1]) && !string.IsNullOrEmpty(fromTo[1].Trim()))
                    to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                priceRanges.Add(new PriceRange { From = from, To = to });
            }
            return priceRanges;
        }

        [NonAction]
        protected virtual void PrepareFilterItems(IList<int> alreadyFilteredSpecOptionIds, IList<int> filterableSpecificationAttributeOptionIds, CategoryListingResponse model, int languageId)
        {
            var allFilters = new List<SpecificationAttributeOptionFilter>();
            var specificationAttributeOptions = _specificationAttributeService
                .GetSpecificationAttributeOptionsByIds(filterableSpecificationAttributeOptionIds != null ?
                filterableSpecificationAttributeOptionIds.ToArray() : null);

            foreach (var sao in specificationAttributeOptions)
            {
                var sa = sao.SpecificationAttribute;
                if (sa != null)
                {
                    allFilters.Add(new SpecificationAttributeOptionFilter
                    {
                        SpecificationAttributeId = sa.Id,
                        SpecificationAttributeName = _localizationService.GetLocalized(sa, x => x.Name, languageId: languageId),
                        SpecificationAttributeDisplayOrder = sa.DisplayOrder,
                        SpecificationAttributeOptionId = sao.Id,
                        SpecificationAttributeOptionName = _localizationService.GetLocalized(sao, x => x.Name, languageId: languageId),
                        SpecificationAttributeOptionDisplayOrder = sao.DisplayOrder
                    });
                }
            }

            //sort loaded options

            allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeOptionDisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();

            //get already filtered specification options

            var alreadyFilteredOptions = allFilters
                .Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeOptionId))
                .Select(x => x)
                .ToList();

            //get not filtered specification options

            var notFilteredOptions = new List<SpecificationAttributeOptionFilter>();

            foreach (var saof in allFilters)
            {
                //do not add already filtered specification options
                if (alreadyFilteredOptions.FirstOrDefault(x => x.SpecificationAttributeId == saof.SpecificationAttributeId) != null)
                    continue;

                //else add it
                notFilteredOptions.Add(saof);
            }

            //prepare the model properties

            var AlreadyFilteredItems = new List<SpecificationFilterItemResponse>();
            if (alreadyFilteredOptions.Count > 0 || notFilteredOptions.Count > 0)
            {
                AlreadyFilteredItems = alreadyFilteredOptions.ToList().Select(x =>
                {
                    var item = new SpecificationFilterItemResponse
                    {
                        SpecificationAttributeName = x.SpecificationAttributeName,
                        SpecificationAttributeOptionName = x.SpecificationAttributeOptionName
                    };
                    return item;

                }).ToList();

                model.AlreadyFilteredItems = new List<SpecificationFilterItemResponse>();

                model.AlreadyFilteredItems = alreadyFilteredOptions.ToList().Select(x =>
                {
                    var item = new SpecificationFilterItemResponse
                    {
                        SpecificationAttributeId = x.SpecificationAttributeId,
                        SpecificationAttributeName = x.SpecificationAttributeName,
                        SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                        SpecificationAttributeOptionName = x.SpecificationAttributeOptionName
                    };

                    return item;

                }).ToList();

                model.NotFilteredItems = new List<SpecificationFilterItemResponse>();
                model.NotFilteredItems = notFilteredOptions.ToList().Select(x =>
                {
                    var item = new SpecificationFilterItemResponse
                    {
                        SpecificationAttributeId = x.SpecificationAttributeId,
                        SpecificationAttributeName = x.SpecificationAttributeName,
                        SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                        SpecificationAttributeOptionName = x.SpecificationAttributeOptionName
                    };

                    return item;

                }).ToList();
            }
        }

        [NonAction]
        public virtual IList<SpecificationFilters> PrepareSpecsFilters(IList<int> alreadyFilteredSpecOptionIds, IList<int> filterableSpecificationAttributeOptionIds, int languageId)
        {
            var allFilters = new List<SpecificationFilters>();
            var specificationAttributeOptions = _specificationAttributeService
                .GetSpecificationAttributeOptionsByIds(filterableSpecificationAttributeOptionIds != null ?
                filterableSpecificationAttributeOptionIds.ToArray() : null);
            foreach (var sao in specificationAttributeOptions)
            {
                var sa = sao.SpecificationAttribute;
                if (sa != null)
                {
                    allFilters.Add(new SpecificationFilters
                    {
                        SpecificationAttributeName = _localizationService.GetLocalized(sa, x => x.Name, languageId: languageId),
                        SpecificationAttributeOptionId = sao.Id,
                        SpecificationAttributeDisplayOrder = sa.DisplayOrder,
                        SpecificationAttributeOptionName = _localizationService.GetLocalized(sao, x => x.Name, languageId: languageId),
                    });
                }
            }

            //sort loaded options
            allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();

            //get already filtered specification options
            var alreadyFilteredOptions = allFilters
                .Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeOptionId))
                .Select(x => x)
                .ToList();

            //get not filtered specification options
            var notFilteredOptions = new List<SpecificationAttributeOptionFilter>();

            return allFilters;
        }

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Manufacturer model</returns>
        [NonAction]
        public virtual CustomCategoryProductModel PrepareManufacturerModel(Manufacturer manufacturer, ProductByManufacturerRequest requestModel
            , Customer currentCustomer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            var model = new CustomCategoryProductModel();

            //sorting
            requestModel.PagingFilteringContext = new CatalogPagingFilteringResponse();
            PrepareSortingOptions(requestModel.LanguageId, requestModel.PagingFilteringContext, requestModel.CatalogPagingResponse);

            //products
            var products = _productService.SearchProducts(out IList<int> _, true,
                manufacturerId: manufacturer.Id,
                storeId: requestModel.StoreId,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                orderBy: (ProductSortingEnum)requestModel.CatalogPagingResponse.OrderBy,
                pageIndex: requestModel.CatalogPagingResponse.PageNumber - 1,
                pageSize: requestModel.CatalogPagingResponse.PageSize);

            model.Products = PrepareProuctsModel(requestModel.StoreId, requestModel.CurrencyId,
            requestModel.LanguageId, currentCustomer, products);


            return model;
        }

        [NonAction]
        public virtual IPagedList<Manufacturer> GetAllManufacturers(Customer customer, string manufacturerName = "",
         int storeId = 0,
         int pageIndex = 0,
         int pageSize = int.MaxValue,
         bool showHidden = false)
        {
            var query = _manufacturerRepository.Table;
            if (!showHidden)
                query = query.Where(m => m.Published);
            if (!string.IsNullOrWhiteSpace(manufacturerName))
                query = query.Where(m => m.Name.Contains(manufacturerName));
            query = query.Where(m => !m.Deleted);
            query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

            if ((storeId <= 0 || _catalogSettings.IgnoreStoreLimitations) && (showHidden || _catalogSettings.IgnoreAcl))
                return new PagedList<Manufacturer>(query, pageIndex, pageSize);

            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //ACL (access control list)
                var allowedCustomerRolesIds = customer.GetCustomerRoleIds();
                query = from m in query
                        join acl in _aclRepository.Table
                            on new { c1 = m.Id, c2 = _entityName } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                        from acl in m_acl.DefaultIfEmpty()
                        where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select m;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from m in query
                        join sm in _storeMappingRepository.Table
                            on new { c1 = m.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                        from sm in m_sm.DefaultIfEmpty()
                        where !m.LimitedToStores || storeId == sm.StoreId
                        select m;
            }

            query = query.Distinct().OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

            return new PagedList<Manufacturer>(query, pageIndex, pageSize);
        }

        #endregion

        #region Method

        [HttpPost]
        public virtual IActionResult TopMenu([FromBody]TopMenuRequest topMenuRequest)
        {
            if (topMenuRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var currentCustomer = _customerService.GetCustomerByGuid(topMenuRequest.CustomerGUID.Value);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //categories
            var customerRolesIds = currentCustomer.CustomerRoles
               .Where(cr => cr.Active).Select(cr => cr.Id).ToList();

            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
           topMenuRequest.LanguageId, string.Join(",", customerRolesIds), topMenuRequest.StoreId);

         //   var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModels(currentCustomer, topMenuRequest.LanguageId, topMenuRequest.StoreId, 0));
            // added by Phanendra on 04-05-2020 to get only Gourmet related products 
          var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModelsBYSiteStatus(currentCustomer, topMenuRequest.LanguageId, topMenuRequest.StoreId, 50,true));

            //top menu topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY,
                topMenuRequest.LanguageId, topMenuRequest.StoreId, string.Join(",", currentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(topMenuRequest.StoreId)
                .Where(t => t.IncludeInTopMenu)
                .Select(t => new CategoryListingResponse.TopMenuTopicModel
                {
                    Id = t.Id,
                    Name = _localizationService.GetLocalized(t, x => x.Title, languageId: topMenuRequest.LanguageId),
                    SeName = _urlRecordService.GetSeName(t, languageId: topMenuRequest.LanguageId)
                })
                .ToList()
            );
            var model = new CategoryListingResponse
            {
                Categories = cachedCategoriesModel,
                Topics = cachedTopicModel,
                RecentlyAddedProductsEnabled = _catalogSettings.NewProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };
            model.CustomerGuid = currentCustomer.CustomerGuid;



            return Ok(model);
        }

        // added by Phanendra on 18-2-2020 to get only Gourmet related products start
        [HttpPost]
        public virtual IActionResult POSTopMenu([FromBody]TopMenuRequest topMenuRequest)
        {
            if (topMenuRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var currentCustomer = _customerService.GetCustomerByGuid(topMenuRequest.CustomerGUID.Value);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();

            //categories
            var customerRolesIds = currentCustomer.CustomerRoles
               .Where(cr => cr.Active).Select(cr => cr.Id).ToList();

            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
           topMenuRequest.LanguageId, string.Join(",", customerRolesIds), topMenuRequest.StoreId);

            // added by Phanendra on 04-05-2020 to get only Gourmet related products 
            //var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModels(currentCustomer, topMenuRequest.LanguageId, topMenuRequest.StoreId, 0));
            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModelsBYSiteStatusPOS(currentCustomer, topMenuRequest.LanguageId, topMenuRequest.StoreId, 50, false));

            //top menu topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY,
                topMenuRequest.LanguageId, topMenuRequest.StoreId, string.Join(",", currentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(topMenuRequest.StoreId)
                .Where(t => t.IncludeInTopMenu)
                .Select(t => new CategoryListingResponse.TopMenuTopicModel
                {
                    Id = t.Id,
                    Name = _localizationService.GetLocalized(t, x => x.Title, languageId: topMenuRequest.LanguageId),
                    SeName = _urlRecordService.GetSeName(t, languageId: topMenuRequest.LanguageId)
                })
                .ToList()
            );
            var model = new CategoryListingResponse
            {
                Categories = cachedCategoriesModel,
                Topics = cachedTopicModel,
                RecentlyAddedProductsEnabled = _catalogSettings.NewProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };
            model.CustomerGuid = currentCustomer.CustomerGuid;
            return Ok(model);
        }
        // added by Phanendra on 18-2-2020 to get only Gourmet related products end


        [HttpPost]
        public virtual IActionResult Manufacturer([FromBody]ManufactureRequest manufactureRequest)
        {
            var model = new List<ManufactureResponse>();

            var currentCustomer = _customerService.GetCustomerByGuid(manufactureRequest.CustomerGuid);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var manufacturers = GetAllManufacturers(currentCustomer, storeId: manufactureRequest.StoreId);
            if (manufacturers == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.Manufacturer.StoreAvailability")).BadRequest();

            foreach (var manufacturer in manufacturers)
            {
                var objManufacturerResponse = new ManufactureResponse
                {
                    Id = manufacturer.Id,
                    Name = _localizationService.GetLocalized(manufacturer, x => x.Name, languageId: manufactureRequest.LanguageId),
                    Description = _localizationService.GetLocalized(manufacturer, x => x.Description, languageId: manufactureRequest.LanguageId),
                    MetaDescription = _localizationService.GetLocalized(manufacturer, x => x.MetaDescription, languageId: manufactureRequest.LanguageId),
                    MetaTitle = _localizationService.GetLocalized(manufacturer, x => x.MetaTitle, languageId: manufactureRequest.LanguageId),
                    MetaKeywords = _localizationService.GetLocalized(manufacturer, x => x.MetaKeywords, languageId: manufactureRequest.LanguageId),
                    SeName = _urlRecordService.GetSeName(manufacturer, languageId: manufactureRequest.LanguageId),
                    CustomerGuid = currentCustomer.CustomerGuid
                };
                //prepare picture model
                int pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
                var picture = _pictureService.GetPictureById(manufacturer.PictureId);

                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                    Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), manufacturer.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), manufacturer.Name)
                };
                objManufacturerResponse.PictureModel = pictureModel;
                model.Add(objManufacturerResponse);
            }
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult PopularProductTags([FromBody]PopularProductTagsRequest popularProductTagsRequest)
        {
            if (popularProductTagsRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_POPULAR_MODEL_KEY, popularProductTagsRequest.LanguageId, popularProductTagsRequest.StoreId);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new PopularTagsResponse
                {
                    Tags = new List<ProductTagModel>()
                };
                //get all tags
                var allTags = _productTagService
                .GetAllProductTags()
                //filter by current store
                .Where(x => _productTagService.GetProductCount(x.Id, popularProductTagsRequest.StoreId) > 0)
                //order by product count
                .OrderByDescending(x => _productTagService.GetProductCount(x.Id, popularProductTagsRequest.StoreId))
                .ToList();

                var tags = allTags
                    .Take(_catalogSettings.NumberOfProductTags)
                    .ToList();
                //sorting
                tags = tags.OrderBy(x => _localizationService.GetLocalized(x, y => y.Name, languageId: popularProductTagsRequest.LanguageId)).ToList();

                model.TotalTags = allTags.Count;
                foreach (var tag in tags)
                    model.Tags.Add(new ProductTagModel
                    {
                        Id = tag.Id,
                        Name = _localizationService.GetLocalized(tag, y => y.Name, popularProductTagsRequest.LanguageId),
                        SeName = _urlRecordService.GetSeName(tag, languageId: popularProductTagsRequest.LanguageId),
                        ProductCount = _productTagService.GetProductCount(tag.Id, popularProductTagsRequest.StoreId)
                    });
                return model;
            });

            if (cacheModel.Tags.Count == 0)
                return null;

            return Ok(cacheModel);
        }

        [HttpPost]
        public virtual IActionResult ProductByTag([FromBody]ProductByTagRequest productByTagRequest)
        {
            if (productByTagRequest == null)
                return new ResponseObject(_localizationService
                    .GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty"))
                    .BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var productTag = _productTagService.GetProductTagById(productByTagRequest.ProductTagId);
            if (productTag == null)
                return new ResponseObject(_localizationService
                    .GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoProductTagFound"))
                    .BadRequest();

            var currentCustomer = _customerService.GetCustomerByGuid(productByTagRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var model = new PopularTagsResponse
            {
                Id = productTag.Id,
                TagName = _localizationService.GetLocalized(productTag, x => x.Name, languageId: productByTagRequest.LanguageId),
                TagSeName = _urlRecordService.GetSeName(productTag, languageId: productByTagRequest.LanguageId)
            };

            //products
            var products = _productService.SearchProducts(
                storeId: productByTagRequest.StoreId,
                productTagId: productTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)productByTagRequest.CatalogPagingResponse.OrderBy,
                pageIndex: productByTagRequest.CatalogPagingResponse.PageNumber - 1,
                pageSize: productByTagRequest.CatalogPagingResponse.PageSize);
            model.Products = PrepareProductOverviewModels(productByTagRequest.StoreId, productByTagRequest.CurrencyId,
                productByTagRequest.LanguageId, currentCustomer, products);

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult SearchProduct([FromBody]SearchProductRequest searchProductRequest)
        {
            if (searchProductRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(searchProductRequest.CustomerGUID);
            if (currentCustomer == null)
            {
                currentCustomer = _customerService.InsertGuestCustomer();
                searchProductRequest.CustomerGUID = currentCustomer.CustomerGuid;
            }
            var workingCurrency = _currencyService.GetCurrencyById(searchProductRequest.CurrencyId);

            if (searchProductRequest.SearchListResponse.Q == null)
                searchProductRequest.SearchListResponse.Q = "";
            searchProductRequest.SearchListResponse.Q = searchProductRequest.SearchListResponse.Q.Trim();
            var model = new List<CustomCategoryProductModel>();

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            if (searchProductRequest.SearchListResponse.Q != null)
            {
                if (searchProductRequest.SearchListResponse.Q.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    searchProductRequest.SearchListResponse.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    //products
                    products = _productService.SearchProducts(
                        storeId: searchProductRequest.StoreId,
                        visibleIndividuallyOnly: true,
                        keywords: searchProductRequest.SearchListResponse.Q,
                        searchDescriptions:true,
                        languageId: searchProductRequest.LanguageId,
                        orderBy: (ProductSortingEnum)searchProductRequest.CatalogPagingResponse.OrderBy,
                        pageIndex: searchProductRequest.CatalogPagingResponse.PageNumber - 1,
                        pageSize: searchProductRequest.CatalogPagingResponse.PageSize);
                    model = PrepareProuctsModel(searchProductRequest.StoreId, searchProductRequest.CurrencyId, searchProductRequest.LanguageId, currentCustomer, products).ToList();
                    //searchProductRequest.SearchListResponse.Products = PrepareProductOverviewModels(searchProductRequest.StoreId, searchProductRequest.CurrencyId, searchProductRequest.LanguageId, currentCustomer, products).ToList();
                    //searchProductRequest.SearchListResponse.NoResults = !searchProductRequest.SearchListResponse.Products.Any();

                    //search term statistics
                    if (!string.IsNullOrEmpty(searchProductRequest.SearchListResponse.Q))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchProductRequest.SearchListResponse.Q, searchProductRequest.StoreId);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchProductRequest.SearchListResponse.Q,
                                StoreId = searchProductRequest.StoreId,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new ProductSearchEvent
                    {
                        SearchTerm = searchProductRequest.SearchListResponse.Q,
                        WorkingLanguageId = searchProductRequest.LanguageId
                    });
                }
            }

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult AdvancedSearch([FromBody]AdvancedSearchRequest advancedSearchRequest)
        {
            if (advancedSearchRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerById(advancedSearchRequest.CustomerId);
            if (currentCustomer == null)
            {
                currentCustomer = _customerService.InsertGuestCustomer();
                advancedSearchRequest.CustomerId = currentCustomer.Id;
            }

            var workingCurrency = _currencyService.GetCurrencyById(advancedSearchRequest.CurrencyId);

            var searchTerms = advancedSearchRequest.AdvanceSearchResponse.Q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();


            //sorting
            PrepareSortingOptions(advancedSearchRequest.LanguageId, advancedSearchRequest.AdvanceSearchResponse.PagingFilteringContext, advancedSearchRequest.CatalogPagingResponse);

            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", currentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
                var allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                foreach (var c in allCategories)
                {
                    //generate full category name (breadcrumb)
                    string categoryBreadcrumb = "";
                    var breadcrumb = _categoryService.GetCategoryBreadCrumb(c, allCategories);
                    for (var i = 0; i <= breadcrumb.Count - 1; i++)
                    {
                        categoryBreadcrumb += _localizationService.GetLocalized(breadcrumb[i], x => x.Name);
                        if (i != breadcrumb.Count - 1)
                            categoryBreadcrumb += " >> ";
                    }
                    categoriesModel.Add(new SearchModel.CategoryModel
                    {
                        Id = c.Id,
                        Breadcrumb = categoryBreadcrumb
                    });
                }
                return categoriesModel;
            });
            if (categories.Any())
            {
                //first empty entry
                advancedSearchRequest.AdvanceSearchResponse.AvailableCategories.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                //all other categories
                foreach (var c in categories)
                {
                    advancedSearchRequest.AdvanceSearchResponse.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = advancedSearchRequest.AdvanceSearchResponse.cid == c.Id
                    });
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
            if (manufacturers.Any())
            {
                advancedSearchRequest.AdvanceSearchResponse.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                foreach (var m in manufacturers)
                    advancedSearchRequest.AdvanceSearchResponse.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = _localizationService.GetLocalized(m, x => x.Name, languageId: advancedSearchRequest.LanguageId),
                        Selected = advancedSearchRequest.AdvanceSearchResponse.mid == m.Id
                    });
            }

            advancedSearchRequest.AdvanceSearchResponse.asv = _vendorSettings.AllowSearchByVendor;
            if (advancedSearchRequest.AdvanceSearchResponse.asv)
            {
                var vendors = _vendorService.GetAllVendors();
                if (vendors.Any())
                {
                    advancedSearchRequest.AdvanceSearchResponse.AvailableVendors.Add(new SelectListItem
                    {
                        Value = "0",
                        Text = _localizationService.GetResource("Common.All")
                    });
                    foreach (var vendor in vendors)
                        advancedSearchRequest.AdvanceSearchResponse.AvailableVendors.Add(new SelectListItem
                        {
                            Value = vendor.Id.ToString(),
                            Text = _localizationService.GetLocalized(vendor, x => x.Name, languageId: advancedSearchRequest.LanguageId),
                            Selected = advancedSearchRequest.AdvanceSearchResponse.vid == vendor.Id
                        });
                }
            }

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            var isSearchTermSpecified = false;
            try
            {
                // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
                isSearchTermSpecified = advancedSearchRequest.AdvanceSearchResponse.Q != null;
            }
            catch
            {
                //the "A potentially dangerous Request.QueryString value was detected from the client" exception could be thrown here when some wrong char is specified (e.g. <)
                //although we [ValidateInput(false)] attribute here we try to access "Request.Params" directly
                //that's why we do not re-throw it

                //just ensure that some search term is specified (0 length is not supported inthis case)
                isSearchTermSpecified = !string.IsNullOrEmpty(searchTerms);
            }
            if (isSearchTermSpecified)
            {
                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    advancedSearchRequest.AdvanceSearchResponse.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    int vendorId = 0;
                    if (advancedSearchRequest.AdvanceSearchResponse.adv)
                    {
                        //advanced search
                        var categoryId = advancedSearchRequest.AdvanceSearchResponse.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (advancedSearchRequest.AdvanceSearchResponse.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(GetChildCategoryIds(categoryId,currentCustomer));
                            }
                        }

                        manufacturerId = advancedSearchRequest.AdvanceSearchResponse.mid;

                        //min price
                        if (!string.IsNullOrEmpty(advancedSearchRequest.AdvanceSearchResponse.pf))
                        {
                            if (decimal.TryParse(advancedSearchRequest.AdvanceSearchResponse.pf, out decimal minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                        }
                        //max price
                        if (!string.IsNullOrEmpty(advancedSearchRequest.AdvanceSearchResponse.pt))
                        {
                            if (decimal.TryParse(advancedSearchRequest.AdvanceSearchResponse.pt, out decimal maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                        }

                        if (advancedSearchRequest.AdvanceSearchResponse.asv)
                            vendorId = advancedSearchRequest.AdvanceSearchResponse.vid;

                        searchInDescriptions = advancedSearchRequest.AdvanceSearchResponse.sid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;

                    //products
                    products = _productService.SearchProducts(
                        categoryIds: categoryIds,
                        manufacturerId: manufacturerId,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        priceMin: minPriceConverted,
                        priceMax: maxPriceConverted,
                        keywords: searchTerms,
                        searchDescriptions: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: _workContext.WorkingLanguage.Id,
                        orderBy: (ProductSortingEnum)advancedSearchRequest.CatalogPagingResponse.OrderBy,
                        pageIndex: advancedSearchRequest.CatalogPagingResponse.PageNumber - 1,
                        pageSize: advancedSearchRequest.CatalogPagingResponse.PageSize,
                        vendorId: vendorId);
                    advancedSearchRequest.AdvanceSearchResponse.Products = PrepareProductOverviewModels(advancedSearchRequest.StoreId, advancedSearchRequest.CurrencyId, advancedSearchRequest.LanguageId, currentCustomer, products).ToList();

                    advancedSearchRequest.AdvanceSearchResponse.NoResults = !advancedSearchRequest.AdvanceSearchResponse.Products.Any();

                    //search term statistics
                    if (!string.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms, _storeContext.CurrentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = _storeContext.CurrentStore.Id,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new ProductSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = _workContext.WorkingLanguage.Id,
                        VendorId = vendorId
                    });
                }
            }

            advancedSearchRequest.AdvanceSearchResponse.PagingFilteringContext.LoadPagedList(products);
            return Ok(advancedSearchRequest.AdvanceSearchResponse);
        }

        /*Added by sree for loading products based on CategoryId 21_01_2019 End*/
        [HttpPost]
        public virtual IActionResult GetProductByCategory([FromBody]ProductByCategoryRequest productByCategoryRequest)
        {
            if (productByCategoryRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);

            }
            var currentCustomer = _customerService.GetCustomerByGuid(productByCategoryRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(productByCategoryRequest.CurrencyId);

            var category = _categoryService.GetCategoryById(productByCategoryRequest.CategoryId);
            if (category == null || category.Deleted)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryEmptyOrDeleted")).BadRequest();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryNotPublishedOrUnauthorized")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(category, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoAccessToCategory")).BadRequest();

            //Store mapping
            if (!_storeMappingService.Authorize(category, productByCategoryRequest.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.StoreMappinUnauthorized")).BadRequest();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(currentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                productByCategoryRequest.StoreId);

            var model = new CustomCategoryProductModel();
            var customerRolesIds = currentCustomer.CustomerRoles
            .Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                //We cache a value indicating whether we have featured products
                IPagedList<Product> featuredProducts = null;
                string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HAS_FEATURED_PRODUCTS_KEY, productByCategoryRequest.CategoryId,
                    string.Join(",", customerRolesIds), productByCategoryRequest.StoreId);
                var hasFeaturedProductsCache = _cacheManager.Get(cacheKey, () =>
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       storeId: productByCategoryRequest.StoreId,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                    return featuredProducts.TotalCount > 0;
                });
                if (hasFeaturedProductsCache && featuredProducts == null)
                {
                    //cache indicates that the category has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       storeId: productByCategoryRequest.StoreId,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = PrepareProuctsModel(productByCategoryRequest.StoreId, productByCategoryRequest.CurrencyId, productByCategoryRequest.LanguageId, currentCustomer, featuredProducts).ToList();
                }
            }

            var categoryIds = new List<int>
            {
                category.Id
            };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(currentCustomer, productByCategoryRequest.StoreId, category.Id));
            }
            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds, false,
                categoryIds: categoryIds,
                storeId: productByCategoryRequest.StoreId,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                orderBy: (ProductSortingEnum)productByCategoryRequest.CatalogPagingResponse.OrderBy,
                pageIndex: productByCategoryRequest.CatalogPagingResponse.PageNumber - 1,
                pageSize: productByCategoryRequest.CatalogPagingResponse.PageSize);

            model.Products = PrepareProuctsModelWithCategoryName(productByCategoryRequest.CategoryId, productByCategoryRequest.StoreId, productByCategoryRequest.CurrencyId, productByCategoryRequest.LanguageId, currentCustomer, products).ToList();
            var categoryData = _categoryService.GetCategoryById(productByCategoryRequest.CategoryId);
            int languageId = productByCategoryRequest.LanguageId;
            model.CategoryName = categoryData.Name;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name));

            return Ok(model);
        }
        /*Added by sree for loading products based on CategoryId 21_01_2019 End*/


        [HttpPost]
        public virtual IActionResult GetProductByVendorId([FromBody]ProductByVendorRequest productByVendorRequest)
        {
            if (productByVendorRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }
            var currentCustomer = _customerService.GetCustomerByGuid(productByVendorRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(productByVendorRequest.CurrencyId);

            var category = _categoryService.GetCategoryById(productByVendorRequest.CategoryId);
            if (category == null || category.Deleted)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryEmptyOrDeleted")).BadRequest();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryNotPublishedOrUnauthorized")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(category,currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoAccessToCategory")).BadRequest();

            //Store mapping
            if (!_storeMappingService.Authorize(category,productByVendorRequest.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.StoreMappinUnauthorized")).BadRequest();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(currentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                productByVendorRequest.StoreId);

            var model = new CategoryListingResponse();
            var customerRolesIds = currentCustomer.CustomerRoles
            .Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                //We cache a value indicating whether we have featured products
                IPagedList<Product> featuredProducts = null;
                string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HAS_FEATURED_PRODUCTS_KEY, productByVendorRequest.CategoryId,
                    string.Join(",", customerRolesIds), productByVendorRequest.StoreId);
                var hasFeaturedProductsCache = _cacheManager.Get(cacheKey, () =>
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       vendorId: productByVendorRequest.VendorId,
                       storeId: productByVendorRequest.StoreId,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                    return featuredProducts.TotalCount > 0;
                });
                if (hasFeaturedProductsCache && featuredProducts == null)
                {
                    //cache indicates that the category has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       storeId: productByVendorRequest.StoreId,
                       vendorId: productByVendorRequest.VendorId,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = PrepareProductOverviewModels(productByVendorRequest.StoreId, productByVendorRequest.CurrencyId, productByVendorRequest.LanguageId, currentCustomer, featuredProducts).ToList();
                }
            }

            var categoryIds = new List<int>
            {
                category.Id
            };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(currentCustomer, productByVendorRequest.StoreId, category.Id));
            }

            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                categoryIds: categoryIds,
                storeId: productByVendorRequest.StoreId,
                vendorId: productByVendorRequest.VendorId,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                orderBy: (ProductSortingEnum)productByVendorRequest.CatalogPagingResponse.OrderBy,
                pageIndex: productByVendorRequest.CatalogPagingResponse.PageNumber - 1,
                pageSize: productByVendorRequest.CatalogPagingResponse.PageSize);
            model.Products = PrepareProductOverviewModels(productByVendorRequest.StoreId, productByVendorRequest.CurrencyId, productByVendorRequest.LanguageId, currentCustomer, products).ToList();
            model.CustomerGuid = currentCustomer.CustomerGuid;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name));

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult GetProductByManufacturerId([FromBody]ProductByManufacturerRequest productByManufacturerRequest)
        {
            //CatalogController->Manufacturer
            if (productByManufacturerRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(productByManufacturerRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(productByManufacturerRequest.CurrencyId);

            var manufacturer = _manufacturerService.GetManufacturerById(productByManufacturerRequest.ManufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return new ResponseObject(_localizationService.GetResource("plugins.xcellenceit.restapi.message.manufactureremptyordeleted"))
                    .BadRequest();

            var notAvailable =
             //published?
             !manufacturer.Published ||
             //ACL (access control list) 
             !_aclService.Authorize(manufacturer,currentCustomer) ||
             //Store mapping
             !_storeMappingService.Authorize(manufacturer,productByManufacturerRequest.StoreId);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return new ResponseObject(_localizationService
                    .GetResource("Plugins.XcellenceIT.WebApiClient.Message.ManufacturerNotPublishedOrUnauthorized"))
                    .BadRequest();

            //model
            //var model = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
            var model = PrepareManufacturerModel(manufacturer, productByManufacturerRequest, currentCustomer);
            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult LoadFilter([FromBody]LoadFilterRequest loadFilterRequest)
        {
            if (loadFilterRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(loadFilterRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(loadFilterRequest.CurrencyId);

            var category = _categoryService.GetCategoryById(loadFilterRequest.CategoryId);
            if (category == null || category.Deleted)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryEmptyOrDeleted")).BadRequest();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryNotPublishedOrUnauthorized")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(category,currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoAccessToCategory")).BadRequest();

            //Store mapping
            if (!_storeMappingService.Authorize(category,loadFilterRequest.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.StoreMappinUnauthorized")).BadRequest();

            var model = new CategoryListingResponse
            {

                //price ranges
                PriceRangeFilters = LoadPriceRangeFilters(category.PriceRanges, _webHelper, _priceFormatter)
            };

            var customerRolesIds = currentCustomer.CustomerRoles
                .Where(cr => cr.Active).Select(cr => cr.Id).ToList();

            var categoryIds = new List<int>
            {
                category.Id
            };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(currentCustomer, loadFilterRequest.StoreId, category.Id));
            }
            //products
            IList<int> alreadyFilteredSpecOptionIds = GetAlreadyFilteredSpecOptionIds(_webHelper);
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds, true,
                categoryIds: categoryIds,
                storeId: loadFilterRequest.StoreId,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false);
            //specs
            model.SpecificationFilter = PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
            filterableSpecificationAttributeOptionIds, loadFilterRequest.LanguageId);

            model.CustomerGuid = currentCustomer.CustomerGuid;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name));

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult LoadFilter2([FromBody]LoadFilterRequest loadFilterRequest)
        {
            if (loadFilterRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(loadFilterRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(loadFilterRequest.CurrencyId);

            var category = _categoryService.GetCategoryById(loadFilterRequest.CategoryId);
            if (category == null || category.Deleted)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryEmptyOrDeleted")).BadRequest();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryNotPublishedOrUnauthorized")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(category,currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoAccessToCategory")).BadRequest();

            //Store mapping
            if (!_storeMappingService.Authorize(category,loadFilterRequest.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.StoreMappinUnauthorized")).BadRequest();

            var model = new CategoryListingLoadFilterResponse
            {

                //price ranges
                PriceRangeFilters = LoadPriceRangeFilters(category.PriceRanges, _webHelper, _priceFormatter)
            };

            var customerRolesIds = currentCustomer.CustomerRoles
                .Where(cr => cr.Active).Select(cr => cr.Id).ToList();

            var categoryIds = new List<int>
            {
                category.Id
            };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(currentCustomer, loadFilterRequest.StoreId, category.Id));
            }
            //products
            IList<int> alreadyFilteredSpecOptionIds = GetAlreadyFilteredSpecOptionIds(_webHelper);
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds, true,
                categoryIds: categoryIds,
                storeId: loadFilterRequest.StoreId,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false);
            //specs
            model.SpecificationFilter = PrepareSpecsLoadFilters(alreadyFilteredSpecOptionIds,
            filterableSpecificationAttributeOptionIds, loadFilterRequest.LanguageId);

            model.CustomerGuid = currentCustomer.CustomerGuid;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name));

            return Ok(model);
        }

        [HttpPost]
        public virtual IActionResult ApplyFilter([FromBody]ApplyFilterRequest applyFilterRequest)
        {
            if (applyFilterRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var currentCustomer = _customerService.GetCustomerByGuid(applyFilterRequest.CustomerGUID);
            if (currentCustomer == null)
                currentCustomer = _customerService.InsertGuestCustomer();
            var workingCurrency = _currencyService.GetCurrencyById(applyFilterRequest.CurrencyId);

            var category = _categoryService.GetCategoryById(applyFilterRequest.CategoryId);
            if (category == null || category.Deleted)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryEmptyOrDeleted")).BadRequest();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories.SystemName, currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.CategoryNotPublishedOrUnauthorized")).BadRequest();

            //ACL (access control list)
            if (!_aclService.Authorize(category,currentCustomer))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.NoAccessToCategory")).BadRequest();

            //Store mapping
            if (!_storeMappingService.Authorize(category,applyFilterRequest.StoreId))
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.StoreMappinUnauthorized")).BadRequest();

            var model = new CategoryListingResponse();

            var customerRolesIds = currentCustomer.CustomerRoles
                .Where(cr => cr.Active).Select(cr => cr.Id).ToList();

            var categoryIds = new List<int>
            {
                category.Id
            };

            //include subcategories
            if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                categoryIds.AddRange(GetChildCategoryIds(category.Id,currentCustomer));

            //products
            IList<int> alreadyFilteredSpecOptionIds = applyFilterRequest.specIds;
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds, true,
                    categoryIds: categoryIds,
                    storeId: applyFilterRequest.StoreId,
                    visibleIndividuallyOnly: true,
                    featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                    priceMin: applyFilterRequest.MinPrice,
                    priceMax: applyFilterRequest.MaxPrice,
                    orderBy: (ProductSortingEnum)applyFilterRequest.CatalogPagingResponse.OrderBy,
                    pageIndex: applyFilterRequest.CatalogPagingResponse.PageNumber - 1,
                    pageSize: applyFilterRequest.CatalogPagingResponse.PageSize,
                    filteredSpecs: alreadyFilteredSpecOptionIds);
            model.Products = PrepareProductOverviewModels(applyFilterRequest.StoreId, applyFilterRequest.CurrencyId, applyFilterRequest.LanguageId, currentCustomer, products).ToList();

            //specs
            PrepareFilterItems(alreadyFilteredSpecOptionIds, filterableSpecificationAttributeOptionIds, model, applyFilterRequest.LanguageId);

            //sorting
            model.PagingFilteringContext = new CatalogPagingFilteringResponse();
            PrepareSortingOptions(applyFilterRequest.LanguageId, model.PagingFilteringContext, applyFilterRequest.CatalogPagingResponse);
            model.PagingFilteringContext.LoadPagedList(products);

            model.CustomerGuid = currentCustomer.CustomerGuid;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name));

            return Ok(model);

        }

        [HttpPost]
        public virtual IActionResult VendorAll([FromBody]VendorAllRequest vendorAllRequest)
        {
            if (vendorAllRequest == null)
                return new ResponseObject(_localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.MessageCannotBeEmpty")).BadRequest();

            if (!ModelState.IsValid)
            {
                _logger.Error(ModelState.ToString());
                return new UnprocessableEntity(ModelState);
            }

            var model = new List<VendorAllResponse>();
            var vendors = _vendorService.GetAllVendors();
            foreach (var vendor in vendors)
            {
                var vendorModel = new VendorAllResponse
                {
                    Id = vendor.Id,
                    Name = _localizationService.GetLocalized(vendor, x => x.Name, vendorAllRequest.LanguageId),
                    Description = _localizationService.GetLocalized(vendor, x => x.Description, vendorAllRequest.LanguageId),
                    MetaKeywords = _localizationService.GetLocalized(vendor, x => x.MetaKeywords, vendorAllRequest.LanguageId),
                    MetaDescription = _localizationService.GetLocalized(vendor, x => x.MetaDescription, vendorAllRequest.LanguageId),
                    MetaTitle = _localizationService.GetLocalized(vendor, x => x.MetaTitle, vendorAllRequest.LanguageId),
                    SeName = _urlRecordService.GetSeName(vendor, languageId: vendorAllRequest.LanguageId),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
                };

                //prepare picture model
                var pictureSize = _mediaSettings.VendorThumbPictureSize;
                var pictureCacheKey = string.Format(ModelCacheEventConsumer.VENDOR_PICTURE_MODEL_KEY, vendor.Id, pictureSize,
                    true, vendorAllRequest.LanguageId, _webHelper.IsCurrentConnectionSecured(), vendorAllRequest.StoreId);
                vendorModel.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(vendor.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };
                    return pictureModel;
                });
                model.Add(vendorModel);
            }

            return Ok(model);
        }

        /*Added by sree for loading products based on Category 19_01_2019 Start*/
        [HttpPost]
        public virtual IActionResult GetProductByParentCategoryId([FromBody]ProductByCategoryRequest productByCategoryRequest)
        {
            var products = _categoryService.GetProductsForGrocer(productByCategoryRequest.CategoryId);

            var categories = _categoryService.GetAllCategoriesByParentCategoryId(productByCategoryRequest.CategoryId);

            CategoryListProductModel categoryListProductModel = new CategoryListProductModel();
            categoryListProductModel.subCategories = new List<ProductSubCategory>();
            
            
            foreach (var category in categories)
            {
                categoryListProductModel.productSubCategory = new ProductSubCategory();
                categoryListProductModel.productSubCategory.Id = category.Id;
                categoryListProductModel.productSubCategory.Name = category.Name;
                categoryListProductModel.productSubCategory.Products = new List<CustomCategoryProductModel>();
                foreach (var product in products)
                {
                    if (!product.Product.Deleted)
                    {
                        if (product.Category.ParentCategoryId == category.Id)
                        {
                            CustomCategoryProductModel customCategoryProductModel = new CustomCategoryProductModel();
                            customCategoryProductModel.CategoryId = product.CategoryId;
                            customCategoryProductModel.ProductId = product.ProductId;
                            customCategoryProductModel.ParentSubCategoryId = product.Category.ParentCategoryId;
                            customCategoryProductModel.ManufacturerName = product.Product.ProductManufacturers.FirstOrDefault() == null ? null : product.Product.ProductManufacturers.FirstOrDefault().Manufacturer.Name;
                            customCategoryProductModel.ProductName = product.Product.Name;
                            customCategoryProductModel.Price = product.Product.Price;
                            customCategoryProductModel.ShortDescription = product.Product.ShortDescription;
                            customCategoryProductModel.Sku = product.Product.Sku;
                            customCategoryProductModel.ApprovedRatingSum = product.Product.ApprovedRatingSum;
                            customCategoryProductModel.ApprovedTotalReviews = product.Product.ApprovedTotalReviews;
                            customCategoryProductModel.AverageProductRating = (customCategoryProductModel.ApprovedRatingSum != 0) ? (customCategoryProductModel.ApprovedRatingSum / customCategoryProductModel.ApprovedTotalReviews) : 0;
                            int pictureSize = _mediaSettings.ProductThumbPictureSize;
                            //prepare picture model
                            var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.ProductId, pictureSize, true, 1, _webHelper.IsCurrentConnectionSecured(), 2);
                            customCategoryProductModel.DefaultPictureModel = new PictureModel();
                            customCategoryProductModel.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                            {
                                var picture = _pictureService.GetPicturesByProductId(product.ProductId, 1).FirstOrDefault();
                                var pictureModel = new PictureModel
                                {
                                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                                    ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), product.Product.Name),
                                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), product.Product.Name)
                                };
                                return pictureModel;
                            });

                            categoryListProductModel.productSubCategory.Products.Add(customCategoryProductModel);
                        }
                    }
                    
                }

                categoryListProductModel.subCategories.Add(categoryListProductModel.productSubCategory);
            }


            return Ok(categoryListProductModel);
        }
        /*Added by sree for loading products based on ParentCategory 19_01_2019 End*/

        /*Added by sree for loading products based on CategoryId 21_01_2019 Start*/
        [NonAction]
        public IList<CustomCategoryProductModel> PrepareProuctsModel(int storeId, int currencyId, int languageId, Customer currentCustomer,
         IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true,
         int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);

            if (products == null)
                return null;
           
            var models = new List<CustomCategoryProductModel>();


            foreach (var product in products)
            {
                var model = new CustomCategoryProductModel();
                
                model.ProductId = product.Id;
                model.ProductName = _localizationService.GetLocalized(product, x => x.Name, languageId: languageId);
                model.ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, languageId: languageId);
                model.ManufacturerId = product.ProductManufacturers.FirstOrDefault() == null ? 0 : product.ProductManufacturers.FirstOrDefault().ManufacturerId;
                model.ManufacturerDescription = product.ProductManufacturers.FirstOrDefault() == null ? "" : product.ProductManufacturers.FirstOrDefault().Manufacturer.Description;
                model.CategoryId = (product.ProductCategories.Count != 0) ? product.ProductCategories.FirstOrDefault().Id : 0;
                model.ParentSubCategoryId =(product.ProductCategories.Count!=0)?product.ProductCategories.FirstOrDefault().Category.ParentCategoryId:0;
                model.ManufacturerName = product.ProductManufacturers.FirstOrDefault() == null ? null : product.ProductManufacturers.FirstOrDefault().Manufacturer.Name;
                model.Price = product.Price;
                model.Sku = product.Sku;
                model.ApprovedRatingSum = product.ApprovedRatingSum;
                model.ApprovedTotalReviews = product.ApprovedTotalReviews;
                model.AverageProductRating = (product.ApprovedRatingSum != 0) ? (product.ApprovedRatingSum / product.ApprovedTotalReviews) : 0;


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
                            Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.ProductName),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.ProductName)
                        };
                        return pictureModel;
                    });

                    #endregion
                }
                models.Add(model);
            }
            return models;
        }
        /*Added by sree for loading products based on CategoryId 21_01_2019 End*/

        /*Added by Surakshith for loading products based on CategoryId with Category Name Start*/
        [NonAction]
        public IList<CustomCategoryProductModel> PrepareProuctsModelWithCategoryName(int CategoryId,int storeId, int currencyId, int languageId, Customer currentCustomer,
         IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true,
         int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            var workingCurrency = _currencyService.GetCurrencyById(currencyId);

            if (products == null)
                return null;
            var catId = CategoryId;
            var category = _categoryService.GetCategoryById(catId);
            var models = new List<CustomCategoryProductModel>();
            foreach (var product in products)
            {
                var model = new CustomCategoryProductModel();
                model.CategoryId = category.Id;
                model.CategoryName = category.Name;
                model.ProductId = product.Id;
                model.ProductName = _localizationService.GetLocalized(product, x => x.Name, languageId: languageId);
                model.ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, languageId: languageId);
                model.ManufacturerId = product.ProductManufacturers.FirstOrDefault() == null ? 0 : product.ProductManufacturers.FirstOrDefault().ManufacturerId;
                model.ManufacturerDescription = product.ProductManufacturers.FirstOrDefault() == null ? "" : product.ProductManufacturers.FirstOrDefault().Manufacturer.Description;
                model.CategoryId = (product.ProductCategories.Count != 0) ? product.ProductCategories.FirstOrDefault().Id : 0;
                model.ParentSubCategoryId = (product.ProductCategories.Count != 0) ? product.ProductCategories.FirstOrDefault().Category.ParentCategoryId : 0;
                model.ManufacturerName = product.ProductManufacturers.FirstOrDefault() == null ? null : product.ProductManufacturers.FirstOrDefault().Manufacturer.Name;
                model.Price = product.Price;
                //Added by surakshith to get priceinclusion of tax start on 16-07-2020
                model.PriceIncludingTax = _taxService.GetProductPrice(product, product.Price, true, currentCustomer, out _);
                //Added by surakshith to get priceinclusion of tax end on 16-07-2020
                model.Sku = product.Sku;
                model.ApprovedRatingSum = product.ApprovedRatingSum;
                model.ApprovedTotalReviews = product.ApprovedTotalReviews;
                model.AverageProductRating = (product.ApprovedRatingSum != 0) ? (product.ApprovedRatingSum / product.ApprovedTotalReviews) : 0;


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
                            Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.ProductName),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.ProductName)
                        };
                        return pictureModel;
                    });

                    #endregion
                }
                models.Add(model);
            }
            return models;
        }
        /*Added by Surakshith for loading products based on CategoryId with Category Name End*/

        [HttpPost]
        public virtual IActionResult GlobalSearchForGourmet([FromBody]GlobalSearchRequest  globalSearchRequest)
        {
            if (globalSearchRequest.KeyWord.Length <= 2)
            {
                return BadRequest();
            }
            else
            {
                Customer currentCustomer = _customerService.GetCustomerByGuid(globalSearchRequest.CustomerGUID);
                if (currentCustomer == null)
                    currentCustomer = _customerService.InsertGuestCustomer();
                string Serachkeywords = globalSearchRequest.KeyWord;
                string[] words = Serachkeywords.Split(' ');
                List<CustomCategoryProductModel> customCategoryProductModel = new List<CustomCategoryProductModel>();
                foreach (string word in words)
                {
                    var products = _categoryService.GetGrocerProductsBasedOnSearch(word);
                    if (products.Count == 0)
                    {
                        return Ok(customCategoryProductModel);
                    }
                    else
                    {
                        customCategoryProductModel = PrepareProuctsModel(globalSearchRequest.StoreId, globalSearchRequest.CurrencyId,
                        globalSearchRequest.LanguageId, currentCustomer, products).ToList();
                    }
                }
                return Ok(customCategoryProductModel);
            }
        }

        //Added By surakshith for global serach for gourmet products based end on 18-06-2020

        #endregion
    }
}
