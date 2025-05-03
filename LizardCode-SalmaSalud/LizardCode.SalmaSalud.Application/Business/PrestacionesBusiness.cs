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
using LizardCode.SalmaSalud.Application.Models.Prestaciones;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PrestacionesBusiness : BaseBusiness, IPrestacionesBusiness
    {
        private readonly ILogger<PrestacionesBusiness> _logger;
        private readonly IPrestacionesRepository _prestacionesRepository;
        private readonly IPrestacionesProfesionalesRepository _prestacionesProfesionalesRepository;

        public PrestacionesBusiness(IPrestacionesRepository prestacionesRepository,
                                    ILogger<PrestacionesBusiness> logger,
                                    IPrestacionesProfesionalesRepository prestacionesProfesionalesRepository)
        {
            _prestacionesRepository = prestacionesRepository;
            _logger = logger;
            _prestacionesProfesionalesRepository = prestacionesProfesionalesRepository;
        }

        public async Task New(PrestacionViewModel model)
        {
            var prestacion = _mapper.Map<Prestacion>(model);

            Validate(prestacion);

            prestacion.Codigo = prestacion.Codigo.ToUpper().Trim();
            prestacion.Descripcion = prestacion.Descripcion.ToUpper().Trim();
            prestacion.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            prestacion.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;

            var tran = _uow.BeginTransaction();

            var id = await _prestacionesRepository.Insert(prestacion, tran);

            await UpdateProfesionales(id, model.Profesionales, tran);    

            tran.Commit();
        }

        public async Task<PrestacionViewModel> Get(int idPrestacion)
        {
            var prestacion = await _prestacionesRepository.GetById<Prestacion>(idPrestacion);

            if (prestacion == null)
                return null;

            var model = _mapper.Map<PrestacionViewModel>(prestacion);

            var profesionales = await _prestacionesProfesionalesRepository.GetAllByIdPrestacion(idPrestacion);
            model.Profesionales = _mapper.Map<List<PrestacionProfesionalViewModel>>(profesionales);

            return model;
        }

        //public async Task<DataTablesResponse<Custom.Prestacion>> GetAll(DataTablesRequest request) =>
        //    await _dataTablesService.Resolve<Custom.Prestacion>(request);

        public async Task<DataTablesResponse<Prestacion>> GetAll(DataTablesRequest request)
        {
            var customQuery = _prestacionesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            //if (filters.ContainsKey("FiltroCUIT"))
            //    builder.Append($"AND CUIT = {filters["FiltroCUIT"]}");

            //if (filters.ContainsKey("FiltroNombre"))
            //    builder.Append($"AND Nombre LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            builder.Append($"AND idEmpresa = {_permissionsBusiness.Value.User.IdEmpresa} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Prestacion>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(PrestacionViewModel model)
        {
            var prestacion = _mapper.Map<Prestacion>(model);

            Validate(prestacion);

            var dbPrestacion = await _prestacionesRepository.GetById<Prestacion>(prestacion.IdPrestacion);

            if (dbPrestacion == null)
            {
                throw new ArgumentException("Prestacion inexistente");
            }

            dbPrestacion.Codigo = prestacion.Codigo.ToUpper().Trim();
            dbPrestacion.Descripcion = prestacion.Descripcion.ToUpper().Trim();
            dbPrestacion.Valor = prestacion.Valor;
            dbPrestacion.ValorFijo = prestacion.ValorFijo;
            dbPrestacion.Porcentaje = prestacion.Porcentaje;
            dbPrestacion.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _prestacionesRepository.Update(dbPrestacion, tran);

            await UpdateProfesionales(dbPrestacion.IdPrestacion, model.Profesionales, tran);

            tran.Commit();
        }

        public async Task Remove(int idPrestacion)
        {
            var prestacion = await _prestacionesRepository.GetById<Prestacion>(idPrestacion);

            if (prestacion == null)
            {
                throw new ArgumentException("Prestacion inexistente");
            }

            prestacion.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _prestacionesRepository.Update(prestacion);
        }

        private void Validate(Prestacion prestacion)
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
        public async Task<bool> ValidarCodigo(string codigo)
        {
            codigo = codigo.ToUpperInvariant().Trim();

            var prestacion = await _prestacionesRepository.GetByCodigo(codigo);            

            return (prestacion == null);
        }

        public async Task<PrestacionesImportarExcelResultViewModel> ImportarPrestacionesExcel(IFormFile file)
        {
            var iResults = new PrestacionesImportarExcelResultViewModel();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<PrestacionXLSViewModel>();

                //if (excelRows.Count > 50)
                //    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

                var resultados = new List<PrestacionViewModel>();
                foreach (var item in excelRows)
                {
                    resultados.Add(new PrestacionViewModel
                    {
                        Codigo = item.Codigo,
                        Descripcion = item.Descripcion,
                        Valor = item.Valor,

                        ValorFijo = item.ValorFijo,
                        Porcentaje = item.Porcentaje
                    });
                }

                if (resultados == null || resultados.Count == 0)
                    throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

                var tran = _uow.BeginTransaction();
                foreach (var resultado in resultados)                
                {
                    iResults.Cantidad++;
                    iResults.Procesados++;
                    var prestacion = await _prestacionesRepository.GetByCodigo(resultado.Codigo.ToUpperInvariant(), tran);

                    if (prestacion != null)
                    {
                        prestacion.Descripcion = resultado.Descripcion;
                        prestacion.Valor = resultado.Valor;
                        prestacion.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                        prestacion.ValorFijo = resultado.ValorFijo;
                        prestacion.Porcentaje = resultado.Porcentaje;

                        await _prestacionesRepository.Update(prestacion, tran);
                        iResults.Actualizados++;
                    }
                    else
                    {
                        await _prestacionesRepository.Insert(new Prestacion
                        {
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            Codigo = resultado.Codigo,   
                            Descripcion = resultado.Descripcion,    
                            Valor = resultado.Valor,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,

                            ValorFijo = resultado.ValorFijo,
                            Porcentaje = resultado.Porcentaje
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

        private async Task UpdateProfesionales(long idPrestacion, List<PrestacionProfesionalViewModel> profesionales, IDbTransaction tran)
        {
            //Borro valores actuales
            await _prestacionesProfesionalesRepository.RemoveByIdPrestacion(idPrestacion, tran);

            if (profesionales != null && profesionales.Count > 0)
            { 
                foreach (var profesional in profesionales)
                {
                    await _prestacionesProfesionalesRepository.Insert(new PrestacionProfesional()
                    {
                        IdPrestacion = idPrestacion,
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