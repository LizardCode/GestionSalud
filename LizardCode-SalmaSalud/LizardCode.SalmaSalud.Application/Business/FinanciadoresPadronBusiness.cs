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
using LizardCode.SalmaSalud.Application.Models.FinanciadoresPadron;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Excel;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class FinanciadoresPadronBusiness : BaseBusiness, IFinanciadoresPadronBusiness
    {
        private readonly ILogger<PrestacionesFinanciadorBusiness> _logger;
        private readonly IFinanciadoresPadronRepository _financiadoresPadronRepository;
        private readonly IFinanciadoresPlanesRepository _financiadoresPlanesRepository;
        private readonly IPrestacionesRepository _prestacionesRepository;
        private readonly IFinanciadoresPrestacionesProfesionalesRepository _financiadoresPrestacionesProfesionalesRepository;

        public FinanciadoresPadronBusiness(ILogger<PrestacionesFinanciadorBusiness> logger,
                                                IFinanciadoresPadronRepository prestacionesFinanciadorRepository,
                                                IFinanciadoresPlanesRepository financiadoresPlanesRepository,
                                                IPrestacionesRepository prestacionesRepository,
                                                IFinanciadoresPrestacionesProfesionalesRepository financiadoresPrestacionesProfesionalesRepository)
        {
            _logger = logger;
            _financiadoresPadronRepository = prestacionesFinanciadorRepository;
            _financiadoresPlanesRepository = financiadoresPlanesRepository;
            _prestacionesRepository = prestacionesRepository;
            _financiadoresPrestacionesProfesionalesRepository = financiadoresPrestacionesProfesionalesRepository;
        }

        public async Task New(FinanciadorPadronViewModel model, int idFinanciador)
        {
            var padron = _mapper.Map<FinanciadorPadron>(model);

            Validate(padron);

            padron.IdFinanciador = idFinanciador;
            padron.Fecha = DateTime.Now;

            var tran = _uow.BeginTransaction();
            await _financiadoresPadronRepository.Insert(padron, tran);
            tran.Commit();
        }

        public async Task<FinanciadorPadronViewModel> Get(int idPrestacion)
        {
            var prestacion = await _financiadoresPadronRepository.GetById<FinanciadorPadron>(idPrestacion);

            if (prestacion == null)
                return null;

            var model = _mapper.Map<FinanciadorPadronViewModel>(prestacion);

            return model;
        }

        public async Task<DataTablesResponse<FinanciadorPadron>> GetAll(DataTablesRequest request, int idFinanciador)
        {
            var customQuery = _financiadoresPadronRepository.GetAllCustomQuery();

            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();
            builder.Append($"AND IdFinanciador = {idFinanciador} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<FinanciadorPadron>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(FinanciadorPadronViewModel model, int idFinanciador)
        {
            var padron = _mapper.Map<FinanciadorPadron>(model);

            Validate(padron);

            var dbPadron = await _financiadoresPadronRepository.GetById<FinanciadorPadron>(padron.IdFinanciadorPadron);

            if (dbPadron == null)
            {
                throw new ArgumentException("Afiliado inexistente en padrón");
            }

            dbPadron.Documento = padron.Documento.Trim();
            dbPadron.FinanciadorNro = padron.FinanciadorNro.Trim();
            dbPadron.Nombre = padron.Nombre;
            dbPadron.Fecha = DateTime.Now;

            using var tran = _uow.BeginTransaction();
            await _financiadoresPadronRepository.Update(dbPadron, tran);
            tran.Commit();
        }

        public async Task Remove(int idFinanciadorPadron)
        {
            var padron = await _financiadoresPadronRepository.GetById<FinanciadorPadron>(idFinanciadorPadron);

            if (padron == null)
            {
                throw new ArgumentException("Afiliado inexistente en padrón");
            }

            await _financiadoresPadronRepository.RemoveById(idFinanciadorPadron);
        }

        private void Validate(FinanciadorPadron padron)
        {
            if (padron.Documento.IsNull())
            {
                throw new BusinessException(nameof(padron.Documento));
            }

            if (padron.FinanciadorNro.IsNull())
            {
                throw new BusinessException(nameof(padron.FinanciadorNro));
            }
        }

        public async Task<FinanciadorPadron> GetByDocumento(string documento, int idFinanciador)
        {
            var padron = await _financiadoresPadronRepository.GetByIdFinanciadorAndDocumento(idFinanciador, documento);

            return padron;
        }

        public async Task<bool> ValidarDocumento(string documento, int idFinanciador)
        {
            var prestacion = await _financiadoresPadronRepository.GetByIdFinanciadorAndDocumento(idFinanciador, documento);

            return (prestacion == null);
        }

        public async Task<PadronImportarExcelResultViewModel> ImportarPrestacionesExcel(IFormFile file, int idFinanciador)
        {
            var iResults = new PadronImportarExcelResultViewModel();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<FinanciadorPadronXLSViewModel>();

                //if (excelRows.Count > 50)
                //    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

                var resultados = new List<FinanciadorPadronViewModel>();
                foreach (var item in excelRows)
                {
                    resultados.Add(new FinanciadorPadronViewModel
                    {
                        IdFinanciador = idFinanciador,
                        Documento = item.Documento,
                        FinanciadorNro = item.FinanciadorNro,
                        Nombre = item.Nombre
                    });
                }

                if (resultados == null || resultados.Count == 0)
                    throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

                var tran = _uow.BeginTransaction();

                //Elimino lo viejo
                await _financiadoresPadronRepository.RemoveByIdFinanciador(idFinanciador, tran);

                var fechaPadron = DateTime.Now;
                foreach (var resultado in resultados)
                {
                    iResults.Cantidad++;
                    iResults.Procesados++;

                    await _financiadoresPadronRepository.Insert(new FinanciadorPadron
                    {
                        IdFinanciador = idFinanciador,
                        Documento = resultado.Documento,
                        FinanciadorNro = resultado.FinanciadorNro,
                        Nombre = resultado.Nombre,
                        Fecha = fechaPadron
                    }, tran);

                    iResults.Nuevos++;
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

    }
}