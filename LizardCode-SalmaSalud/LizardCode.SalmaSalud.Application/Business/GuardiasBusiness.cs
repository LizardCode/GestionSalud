using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Guardias;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    internal class GuardiasBusiness : BaseBusiness, IGuardiasBusiness
    {
        private readonly ILogger<GuardiasBusiness> _logger;
        private readonly IGuardiasRepository _guardiasRepository;

        public GuardiasBusiness(IGuardiasRepository guardiasRepository, ILogger<GuardiasBusiness> logger)
        {
            _guardiasRepository = guardiasRepository;
            _logger = logger;
        }

        public async Task<GuardiasViewModel> Get(int idGuardia)
        {
            var guardia = await _guardiasRepository.GetById<Guardia>(idGuardia);
            if (guardia == null)
                return null;

            var model = _mapper.Map<GuardiasViewModel>(guardia);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Guardia>> GetAll(DataTablesRequest request)
        {
            var customQuery = _guardiasRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaFiltroDesde") && filters["FechaFiltroDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaFiltroDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaFiltroHasta") && filters["FechaFiltroHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaFiltroHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND Fecha <= {date.AddDays(1)}");
            }

            if (filters.ContainsKey("IdProfesional"))
                builder.Append($"AND IdProfesional = {filters["IdProfesional"]}");

            if (filters.ContainsKey("IdEstadoGuardia"))
                builder.Append($"AND IdEstadoGuardia = {filters["IdEstadoGuardia"]}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Guardia>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task<Custom.Guardia> GetCustomById(int idGuardia)
            => await _guardiasRepository.GetCustomById(idGuardia);


        public async Task New(GuardiasViewModel model)
        {
            var guardia = _mapper.Map<Guardia>(model);

            Validate(guardia);

            var profesionales = await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa);
            var profesional = profesionales.Where(p => p.IdProfesional == model.IdProfesional);
            if (profesional == null)
            {
                throw new BusinessException("No se encontró el Profesional.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                guardia.Fecha = DateTime.Now;
                guardia.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                guardia.IdUsuario = _permissionsBusiness.Value.User.Id;
                guardia.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                guardia.IdEstadoGuardia = (int)EstadoGuardia.Pendiente;

                guardia.Total = model.Total;

                await _guardiasRepository.Insert(guardia, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idGuardia)
        {
            var guardia = await _guardiasRepository.GetById<Guardia>(idGuardia);

            if (guardia == null)
            {
                throw new ArgumentException("Liquidación inexistente.");
            }

            if (guardia.IdEstadoGuardia == (int)EstadoGuardia.Liquidada)
            {
                throw new BusinessException("El registro se encuentra en una liquidación. No se puede eliminar.");
            }

            var guardiaLiquidada = await _guardiasRepository.ValidarGuardiaLiquidada(idGuardia);
            if (guardiaLiquidada)
            {
                throw new BusinessException("Ya se ha liquidado la guardía. No se puede eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                guardia.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _guardiasRepository.Update(guardia, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        private void Validate(Guardia liquidacion)
        {
            if (liquidacion.IdProfesional == 0)
            {
                throw new BusinessException("Ingrese un profesional válido.");
            }
        }
    }
}
