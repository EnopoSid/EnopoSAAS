using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Catalog
{
    public partial interface IFcartService
    {
        void InsertFreshCart(FCart fCart);

        ///<summary>
        ///Update freshCartItem
        ///</summary>
        ///<param name="freshCart">FreshCart</param>
        void UpdateFreshCartItem(FCart freshCart);

        ///<summary>
        ///Gets freshCart Item Based on the ShoppingCartId
        /// </summary>
        /// <param name="ShoppingCartId">ShoppingCartId</param>
        FCart GetFreshCartByShoppingCartId(int ShoppingCartId, int mealNo);

        FCart GetFreshCartByShoppingCartId(int ShoppingCartId);

        IList<FCart> GetFreshCartByCustomerId(int customerId);

        void RemoveCartByShoppingCartId(int fCartId);
    }

    public partial class FcartService: IFcartService
    {
        private readonly IRepository<FCart> _freshCartRepository = EngineContext.Current.Resolve<IRepository<FCart>>();

        public FcartService()
        {

        }

        /// <summary>
        /// Inserts Item into FreshCart
        /// </summary>
        /// <param name="freshCart">FreshCart</param>
        public virtual void InsertFreshCart(FCart fCart)
        {
            if (fCart == null)
                throw new ArgumentNullException(nameof(FCart));

            //if (freshCart is IEntityForCaching)
            //    throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _freshCartRepository.Insert(fCart);

           
        }


        /// <summary>
        /// Updates an Item in FreshCart
        /// </summary>
        /// <param name="freshCart">>FreshCart</param>
        public virtual void UpdateFreshCartItem(FCart freshCart)
        {
            if (freshCart == null)
                throw new ArgumentNullException(nameof(freshCart));

            _freshCartRepository.Update(freshCart);
        }

        public virtual FCart GetFreshCartByShoppingCartId(int ShoppingCartId,int mealNo)
        {
            var query = _freshCartRepository.Table;
            query = query.Where(c => c.ShoppingCartId == ShoppingCartId && c.MealNo==mealNo);
            var freshCart = query.FirstOrDefault();
            return freshCart;
        }

        public virtual FCart GetFreshCartByShoppingCartId(int ShoppingCartId)
        {
            var query = _freshCartRepository.Table;
            query = query.Where(c => c.ShoppingCartId == ShoppingCartId);
            var freshCart = query.FirstOrDefault();
            return freshCart;
        }

        public virtual IList<FCart> GetFreshCartByCustomerId(int customerId)
        {
            var query = _freshCartRepository.Table;
            query = query.Where(c => c.CustomerId == customerId);
            var fCart = query.ToList();
            return fCart;
        }

        public virtual void RemoveCartByShoppingCartId(int fCartId)
        {
            var query = _freshCartRepository.Table;
            query = query.Where(c => c.Id == fCartId);

            _freshCartRepository.Delete(query);
        }

    }
}
