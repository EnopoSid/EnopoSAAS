using Nop.Core.Data;
using Nop.Core.Domain.Fresh;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nop.Services.Fresh
{
    public partial class FreshPackageTypesService: IFreshPackageTypesService
    {
        private readonly IRepository<FreshPackageTypes> _freshPackageTypesRepository = EngineContext.Current.Resolve<IRepository<FreshPackageTypes>>();

        public FreshPackageTypesService()
        {

        }

        public List<FreshPackageTypes> GetPackagingTypes()
        {
            var query = _freshPackageTypesRepository.Table;
            var packagingTypes = query.ToList();
            return packagingTypes;
        }

        public FreshPackageTypes GetPackageTypeById(int id)
        {
            var query = _freshPackageTypesRepository.Table.Where(x => x.Id == id).FirstOrDefault();
            return query;
        }
    }
}
