using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.RangosHorarios;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class RangosHorariosBusiness : BaseBusiness, IRangosHorariosBusiness
    {
        private readonly ILogger<RangosHorariosBusiness> _logger;
        private readonly IRangosHorariosRepository _rangosHorariosRepository;


        public RangosHorariosBusiness(
            IRangosHorariosRepository rangosHorariosRepository,
            ILogger<RangosHorariosBusiness> logger)
        {
            _rangosHorariosRepository = rangosHorariosRepository;
            _logger = logger;
        }


        public async Task New(RangosHorariosViewModel model)
        {
            var rango = _mapper.Map<TipoRangoHorario>(model);

            Validate(rango);

            var tran = _uow.BeginTransaction();

            try
            {
                rango.Descripcion = rango.Descripcion.ToUpper().Trim();
                rango.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                await _rangosHorariosRepository.Insert(rango, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task<RangosHorariosViewModel> Get(int idRango)
        {
            var rango = await _rangosHorariosRepository.GetById<TipoRangoHorario>(idRango);

            if (rango == null)
                return null;

            var model = _mapper.Map<RangosHorariosViewModel>(rango);

            return model;
        }

        public async Task<DataTablesResponse<Custom.TipoRangoHorario>> GetAll(DataTablesRequest request)
        {
            var customQuery = _rangosHorariosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Append($"AND IdEspecialidad > {0}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.TipoRangoHorario>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }


        public async Task Update(RangosHorariosViewModel model)
        {
            var rango = _mapper.Map<TipoRangoHorario>(model);

            Validate(rango);

            var dbrango = await _rangosHorariosRepository.GetById<TipoRangoHorario>(rango.IdRangoHorario);

            if (dbrango == null)
            {
                throw new BusinessException("Rango Horario Inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbrango.Descripcion = rango.Descripcion.ToUpper().Trim();
                dbrango.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _rangosHorariosRepository.Update(dbrango, tran);

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
            var rango = await _rangosHorariosRepository.GetById<TipoRangoHorario>(idRubro);

            if (rango == null)
            {
                throw new BusinessException("Rango Horario inexistente");
            }

            rango.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _rangosHorariosRepository.Update(rango);
        }


        private void Validate(TipoRangoHorario rango)
        {
            if (rango.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Descripción para el Rngo Horario");
            }
        }

    }
}