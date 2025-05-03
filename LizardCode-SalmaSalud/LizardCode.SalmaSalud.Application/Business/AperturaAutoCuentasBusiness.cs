using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.AperturaAutoCuentas;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class AperturaAutoCuentasBusiness : BaseBusiness, IAperturaAutoCuentasBusiness
    {
        private readonly ILogger<AperturaAutoCuentasBusiness> _logger;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;
        private readonly IAsientosAperturaRepository _asientosAperturaRepository;
        private readonly IAsientosAperturaAsientoRepository _asientosAperturaAsientoRepository;


        public AperturaAutoCuentasBusiness(
            ILogger<AperturaAutoCuentasBusiness> logger,
            IAsientosRepository asientosRepository,
            IEjerciciosRepository ejerciciosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            IAsientosAperturaRepository asientosAperturaRepository,
            IAsientosAperturaAsientoRepository asientosAperturaAsientoRepository)
        {
            _logger = logger;
            _asientosRepository = asientosRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _asientosAperturaRepository = asientosAperturaRepository;
            _asientosAperturaAsientoRepository = asientosAperturaAsientoRepository;
        }

        public async Task<AperturaAutoCuentasViewModel> Get(int idAsientoApertura)
        {
            var asientoApertura = await _asientosAperturaRepository.GetById<AsientoAperturaCierre>(idAsientoApertura);

            if (asientoApertura == null)
                return null;

            var asiento = await _asientosAperturaAsientoRepository.GetByIdAsientoApertura(idAsientoApertura);
            var items = await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);

            var model = _mapper.Map<AperturaAutoCuentasViewModel>(asiento);
            model.Items = _mapper.Map<List<AperturaAutoCuentasDetalle>>(items);

            return model;
        }

        public async Task<Custom.AsientoAperturaCierre> GetCustom(int idAsientoApertura)
        {
            var asientoApertura = await _asientosAperturaRepository.GetByIdCustom(idAsientoApertura);
            var asiento = await _asientosAperturaAsientoRepository.GetByIdAsientoApertura(idAsientoApertura);

            asientoApertura.Items = await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento); ;

            return asientoApertura;
        }

        public async Task<DataTablesResponse<Custom.AsientoAperturaCierre>> GetAll(DataTablesRequest request)
        {
            var customQuery = _asientosAperturaRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEjercicio"))
                builder.Append($"AND IdEjercicio = {filters["IdEjercicio"]}");

            if (filters.ContainsKey("Descripcion"))
                builder.Append($"AND Descripcion LIKE {string.Concat("%", filters["Descripcion"], "%")}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");
            builder.Append($"AND IdTipoAsientoAuto = {TipoAsientoAuto.AsientoAperturaCuentas}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.AsientoAperturaCierre>(request, customQuery.Sql, customQuery.Parameters, true,staticWhere: builder.Sql);
        }

        public async Task New(AperturaAutoCuentasViewModel model)
        {
            var asientoApertura = _mapper.Map<AsientoAperturaCierre>(model);
            var items = _mapper.Map<List<AsientoDetalle>>(model.Items);

            Validate(asientoApertura, items);

            var tran = _uow.BeginTransaction();

            try
            {
                var ejercicio = await _ejerciciosRepository.GetById<Ejercicio>(asientoApertura.IdEjercicio);

                asientoApertura.Fecha = ejercicio.FechaInicio;
                asientoApertura.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                asientoApertura.Descripcion = asientoApertura.Descripcion.ToUpper().Trim();
                asientoApertura.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                asientoApertura.IdUsuario = _permissionsBusiness.Value.User.Id;
                asientoApertura.FechaIngreso = DateTime.Now;
                asientoApertura.IdTipoAsientoAuto = (int)TipoAsientoAuto.AsientoAperturaCuentas;

                var id = await _asientosAperturaRepository.Insert(asientoApertura, tran);

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = asientoApertura.IdEjercicio,
                    Descripcion = asientoApertura.Descripcion.ToUpper().Trim(),
                    Fecha = ejercicio.FechaInicio,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = asientoApertura.FechaIngreso
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                int iItem = 1;
                foreach (var item in items)
                {
                    if (item.Debitos != 0 || item.Creditos != 0)
                    {
                        item.IdAsiento = (int)idAsiento;
                        item.Item = iItem++;
                        item.Detalle = item.Detalle.ToUpper().Trim();

                        await _asientosDetalleRepository.Insert(item, tran);
                    }
                }

                await _asientosAperturaAsientoRepository.Insert(new AsientoAperturaCierreAsiento { IdAsientoAperturaCierre = (int)id, IdAsiento = (int)idAsiento }, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Remove(int idAsientoApertura)
        {
            var asientoApertura = await _asientosAperturaRepository.GetById<AsientoAperturaCierre>(idAsientoApertura);

            if (asientoApertura == null)
            {
                throw new BusinessException("Asiento inexistente");
            }

            var asientoAperturaAsiento = await _asientosAperturaAsientoRepository.GetByIdAsientoApertura(idAsientoApertura);
            var asiento = await _asientosRepository.GetById<Asiento>(asientoAperturaAsiento.IdAsiento);


            var tran = _uow.BeginTransaction();

            try
            {
                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _asientosDetalleRepository.DeleteByIdAsiento(asiento.IdAsiento, tran);
                await _asientosRepository.Update(asiento, tran);

                await _asientosAperturaAsientoRepository.DeleteByIdAsientoApertura(idAsientoApertura, tran);

                asientoApertura.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosAperturaRepository.Update(asientoApertura, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }


        public async Task Update(AperturaAutoCuentasViewModel model)
        {
            var asientoApertura = _mapper.Map<AsientoAperturaCierre>(model);
            var items = _mapper.Map<List<AsientoDetalle>>(model.Items);

            Validate(asientoApertura, items);

            var dbAsientoApertura = await _asientosAperturaRepository.GetById<AsientoAperturaCierre>(asientoApertura.IdAsientoAperturaCierre);

            if (dbAsientoApertura == null)
            {
                throw new BusinessException("Asiento Contable inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbAsientoApertura.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbAsientoApertura.Descripcion = asientoApertura.Descripcion.ToUpper().Trim();
                dbAsientoApertura.IdTipoAsientoAuto = asientoApertura.IdTipoAsientoAuto;
                dbAsientoApertura.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _asientosAperturaRepository.Update(dbAsientoApertura, tran);

                var asientoAperturaAsiento = await _asientosAperturaAsientoRepository.GetByIdAsientoApertura(dbAsientoApertura.IdAsientoAperturaCierre, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(asientoAperturaAsiento.IdAsiento, tran);

                asiento.Descripcion = asientoApertura.Descripcion.ToUpper().Trim();
                asiento.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _asientosRepository.Update(asiento, tran);

                await _asientosDetalleRepository.DeleteByIdAsiento(asientoAperturaAsiento.IdAsiento, tran);

                foreach (var item in items)
                {
                    if (item.Debitos != 0 || item.Creditos != 0)
                    {
                        item.IdAsiento = asientoAperturaAsiento.IdAsiento;
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

        private void Validate(AsientoAperturaCierre asientoApertura, List<AsientoDetalle> items)
        {
            if (asientoApertura.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(asientoApertura.Descripcion));
            }

            if (_ejerciciosRepository.EjercicioCerrado(asientoApertura.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if(items.Sum(d => d.Debitos) != items.Sum(d => d.Creditos))
            {
                throw new BusinessException("El Asiento está Desbalanceado. Verifique.");
            }
        }
    }
}
