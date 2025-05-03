using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Financiadores;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.Framework.Helpers.Excel;
using Microsoft.AspNetCore.Http;
using System.IO;
using static NPOI.HSSF.Util.HSSFColor;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class FinanciadoresBusiness : BaseBusiness, IFinanciadoresBusiness
    {
        private readonly ILogger<FinanciadoresBusiness> _logger;
        private readonly IFinanciadoresRepository _financiadoresRepository;
        private readonly IFinanciadoresPlanesRepository _financiadoresPlanesRepository;
        private readonly IFinanciadoresPrestacionesRepository _financiadoresPrestacionesRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IClientesRepository _clientesRepository;

        public FinanciadoresBusiness(
            IFinanciadoresRepository FinanciadoresRepository,
            ILogger<FinanciadoresBusiness> logger,
            IAfipAuthRepository afipAuthRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IFinanciadoresPlanesRepository FinanciadoresPlanesRepository,
            IFinanciadoresPrestacionesRepository financiadoresPrestacionesRepository,
            IClientesRepository clientesRepository)
        {
            _financiadoresRepository = FinanciadoresRepository;
            _logger = logger;
            _financiadoresPlanesRepository = FinanciadoresPlanesRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _afipAuthRepository = afipAuthRepository;
            _financiadoresPrestacionesRepository = financiadoresPrestacionesRepository;
            _clientesRepository = clientesRepository;
        }


        public async Task New(FinanciadorViewModel model)
        {
            var financiador = _mapper.Map<Financiador>(model);
            var planes = model.Items;

            Validate(financiador);

            var tran = _uow.BeginTransaction();

            try
            {
                //Busco si ya Existe el Financiador
                var dbFinanciador = await _financiadoresRepository.GetFinanciadorByCUIT(financiador.CUIT.ToUpper().Trim(), tran);

                if (dbFinanciador == default)
                {
                    financiador.Nombre = financiador.Nombre.ToUpper().Trim();
                    financiador.IdTipoIVA = financiador.IdTipoIVA;
                    financiador.CUIT = financiador.CUIT.ToUpper().Trim();
                    financiador.NroIBr = financiador.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    financiador.Email = financiador.Email.ToLower().Trim();
                    financiador.Direccion = financiador.Direccion.ToUpper().Trim();
                    financiador.CodigoPostal = financiador.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    financiador.Piso = financiador.Piso?.ToUpper().Trim() ?? string.Empty;
                    financiador.Departamento = financiador.Departamento?.ToUpper().Trim() ?? string.Empty;
                    financiador.Localidad = financiador.Localidad?.ToUpper().Trim() ?? string.Empty;
                    financiador.Provincia = financiador.Provincia?.ToUpper().Trim() ?? string.Empty;
                    financiador.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                    var id = await _financiadoresRepository.Insert(financiador, tran);

                    await UpdatePlanes(id, planes, tran);

                    //await UpdatePrestaciones(id, model.Prestaciones, tran);

                    financiador.IdFinanciador = (int)id;
                    await NewCliente(financiador, tran);
                }
                else
                {
                    dbFinanciador.Nombre = financiador.Nombre.ToUpper().Trim();
                    dbFinanciador.IdTipoIVA = financiador.IdTipoIVA;
                    dbFinanciador.CUIT = financiador.CUIT.ToUpper().Trim();
                    dbFinanciador.NroIBr = financiador.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    dbFinanciador.Email = financiador.Email.ToLower().Trim();
                    dbFinanciador.Direccion = financiador.Direccion.ToUpper().Trim();
                    dbFinanciador.CodigoPostal = financiador.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    dbFinanciador.Piso = financiador.Piso?.ToUpper().Trim() ?? string.Empty;
                    dbFinanciador.Departamento = financiador.Departamento?.ToUpper().Trim() ?? string.Empty;
                    dbFinanciador.Localidad = financiador.Localidad?.ToUpper().Trim() ?? string.Empty;
                    dbFinanciador.Provincia = financiador.Provincia?.ToUpper().Trim() ?? string.Empty;
                    dbFinanciador.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                    await _financiadoresRepository.Update(dbFinanciador, tran);

                    await UpdatePlanes(dbFinanciador.IdFinanciador, planes, tran);

                    //await UpdatePrestaciones(dbFinanciador.IdFinanciador, model.Prestaciones, tran);

                    await UpdateCliente(dbFinanciador, tran);
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

        public async Task<FinanciadorViewModel> Get(int idFinanciador)
        {
            var financiador = await _financiadoresRepository.GetById<Financiador>(idFinanciador);
            var planes = await _financiadoresPlanesRepository.GetAllByIdFinanciador(idFinanciador);

            if (financiador == null)
                return null;

            var model = _mapper.Map<FinanciadorViewModel>(financiador);
            model.Items = _mapper.Map<List<FinanciadorPlanViewModel>>(planes);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Financiador>> GetAll(DataTablesRequest request)
        {
            var customQuery = _financiadoresRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FiltroNombre"))
                builder.Append($"AND Nombre LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Financiador>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(FinanciadorViewModel model)
        {
            var financiador = _mapper.Map<Financiador>(model);
            var planes = model.Items;

            Validate(financiador);

            var dbFinanciador = await _financiadoresRepository.GetById<Financiador>(financiador.IdFinanciador);

            if (dbFinanciador == null)
            {
                throw new ArgumentException("Financiador inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbFinanciador.Nombre = financiador.Nombre.ToUpper().Trim();
                dbFinanciador.CUIT = financiador.CUIT.ToUpper().Trim();
                dbFinanciador.IdTipoIVA = financiador.IdTipoIVA;
                dbFinanciador.NroIBr = financiador.NroIBr?.ToUpper().Trim() ?? string.Empty;
                dbFinanciador.Email = financiador.Email.ToLower().Trim();
                dbFinanciador.Direccion = financiador.Direccion.ToUpper().Trim();
                dbFinanciador.CodigoPostal = financiador.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                dbFinanciador.Piso = financiador.Piso?.ToUpper().Trim() ?? string.Empty;
                dbFinanciador.Departamento = financiador.Departamento?.ToUpper().Trim() ?? string.Empty;
                dbFinanciador.Localidad = financiador.Localidad?.ToUpper().Trim() ?? string.Empty;
                dbFinanciador.Provincia = financiador.Provincia?.ToUpper().Trim() ?? string.Empty;
                dbFinanciador.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _financiadoresRepository.Update(dbFinanciador, tran);

                await UpdatePlanes(dbFinanciador.IdFinanciador, planes, tran);

                //await UpdatePrestaciones(dbFinanciador.IdFinanciador, model.Prestaciones, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idFinanciador)
        {
            var financiador = await _financiadoresRepository.GetById<Financiador>(idFinanciador);

            if (financiador == null)
            {
                throw new ArgumentException("Financiador inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                await _financiadoresPlanesRepository.RemoveByIdFinanciador(financiador.IdFinanciador, tran);

                //Verifico que el Financiador no tenga Relacion en otra empresa.
                var lstFinanciadores = await _financiadoresPlanesRepository.GetAllByIdFinanciador(financiador.IdFinanciador, tran);
                if (lstFinanciadores.Count == 0)
                {
                    financiador.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _financiadoresRepository.Update(financiador, tran);
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

        private void Validate(Financiador financiador)
        {
            if (financiador.Nombre.IsNull())
            {
                throw new BusinessException("Ingrese el Nombre de Fantasia para Financiador");
            }
            if (financiador.CUIT.IsNull() && financiador.IdTipoIVA != (int)TipoIVA.ConsumidorFinal)
            {
                throw new BusinessException("Ingrese un CUIT Válido");
            }

            if (financiador.Telefono.IsNull())
            {
                throw new BusinessException("Ingrese un Teléfono para el Financiador");
            }

            if (financiador.Email.IsNull())
            {
                throw new BusinessException("Ingrese un E-Mail para el Financiador");
            }

            if (financiador.Direccion.IsNull())
            {
                throw new BusinessException("Ingrese un Dirección para el Financiador");
            }
        }

        public async Task<string> ValidarNroCUIT(string cuit, int? idFinanciador)
        {
            if (!cuit.ValidarCUIT())
            {
                return new string("C.U.I.T. Invalido");
            }

            var result = await _financiadoresRepository.ValidarCUITExistente(cuit, idFinanciador);
            if (result)
                return new string("Existe un Financiador con el nro de C.U.I.T. Ingresado. Verifique");
            return null;
        }

        public async Task<Custom.Contribuyente> GetPadron(string cuit)
        {
            try
            {
                if (!cuit.ValidarCUIT())
                {
                    throw new BusinessException("C.U.I.T. Inválido");
                }

                var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                var afipAuth = await _afipAuthRepository.GetValidSignToken(_permissionsBusiness.Value.User.IdEmpresa, ServicioAFIP.WEB_SERVICE_PADRON.Description());
                var cuitEmpresa = string.Empty;
                var padronProd = true;

                if (afipAuth == null)
                {
                    var tran = _uow.BeginTransaction();

                    try
                    {
                        var crt = await _empresasCertificadosRepository.GetValidEmpresaCerificadoByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa, tran);

                        if (crt == default)
                        {
                            var genericCrt = "AFIP-CRT".FromAppSettings<string>(notFoundException: true);
                            var genericPk = "AFIP-PK".FromAppSettings<string>(notFoundException: true);
                            var genericCuit = "AFIP-CUIT".FromAppSettings<string>(notFoundException: true);

                            cuitEmpresa = genericCuit;
                            afipAuth = await _afipAuthRepository.NewSignToken(_permissionsBusiness.Value.User.IdEmpresa, genericCrt, genericPk, genericCuit, ServicioAFIP.WEB_SERVICE_PADRON.Description(), true);
                        }
                        else
                        {
                            cuitEmpresa = empresa.CUIT.Replace("-", string.Empty);
                            afipAuth = await _afipAuthRepository.NewSignToken(_permissionsBusiness.Value.User.IdEmpresa, crt.CRT, empresa.PrivateKey, cuitEmpresa, ServicioAFIP.WEB_SERVICE_PADRON.Description(), empresa.EnableProdAFIP);
                            padronProd = empresa.EnableProdAFIP;
                        }

                        await _afipAuthRepository.Insert(afipAuth, tran);
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, null);
                        tran.Rollback();
                        throw new BusinessException(ex.Message);
                    }
                }

                Custom.Contribuyente contribuyente;
                try
                {
                    contribuyente = await _empresasRepository.GetPadronByCUIT(afipAuth, afipAuth.CUIT, cuit.Replace("-", string.Empty), padronProd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                    throw new BusinessException(ex.Message);
                }

                var respInsc = Enum.GetValues(typeof(ImpuestosAFIPRespInscripto)).Cast<ImpuestosAFIPRespInscripto>().ToList()
                    .Any(i => contribuyente.Impuestos.Contains((int)i));

                var monotributo = Enum.GetValues(typeof(ImpuestosAFIPMonotributo)).Cast<ImpuestosAFIPMonotributo>().ToList()
                    .Any(i => contribuyente.Impuestos.Contains((int)i));

                var exento = Enum.GetValues(typeof(ImpuestosAFIPExento)).Cast<ImpuestosAFIPExento>().ToList()
                    .Any(i => contribuyente.Impuestos.Contains((int)i));

                contribuyente.IdTipoIVA = 0;
                if (respInsc)
                    contribuyente.IdTipoIVA = (int)TipoIVA.ResponsableInscripto;
                if (monotributo)
                    contribuyente.IdTipoIVA = (int)TipoIVA.Monotributo;
                if (exento)
                    contribuyente.IdTipoIVA = (int)TipoIVA.Exento;

                return contribuyente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new BusinessException(ex.Message);
            }
        }

        public async Task<IList<FinanciadorPlan>> GetAllByFinanciadorId(int idFinanciador)
            => await _financiadoresPlanesRepository.GetAllByIdFinanciador(idFinanciador);

        private async Task UpdatePlanes(long idFinanciador, List<FinanciadorPlanViewModel> planes, IDbTransaction tran)
        {
            //Me traigo los planes actuales...
            var dbPlanes = await _financiadoresPlanesRepository.GetAllByIdFinanciador(idFinanciador, tran);
            if (dbPlanes != null && dbPlanes.Count > 0)
            {
                if (planes != null && planes.Count > 0)
                {
                    //Me tengo que fijar de que los planes que vienen se actualizen y no se borren, si falta alguno...
                    var idsPlanes = planes.Select(s => s.IdFinanciadorPlan).ToList();
                    var planesToRemove = new List<int>();
                    planesToRemove = dbPlanes.Where(i => !idsPlanes.Contains(i.IdFinanciadorPlan))?.Select(s => s.IdFinanciadorPlan).ToList();

                    if (planesToRemove.Count > 0)
                    {
                        foreach (var planToRemove in planesToRemove)
                        {
                            //Antes de poder borrarlo me fijo si no existe asociado a algún paciente...    
                            var existe = true; //await _financiadoresPlanesRepository.ExistePacienteAsociado(planToRemove, tran);
                            if (existe) //EXISTE
                            {
                                var pToRemove = dbPlanes.FirstOrDefault(f => f.IdFinanciadorPlan == planToRemove);
                                throw new BusinessException(string.Format("El plan {0}, se encuentnra asociado a uno o mas pacientes. No se puede eliminar el Plan.", pToRemove.Nombre));
                            }
                            else
                            {
                                await _financiadoresPlanesRepository.RemoveById(planToRemove, tran);
                            }
                        }
                    }

                    var i = 0;
                    foreach (var plan in planes)
                    {
                        var planExistente = dbPlanes.FirstOrDefault(f => f.IdFinanciadorPlan == plan.IdFinanciadorPlan);
                        if (planExistente != null)
                        {
                            planExistente.Item = i;
                            planExistente.Nombre = plan.Nombre;
                            planExistente.Codigo = plan.Codigo;
                            await _financiadoresPlanesRepository.Update(planExistente, tran);
                        }
                        else
                        {
                            await _financiadoresPlanesRepository.Insert(new FinanciadorPlan() { IdFinanciador = (int)idFinanciador, Item = plan.Item, Nombre = plan.Nombre, Codigo = plan.Codigo }, tran);
                        }

                        i++;
                    }
                }
            }
            else
            {
                if (planes != null && planes.Count > 0)
                {
                    foreach (var plan in planes)
                    {
                        await _financiadoresPlanesRepository.Insert(new FinanciadorPlan() { IdFinanciador = (int)idFinanciador, Item = plan.Item, Nombre = plan.Nombre }, tran);
                    }
                }
            }
        }

        //public async Task<List<FinanciadorPrestacionViewModel>> ProcesarExcel(IFormFile file)
        //{
        //    var resultados = new List<FinanciadorPrestacionViewModel>();
        //    var memoryStream = new MemoryStream();

        //    try
        //    {
        //        await file.CopyToAsync(memoryStream);
        //        memoryStream.Position = 0; // <-- Add this, to make it work
        //        var eWrapper = new ExcelWrapper(memoryStream);
        //        var excelRows = eWrapper.GetListFromSheet<FinanciadorPrestacionXLSViewModel>();

        //        //if (excelRows.Count > 50)
        //        //    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

        //        var iItem = 1;
        //        foreach (var item in excelRows)
        //        {
        //            resultados.Add(new FinanciadorPrestacionViewModel
        //            {
        //                Item = iItem,
        //                Codigo = item.Codigo,
        //                Descripcion = item.Descripcion,
        //                Valor = item.Valor
        //            });
        //            iItem++;
        //        }

        //        if (resultados == null || resultados.Count == 0)
        //            throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

        //        return resultados;
        //    }
        //    catch (BusinessException ex)
        //    {
        //        _logger.LogError(ex, null);
        //        throw ex;
        //    }
        //    catch (InvalidCastException ex)
        //    {
        //        _logger.LogError(ex, null);
        //        throw new BusinessException(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, null);
        //        throw new InternalException();
        //    }
        //}

        //private async Task UpdatePrestaciones(long idFinanciador, List<FinanciadorPrestacionViewModel> prestaciones, IDbTransaction transaction)
        //{
        //    await _financiadoresPrestacionesRepository.RemoveByIdFinanciador(idFinanciador, transaction);

        //    if (prestaciones != null && prestaciones.Count > 0)
        //    { 
        //        var iItem = 1;
        //        foreach (var item in prestaciones)
        //        {
        //            await _financiadoresPrestacionesRepository.Insert(new FinanciadorPrestacion
        //            {
        //                IdFinanciador = idFinanciador,
        //                Item = iItem,
        //                Codigo = item.Codigo,
        //                Descripcion = item.Descripcion,
        //                Valor = item.Valor
                    
        //            }, transaction);

        //            iItem++;
        //        }
        //    }
        //}

        public async Task<IList<FinanciadorPrestacion>> GetAllPrestacionesByFinanciadorId(int idFinanciador)
            => await _financiadoresPrestacionesRepository.GetAllByIdFinanciador(idFinanciador);

        public async Task<IList<FinanciadorPrestacion>> GetAllPrestacionesByIdFinanciadorAndIdPlan(int idFinanciador, int idFinanciadorPlan)
            => await _financiadoresPrestacionesRepository.GetAllByIdFinanciadorAndIdPlan(idFinanciador, idFinanciadorPlan);

        public async Task<FinanciadorPrestacion> GetFinanciadorPrestacionById(int idFinanciadorPrestacion)
            => await _financiadoresPrestacionesRepository.GetFinanciadorPrestacionById(idFinanciadorPrestacion);

        public async Task<FinanciadorPlan> GetPlanById(int idFinanciadorPlan)
        {
            var plan = await _financiadoresPlanesRepository.GetById<FinanciadorPlan>(idFinanciadorPlan);

            return plan;
        }

        private async Task NewCliente(Financiador financiador, IDbTransaction transaction = null)
        {
            var cliente = new Cliente
            {
                IdFinanciador = (int)financiador.IdFinanciador,
                RazonSocial = financiador.Nombre,
                NombreFantasia = financiador.Nombre,
                CUIT = financiador.CUIT,
                IdTipoDocumento = (int)TipoDocumento.CUIT,
                IdTipoIVA = financiador.IdTipoIVA,
                Email = financiador.Email,
                IdTipoTelefono = financiador.IdTipoTelefono,
                Telefono = financiador.Telefono,
                Direccion = financiador.Direccion,
                CodigoPostal = financiador.CodigoPostal,
                Piso = financiador.Piso,
                Departamento = financiador.Departamento,
                Localidad = financiador.Localidad,
                Provincia = financiador.Provincia,
                Latitud = financiador.Latitud,
                Longitud = financiador.Longitud,
                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
            };

            var idCliente = await _clientesRepository.Insert(cliente, transaction);
        }

        private async Task UpdateCliente(Financiador financiador, IDbTransaction transaction = null)
        {
            var cliente = await _clientesRepository.GetClienteByIdFinanciador(financiador.IdFinanciador, transaction);

            cliente.RazonSocial = financiador.Nombre;
            cliente.NombreFantasia = financiador.Nombre;
            cliente.CUIT = financiador.CUIT;
            cliente.IdTipoIVA = financiador.IdTipoIVA;
            cliente.Email = financiador.Email;
            cliente.IdTipoTelefono = financiador.IdTipoTelefono;
            cliente.Telefono = financiador.Telefono;
            cliente.Direccion = financiador.Direccion;
            cliente.CodigoPostal = financiador.CodigoPostal;
            cliente.Piso = financiador.Piso;
            cliente.Departamento = financiador.Departamento;
            cliente.Localidad = financiador.Localidad;
            cliente.Provincia = financiador.Provincia;
            cliente.Latitud = financiador.Latitud;
            cliente.Longitud = financiador.Longitud;
            cliente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            await _clientesRepository.Update(cliente, transaction);
        }

        public async Task ADMIN_CreateClientes()
        {
            var financiadores = await _financiadoresRepository.GetAll<Financiador>();
            var tran = _uow.BeginTransaction();
            try
            {
                foreach (var financiador in financiadores)
                {
                    await NewCliente(financiador, tran);
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
    }
}
