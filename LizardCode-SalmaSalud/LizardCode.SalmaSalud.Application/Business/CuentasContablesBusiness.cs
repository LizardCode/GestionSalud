using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CuentasContables;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CuentasContablesBusiness: BaseBusiness, ICuentasContablesBusiness
    {
        private readonly ICuentasContablesRepository _cuentasContablesRepository;

        public CuentasContablesBusiness(ICuentasContablesRepository cuentasContablesRepository)
        {
            _cuentasContablesRepository = cuentasContablesRepository;
        }


        public async Task New(CuentasContablesViewModel model)
        {
            var cta = _mapper.Map<CuentaContable>(model);

            Validate(cta);

            cta.Descripcion = cta.Descripcion.ToUpper().Trim();
            cta.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
            cta.IdCodigoObservacion = cta.IdCodigoObservacion == 0 ? default : cta.IdCodigoObservacion;
            cta.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            await _cuentasContablesRepository.Insert(cta, tran);

            tran.Commit();
        }

        public async Task<CuentasContablesViewModel> Get(int idCuenta)
        {
            var cta = await _cuentasContablesRepository.GetById<CuentaContable>(idCuenta);

            if (cta == null)
                return null;

            var model = _mapper.Map<CuentasContablesViewModel>(cta);

            return model;
        }

        public async Task<DataTablesResponse<Custom.CuentaContable>> GetAll(DataTablesRequest request)
        {
            var customQuery = _cuentasContablesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.CuentaContable>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(CuentasContablesViewModel model)
        {
            var cta = _mapper.Map<CuentaContable>(model);

            Validate(cta);

            var dbCuenta = await _cuentasContablesRepository.GetById<CuentaContable>(cta.IdCuentaContable);

            if (dbCuenta == null)
            {
                throw new BusinessException("Cuenta Contable inexistente");
            }

            dbCuenta.CodigoCuenta = cta.CodigoCuenta.PadLeft(8, '0').ToUpper().Trim();
            dbCuenta.Descripcion = cta.Descripcion.ToUpper().Trim();
            dbCuenta.IdRubroContable = cta.IdRubroContable;
            dbCuenta.IdCodigoObservacion = cta.IdCodigoObservacion == 0 ? default : cta.IdCodigoObservacion;
            dbCuenta.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
            dbCuenta.EsCtaGastos = cta.EsCtaGastos;
            dbCuenta.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _cuentasContablesRepository.Update(dbCuenta, tran);

            tran.Commit();
        }

        public async Task Remove(int idRubro)
        {
            var rubro = await _cuentasContablesRepository.GetById<CuentaContable>(idRubro);

            if (rubro == null)
            {
                throw new BusinessException("Cuenta Contable inexistente");
            }

            rubro.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _cuentasContablesRepository.Update(rubro);
        }

        private void Validate(CuentaContable cta)
        {
            if (cta.Descripcion.IsNull())
            {
                throw new BusinessException($"Ingrese una Descripción para la Cuenta Contale");
            }

            if (cta.CodigoCuenta.IsNull())
            {
                throw new BusinessException($"Ingrese un Codigo para la Cuenta Contable");
            }
        }
    }
}
