using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CentrosCosto;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CentrosCostoBusiness: BaseBusiness, ICentrosCostoBusiness
    {
        private readonly ILogger<CentrosCostoBusiness> _logger;
        private readonly ICentrosCostoRepository _centrosCostoRepository;

        public CentrosCostoBusiness(ICentrosCostoRepository centrosCostoRepository, ILogger<CentrosCostoBusiness> logger)
        {
            _centrosCostoRepository = centrosCostoRepository;
            _logger = logger;
        }

        public async Task New(CentrosCostoViewModel model)
        {
            var ceco = _mapper.Map<CentroCosto>(model);

            Validate(ceco);

            var tran = _uow.BeginTransaction();

            try
            {
                ceco.Descripcion = ceco.Descripcion.ToUpper().Trim();
                ceco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                ceco.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                await _centrosCostoRepository.Insert(ceco);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task<CentrosCostoViewModel> Get(int idCentroCosto)
        {
            var ceco = await _centrosCostoRepository.GetById<CentroCosto>(idCentroCosto);

            if (ceco == null)
                return null;

            var model = _mapper.Map<CentrosCostoViewModel>(ceco);
            
            return model;
        }

        public async Task<DataTablesResponse<CentroCosto>> GetAll(DataTablesRequest request)
        {
            var customQuery = _centrosCostoRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<CentroCosto>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }


        public async Task Update(CentrosCostoViewModel model)
        {
            var ceco = _mapper.Map<CentroCosto>(model);

            Validate(ceco);

            var dbCeco = await _centrosCostoRepository.GetById<CentroCosto>(ceco.IdCentroCosto);

            if (dbCeco == null)
            {
                throw new BusinessException("Centro de Costos inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbCeco.Descripcion = ceco.Descripcion.ToUpper().Trim();
                dbCeco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbCeco.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _centrosCostoRepository.Update(dbCeco, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task Remove(int idCentroCosto)
        {
            var ceco = await _centrosCostoRepository.GetById<CentroCosto>(idCentroCosto);

            if (ceco == null)
            {
                throw new BusinessException("Centro de Costos inexistente");
            }

            ceco.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _centrosCostoRepository.Update(ceco);
        }


        private void Validate(CentroCosto ceco)
        {
            if (ceco.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Descripción para el Centro de Costos");
            }
        }

    }
}
