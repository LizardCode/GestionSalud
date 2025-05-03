using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Clientes;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Data;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ClientesBusiness: BaseBusiness, IClientesBusiness
    {
        private readonly ILogger<ClientesBusiness> _logger;
        private readonly IClientesRepository _clientesRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;


        public ClientesBusiness(
            IClientesRepository clientesRepository,
            ILogger<ClientesBusiness> logger,
            IAfipAuthRepository afipAuthRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository)
        {
            _clientesRepository = clientesRepository;
            _logger = logger;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _afipAuthRepository = afipAuthRepository;
        }


        public async Task New(ClienteViewModel model)
        {
            var client = _mapper.Map<Cliente>(model);

            Validate(client);

            var tran = _uow.BeginTransaction();

            try
            {
                //Busco si ya no Existe el Cliente en otra Empresa
                var dbCliente = await _clientesRepository.GetClienteByCUIT(client.CUIT.ToUpper().Trim(), tran);

                if (dbCliente == default)
                    dbCliente = await _clientesRepository.GetClienteByDocumento(client.Documento.ToUpper().Trim(), tran);

                if (dbCliente == default)
                {
                    client.RazonSocial = client.RazonSocial.ToUpper().Trim();
                    client.NombreFantasia = client.NombreFantasia.ToUpper().Trim();
                    client.IdTipoIVA = client.IdTipoIVA;
                    client.Documento = client.Documento?.ToUpper().Trim();
                    client.CUIT = client.CUIT?.ToUpper().Trim();
                    client.IdTipoDocumento = !string.IsNullOrEmpty(client.CUIT) ? (int)TipoDocumento.CUIT : (int)TipoDocumento.DNI;
                    client.NroIBr = client.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    client.Email = client.Email.ToLower().Trim();
                    client.Direccion = client.Direccion.ToUpper().Trim();
                    client.CodigoPostal = client.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    client.Piso = client.Piso?.ToUpper().Trim() ?? string.Empty;
                    client.Departamento = client.Departamento?.ToUpper().Trim() ?? string.Empty;
                    client.Localidad = client.Localidad?.ToUpper().Trim() ?? string.Empty;
                    client.Provincia = client.Provincia?.ToUpper().Trim() ?? string.Empty;
                    client.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                    var id = await _clientesRepository.Insert(client, tran);
                }
                else
                {
                    dbCliente.RazonSocial = client.RazonSocial.ToUpper().Trim();
                    dbCliente.NombreFantasia = client.NombreFantasia.ToUpper().Trim();
                    dbCliente.IdTipoIVA = client.IdTipoIVA;
                    dbCliente.Documento = client.Documento?.ToUpper().Trim();
                    dbCliente.CUIT = client.CUIT?.ToUpper().Trim();
                    dbCliente.IdTipoDocumento = !string.IsNullOrEmpty(client.CUIT) ? (int)TipoDocumento.CUIT : (int)TipoDocumento.DNI;
                    dbCliente.NroIBr = client.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    dbCliente.Email = client.Email.ToLower().Trim();
                    dbCliente.Direccion = client.Direccion.ToUpper().Trim();
                    dbCliente.CodigoPostal = client.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    dbCliente.Piso = client.Piso?.ToUpper().Trim() ?? string.Empty;
                    dbCliente.Departamento = client.Departamento?.ToUpper().Trim() ?? string.Empty;
                    dbCliente.Localidad = client.Localidad?.ToUpper().Trim() ?? string.Empty;
                    dbCliente.Provincia = client.Provincia?.ToUpper().Trim() ?? string.Empty;
                    dbCliente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                    await _clientesRepository.Update(dbCliente, tran);
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

        public async Task<ClienteViewModel> Get(int idCliente)
        {
            var client = await _clientesRepository.GetById<Cliente>(idCliente);

            if (client == null)
                return null;

            var model = _mapper.Map<ClienteViewModel>(client);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Cliente>> GetAll(DataTablesRequest request)
        {
            var customQuery = _clientesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FiltroCUIT"))
                builder.Append($"AND CUIT = {filters["FiltroCUIT"]}");

            if (filters.ContainsKey("FiltroRazonSocial"))
                builder.Append($"AND RazonSocial LIKE {string.Concat("%", filters["FiltroRazonSocial"], "%")}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Cliente>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(ClienteViewModel model)
        {
            var client = _mapper.Map<Cliente>(model);

            Validate(client);

            var dbClient = await _clientesRepository.GetById<Cliente>(client.IdCliente);

            if (dbClient == null)
            {
                throw new ArgumentException("Cliente inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbClient.RazonSocial = client.RazonSocial.ToUpper().Trim();
                dbClient.NombreFantasia = client.NombreFantasia.ToUpper().Trim();
                dbClient.IdTipoIVA = client.IdTipoIVA;
                dbClient.Documento = client.Documento?.ToUpper().Trim();
                dbClient.CUIT = client.CUIT?.ToUpper().Trim();
                dbClient.IdTipoDocumento = !string.IsNullOrEmpty(client.CUIT) ? (int)TipoDocumento.CUIT : (int)TipoDocumento.DNI;
                dbClient.NroIBr = client.NroIBr?.ToUpper().Trim() ?? string.Empty;
                dbClient.Email = client.Email.ToLower().Trim();
                dbClient.Direccion = client.Direccion.ToUpper().Trim();
                dbClient.CodigoPostal = client.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                dbClient.Piso = client.Piso?.ToUpper().Trim() ?? string.Empty;
                dbClient.Departamento = client.Departamento?.ToUpper().Trim() ?? string.Empty;
                dbClient.Localidad = client.Localidad?.ToUpper().Trim() ?? string.Empty;
                dbClient.Provincia = client.Provincia?.ToUpper().Trim() ?? string.Empty;
                dbClient.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _clientesRepository.Update(dbClient, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idClient)
        {
            var client = await _clientesRepository.GetById<Cliente>(idClient);

            if (client == null)
            {
                throw new ArgumentException("Cliente inexistente");
            }

            if (client.IdPaciente.HasValue)
            {
                throw new BusinessException("El cliente se encuentra asocido a un PACIENTE. No se puede eliminar el Cliente.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                client.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _clientesRepository.Update(client, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        private void Validate(Cliente client)
        {
            if (client.RazonSocial.IsNull())
            {
                throw new BusinessException("Ingrese la Razón Social del Cliente");
            }

            if (client.NombreFantasia.IsNull())
            {
                throw new BusinessException("Ingrese el Nombre de Fantasia para Cliente");
            }

            if (client.Documento.IsNull() && client.IdTipoIVA == (int)TipoIVA.ConsumidorFinal)
            {
                throw new BusinessException("Ingrese un Documento Válido");
            }

            if (client.CUIT.IsNull() && client.IdTipoIVA != (int)TipoIVA.ConsumidorFinal)
            {
                throw new BusinessException("Ingrese un CUIT Válido");
            }

            if (client.Telefono.IsNull())
            {
                throw new BusinessException("Ingrese un Teléfono para el Cliente");
            }

            if (client.Email.IsNull())
            {
                throw new BusinessException("Ingrese un E-Mail para el Cliente");
            }

            if (client.Direccion.IsNull())
            {
                throw new BusinessException("Ingrese un Dirección para el Cliente");
            }
        }

        public async Task<string> ValidarNroCUIT(string cuit, int? idCliente, int? idTipoDocumento)
        {
            if (idTipoDocumento.Value != (int)TipoDocumento.CUIL && idTipoDocumento.Value != (int)TipoDocumento.CUIT)
                return null;

            if (!cuit.ValidarCUIT())
            {
                return new string("C.U.I.T. Invalido");
            }

            var result = await _clientesRepository.ValidarCUITExistente(cuit, idCliente);
            if (result)
                return new string("Existe un Cliente con el nro de C.U.I.T. Ingresado. Verifique");
            return null;
        }

        public async Task<string> ValidarNroDocumento(string documento, int? idCliente)
        {
            var result = await _clientesRepository.ValidarDocumentoExistente(documento, idCliente);
            if (result)
                return new string("Existe un Cliente con el nro de Documento Ingresado. Verifique");
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
    }
}
