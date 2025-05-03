using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Asientos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class AsientosBusiness: BaseBusiness, IAsientosBusiness
    {
        private readonly ILogger<AsientosBusiness> _logger;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;
        private readonly ITiposAsientosRepository _tiposAsientosRepository;

        public AsientosBusiness(
            ILogger<AsientosBusiness> logger,
            IAsientosRepository asientosRepository,
            IEjerciciosRepository ejerciciosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            ITiposAsientosRepository tiposAsientosRepository)
        {
            _logger = logger;
            _asientosRepository = asientosRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _tiposAsientosRepository = tiposAsientosRepository;
        }

        public async Task<AsientosViewModel> Get(int idAsiento)
        {
            var asiento = await _asientosRepository.GetById<Asiento>(idAsiento);

            if (asiento == null)
                return null;

            var items = await _asientosDetalleRepository.GetAllByIdAsiento(idAsiento);

            var model = _mapper.Map<AsientosViewModel>(asiento);
            model.Items = _mapper.Map<List<AsientosDetalle>>(items);

            return model;
        }

        public async Task<Custom.Asiento> GetCustom(int idAsiento)
        {
            var asiento = await _asientosRepository.GetByIdCustom(idAsiento);
            var items = await _asientosDetalleRepository.GetAllByIdAsiento(idAsiento);

            asiento.Items = items;

            return asiento;
        }

        public async Task<DataTablesResponse<Custom.Asiento>> GetAll(DataTablesRequest request)
        {
            var customQuery = _asientosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEjercicio"))
                builder.Append($"AND IdEjercicio = {filters["IdEjercicio"]}");

            if (filters.ContainsKey("Descripcion"))
                builder.Append($"AND Descripcion LIKE {string.Concat("%", filters["Descripcion"], "%")}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");
            builder.Append($"AND IdTipoAsiento IS NOT NULL");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Asiento>(request, customQuery.Sql, customQuery.Parameters, true,staticWhere: builder.Sql);
        }

        public async Task New(AsientosViewModel model)
        {
            var asiento = _mapper.Map<Asiento>(model);
            var items = _mapper.Map<List<AsientoDetalle>>(model.Items);

            Validate(asiento, items);

            var tran = _uow.BeginTransaction();

            try
            {
                asiento.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                asiento.Descripcion = asiento.Descripcion.ToUpper().Trim();
                asiento.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                asiento.IdUsuario = _permissionsBusiness.Value.User.Id;
                asiento.IdTipoAsiento = model.IdTipoAsiento;
                asiento.FechaIngreso = DateTime.Now;

                var id = await _asientosRepository.Insert(asiento, tran);

                foreach (var item in items)
                {
                    if (item.Debitos != 0 || item.Creditos != 0)
                    {
                        item.IdAsiento = (int)id;
                        item.Detalle = item.Detalle.ToUpper().Trim();

                        await _asientosDetalleRepository.Insert(item, tran);
                    }
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Remove(int idAsiento)
        {
            var asiento = await _asientosRepository.GetById<Asiento>(idAsiento);

            if (asiento == null)
            {
                throw new BusinessException("Asiento inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _asientosDetalleRepository.DeleteByIdAsiento(idAsiento, tran);
                await _asientosRepository.Update(asiento, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }


        public async Task Update(AsientosViewModel model)
        {
            var asiento = _mapper.Map<Asiento>(model);
            var items = _mapper.Map<List<AsientoDetalle>>(model.Items);

            Validate(asiento, items);

            var dbAsiento = await _asientosRepository.GetById<Asiento>(asiento.IdAsiento);

            if (dbAsiento == null)
            {
                throw new BusinessException("Asiento Contable inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbAsiento.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbAsiento.IdEjercicio = asiento.IdEjercicio;
                dbAsiento.Descripcion = asiento.Descripcion.ToUpper().Trim();
                dbAsiento.Fecha = asiento.Fecha;
                dbAsiento.IdTipoAsiento = asiento.IdTipoAsiento;
                dbAsiento.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _asientosRepository.Update(dbAsiento, tran);
                await _asientosDetalleRepository.DeleteByIdAsiento(dbAsiento.IdAsiento, tran);

                foreach (var item in items)
                {
                    if (item.Debitos != 0 || item.Creditos != 0)
                    {
                        item.IdAsiento = dbAsiento.IdAsiento;
                        item.Detalle = item.Detalle.ToUpper().Trim();

                        await _asientosDetalleRepository.Insert(item, tran);
                    }
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }
        public async Task<List<AsientosDetalle>> GetCuentasByTipoAsiento(int idTipoAsiento)
        {
            List<AsientosDetalle> detalle = new List<AsientosDetalle>();
            var cuentas = await _tiposAsientosRepository.GetItemsByIdTipoAsiento(idTipoAsiento);
            var iItem = 1;
            foreach (var cuenta in cuentas) { 
                detalle.Add(new AsientosDetalle { 
                    IdAsiento = 0,
                    Item = iItem,
                    IdCuentaContable = cuenta.IdCuentaContable,
                    Detalle = cuenta.Descripcion
                });
                iItem++;
            }

            return detalle;
        }

        private void Validate(Asiento asiento, List<AsientoDetalle> items)
        {
            if (asiento.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(asiento.Descripcion));
            }

            if (_ejerciciosRepository.EjercicioCerrado(asiento.IdEjercicio.Value, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(asiento.IdEjercicio.Value, asiento.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Asiento no es Valida para el Ejercicio seleccionado. Verifique.");
            }

            if(items.Sum(d => d.Debitos) != items.Sum(d => d.Creditos))
            {
                throw new BusinessException("El Asiento está Desbalanceado. Verifique.");
            }
        }
    }
}
