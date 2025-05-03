using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.DepositosBanco;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class DepositosBancoBusiness: BaseBusiness, IDepositosBancoBusiness
    {
        private readonly ILogger<DepositosBancoBusiness> _logger;
        private readonly IDepositosBancoRepository _depositosBancoRepository;
        private readonly IDepositosBancoAsientoRepository _depositosBancoAsientoRepository;
        private readonly IDepositosBancoDetalleRepository _depositosBancoDetalleRepository;
        private readonly IChequesRepository _chequesRepository;
        private readonly ITransferenciasRepository _transferenciasRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public DepositosBancoBusiness(
            ILogger<DepositosBancoBusiness> logger,
            IDepositosBancoRepository depositosBancoRepository,
            IDepositosBancoAsientoRepository depositosBancoAsientoRepository,
            IDepositosBancoDetalleRepository depositosBancoDetalleRepository,
            IChequesRepository chequesRepository,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            ITransferenciasRepository transferenciasRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IBancosRepository bancosRepository)
        {
            _logger = logger;
            _depositosBancoRepository = depositosBancoRepository;
            _depositosBancoAsientoRepository = depositosBancoAsientoRepository;
            _depositosBancoDetalleRepository = depositosBancoDetalleRepository;
            _chequesRepository = chequesRepository;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _bancosRepository = bancosRepository;
            _transferenciasRepository = transferenciasRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
        }

        public async Task<DepositosBancoViewModel> Get(int idDepositoBanco)
        {
            var depositoBanco = await _depositosBancoRepository.GetById<DepositoBanco>(idDepositoBanco);

            if (depositoBanco == null)
                return null;

            var items = await _depositosBancoDetalleRepository.GetAllByIdDepositoBanco(idDepositoBanco);

            var model = _mapper.Map<DepositosBancoViewModel>(depositoBanco);
            model.Items = items.Select(item => new DepositosBancoDetalle
            {
                IdTipoDeposito = item.IdTipoDeposito,
                Descripcion = item.Descripcion,
                IdBancoChequeComun = item.IdBanco,
                BancoChequeComun = item.BancoCheque,
                FechaChequeComun = item.FechaEmision,
                NumeroChequeComun = item.NroCheque,
                IdBancoEChequeComun = item.IdBanco,
                BancoEChequeComun = item.BancoCheque,
                FechaEChequeComun = item.FechaEmision,
                NumeroEChequeComun = item.NroCheque,
                IdBancoChequeDiferido = item.IdBanco,
                BancoChequeDiferido = item.BancoCheque,
                FechaChequeDiferido = item.FechaEmision,
                FechaDiferidoChequeDiferido = item.FechaVto,
                NumeroChequeDiferido = item.NroCheque,
                IdBancoEChequeDiferido = item.IdBanco,
                BancoEChequeDiferido = item.BancoCheque,
                FechaEChequeDiferido = item.FechaEmision,
                FechaDiferidoEChequeDiferido = item.FechaVto,
                NumeroEChequeDiferido = item.NroCheque,
                IdChequeTerceros = item.IdCheque,
                BancoChequeTerceros = item.BancoCheque,
                FechaChequeTerceros = item.FechaEmision,
                FechaDiferidoChequeTerceros = item.FechaVto,
                NumeroChequeTerceros = item.NroCheque,
                IdBancoTranferencia = item.IdBancoTranferencia,
                BancoTranferencia = item.BancoCheque,
                FechaTransferencia = item.FechaTransferencia,
                NumeroTransferencia = item.NroTransferencia,
                Importe = item.Importe
            }).ToList();

            return model;
        }

        public async Task<DataTablesResponse<Custom.DepositoBanco>> GetAll(DataTablesRequest request)
        {
            var customQuery = _depositosBancoRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdBanco"))
                builder.Append($"AND IdBanco = {filters["IdBanco"]}");

            if (filters.ContainsKey("NumeroRecibo"))
                builder.Append($"AND IdDepositoBanco = {filters["NumeroDepositoBanco"]}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.DepositoBanco>(request, customQuery.Sql, customQuery.Parameters, true,staticWhere: builder.Sql);
        }

        public async Task New(DepositosBancoViewModel model)
        {
            var depositoBanco = _mapper.Map<DepositoBanco>(model);

            Validate(model);

            var tran = _uow.BeginTransaction();

            try
            {
                depositoBanco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                depositoBanco.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                depositoBanco.Importe = model.Items.Sum(i => i.Importe);
                depositoBanco.IdUsuario = _permissionsBusiness.Value.User.Id;
                depositoBanco.Moneda = model.IdMoneda;
                depositoBanco.Cotizacion = model.Cotizacion ?? 1;
                depositoBanco.FechaIngreso = DateTime.Now;

                var id = await _depositosBancoRepository.Insert(depositoBanco, tran);

                foreach (var item in model.Items)
                {
                    switch(item.IdTipoDeposito)
                    {
                        case (int)TipoDeposito.Efectivo:
                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe
                            }, tran);
                            break;

                        case (int)TipoDeposito.ChequeComun:
                            var chequeComun = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeComun, item.IdBancoChequeComun.Value, (int)TipoCheque.Comun, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            chequeComun.FechaEmision = item.FechaChequeComun;
                            chequeComun.Importe = item.Importe;
                            chequeComun.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(chequeComun, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeComun.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.EChequeComun:
                            var eChequeComun = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeComun, item.IdBancoEChequeComun.Value, (int)TipoCheque.E_ChequeComun, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            eChequeComun.FechaEmision = item.FechaEChequeComun;
                            eChequeComun.Importe = item.Importe;
                            eChequeComun.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(eChequeComun, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = eChequeComun.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.ChequeDiferido:
                            var chequeDiferido = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeDiferido, item.IdBancoChequeDiferido.Value, (int)TipoCheque.Diferido, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            chequeDiferido.FechaEmision = item.FechaChequeDiferido;
                            chequeDiferido.FechaVto = item.FechaDiferidoChequeDiferido;
                            chequeDiferido.Importe = item.Importe;
                            chequeDiferido.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(chequeDiferido, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeDiferido.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.EChequeDiferido:
                            var eChequeDiferido = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeDiferido, item.IdBancoEChequeDiferido.Value, (int)TipoCheque.E_ChequeDiferido, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            eChequeDiferido.FechaEmision = item.FechaEChequeDiferido;
                            eChequeDiferido.FechaVto = item.FechaDiferidoEChequeDiferido;
                            eChequeDiferido.Importe = item.Importe;
                            eChequeDiferido.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(eChequeDiferido, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = eChequeDiferido.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.ChequeTerceros:
                            var chequeTerceros = await _chequesRepository.GetById<Cheque>(item.IdChequeTerceros.Value, tran);

                            chequeTerceros.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(chequeTerceros, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeTerceros.IdCheque
                            }, tran);
                            break;

                        case (int)TipoPago.Transferencia:
                            var idTransferencia = await _transferenciasRepository.Insert(new Transferencia
                            {
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                Fecha = item.FechaTransferencia.Value,
                                NroTransferencia = item.NumeroTransferencia ?? String.Empty,
                                IdBanco = item.IdBancoTranferencia,
                                BancoOrigen = String.Empty,
                                Importe = item.Importe
                            }, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = (int)id,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdTransferencia = (int)idTransferencia
                            }, tran);
                            break;
                    }
                }

                #region Asiento Contable

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = depositoBanco.IdEjercicio,
                    Descripcion = depositoBanco.Descripcion,
                    Fecha = depositoBanco.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var indItem = 1;
                CuentaContable cuentaContable;
                foreach (var item in model.Items)
                {
                    switch (item.IdTipoDeposito)
                    {
                        case (int)TipoDeposito.Efectivo:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                            break;
                        case (int)TipoDeposito.ChequeComun:
                        case (int)TipoDeposito.EChequeComun:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.BANCOS, tran);
                            break;
                        case (int)TipoDeposito.ChequeDiferido:
                        case (int)TipoDeposito.EChequeDiferido:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.BANCOS, tran);
                            break;
                        case (int)TipoDeposito.ChequeTerceros:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.VALORES_A_DEPOSITAR, tran);
                            break;
                        case (int)TipoDeposito.Transferencia:
                            var banco = await _bancosRepository.GetById<Banco>(item.IdBancoTranferencia.Value, tran);
                            cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(banco.IdCuentaContable, tran);
                            break;

                        default:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                            break;
                    }

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = item.Importe * (model.Cotizacion ?? 1),
                        Creditos = 0
                    }, tran);
                }

                var cuentaBanco = await _bancosRepository.GetById<Banco>(depositoBanco.IdBanco, tran);
                cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(cuentaBanco.IdCuentaContable, tran);

                if (cuentaContable == default)
                    throw new BusinessException("No Existe la Cuenta Contable para el Banco Seleccionado. Verfique.");

                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)idAsiento,
                    Item = indItem++,
                    IdCuentaContable = cuentaContable.IdCuentaContable,
                    Detalle = cuentaContable.Descripcion,
                    Debitos = 0,
                    Creditos = model.Items.Sum(i => i.Importe) * (model.Cotizacion ?? 1)
                }, tran);

                await _depositosBancoAsientoRepository.Insert(new DepositoBancoAsiento
                {
                    IdAsiento = (int)idAsiento,
                    IdDepositoBanco = (int)id
                }, tran);

                #endregion

                tran.Commit();
            }
            catch (BusinessException ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw ex;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Remove(int idDepositoBanco)
        {
            var depositoBanco = await _depositosBancoRepository.GetById<DepositoBanco>(idDepositoBanco);

            if (depositoBanco == null)
            {
                throw new BusinessException("Número de Depósito Bancario Inexistente");
            }

            if (_cierreMesRepository.MesCerrado(depositoBanco.IdEjercicio, depositoBanco.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Recibo se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var depositoBancoAsiento = await _depositosBancoAsientoRepository.GetByIdDepositoBanco(idDepositoBanco, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(depositoBancoAsiento?.IdAsiento ?? 0, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(depositoBancoAsiento?.IdAsiento ?? 0, tran);
                await _depositosBancoAsientoRepository.DeleteByIdDepositoBanco(idDepositoBanco, tran);

                if (asiento != null)
                {
                    asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _asientosRepository.Update(asiento, tran);
                }

                //Borro Detalle Previo y Analizar Detalle
                var depositoBancoDetalle = await _depositosBancoDetalleRepository.GetAllByIdDepositoBanco(idDepositoBanco, tran);
                await _depositosBancoDetalleRepository.DeleteByIdDepositoBanco(idDepositoBanco, tran);
                foreach (var detalle in depositoBancoDetalle)
                {
                    if (detalle.IdCheque.HasValue)
                    {
                        var cheque = await _chequesRepository.GetById<Cheque>(detalle.IdCheque.Value, tran);
                        cheque.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                        cheque.FechaEmision = default;
                        cheque.FechaVto = default;
                        cheque.Importe = 0;
                        cheque.IdEstadoCheque = (int)EstadoCheque.EnCartera;

                        await _chequesRepository.Update(cheque, tran);
                    }
                    if (detalle.IdTransferencia.HasValue)
                    {
                        await _transferenciasRepository.DeleteById(detalle.IdTransferencia.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                    }
                }

                depositoBanco.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _depositosBancoRepository.Update(depositoBanco, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Update(DepositosBancoViewModel model)
        {
            var depositoBanco = _mapper.Map<DepositoBanco>(model);

            Validate(model);

            var dbDepositoBanco = await _depositosBancoRepository.GetById<DepositoBanco>(depositoBanco.IdDepositoBanco);

            if (dbDepositoBanco == null)
            {
                throw new BusinessException("Número de Depósito Bancario Inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbDepositoBanco.Fecha = depositoBanco.Fecha;
                dbDepositoBanco.IdEjercicio = depositoBanco.IdEjercicio;
                dbDepositoBanco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbDepositoBanco.Descripcion = depositoBanco.Descripcion.ToUpper().Trim();
                dbDepositoBanco.Importe = model.Items.Sum(i => i.Importe);
                dbDepositoBanco.Moneda = model.IdMoneda;
                dbDepositoBanco.Cotizacion = model.Cotizacion ?? 1;
                dbDepositoBanco.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                
                var id = await _depositosBancoRepository.Update(dbDepositoBanco, tran);

                //Borro Detalle Previo y Analizar Detalle
                var reciboDetalle = await _depositosBancoDetalleRepository.GetAllByIdDepositoBanco(dbDepositoBanco.IdDepositoBanco, tran);
                
                await _depositosBancoDetalleRepository.DeleteByIdDepositoBanco(dbDepositoBanco.IdDepositoBanco, tran);
                
                foreach (var detalle in reciboDetalle)
                {
                    if (detalle.IdCheque.HasValue)
                    {
                        var cheque = await _chequesRepository.GetById<Cheque>(detalle.IdCheque.Value, tran);
                        cheque.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                        cheque.FechaEmision = default;
                        cheque.FechaVto = default;
                        cheque.Importe = 0;
                        cheque.IdEstadoCheque = (int)EstadoCheque.EnCartera;

                        await _chequesRepository.Update(cheque, tran);
                    }
                    if (detalle.IdTransferencia.HasValue)
                    {
                        await _transferenciasRepository.DeleteById(detalle.IdTransferencia.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                    }
                }

                foreach (var item in model.Items)
                {
                    switch (item.IdTipoDeposito)
                    {
                        case (int)TipoDeposito.Efectivo:
                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe
                            }, tran);
                            break;

                        case (int)TipoDeposito.ChequeComun:
                            var chequeComun = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeComun, item.IdBancoChequeComun.Value, (int)TipoCheque.Comun, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            chequeComun.FechaEmision = item.FechaChequeComun;
                            chequeComun.Importe = item.Importe;
                            chequeComun.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(chequeComun, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeComun.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.EChequeComun:
                            var eChequeComun = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeComun, item.IdBancoEChequeComun.Value, (int)TipoCheque.E_ChequeComun, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            eChequeComun.FechaEmision = item.FechaEChequeComun;
                            eChequeComun.Importe = item.Importe;
                            eChequeComun.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(eChequeComun, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = eChequeComun.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.ChequeDiferido:
                            var chequeDiferido = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeDiferido, item.IdBancoChequeDiferido.Value, (int)TipoCheque.Diferido, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            chequeDiferido.FechaEmision = item.FechaChequeDiferido;
                            chequeDiferido.FechaVto = item.FechaDiferidoChequeDiferido;
                            chequeDiferido.Importe = item.Importe;
                            chequeDiferido.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(chequeDiferido, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeDiferido.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.EChequeDiferido:
                            var eChequeDiferido = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeDiferido, item.IdBancoEChequeDiferido.Value, (int)TipoCheque.E_ChequeDiferido, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            eChequeDiferido.FechaEmision = item.FechaEChequeDiferido;
                            eChequeDiferido.FechaVto = item.FechaDiferidoEChequeDiferido;
                            eChequeDiferido.Importe = item.Importe;
                            eChequeDiferido.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(eChequeDiferido, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = eChequeDiferido.IdCheque
                            }, tran);
                            break;

                        case (int)TipoDeposito.ChequeTerceros:
                            var chequeTerceros = await _chequesRepository.GetById<Cheque>(item.IdChequeTerceros.Value, tran);

                            chequeTerceros.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;

                            await _chequesRepository.Update(chequeTerceros, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeTerceros.IdCheque
                            }, tran);
                            break;

                        case (int)TipoPago.Transferencia:
                            var idTransferencia = await _transferenciasRepository.Insert(new Transferencia
                            {
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                Fecha = item.FechaTransferencia.Value,
                                NroTransferencia = item.NumeroTransferencia ?? String.Empty,
                                IdBanco = item.IdBancoTranferencia,
                                BancoOrigen = String.Empty,
                                Importe = item.Importe
                            }, tran);

                            await _depositosBancoDetalleRepository.Insert(new DepositoBancoDetalle
                            {
                                IdDepositoBanco = dbDepositoBanco.IdDepositoBanco,
                                IdTipoDeposito = item.IdTipoDeposito,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdTransferencia = (int)idTransferencia
                            }, tran);
                            break;

                    }
                }

                #region Asiento Contable

                var depositoBancoAsiento = await _depositosBancoAsientoRepository.GetByIdDepositoBanco(dbDepositoBanco.IdDepositoBanco, tran);
                var dbAsiento = await _asientosRepository.GetById<Asiento>(depositoBancoAsiento?.IdAsiento ?? 0, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(depositoBancoAsiento?.IdAsiento ?? 0, tran);
                await _depositosBancoAsientoRepository.DeleteByIdDepositoBanco(dbDepositoBanco.IdDepositoBanco, tran);

                if (dbAsiento != null)
                {
                    dbAsiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _asientosRepository.Update(dbAsiento, tran);
                }

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = depositoBanco.IdEjercicio,
                    Descripcion = depositoBanco.Descripcion,
                    Fecha = depositoBanco.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var indItem = 1;
                CuentaContable cuentaContable;
                foreach (var item in model.Items)
                {
                    switch (item.IdTipoDeposito)
                    {
                        case (int)TipoDeposito.Efectivo:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                            break;
                        case (int)TipoDeposito.ChequeDiferido:
                        case (int)TipoDeposito.EChequeDiferido:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.BANCOS, tran);
                            break;
                        case (int)TipoDeposito.ChequeComun:
                        case (int)TipoDeposito.EChequeComun:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.BANCOS, tran);
                            break;
                        case (int)TipoDeposito.ChequeTerceros:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.VALORES_A_DEPOSITAR, tran);
                            break;
                        case (int)TipoDeposito.Transferencia:
                            var banco = await _bancosRepository.GetById<Banco>(item.IdBancoTranferencia.Value, tran);
                            cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(banco.IdCuentaContable, tran);
                            break;

                        default:
                            cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                            break;
                    }

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = item.Importe * (model.Cotizacion ?? 1),
                        Creditos = 0
                    }, tran);
                }

                var cuentaBanco = await _bancosRepository.GetById<Banco>(dbDepositoBanco.IdBanco, tran);
                cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(cuentaBanco.IdCuentaContable, tran);

                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)idAsiento,
                    Item = indItem++,
                    IdCuentaContable = cuentaContable.IdCuentaContable,
                    Detalle = cuentaContable.Descripcion,
                    Debitos = 0,
                    Creditos = model.Items.Sum(i => i.Importe) * (model.Cotizacion ?? 1)
                }, tran);

                await _depositosBancoAsientoRepository.Insert(new DepositoBancoAsiento
                {
                    IdAsiento = (int)idAsiento,
                    IdDepositoBanco = dbDepositoBanco.IdDepositoBanco
                }, tran);

                #endregion

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        private void Validate(DepositosBancoViewModel depositosBanco)
        {
            if (depositosBanco.Items == null || depositosBanco.Items.Count == 0)
            {
                throw new BusinessException("Ingrese al menos un Item para el Depósito Bancario.");
            }

            if (_ejerciciosRepository.EjercicioCerrado(depositosBanco.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(depositosBanco.IdEjercicio, depositosBanco.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Depósito Bancario no es Válida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(depositosBanco.IdEjercicio, depositosBanco.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha del Depósito Bancario esta Cerrado. Verifique.");
            }
        }

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {
            var asiento = await _depositosBancoAsientoRepository.GetByIdDepositoBanco(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
        }

        public async Task<bool> VerificarChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
            => ((await _chequesRepository.GetByNumeroCheque(nroCheque, idBanco, idTipoCheque, _permissionsBusiness.Value.User.IdEmpresa)) != default);
    }
}

