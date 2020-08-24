using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services
{
    public partial class CustomDiscountService: ICustomDiscountService
    {

        #region Fields

        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Discount> _discountRepository;

        #endregion

        public CustomDiscountService(ICategoryService categoryService,
            IRepository<Category> categoryRepository,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository)
        {
            this._categoryRepository = categoryRepository;
            this._discountRepository = discountRepository;
        }

        public virtual List<Discount> GetDiscountsByName(List<String> discounts)
        {
            var query = _discountRepository.Table;
            List<Discount> discountInfo = new List<Discount>();
            foreach(var discount in discounts)
            {
                query = query.Where(x => x.CouponCode == discount);
                discountInfo.Add(query.SingleOrDefault());
            }
            return discountInfo;
        }
       
    }
}
