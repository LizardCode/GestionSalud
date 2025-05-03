using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.RubrosArticulos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class RubrosArticulosBusiness: BaseBusiness, IRubrosArticulosBusiness
    {
        private readonly ILogger<RubrosArticulosBusiness> _logger;
        private readonly IRubrosArticulosRepository _rubrosArticulosRepository;


        public RubrosArticulosBusiness(
            IRubrosArticulosRepository rubrosArticulosRepository,
            ILogger<RubrosArticulosBusiness> logger)
        {
            _rubrosArticulosRepository = rubrosArticulosRepository;
            _logger = logger;
        }


        public async Task New(RubrosArticulosViewModel model)
        {
            var rubro = _mapper.Map<RubroArticulo>(model);

            Validate(rubro);

            var tran = _uow.BeginTransaction();

            try
            {
                rubro.Descripcion = rubro.Descripcion.ToUpper().Trim();
                rubro.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                rubro.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task<RubrosArticulosViewModel> Get(int idRubro)
        {
            var rubro = await _rubrosArticulosRepository.GetById<CentroCosto>(idRubro);

            if (rubro == null)
                return null;

            var model = _mapper.Map<RubrosArticulosViewModel>(rubro);

            return model;
        }

        public async Task<DataTablesResponse<RubroArticulo>> GetAll(DataTablesRequest request)
        {
            var customQuery = _rubrosArticulosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");
            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<RubroArticulo>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }
            

        public async Task Update(RubrosArticulosViewModel model)
        {
            var rubro = _mapper.Map<RubroArticulo>(model);

            Validate(rubro);

            var dbRubro = await _rubrosArticulosRepository.GetById<RubroArticulo>(rubro.IdRubroArticulo);

            if (dbRubro == null)
            {
                throw new BusinessException("Rubro de Articulos inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbRubro.Descripcion = rubro.Descripcion.ToUpper().Trim();
                dbRubro.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbRubro.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _rubrosArticulosRepository.Update(dbRubro, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task Remove(int idRubro)
        {
            var rubro = await _rubrosArticulosRepository.GetById<RubroArticulo>(idRubro);

            if (rubro == null)
            {
                throw new BusinessException("Rubro de Articulos inexistente");
            }

            rubro.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _rubrosArticulosRepository.Update(rubro);
        }


        private void Validate(RubroArticulo rubro)
        {
            if (rubro.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Descripción para el Rubro de Articulos");
            }
        }

    }
}
