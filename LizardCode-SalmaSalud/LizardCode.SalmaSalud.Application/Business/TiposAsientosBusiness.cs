using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.TiposAsientos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class TiposAsientosBusiness: BaseBusiness, ITiposAsientosBusiness
    {
        private readonly ILogger<RubrosArticulosBusiness> _logger;
        private readonly ITiposAsientosRepository _tiposAsientosRepository;
        private readonly ITiposAsientosCuentasRepository _tiposAsientosCuentasRepository;

        public TiposAsientosBusiness(
            ITiposAsientosRepository tiposAsientosRepository,
            ITiposAsientosCuentasRepository tiposAsientosCuentasRepository,
            ILogger<RubrosArticulosBusiness> logger)
        {
            _tiposAsientosRepository = tiposAsientosRepository;
            _tiposAsientosCuentasRepository = tiposAsientosCuentasRepository;
            _logger = logger;
        }


        public async Task New(TiposAsientosViewModel model)
        {
            var tipoAsiento = _mapper.Map<TipoAsiento>(model);

            Validate(tipoAsiento);

            var tran = _uow.BeginTransaction();

            try
            {
                tipoAsiento.Descripcion = tipoAsiento.Descripcion.ToUpper().Trim();
                tipoAsiento.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                tipoAsiento.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                var idTipoAsiento = await _tiposAsientosRepository.Insert(tipoAsiento, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {

                    await _tiposAsientosCuentasRepository.Insert(new TipoAsientoCuenta
                    {
                        IdTipoAsiento = idTipoAsiento,
                        IdCuentaContable = item.IdCuentaContable,
                        Descripcion = item.Descripcion,
                        Item = iItem
                    }, tran);

                    iItem++;
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<TiposAsientosViewModel> Get(int idTipoAsiento)
        {
            var tipoAsiento = await _tiposAsientosRepository.GetById<TipoAsiento>(idTipoAsiento);

            if (tipoAsiento == null)
                return null;

            var model = _mapper.Map<TiposAsientosViewModel>(tipoAsiento);

            return model;
        }

        public async Task<TiposAsientosViewModel> GetCustom(int idTipoAsiento)
        {
            var model = await Get(idTipoAsiento);

            model.Items = _mapper.Map<List<TiposAsientosCuentas>>(await _tiposAsientosRepository.GetItemsByIdTipoAsiento(idTipoAsiento));

            return model;
        }

        public async Task<DataTablesResponse<TipoAsiento>> GetAll(DataTablesRequest request)
        {
            var customQuery = _tiposAsientosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<TipoAsiento>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }
            

        public async Task Update(TiposAsientosViewModel model)
        {
            var rubro = _mapper.Map<TipoAsiento>(model);

            Validate(rubro);

            var dbTipoAsiento = await _tiposAsientosRepository.GetById<TipoAsiento>(rubro.IdTipoAsiento);

            if (dbTipoAsiento == null)
            {
                throw new ArgumentException("Tipo de Asiento inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbTipoAsiento.Descripcion = rubro.Descripcion.ToUpper().Trim();
                dbTipoAsiento.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbTipoAsiento.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _tiposAsientosRepository.Update(dbTipoAsiento, tran);

                //Borro todas las cuentas ya generadas...
                await _tiposAsientosCuentasRepository.RemoveAllByIdTipoAsiento(dbTipoAsiento.IdTipoAsiento, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {

                    await _tiposAsientosCuentasRepository.Insert(new TipoAsientoCuenta
                    {
                        IdTipoAsiento = dbTipoAsiento.IdTipoAsiento,
                        IdCuentaContable = item.IdCuentaContable,
                        Descripcion = item.Descripcion,
                        Item = iItem
                    }, tran);

                    iItem++;
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idRubro)
        {
            var rubro = await _tiposAsientosRepository.GetById<TipoAsiento>(idRubro);

            if (rubro == null)
            {
                throw new ArgumentException("Tipo de Asiento inexistente");
            }

            rubro.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _tiposAsientosRepository.Update(rubro);
        }


        private void Validate(TipoAsiento tipoAsiento)
        {
            if (tipoAsiento.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(TipoAsiento.Descripcion));
            }
        }
    }
}
