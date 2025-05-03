using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Bancos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class BancosBusiness: BaseBusiness, IBancosBusiness
    {
        private readonly ILogger<BancosBusiness> _logger;
        private readonly IBancosRepository _bancosRepository;

        public BancosBusiness(IBancosRepository bancosRepository, ILogger<BancosBusiness> logger)
        {
            _bancosRepository = bancosRepository;
            _logger = logger;
        }

        public async Task New(BancosViewModel model)
        {
            var banco = _mapper.Map<Banco>(model);

            Validate(banco);

            var tran = _uow.BeginTransaction();

            try
            {
                if(banco.EsDefault)
                {
                    await _bancosRepository.UpdateEsDefault(false, tran);
                }

                banco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                banco.Descripcion = banco.Descripcion.ToUpper().Trim();
                banco.CUIT = banco.CUIT.ToUpper().Trim() ?? String.Empty;
                banco.NroCuenta = banco.NroCuenta?.ToUpper().Trim() ?? String.Empty;
                banco.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                banco.CBU = banco.CBU.ToUpper().Trim();

                await _bancosRepository.Insert(banco, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();

            }
        }

        public async Task<BancosViewModel> Get(int idBanco)
        {
            var banco = await _bancosRepository.GetById<Banco>(idBanco);

            if (banco == null)
                return null;

            var model = _mapper.Map<BancosViewModel>(banco);

            return model;
        }

        public async Task<DataTablesResponse<Banco>> GetAll(DataTablesRequest request)
        {
            var customQuery = _bancosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Banco>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }            

        public async Task Update(BancosViewModel model)
        {
            var banco = _mapper.Map<Banco>(model);

            Validate(banco);

            var dbBanco = await _bancosRepository.GetById<Banco>(banco.IdBanco);

            if (dbBanco == null)
            {
                throw new BusinessException("Banco inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                if (banco.EsDefault)
                {
                    await _bancosRepository.UpdateEsDefault(false, tran);
                }

                dbBanco.IdCuentaContable = banco.IdCuentaContable;
                dbBanco.IdMoneda = banco.IdMoneda;
                dbBanco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbBanco.Descripcion = banco.Descripcion.ToUpper().Trim();
                dbBanco.CUIT = banco.CUIT?.ToUpper().Trim() ?? String.Empty;
                dbBanco.NroCuenta = banco.NroCuenta?.ToUpper().Trim() ?? String.Empty;
                dbBanco.SaldoDescubierto = banco.SaldoDescubierto;
                dbBanco.IdProveedor = banco.IdProveedor;
                dbBanco.CBU = banco.CBU.ToUpper().Trim();
                dbBanco.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbBanco.EsDefault = banco.EsDefault;

                await _bancosRepository.Update(dbBanco, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Remove(int idBanco)
        {
            var banco = await _bancosRepository.GetById<Banco>(idBanco);

            if (banco == null)
            {
                throw new BusinessException("Banco inexistente");
            }

            banco.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _bancosRepository.Update(banco);
        }


        private void Validate(Banco banco)
        {
            if (banco.Descripcion.IsNull())
            {
                throw new BusinessException($"Ingrese un Nombre para el Banco");
            }

            if (banco.CUIT.IsNull())
            {
                throw new BusinessException($"Ingrese un CUIT para el Banco");
            }

            if (banco.NroCuenta.IsNull())
            {
                throw new BusinessException($"Ingrese un Número de Cuenta para el Banco");
            }

            if (banco.CBU.IsNull())
            {
                throw new BusinessException($"Ingrese el CBU para la cuenta del Banco");
            }
        }

    }
}
