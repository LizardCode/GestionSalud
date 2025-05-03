using Dapper.DataTables.Interfaces;
using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Application.Interfaces.Context;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class BaseBusiness
    {
        protected readonly IMapper _mapper;
        protected readonly IDbContext _dbContext;
        protected readonly IUnitOfWork _uow;
        protected readonly IDataTablesService _dataTablesService;
        protected readonly Lazy<ILookupsBusiness> _lookupsBusiness;
        protected readonly Lazy<IPermisosBusiness> _permissionsBusiness;

        public BaseBusiness()
        {
            _mapper = HttpContextHelper.Current.RequestServices.GetService<IMapper>();
            _dbContext = HttpContextHelper.Current.RequestServices.GetService<IDbContext>();
            _uow = HttpContextHelper.Current.RequestServices.GetService<IUnitOfWork>();
            _dataTablesService = HttpContextHelper.Current.RequestServices.GetService<IDataTablesService>();
            _lookupsBusiness = HttpContextHelper.Current.RequestServices.GetService<Lazy<ILookupsBusiness>>();
            _permissionsBusiness = HttpContextHelper.Current.RequestServices.GetService<Lazy<IPermisosBusiness>>();
        }
    }
}
