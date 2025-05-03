using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Financiadores;
using LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PrestacionesFinanciadorBusiness : BaseBusiness, IPrestacionesFinanciadorBusiness
    {
        private readonly ILogger<PrestacionesFinanciadorBusiness> _logger;
        private readonly IFinanciadoresPrestacionesRepository _prestacionesFinanciadorRepository;
        private readonly IFinanciadoresPlanesRepository _financiadoresPlanesRepository;
        private readonly IPrestacionesRepository _prestacionesRepository;
        private readonly IFinanciadoresPrestacionesProfesionalesRepository _financiadoresPrestacionesProfesionalesRepository;

        public PrestacionesFinanciadorBusiness(ILogger<PrestacionesFinanciadorBusiness> logger,
                                                IFinanciadoresPrestacionesRepository prestacionesFinanciadorRepository,
                                                IFinanciadoresPlanesRepository financiadoresPlanesRepository,
                                                IPrestacionesRepository prestacionesRepository,
                                                IFinanciadoresPrestacionesProfesionalesRepository financiadoresPrestacionesProfesionalesRepository)
        {
            _logger = logger;
            _prestacionesFinanciadorRepository = prestacionesFinanciadorRepository;
            _financiadoresPlanesRepository = financiadoresPlanesRepository;
            _prestacionesRepository = prestacionesRepository;
            _financiadoresPrestacionesProfesionalesRepository = financiadoresPrestacionesProfesionalesRepository;
        }

        public async Task New(PrestacionViewModel model, int idFinanciador)
        {
            var prestacion = _mapper.Map<FinanciadorPrestacion>(model);

            Validate(prestacion);

            prestacion.IdFinanciador = idFinanciador;
            prestacion.Codigo = prestacion.Codigo.ToUpper().Trim();
            prestacion.Descripcion = prestacion.Descripcion.ToUpper().Trim();

            var financiadorPlan = await _financiadoresPlanesRepository.GetById<FinanciadorPlan>(model.IdFinanciadorPlan);
            if (financiadorPlan == null)
                throw new BusinessException("Plan no encontrado.");

            if (financiadorPlan.IdFinanciador != idFinanciador)
                throw new BusinessException("El plan no pertenece al financiador.");

            prestacion.IdFinanciadorPlan = model.IdFinanciadorPlan;
            prestacion.CodigoFinanciadorPlan = financiadorPlan.Codigo;

            if (model.IdPrestacion > 0)
            { 
                var prestacionInterna = await _prestacionesRepository.GetById<Prestacion>(model.IdPrestacion);
                if (prestacionInterna == null)
                    throw new BusinessException("Prestación Interna no encontrada.");

                prestacion.IdPrestacion = prestacionInterna.IdPrestacion;
                prestacion.CodigoPrestacion = prestacionInterna.Codigo;
            }
            else
            {
                prestacion.IdPrestacion = null;
                prestacion.CodigoPrestacion = null;
            }

            prestacion.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            var id = await _prestacionesFinanciadorRepository.Insert(prestacion, tran);
            await UpdateProfesionales(id, model.Profesionales, tran);

            tran.Commit();
        }

        public async Task<PrestacionViewModel> Get(int idPrestacion)
        {
            var prestacion = await _prestacionesFinanciadorRepository.GetById<FinanciadorPrestacion>(idPrestacion);

            if (prestacion == null)
                return null;

            var model = _mapper.Map<PrestacionViewModel>(prestacion);

            var profesionales = await _financiadoresPrestacionesProfesionalesRepository.GetAllByIdFinanciadorPrestacion(idPrestacion);
            model.Profesionales = _mapper.Map<List<PrestacionFinanciadorProfesionalViewModel>>(profesionales);

            return model;
        }

        public async Task<DataTablesResponse<Custom.FinanciadorPrestacion>> GetAll(DataTablesRequest request, int idFinanciador)
        {
            var customQuery = _prestacionesFinanciadorRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdFinanciadorPlan"))
                builder.Append($"AND IdFinanciadorPlan = {filters["IdFinanciadorPlan"]}");

            if (filters.ContainsKey("IdPrestacion"))
                builder.Append($"AND IdPrestacion = {filters["IdPrestacion"]}");

            builder.Append($"AND IdFinanciador = {idFinanciador} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.FinanciadorPrestacion>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(PrestacionViewModel model, int idFinanciador)
        {
            var prestacion = _mapper.Map<FinanciadorPrestacion>(model);

            Validate(prestacion);

            var dbPrestacion = await _prestacionesFinanciadorRepository.GetById<FinanciadorPrestacion>(prestacion.IdFinanciadorPrestacion);

            if (dbPrestacion == null)
            {
                throw new ArgumentException("Prestacion inexistente");
            }

            dbPrestacion.Codigo = prestacion.Codigo.ToUpper().Trim();
            dbPrestacion.Descripcion = prestacion.Descripcion.ToUpper().Trim();
            dbPrestacion.Valor = prestacion.Valor;

            dbPrestacion.ValorFijo = prestacion.ValorFijo;
            dbPrestacion.Porcentaje = prestacion.Porcentaje;
            dbPrestacion.CoPago = prestacion.CoPago;

            if (model.IdPrestacion > 0)
            {
                if (dbPrestacion.IdPrestacion != model.IdPrestacion)
                { 
                    var prestacionInterna = await _prestacionesRepository.GetById<Prestacion>(model.IdPrestacion);
                    if (prestacionInterna == null)
                        throw new BusinessException("Prestación Interna no encontrada.");

                    dbPrestacion.IdPrestacion = prestacionInterna.IdPrestacion;
                    dbPrestacion.CodigoPrestacion = prestacionInterna.Codigo;
                }
            }
            else
            {
                dbPrestacion.IdPrestacion = null;
                dbPrestacion.CodigoPrestacion = null;
            }

            dbPrestacion.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _prestacionesFinanciadorRepository.Update(dbPrestacion, tran);

            await UpdateProfesionales(dbPrestacion.IdFinanciadorPrestacion, model.Profesionales, tran);

            tran.Commit();
        }

        public async Task Remove(int idPrestacion)
        {
            var prestacion = await _prestacionesFinanciadorRepository.GetById<FinanciadorPrestacion>(idPrestacion);

            if (prestacion == null)
            {
                throw new ArgumentException("Prestacion inexistente");
            }

            prestacion.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _prestacionesFinanciadorRepository.Update(prestacion);
            await _financiadoresPrestacionesProfesionalesRepository.RemoveByIdFinanciadorPrestacion(prestacion.IdFinanciadorPrestacion);
        }

        private void Validate(FinanciadorPrestacion prestacion)
        {
            if (prestacion.Codigo.IsNull())
            {
                throw new BusinessException(nameof(prestacion.Codigo));
            }

            if (prestacion.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(prestacion.Descripcion));
            }
        }

        public async Task<FinanciadorPrestacion> GetByCodigo(string codigo)
        {
            codigo = codigo.ToUpperInvariant().Trim();

            var prestacion = await _prestacionesFinanciadorRepository.GetByCodigo(codigo);

            return prestacion;
        }

        public async Task<bool> ValidarCodigo(string codigo)
        {
            codigo = codigo.ToUpperInvariant().Trim();

            var prestacion = await _prestacionesFinanciadorRepository.GetByCodigo(codigo);

            return (prestacion == null);
        }

        public async Task<PrestacionesImportarExcelResultViewModel> ImportarPrestacionesExcel(IFormFile file, int idFinanciador)
        {
            var iResults = new PrestacionesImportarExcelResultViewModel();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<FinanciadorPrestacionXLSViewModel>();

                //if (excelRows.Count > 50)
                //    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

                var planes = await _financiadoresPlanesRepository.GetAll<FinanciadorPlan>();
                var prestacionesInternas = await _prestacionesRepository.GetAll<Prestacion>();

                var resultados = new List<PrestacionViewModel>();
                foreach (var item in excelRows)
                {
                    var plan = planes.FirstOrDefault(f => f.IdFinanciador == idFinanciador && f.Codigo == item.CodigoPlan);
                    if (plan == null)
                    {
                        throw new BusinessException($"No se encontró el plan con código: {item.CodigoPlan}.");
                    }

                    Prestacion prestacionInterna = null;                    
                    if (!string.IsNullOrEmpty(item.NomencladorInterno))
                    {
                        prestacionInterna = prestacionesInternas.FirstOrDefault(f => f.Codigo == item.NomencladorInterno);
                        if (prestacionInterna == null)
                        {
                            throw new BusinessException($"No se encontró la prestación interna con nomenclador: {item.NomencladorInterno}.");
                        }
                    }

                    resultados.Add(new PrestacionViewModel                    
                    {
                        CodigoPlan = item.CodigoPlan,
                        IdFinanciadorPlan = plan.IdFinanciadorPlan,
                        NomencladorInterno = item.NomencladorInterno,
                        IdPrestacion = prestacionInterna?.IdPrestacion ?? 0,

                        Codigo = item.Nomenclador,
                        Descripcion = item.Descripcion,
                        Valor = item.Valor,

                        ValorFijo = item.ValorFijo,
                        Porcentaje = item.Porcentaje,
                        CoPago = item.CoPago
                    });
                }

                if (resultados == null || resultados.Count == 0)
                    throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

                var tran = _uow.BeginTransaction();
                foreach (var resultado in resultados)
                {
                    iResults.Cantidad++;
                    iResults.Procesados++;
                    var prestacion = await _prestacionesFinanciadorRepository.GetByCodigo(resultado.Codigo.ToUpperInvariant(), tran);

                    if (prestacion != null && prestacion.IdFinanciadorPlan == resultado.IdFinanciadorPlan)
                    {
                        prestacion.Descripcion = resultado.Descripcion;
                        prestacion.Valor = resultado.Valor;
                        prestacion.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                        prestacion.IdFinanciadorPlan = resultado.IdFinanciadorPlan;
                        prestacion.CodigoFinanciadorPlan = resultado.CodigoPlan;
                        prestacion.IdPrestacion = resultado.IdPrestacion; 
                        prestacion.CodigoPrestacion = resultado.NomencladorInterno;

                        prestacion.ValorFijo = resultado.ValorFijo;
                        prestacion.Porcentaje = resultado.Porcentaje;
                        prestacion.CoPago = resultado.CoPago;

                        await _prestacionesFinanciadorRepository.Update(prestacion, tran);
                        iResults.Actualizados++;
                    }
                    else
                    {
                        await _prestacionesFinanciadorRepository.Insert(new FinanciadorPrestacion
                        {
                            Codigo = resultado.Codigo,
                            Descripcion = resultado.Descripcion,
                            Valor = resultado.Valor,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,

                            IdFinanciador = idFinanciador,
                            IdFinanciadorPlan = resultado.IdFinanciadorPlan,
                            CodigoFinanciadorPlan = resultado.CodigoPlan,
                            IdPrestacion = resultado.IdPrestacion,
                            CodigoPrestacion = resultado.NomencladorInterno,

                            ValorFijo = resultado.ValorFijo,
                            Porcentaje = resultado.Porcentaje,
                            CoPago = resultado.CoPago
                        }, tran);

                        iResults.Nuevos++;
                    }
                }
                tran.Commit();

                return iResults;
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, null);
                throw ex;
            }
            catch (InvalidCastException ex)
            {
                _logger.LogError(ex, null);
                throw new BusinessException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        private async Task UpdateProfesionales(long idFinanciadorPrestacion, List<PrestacionFinanciadorProfesionalViewModel> profesionales, IDbTransaction tran)
        {
            //Borro valores actuales
            await _financiadoresPrestacionesProfesionalesRepository.RemoveByIdFinanciadorPrestacion(idFinanciadorPrestacion, tran);

            if (profesionales != null && profesionales.Count > 0)
            { 
                foreach (var profesional in profesionales)
                {
                    await _financiadoresPrestacionesProfesionalesRepository.Insert(new FinanciadorPrestacionProfesional() { 
                        IdFinanciadorPrestacion = idFinanciadorPrestacion,
                        Codigo = string.Empty,
                        IdProfesional = profesional.IdProfesional,
                        ValorFijo = profesional.ValorFijo,
                        Porcentaje = profesional.Porcentaje
                    }, tran);
                }
            }
        }
    }
}