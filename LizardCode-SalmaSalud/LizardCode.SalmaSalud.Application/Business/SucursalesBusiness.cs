using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Sucursales;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SucursalesBusiness: BaseBusiness, ISucursalesBusiness
    {
        private readonly ILogger<SucursalesBusiness> _logger;
        private readonly ISucursalesRepository _sucursalesRepository;
        private readonly ISucursalesNumeracionRepository _sucursalesNumeracionRepository;
        private readonly IComprobantesRepository _comprobantesRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IEmpresasRepository _empresasRepository;


        public SucursalesBusiness(
            ISucursalesRepository sucursalesRepository,
            ISucursalesNumeracionRepository sucursalesNumeracionRepository,
            IComprobantesRepository comprobantesRepository,
            IAfipAuthRepository afipAuthRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IEmpresasRepository empresasRepository,
            ILogger<SucursalesBusiness> logger)
        {
            _comprobantesRepository = comprobantesRepository;
            _sucursalesNumeracionRepository = sucursalesNumeracionRepository;
            _sucursalesRepository = sucursalesRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _empresasRepository = empresasRepository;
            _afipAuthRepository = afipAuthRepository;
            _logger = logger;
        }

        public async Task New(SucursalesViewModel model)
        {
            var sucursal = _mapper.Map<Sucursal>(model);

            Validate(sucursal);

            var tran = _uow.BeginTransaction();

            try
            {
                sucursal.Descripcion = sucursal.Descripcion.ToUpper().Trim();
                sucursal.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                sucursal.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                var id = await _sucursalesRepository.Insert(sucursal, tran);

                var comprobantes = await _comprobantesRepository.GetAll<Comprobante>(tran);

                foreach (var comprobante in comprobantes)
                {
                    await _sucursalesNumeracionRepository.Insert(new SucursalNumeracion
                    {
                        IdComprobante = comprobante.IdComprobante,
                        IdSucursal = (int)id,
                        Numerador = "00000000"
                    }, tran);
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

        public async Task<SucursalesViewModel> Get(int idSucursal)
        {
            var sucursal = await _sucursalesRepository.GetById<Sucursal>(idSucursal);

            if (sucursal == null)
                throw new BusinessException("Sucursal No Existe");

            var model = _mapper.Map<SucursalesViewModel>(sucursal);

            return model;
        }

        public async Task<DataTablesResponse<Sucursal>> GetAll(DataTablesRequest request)
        {
            var customQuery = _sucursalesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Sucursal>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(SucursalesViewModel model)
        {
            var sucursal = _mapper.Map<Sucursal>(model);

            Validate(sucursal);

            var dbSucursal = await _sucursalesRepository.GetById<Sucursal>(sucursal.IdSucursal);

            if (dbSucursal == null)
            {
                throw new BusinessException("Sucursal Inexistente");
            }

            using var tran = _uow.BeginTransaction();

            try
            {
                dbSucursal.CodigoSucursal = sucursal.CodigoSucursal;
                dbSucursal.Descripcion = sucursal.Descripcion.ToUpper().Trim();
                dbSucursal.Exenta = sucursal.Exenta;
                dbSucursal.Webservice = sucursal.Webservice;
                dbSucursal.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _sucursalesRepository.Update(dbSucursal, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<IList<Domain.EntitiesCustom.SucursalNumeracion>> GetSucursalesNumeracionByIdSucursal(int idSucursal) =>
            await _sucursalesNumeracionRepository.GetAllCustomSucursalesNumByIdSucursal(idSucursal, _permissionsBusiness.Value.User.IdEmpresa); 

        public async Task Remove(int idSucursal)
        {
            var sucursal = await _sucursalesRepository.GetById<Sucursal>(idSucursal);

            if (sucursal == null)
            {
                throw new BusinessException("Sucursal Inexistente");
            }

            sucursal.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _sucursalesRepository.Update(sucursal);
        }

        public async Task ActualizaNumeracion(int idSucursal, int idComprobante, string numerador)
        {
            if(numerador.Length != 8)
                throw new BusinessException("En Numero para el Comprobante es Incorrecto");

            var sucursal = await _sucursalesRepository.GetById<Sucursal>(idSucursal);
            if(sucursal.IdEmpresa != _permissionsBusiness.Value.User.IdEmpresa)
                throw new BusinessException("La Sucursal seleccionada no Existe.");

            using var tran = _uow.BeginTransaction();

            try
            {
                var sucusalNum = await _sucursalesNumeracionRepository.GetByIdSucursalAndComprobante(idSucursal, idComprobante, tran);
                sucusalNum.Numerador = numerador;

                await _sucursalesNumeracionRepository.Update(sucusalNum, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }

        }
        public async Task<string> AFIPConsultaNumeracion(int idSucursal, int idComprobante)
        {
            var sucursal = await _sucursalesRepository.GetById<Sucursal>(idSucursal);
            if (sucursal.IdEmpresa != _permissionsBusiness.Value.User.IdEmpresa)
                throw new BusinessException("La Sucursal seleccionada no Existe.");

            var afipAuth = await _afipAuthRepository.GetValidSignToken(_permissionsBusiness.Value.User.IdEmpresa, ServicioAFIP.WEB_SERVICE_FACTURA_ELECTRONICA.Description());
            var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);

            if (afipAuth == null)
            {
                var tran = _uow.BeginTransaction();

                try
                {
                    var crt = await _empresasCertificadosRepository.GetValidEmpresaCerificadoByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);

                    if (crt == null)
                    {
                        throw new BusinessException("No Existe un Certificado Válido para la Empresa Seleccionada");
                    }

                    afipAuth = await _afipAuthRepository.NewSignToken(_permissionsBusiness.Value.User.IdEmpresa, crt.CRT, empresa.PrivateKey, empresa.CUIT.Replace("-", string.Empty), ServicioAFIP.WEB_SERVICE_FACTURA_ELECTRONICA.Description(), empresa.EnableProdAFIP);

                    await _afipAuthRepository.Insert(afipAuth, tran);

                    tran.Commit();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                    tran.Rollback();
                    throw new InternalException();
                }
            }

            var comprobante = await _comprobantesRepository.GetById<Comprobante>(idComprobante);

            return await _sucursalesRepository.GetAFIPConsultaNumeracion(afipAuth, empresa.CUIT.Replace("-", string.Empty), int.Parse(sucursal.CodigoSucursal), comprobante.Codigo, empresa.EnableProdAFIP);

        }

        private void Validate(Sucursal sucursal)
        {
            if (sucursal.Descripcion.IsNull())
            {
                throw new BusinessException("La descripción para la Sucursal es Obligatoria");
            }

            if (sucursal.CodigoSucursal.IsNull())
            {
                throw new BusinessException("El código para la Sucursal es Obligatoria");
            }
        }
    }
}
