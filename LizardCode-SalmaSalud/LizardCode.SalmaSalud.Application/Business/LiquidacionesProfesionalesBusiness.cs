using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using LizardCode.SalmaSalud.Application.Models.LiquidacionesProfesionales;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class LiquidacionesProfesionalesBusiness : BaseBusiness, ILiquidacionesProfesionalesBusiness
    {
        private readonly ILogger<LiquidacionesProfesionalesBusiness> _logger;
        private readonly ILiquidacionesProfesionalesRepository _liquidacionesProfesionalesRepository;
        private readonly ILiquidacionesProfesionalesPrestacionesRepository _liquidacionesProfesionalesPrestacionesRepository;
        private readonly IEvolucionesPrestacionesRepository _evolucionesPrestacionesRepository;
        private readonly IEvolucionesOtrasPrestacionesRepository _evolucionesOtrasPrestacionesRepository;
        private readonly IGuardiasRepository _guardiasRepository;

        public LiquidacionesProfesionalesBusiness(ILiquidacionesProfesionalesRepository liquidacionesProfesionalesProfesionales,
                                                    ILiquidacionesProfesionalesPrestacionesRepository liquidacionesProfesionalesPrestacionesRepository,
                                                    IEvolucionesPrestacionesRepository evolucionesPrestacionesRepository,
                                                    IEvolucionesOtrasPrestacionesRepository evolucionesOtrasPrestacionesRepository,
                                                    IGuardiasRepository guardiasRepository,
                                                    ILogger<LiquidacionesProfesionalesBusiness> logger)
        {
            _liquidacionesProfesionalesRepository = liquidacionesProfesionalesProfesionales;
            _liquidacionesProfesionalesPrestacionesRepository = liquidacionesProfesionalesPrestacionesRepository;
            _evolucionesPrestacionesRepository = evolucionesPrestacionesRepository;
            _evolucionesOtrasPrestacionesRepository = evolucionesOtrasPrestacionesRepository;
            _guardiasRepository = guardiasRepository;
            _logger = logger;
        }

        public async Task<LiquidacionProfesionalViewModel> Get(int idLiquidacionProfesional)
        {
            var liquidacion = await _liquidacionesProfesionalesRepository.GetById<LiquidacionProfesional>(idLiquidacionProfesional);
            if (liquidacion == null)
                return null;

            var model = _mapper.Map<LiquidacionProfesionalViewModel>(liquidacion);

            var prestaciones = await _liquidacionesProfesionalesPrestacionesRepository.GetAllByIdLiquidacionProfesional(idLiquidacionProfesional);
            model.Prestaciones = _mapper.Map<List<LiquidacionProfesionalPrestacionViewModel>>(prestaciones);

            return model;
        }

        public async Task<DataTablesResponse<Custom.LiquidacionProfesional>> GetAll(DataTablesRequest request)
        {
            var customQuery = _liquidacionesProfesionalesRepository.GetAllCustomQuery();
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

            if (filters.ContainsKey("IdEstadoLiquidacionProfesional"))
                builder.Append($"AND IdEstadoLiquidacionProfesional = {filters["IdEstadoLiquidacionProfesional"]}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.LiquidacionProfesional>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task<Custom.LiquidacionProfesional> GetCustomById(int idLiquidacionProfesional)
            => await _liquidacionesProfesionalesRepository.GetCustomById(idLiquidacionProfesional);


        public async Task New(LiquidacionProfesionalViewModel model)
        {
            var liquidacion = _mapper.Map<LiquidacionProfesional>(model);

            Validate(liquidacion);

            if (model.Prestaciones == null || model.Prestaciones.Count == 0)
            {
                throw new BusinessException("No se aplicaron Prestaciones.");
            }

            var profesionales = await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa);
            var profesional = profesionales.Where(p => p.IdProfesional == model.IdProfesional);
            if (profesional == null)
            {
                throw new BusinessException("No se encontró el Profesional.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                liquidacion.Fecha = DateTime.Now;
                liquidacion.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                liquidacion.IdUsuario = _permissionsBusiness.Value.User.Id;
                liquidacion.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                liquidacion.IdEstadoLiquidacionProfesional = (int)EstadoLiquidacionProfesional.Pendiente;

                liquidacion.Total = model.Prestaciones.Sum(s => s.Total);
                
                var id = await _liquidacionesProfesionalesRepository.Insert(liquidacion, tran);

                //Prestaciones
                if (model.Prestaciones != null && model.Prestaciones.Count > 0)
                {
                    foreach (var p in model.Prestaciones)
                    {
                        if (p.IdEvolucionPrestacion > 0)
                        {
                            var ep = await _evolucionesPrestacionesRepository.GetById<EvolucionPrestacion>(p.IdEvolucionPrestacion.Value, tran);

                            if (ep != null)
                            {
                                await _liquidacionesProfesionalesPrestacionesRepository.Insert(new LiquidacionProfesionalPrestacion
                                {
                                    IdLiquidacion = (int)id,
                                    IdPrestacion = p.IdEvolucionPrestacion,
                                    Descripcion = p.Descripcion,
                                    Valor = ep.Valor,
                                    ValorFijo = ep.ValorFijo ?? 0,
                                    Porcentaje = ep.Porcentaje ?? 0,
                                    ValorPorcentaje = (ep.Valor * ep.Porcentaje ?? 0) / 100,
                                    Total = p.Total,                                    
                                }, tran);
                            }
                        }

                        if (p.IdEvolucionOtraPrestacion > 0)
                        {
                            var eop = await _evolucionesOtrasPrestacionesRepository.GetById<EvolucionOtraPrestacion>(p.IdEvolucionOtraPrestacion.Value, tran);

                            await _liquidacionesProfesionalesPrestacionesRepository.Insert(new LiquidacionProfesionalPrestacion
                            {
                                IdLiquidacion = (int)id,
                                IdOtraPrestacion = p.IdEvolucionOtraPrestacion,
                                Descripcion = p.Descripcion,
                                Valor = eop.Valor,
                                ValorFijo = eop.ValorFijo ?? 0,
                                Porcentaje = eop.Porcentaje ?? 0,
                                ValorPorcentaje = (eop.Valor * eop.Porcentaje ?? 0) / 100,
                                Total = p.Total,
                            }, tran);
                        }

                        if (p.IdGuardia > 0)
                        {
                            var gal = await _evolucionesOtrasPrestacionesRepository.GetById<Guardia>(p.IdGuardia.Value, tran);

                            await _liquidacionesProfesionalesPrestacionesRepository.Insert(new LiquidacionProfesionalPrestacion
                            {
                                IdLiquidacion = (int)id,
                                IdGuardia = p.IdGuardia,
                                Descripcion = p.Descripcion,
                                Valor = gal.Total,
                                ValorFijo = 0,
                                Porcentaje = 0,
                                ValorPorcentaje = 0,
                                Total = p.Total,
                            }, tran);
                        }
                    }
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

        public async Task Remove(int idLiquidacionProfesional)
        {
            var liquidacion = await _liquidacionesProfesionalesRepository.GetById<LiquidacionProfesional>(idLiquidacionProfesional);

            if (liquidacion == null)
            {
                throw new ArgumentException("Liquidación inexistente.");
            }

            if (liquidacion.IdEstadoLiquidacionProfesional == (int)EstadoLiquidacionProfesional.Facturada)
            {
                throw new BusinessException("Ya se ha facturado la liquidación. No se puede eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                liquidacion.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _liquidacionesProfesionalesRepository.Update(liquidacion, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<List<LiquidacionProfesionalPrestacionViewModel>> GetPrestacionesALiquidar(DateTime desde, DateTime hasta, int idProfesional)
        {
            List<LiquidacionProfesionalPrestacionViewModel> lppModel = new List<LiquidacionProfesionalPrestacionViewModel>();

            hasta = hasta.Date;
            hasta = hasta.AddHours(23).AddMinutes(59);
            var ep = await _evolucionesPrestacionesRepository.GetPrestacionesALiquidar(desde, hasta, idProfesional);
            if (ep != null || ep.Count > 0)
            {
                foreach (var p in ep)
                {
                    var fijo = p.ValorFijo ?? 0;
                    var porcentaje = p.Porcentaje ?? 0;
                    var valorPorcentaje = (p.Valor * porcentaje) / 100;

                    lppModel.Add(new LiquidacionProfesionalPrestacionViewModel
                    {
                        IdLiquidacionProfesional = 0,
                        IdEvolucionPrestacion = p.IdEvolucionPrestacion,
                        Descripcion = p.DescripcionLiquidacion,
                        Financiador = p.Financiador,
                        Valor = p.Valor,
                        Fijo = fijo,
                        Porcentaje = porcentaje,
                        ValorPorcentaje = valorPorcentaje,
                        Total = fijo + valorPorcentaje
                    });
                }                
            }

            var eop = await _evolucionesOtrasPrestacionesRepository.GetPrestacionesALiquidar(desde, hasta, idProfesional);
            if (eop != null || eop.Count > 0)
            {
                foreach (var p in eop)
                {
                    var fijo = p.ValorFijo ?? 0;
                    var porcentaje = p.Porcentaje ?? 0;
                    var valorPorcentaje = (p.Valor * porcentaje) / 100;

                    lppModel.Add(new LiquidacionProfesionalPrestacionViewModel
                    {
                        IdLiquidacionProfesional = 0,
                        IdEvolucionOtraPrestacion = p.IdEvolucionPrestacion,
                        Descripcion = p.DescripcionLiquidacion,
                        Financiador = "(PARTICULAR)",
                        Valor = p.Valor,
                        Fijo = fijo,
                        Porcentaje = porcentaje,
                        ValorPorcentaje = valorPorcentaje,
                        Total = fijo + valorPorcentaje
                    });
                }
            }

            var gal = await _guardiasRepository.GetGuardiasALiquidar(desde, hasta, idProfesional);
            if (gal != null || gal.Count > 0)
            {
                foreach (var g in gal)
                {
                    var fijo = 0;
                    var porcentaje =0;
                    var valorPorcentaje = 0;

                    lppModel.Add(new LiquidacionProfesionalPrestacionViewModel
                    {
                        IdLiquidacionProfesional = 0,
                        IdGuardia = g.IdGuardia,
                        Descripcion = g.DescripcionLiquidacion,
                        Financiador = "(GUARDIA)",
                        Valor = g.Total,
                        Fijo = fijo,
                        Porcentaje = porcentaje,
                        ValorPorcentaje = valorPorcentaje,
                        Total = g.Total
                    });
                }
            }

            return lppModel;
        }

        private void Validate(LiquidacionProfesional liquidacion)
        {
            if (liquidacion.IdProfesional == 0)
            {
                throw new BusinessException("Ingrese un profesional válido.");
            }
        }
    }
}
