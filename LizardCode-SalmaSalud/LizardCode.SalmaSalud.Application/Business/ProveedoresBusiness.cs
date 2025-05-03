using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Proveedores;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Models.Laboratorios;
using System.Collections.Generic;
using LizardCode.SalmaSalud.Application.Models.Financiadores;
using System.Data;
using LizardCode.Framework.Application.Common.Enums;
using static Mysqlx.Crud.Order.Types;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ProveedoresBusiness: BaseBusiness, IProveedoresBusiness
    {
        private readonly ILogger<ProveedoresBusiness> _logger;
        private readonly IProveedoresRepository _proveedoresRepository;
        private readonly IProveedoresEmpresasRepository _proveedoresEmpresasRepository;
        private readonly IProveedoresCodigosRetencionRepository _proveedoresCodigosRetencionRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IComprobantesRepository _comprobantesRepository;
        private readonly ILaboratoriosServiciosRepository _laboratoriosServiciosRepository;

        public ProveedoresBusiness(
            IProveedoresRepository proveedoresRepository,
            ILogger<ProveedoresBusiness> logger,
            IProveedoresEmpresasRepository proveedoresEmpresasRepository,
            IAfipAuthRepository afipAuthRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IProveedoresCodigosRetencionRepository proveedoresCodigosRetencionRepository,
            IComprobantesRepository comprobantesRepository,
            ILaboratoriosServiciosRepository laboratoriosServiciosRepository)
        {
            _proveedoresRepository = proveedoresRepository;
            _logger = logger;
            _proveedoresEmpresasRepository = proveedoresEmpresasRepository;
            _proveedoresCodigosRetencionRepository = proveedoresCodigosRetencionRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _afipAuthRepository = afipAuthRepository;
            _comprobantesRepository = comprobantesRepository;
            _laboratoriosServiciosRepository = laboratoriosServiciosRepository;
        }

        public async Task New(ProveedorViewModel model)
        {
            var proveedor = _mapper.Map<Proveedor>(model);
            var empresas = model.Empresas;

            Validate(proveedor);

            var tran = _uow.BeginTransaction();

            try
            {
                //Busco si ya no Existe el Cliente en otra Empresa
                var dbProveedor = await _proveedoresRepository.GetProveedorByCUIT(proveedor.CUIT.ToUpper().Trim(), tran);

                if (dbProveedor == default)
                {
                    proveedor.RazonSocial = proveedor.RazonSocial.ToUpper().Trim();
                    proveedor.NombreFantasia = proveedor.NombreFantasia.ToUpper().Trim();
                    proveedor.IdTipoIVA = proveedor.IdTipoIVA;
                    proveedor.CUIT = proveedor.CUIT.ToUpper().Trim();
                    proveedor.NroIBr = proveedor.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Email = proveedor.Email.ToLower().Trim() ?? string.Empty;
                    proveedor.Direccion = proveedor.Direccion.ToUpper().Trim() ?? string.Empty;
                    proveedor.CodigoPostal = proveedor.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Piso = proveedor.Piso?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Departamento = proveedor.Departamento?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Localidad = proveedor.Localidad?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Provincia = proveedor.Provincia?.ToUpper().Trim() ?? string.Empty;
                    proveedor.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                    var id = await _proveedoresRepository.Insert(proveedor, tran);

                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (var empresa in empresas)
                        {
                            await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa() { IdProveedor = (int)id, IdEmpresa = empresa.Value }, tran);
                        }
                    }

                    if (model.IdCodigoRetencionGanancias.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionGanancias.Value });
                    }

                    if (model.IdCodigoRetencionIVA.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionIVA.Value });
                    }

                    if (model.IdCodigoRetencionIBr.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionIBr.Value });
                    }

                    if (model.IdCodigoRetencionSUSS.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionSUSS.Value });
                    }

                    if (model.IdCodigoRetencionGananciasMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionGananciasMonotributo.Value });
                    }

                    if (model.IdCodigoRetencionIVAMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionIVAMonotributo.Value });
                    }
                }
                else
                {
                    dbProveedor.RazonSocial = proveedor.RazonSocial.ToUpper().Trim();
                    dbProveedor.NombreFantasia = proveedor.NombreFantasia.ToUpper().Trim();
                    dbProveedor.CUIT = proveedor.CUIT.ToUpper().Trim();
                    dbProveedor.IdTipoIVA = proveedor.IdTipoIVA;
                    dbProveedor.NroIBr = proveedor.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Email = proveedor.Email.ToLower().Trim() ?? string.Empty;
                    dbProveedor.Direccion = proveedor.Direccion.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.CodigoPostal = proveedor.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Piso = proveedor.Piso?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Departamento = proveedor.Departamento?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Localidad = proveedor.Localidad?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Provincia = proveedor.Provincia?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                    await _proveedoresRepository.Update(dbProveedor, tran);

                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (var empresa in empresas)
                        {
                            await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa() { IdProveedor = (int)dbProveedor.IdProveedor, IdEmpresa = empresa.Value }, tran);
                        }
                    }

                    await _proveedoresCodigosRetencionRepository.RemoveByIdProveedor(dbProveedor.IdProveedor, tran);
                    if (dbProveedor.IdTipoIVA.Equals((int)TipoIVA.Monotributo))
                    {
                        if (model.IdCodigoRetencionGananciasMonotributo.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGananciasMonotributo.Value });
                        }

                        if (model.IdCodigoRetencionIVAMonotributo.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVAMonotributo.Value });
                        }
                    }
                    else
                    {
                        if (model.IdCodigoRetencionGanancias.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGanancias.Value });
                        }

                        if (model.IdCodigoRetencionIVA.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVA.Value });
                        }

                        if (model.IdCodigoRetencionIBr.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIBr.Value });
                        }

                        if (model.IdCodigoRetencionSUSS.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionSUSS.Value });
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

        public async Task<ProveedorViewModel> Get(int idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetById<Proveedor>(idProveedor);
            var empresas = await _proveedoresEmpresasRepository.GetAllByIdProveedor(idProveedor);
            var reten = await _proveedoresCodigosRetencionRepository.GetAllByIdProveedor(idProveedor);

            if (proveedor == null)
                return null;

            var model = _mapper.Map<ProveedorViewModel>(proveedor);
            model.Empresas = empresas.Select(e => e?.IdEmpresa).ToList();

            model.IdCodigoRetencionGanancias = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionIVA = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionIBr = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionSUSS = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionGananciasMonotributo = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionIVAMonotributo = reten.FirstOrDefault()?.IdCodigoRetencion;

            return model;
        }

        public async Task<DataTablesResponse<Custom.Proveedor>> GetAll(DataTablesRequest request, bool esLaboratorio)
        {
            var customQuery = _proveedoresRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            builder.Append($"AND EsLaboratorio = {esLaboratorio} ");

            if (filters.ContainsKey("FiltroCUIT"))
                builder.Append($"AND CUIT = {filters["FiltroCUIT"]}");

            if (filters.ContainsKey("FiltroRazonSocial"))
                builder.Append($"AND RazonSocial LIKE {string.Concat("%", filters["FiltroRazonSocial"], "%")}");

            builder.Append($"AND IdProveedor IN (SELECT IdProveedor FROM ProveedoresEmpresas WHERE IdEmpresa IN (SELECT IdEmpresa FROM UsuariosEmpresas WHERE IdUsuario = {_permissionsBusiness.Value.User.Id}))");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Proveedor>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(ProveedorViewModel model)
        {
            var proveedor = _mapper.Map<Proveedor>(model);
            var empresas = model.Empresas;

            Validate(proveedor);

            var dbProveedor = await _proveedoresRepository.GetById<Proveedor>(proveedor.IdProveedor);

            if (dbProveedor == null)
            {
                throw new ArgumentException("Proveedor inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbProveedor.RazonSocial = proveedor.RazonSocial.ToUpper().Trim();
                dbProveedor.NombreFantasia = proveedor.NombreFantasia.ToUpper().Trim();
                dbProveedor.CUIT = proveedor.CUIT.ToUpper().Trim();
                dbProveedor.IdTipoIVA = proveedor.IdTipoIVA;
                dbProveedor.NroIBr = proveedor.NroIBr?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Email = proveedor.Email.ToLower().Trim() ?? string.Empty;
                dbProveedor.Direccion = proveedor.Direccion.ToUpper().Trim() ?? string.Empty;
                dbProveedor.CodigoPostal = proveedor.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Piso = proveedor.Piso?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Departamento = proveedor.Departamento?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Localidad = proveedor.Localidad?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Provincia = proveedor.Provincia?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _proveedoresRepository.Update(dbProveedor, tran);

                await _proveedoresEmpresasRepository.RemoveByIdProveedor(dbProveedor.IdProveedor, _permissionsBusiness.Value.User.Id, tran);
                if (empresas != null && empresas.Count > 0)
                {
                    foreach (var empresa in empresas)
                    {
                        await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa() { IdProveedor = (int)dbProveedor.IdProveedor, IdEmpresa = empresa.Value }, tran);
                    }
                }

                await _proveedoresCodigosRetencionRepository.RemoveByIdProveedor(dbProveedor.IdProveedor, tran);
                if (dbProveedor.IdTipoIVA.Equals((int)TipoIVA.Monotributo))
                {
                    if (model.IdCodigoRetencionGananciasMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGananciasMonotributo.Value });
                    }

                    if (model.IdCodigoRetencionIVAMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVAMonotributo.Value });
                    }
                }
                else
                {
                    if (model.IdCodigoRetencionGanancias.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGanancias.Value });
                    }

                    if (model.IdCodigoRetencionIVA.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVA.Value });
                    }

                    if (model.IdCodigoRetencionIBr.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIBr.Value });
                    }

                    if (model.IdCodigoRetencionSUSS.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionSUSS.Value });
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

        public async Task Remove(int idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetById<Proveedor>(idProveedor);

            if (proveedor == null)
            {
                throw new ArgumentException("Proveedor inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                await _proveedoresEmpresasRepository.RemoveByIdProveedor(proveedor.IdProveedor, _permissionsBusiness.Value.User.Id, tran);

                //Verifico que el Cliente no tenga Relacion en otra empresa.
                var lstProveedores = await _proveedoresEmpresasRepository.GetAllByIdProveedor(proveedor.IdProveedor, tran);
                if (lstProveedores.Count == 0)
                {
                    proveedor.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _proveedoresRepository.Update(proveedor, tran);
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

        private void Validate(Proveedor proveedor)
        {
            if (proveedor.RazonSocial.IsNull())
            {
                throw new ArgumentNullException(nameof(proveedor.RazonSocial), "RazonSocial");
            }

            if (proveedor.NombreFantasia.IsNull())
            {
                throw new ArgumentNullException(nameof(proveedor.NombreFantasia), "NombreFantasia");
            }

            if (proveedor.CUIT.IsNull() && proveedor.IdTipoIVA != (int)TipoIVA.ConsumidorFinal)
            {
                throw new ArgumentNullException(nameof(proveedor.CUIT), "CUIT");
            }

            if (proveedor.Telefono.IsNull())
            {
                throw new ArgumentNullException(nameof(proveedor.Telefono), "Telefono");
            }

            if (proveedor.Email.IsNull())
            {
                throw new ArgumentNullException(nameof(proveedor.Email), "Email");
            }

            if (proveedor.Direccion.IsNull())
            {
                throw new ArgumentNullException(nameof(proveedor.Direccion), "Direccion");
            }
        }

        public async Task<string> ValidarNroCUIT(string cuit, int? idProveedor)
        {
            if (!cuit.ValidarCUIT())
            {
                return new string("C.U.I.T. Invalido");
            }

            var result = await _proveedoresRepository.ValidarCUITExistente(cuit, idProveedor, _permissionsBusiness.Value.User.IdEmpresa);
            if (result)
                return new string("Existe un Proveedor con el nro de C.U.I.T. Ingresado. Verifique");
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
                        var crt = await _empresasCertificadosRepository.GetValidEmpresaCerificadoByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);

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

        public async Task NewLaboratorio(LaboratorioViewModel model)
        {
            var proveedor = _mapper.Map<Proveedor>(model);
            var empresas = model.Empresas;

            Validate(proveedor);

            var tran = _uow.BeginTransaction();

            try
            {
                //Busco si ya no Existe el Cliente en otra Empresa
                var dbProveedor = await _proveedoresRepository.GetProveedorByCUIT(proveedor.CUIT.ToUpper().Trim(), tran);                

                if (dbProveedor == default)
                {
                    proveedor.RazonSocial = proveedor.RazonSocial.ToUpper().Trim();
                    proveedor.NombreFantasia = proveedor.NombreFantasia.ToUpper().Trim();
                    proveedor.IdTipoIVA = proveedor.IdTipoIVA;
                    proveedor.CUIT = proveedor.CUIT.ToUpper().Trim();
                    proveedor.NroIBr = proveedor.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Email = proveedor.Email.ToLower().Trim() ?? string.Empty;
                    proveedor.Direccion = proveedor.Direccion.ToUpper().Trim() ?? string.Empty;
                    proveedor.CodigoPostal = proveedor.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Piso = proveedor.Piso?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Departamento = proveedor.Departamento?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Localidad = proveedor.Localidad?.ToUpper().Trim() ?? string.Empty;
                    proveedor.Provincia = proveedor.Provincia?.ToUpper().Trim() ?? string.Empty;
                    proveedor.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                    proveedor.EsLaboratorio = true;

                    var id = await _proveedoresRepository.Insert(proveedor, tran);
                    await UpdateServicios(id, model.Servicios, tran);

                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (var empresa in empresas)
                        {
                            await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa() { IdProveedor = (int)id, IdEmpresa = empresa.Value }, tran);
                        }
                    }

                    if (model.IdCodigoRetencionGanancias.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionGanancias.Value });
                    }

                    if (model.IdCodigoRetencionIVA.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionIVA.Value });
                    }

                    if (model.IdCodigoRetencionIBr.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionIBr.Value });
                    }

                    if (model.IdCodigoRetencionSUSS.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionSUSS.Value });
                    }

                    if (model.IdCodigoRetencionGananciasMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionGananciasMonotributo.Value });
                    }

                    if (model.IdCodigoRetencionIVAMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)id, IdCodigoRetencion = model.IdCodigoRetencionIVAMonotributo.Value });
                    }
                }
                else
                {
                    dbProveedor.RazonSocial = proveedor.RazonSocial.ToUpper().Trim();
                    dbProveedor.NombreFantasia = proveedor.NombreFantasia.ToUpper().Trim();
                    dbProveedor.CUIT = proveedor.CUIT.ToUpper().Trim();
                    dbProveedor.IdTipoIVA = proveedor.IdTipoIVA;
                    dbProveedor.NroIBr = proveedor.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Email = proveedor.Email.ToLower().Trim() ?? string.Empty;
                    dbProveedor.Direccion = proveedor.Direccion.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.CodigoPostal = proveedor.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Piso = proveedor.Piso?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Departamento = proveedor.Departamento?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Localidad = proveedor.Localidad?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.Provincia = proveedor.Provincia?.ToUpper().Trim() ?? string.Empty;
                    dbProveedor.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                    await _proveedoresRepository.Update(dbProveedor, tran);
                    await UpdateServicios(dbProveedor.IdProveedor, model.Servicios, tran);

                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (var empresa in empresas)
                        {
                            await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa() { IdProveedor = (int)dbProveedor.IdProveedor, IdEmpresa = empresa.Value }, tran);
                        }
                    }

                    await _proveedoresCodigosRetencionRepository.RemoveByIdProveedor(dbProveedor.IdProveedor, tran);
                    if (dbProveedor.IdTipoIVA.Equals((int)TipoIVA.Monotributo))
                    {
                        if (model.IdCodigoRetencionGananciasMonotributo.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGananciasMonotributo.Value });
                        }

                        if (model.IdCodigoRetencionIVAMonotributo.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVAMonotributo.Value });
                        }
                    }
                    else
                    {
                        if (model.IdCodigoRetencionGanancias.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGanancias.Value });
                        }

                        if (model.IdCodigoRetencionIVA.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVA.Value });
                        }

                        if (model.IdCodigoRetencionIBr.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIBr.Value });
                        }

                        if (model.IdCodigoRetencionSUSS.HasValue)
                        {
                            await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionSUSS.Value });
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

        public async Task<LaboratorioViewModel> GetLaboratorio(int idLaboratorio)
        {
            var proveedor = await _proveedoresRepository.GetById<Proveedor>(idLaboratorio);
            var empresas = await _proveedoresEmpresasRepository.GetAllByIdProveedor(idLaboratorio);
            var reten = await _proveedoresCodigosRetencionRepository.GetAllByIdProveedor(idLaboratorio);
            var servicios = await _laboratoriosServiciosRepository.GetAllByIdLaboratorio(idLaboratorio);

            if (proveedor == null)
                return null;

            var model = _mapper.Map<LaboratorioViewModel>(proveedor);
            model.Empresas = empresas.Select(e => e?.IdEmpresa).ToList();

            model.IdCodigoRetencionGanancias = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionIVA = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionIBr = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionSUSS = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionGananciasMonotributo = reten.FirstOrDefault()?.IdCodigoRetencion;
            model.IdCodigoRetencionIVAMonotributo = reten.FirstOrDefault()?.IdCodigoRetencion;

            model.Servicios = _mapper.Map<List<LaboratorioServicioViewModel>>(servicios);

            return model;
        }

        public async Task UpdateLaboratorio(LaboratorioViewModel model)
        {
            var proveedor = _mapper.Map<Proveedor>(model);
            var empresas = model.Empresas;

            Validate(proveedor);

            var dbProveedor = await _proveedoresRepository.GetById<Proveedor>(proveedor.IdProveedor);

            if (dbProveedor == null)
            {
                throw new ArgumentException("Proveedor inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbProveedor.RazonSocial = proveedor.RazonSocial.ToUpper().Trim();
                dbProveedor.NombreFantasia = proveedor.NombreFantasia.ToUpper().Trim();
                dbProveedor.CUIT = proveedor.CUIT.ToUpper().Trim();
                dbProveedor.IdTipoIVA = proveedor.IdTipoIVA;
                dbProveedor.NroIBr = proveedor.NroIBr?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Email = proveedor.Email.ToLower().Trim() ?? string.Empty;
                dbProveedor.Direccion = proveedor.Direccion.ToUpper().Trim() ?? string.Empty;
                dbProveedor.CodigoPostal = proveedor.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Piso = proveedor.Piso?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Departamento = proveedor.Departamento?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Localidad = proveedor.Localidad?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.Provincia = proveedor.Provincia?.ToUpper().Trim() ?? string.Empty;
                dbProveedor.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _proveedoresRepository.Update(dbProveedor, tran);
                await UpdateServicios(dbProveedor.IdProveedor, model.Servicios, tran);

                await _proveedoresEmpresasRepository.RemoveByIdProveedor(dbProveedor.IdProveedor, _permissionsBusiness.Value.User.Id, tran);
                if (empresas != null && empresas.Count > 0)
                {
                    foreach (var empresa in empresas)
                    {
                        await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa() { IdProveedor = (int)dbProveedor.IdProveedor, IdEmpresa = empresa.Value }, tran);
                    }
                }

                await _proveedoresCodigosRetencionRepository.RemoveByIdProveedor(dbProveedor.IdProveedor, tran);
                if (dbProveedor.IdTipoIVA.Equals((int)TipoIVA.Monotributo))
                {
                    if (model.IdCodigoRetencionGananciasMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGananciasMonotributo.Value });
                    }

                    if (model.IdCodigoRetencionIVAMonotributo.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVAMonotributo.Value });
                    }
                }
                else
                {
                    if (model.IdCodigoRetencionGanancias.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionGanancias.Value });
                    }

                    if (model.IdCodigoRetencionIVA.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIVA.Value });
                    }

                    if (model.IdCodigoRetencionIBr.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionIBr.Value });
                    }

                    if (model.IdCodigoRetencionSUSS.HasValue)
                    {
                        await _proveedoresCodigosRetencionRepository.Insert(new ProveedorCodigoRetencion() { IdProveedor = (int)dbProveedor.IdProveedor, IdCodigoRetencion = model.IdCodigoRetencionSUSS.Value });
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

        public async Task RemoveLaboratorio(int idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetById<Proveedor>(idProveedor);

            if (proveedor == null)
            {
                throw new ArgumentException("Laboratorio inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                await _proveedoresEmpresasRepository.RemoveByIdProveedor(proveedor.IdProveedor, _permissionsBusiness.Value.User.Id, tran);

                await _laboratoriosServiciosRepository.RemoveByIdLaboratorio(proveedor.IdProveedor, tran);

                //Verifico que el Cliente no tenga Relacion en otra empresa.
                var lstProveedores = await _proveedoresEmpresasRepository.GetAllByIdProveedor(proveedor.IdProveedor, tran);
                if (lstProveedores.Count == 0)
                {
                    proveedor.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _proveedoresRepository.Update(proveedor, tran);
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

        private async Task UpdateServicios(long idLaboratorio, List<LaboratorioServicioViewModel> servicios, IDbTransaction tran)
        {
            await _laboratoriosServiciosRepository.RemoveByIdLaboratorio(idLaboratorio, tran);

            if (servicios != null && servicios.Count > 0)
            {
                foreach (var servicio in servicios)
                {
                    await _laboratoriosServiciosRepository.Insert(new LaboratorioServicio() { IdProveedor = (int)idLaboratorio, 
                                                                                                Item = 0, 
                                                                                                Descripcion = servicio.Descripcion, 
                                                                                                Valor = servicio.Valor }
                                                                    , tran);
                }
            }
        }

        public async Task<IList<LaboratorioServicio>> GetServiciosByIdLaboratorio(long idLaboratorio)
            =>  await _laboratoriosServiciosRepository.GetAllByIdLaboratorio(idLaboratorio);

        public async Task<LaboratorioServicio> GetServicioById(int idLaboratorioServicio)
            => await _laboratoriosServiciosRepository.GetById<LaboratorioServicio>(idLaboratorioServicio);
    }
}
