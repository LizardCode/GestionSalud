using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SaldoInicioBanco;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SaldoInicioBancoBusiness : BaseBusiness, ISaldoInicioBancoBusiness
    {
        private readonly ILogger<SaldoInicioBancoBusiness> _logger;
        private readonly ISaldosInicioBancosRepository _saldosInicioBancosRepository;
        private readonly ISaldosInicioBancosChequesRepository _saldosInicioBancosChequesRepository;
        private readonly ISaldosInicioBancosAnticiposRepository _saldosInicioBancosAnticiposRepository;
        private readonly IRecibosAnticiposRepository _recibosAnticiposRepository;
        private readonly IRecibosDetalleRepository _recibosDetalleRepository;
        private readonly IRecibosRepository _recibosRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;
        private readonly IOrdenesPagoRepository _ordenesPagoRepository;
        private readonly IOrdenesPagoDetalleRepository _ordenesPagoDetalleRepository;
        private readonly IOrdenesPagoAnticiposRepository _ordenesPagoAnticiposRepository;
        private readonly IChequesRepository _chequesRepository;
        private readonly IChequesDebitosAsientoRepository _chequesDebitosAsientoRepository;
        private readonly IBancosRepository _bancosRepository;

        public SaldoInicioBancoBusiness(
            ILogger<SaldoInicioBancoBusiness> logger,
            ISaldosInicioBancosRepository saldosInicioBancosRepository,
            ISaldosInicioBancosChequesRepository saldosInicioBancosChequesRepository,
            ISaldosInicioBancosAnticiposRepository saldosInicioBancosAnticiposRepository,
            IRecibosAnticiposRepository recibosAnticiposRepository,
            IRecibosRepository recibosRepository,
            IRecibosDetalleRepository recibosDetalleRepository,
            IEjerciciosRepository ejerciciosRepository,
            IOrdenesPagoRepository ordenesPagoRepository,
            IOrdenesPagoDetalleRepository ordenesPagoDetalleRepository,
            IOrdenesPagoAnticiposRepository ordenesPagoAnticiposRepository,
            IChequesRepository chequesRepository,
            IChequesDebitosAsientoRepository chequesDebitosAsientoRepository,
            IBancosRepository bancosRepository
            )
        {
            _logger = logger;
            _saldosInicioBancosRepository = saldosInicioBancosRepository;
            _saldosInicioBancosChequesRepository = saldosInicioBancosChequesRepository;
            _saldosInicioBancosAnticiposRepository = saldosInicioBancosAnticiposRepository;
            _chequesRepository = chequesRepository;
            _chequesDebitosAsientoRepository = chequesDebitosAsientoRepository;
            _bancosRepository = bancosRepository;
            _recibosRepository = recibosRepository;
            _recibosDetalleRepository = recibosDetalleRepository;
            _recibosAnticiposRepository = recibosAnticiposRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _ordenesPagoRepository = ordenesPagoRepository;
            _ordenesPagoDetalleRepository = ordenesPagoDetalleRepository;
            _ordenesPagoAnticiposRepository = ordenesPagoAnticiposRepository;
        }

        public async Task<SaldoInicioBancoViewModel> Get(int idSaldoInicioBanco)
        {
            var saldoInicio = await _saldosInicioBancosRepository.GetById<SaldoInicioBanco>(idSaldoInicioBanco);

            if (saldoInicio == null)
                return null;

            var anticiposClientes = await _saldosInicioBancosAnticiposRepository.GetAllByIdSaldoInicioBanco(idSaldoInicioBanco, (int)TipoSaldoInicioBancos.AnticiposClientes);
            var anticiposProveedores = await _saldosInicioBancosAnticiposRepository.GetAllByIdSaldoInicioBanco(idSaldoInicioBanco, (int)TipoSaldoInicioBancos.AnticiposProveedores);
            var cheques = await _saldosInicioBancosChequesRepository.GetAllByIdSaldoInicioBanco(idSaldoInicioBanco);

            var model = _mapper.Map<SaldoInicioBancoViewModel>(saldoInicio);
            model.AnticiposClientes = _mapper.Map<List<SaldoInicioBancoAnticiposClientes>>(anticiposClientes);
            model.AnticiposProveedores = _mapper.Map<List<SaldoInicioBancoAnticiposProveedores>>(anticiposProveedores);
            model.Cheques = cheques.Select(item => new SaldoInicioBancoCheques
            {
                IdTipoCheque = item.IdTipoCheque,
                TipoCheque = item.TipoCheque,
                IdBancoChequeComun = item.IdBanco,
                BancoChequeComun = item.Banco,
                FechaChequeComun = item.Fecha,
                NumeroChequeComun = item.NumeroCheque,
                IdBancoEChequeComun = item.IdBanco,
                BancoEChequeComun = item.Banco,
                FechaEChequeComun = item.Fecha,
                NumeroEChequeComun = item.NumeroCheque,
                IdBancoChequeDiferido = item.IdBanco,
                BancoChequeDiferido = item.Banco,
                FechaChequeDiferido = item.Fecha,
                FechaDiferidoChequeDiferido = item.FechaDiferido,
                NumeroChequeDiferido = item.NumeroCheque,
                IdBancoEChequeDiferido = item.IdBanco,
                BancoEChequeDiferido = item.Banco,
                FechaEChequeDiferido = item.Fecha,
                FechaDiferidoEChequeDiferido = item.FechaDiferido,
                NumeroEChequeDiferido = item.NumeroCheque,
                IdChequeTerceros = item.IdCheque,
                BancoChequeTerceros = item.Banco,
                NumeroChequeTerceros = item.NumeroCheque,
                FechaChequeTerceros = item.Fecha,
                FechaDiferidoChequeTerceros = item.FechaDiferido,
                Importe = item.Importe
            }).ToList();

            return model;
        }

        public async Task<DataTablesResponse<Custom.SaldoInicioBanco>> GetAll(DataTablesRequest request)
        {
            var customQuery = _saldosInicioBancosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.SaldoInicioBanco>(request, customQuery.Sql, customQuery.Parameters, true,staticWhere: builder.Sql);
        }

        public async Task New(SaldoInicioBancoViewModel model)
        {
            var saldoInicio = _mapper.Map<SaldoInicioBanco>(model);

            Validate(model);

            var tran = _uow.BeginTransaction();

            try
            {
                saldoInicio.Descripcion = saldoInicio.Descripcion.ToUpper().Trim();
                saldoInicio.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                saldoInicio.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                saldoInicio.IdUsuario = _permissionsBusiness.Value.User.Id;
                saldoInicio.FechaIngreso = DateTime.Now;

                var id = await _saldosInicioBancosRepository.Insert(saldoInicio, tran);

                #region Items Anticipos Clientes

                if (!model.AnticiposClientes.IsNullOrEmpty())
                {
                    foreach (var anticipo in model.AnticiposClientes)
                    {
                        if (anticipo.Importe > 0)
                        {
                            var idRecibo = await _recibosRepository.Insert(new Recibo
                            {
                                IdEjercicio = default,
                                Fecha = anticipo.Fecha,
                                IdCliente = anticipo.IdCliente,
                                IdEstadoRecibo = (int)EstadoRecibo.Finalizado,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                Descripcion = anticipo.Descripcion.ToUpper().Trim(),
                                IdTipoRecibo = (int)TipoRecibo.Anticipo,
                                Importe = anticipo.Importe,
                                IdUsuario = _permissionsBusiness.Value.User.Id,
                                Moneda = anticipo.IdMoneda,
                                Cotizacion = anticipo.Cotizacion,
                                FechaIngreso = DateTime.Now
                            }, tran);

                            await _recibosDetalleRepository.Insert(new ReciboDetalle
                            {
                                IdRecibo = (int)idRecibo,
                                IdTipoCobro = (int)TipoCobro.Efectivo,
                                Descripcion = "COBRO SALDO INICIO",
                                Importe = anticipo.Importe
                            }, tran);

                            await _saldosInicioBancosAnticiposRepository.Insert(new SaldoInicioBancoAnticipo
                            {
                                IdSaldoInicioBanco = (int)id,
                                Descripcion = anticipo.Descripcion.ToUpper().Trim(),
                                IdCliente = anticipo.IdCliente,
                                Cotizacion = anticipo.Cotizacion,
                                Moneda = anticipo.IdMoneda,
                                Fecha = anticipo.Fecha,
                                IdTipoSdoInicio = (int)TipoSaldoInicioBancos.AnticiposClientes,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                IdRecibo = (int)idRecibo,
                                Importe = anticipo.Importe
                            }, tran);
                        }
                    }
                }

                #endregion

                #region Items Anticipos Proveedores

                if (!model.AnticiposProveedores.IsNullOrEmpty())
                {
                    foreach (var anticipo in model.AnticiposProveedores)
                    {
                        if (anticipo.Importe > 0)
                        {
                            var idOrdenPago = await _ordenesPagoRepository.Insert(new OrdenPago
                            {
                                IdEjercicio = default,
                                Fecha = anticipo.Fecha,
                                IdProveedor = anticipo.IdProveedor,
                                IdEstadoOrdenPago = (int)EstadoOrdenPago.Pagado,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                Descripcion = anticipo.Descripcion.ToUpper().Trim(),
                                IdTipoOrdenPago = (int)TipoOrdenPago.Anticipo,
                                Importe = anticipo.Importe,
                                IdUsuario = _permissionsBusiness.Value.User.Id,
                                Moneda = anticipo.IdMoneda,
                                Cotizacion = anticipo.Cotizacion,
                                FechaIngreso = DateTime.Now
                            }, tran);

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = (int)idOrdenPago,
                                IdTipoPago = (int)TipoPago.Efectivo,
                                Descripcion = "PAGO SALDO INICIO",
                                Importe = anticipo.Importe
                            }, tran);

                            await _saldosInicioBancosAnticiposRepository.Insert(new SaldoInicioBancoAnticipo
                            {
                                IdSaldoInicioBanco = (int)id,
                                Descripcion = anticipo.Descripcion.ToUpper().Trim(),
                                IdProveedor = anticipo.IdProveedor,
                                Cotizacion = anticipo.Cotizacion,
                                Moneda = anticipo.IdMoneda,
                                Fecha = anticipo.Fecha,
                                IdTipoSdoInicio = (int)TipoSaldoInicioBancos.AnticiposProveedores,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                IdOrdenPago = (int)idOrdenPago,
                                Importe = anticipo.Importe
                            }, tran);
                        }
                    }
                }

                #endregion

                #region Items Cheques

                if (!model.Cheques.IsNullOrEmpty())
                {
                    foreach (var cheque in model.Cheques)
                    {
                        long idCheque = 0;
                        Banco banco;

                        switch (cheque.IdTipoCheque)
                        {
                            case (int)TipoCheque.Comun:

                                banco = await _bancosRepository.GetById<Banco>(cheque.IdBancoChequeComun.Value);

                                idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.Comun,
                                    NroCheque = cheque.NumeroChequeComun,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    FechaEmision = cheque.FechaChequeComun,
                                    FechaVto = default,
                                    Importe = cheque.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.Librado,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = string.Empty,
                                    CUITFirmante = string.Empty,
                                    Endosante1 = string.Empty,
                                    Endosante2 = string.Empty,
                                    Endosante3 = string.Empty,
                                    CUITEndosante1 = string.Empty,
                                    CUITEndosante2 = string.Empty,
                                    CUITEndosante3 = string.Empty
                                }, tran);

                                await _saldosInicioBancosChequesRepository.Insert(new SaldoInicioBancoCheque
                                {
                                    IdSaldoInicioBanco = (int)id,
                                    IdTipoSdoInicio = (int)TipoSaldoInicioBancos.Cheque,
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    IdTipoCheque = (int)TipoCheque.Comun,
                                    NumeroCheque = cheque.NumeroChequeComun,
                                    Fecha = cheque.FechaChequeComun.Value,
                                    FechaDiferido = default,
                                    Importe = cheque.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);

                                break;

                            case (int)TipoCheque.E_ChequeComun:

                                banco = await _bancosRepository.GetById<Banco>(cheque.IdBancoEChequeComun.Value);

                                idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.E_ChequeComun,
                                    NroCheque = cheque.NumeroEChequeComun,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    FechaEmision = cheque.FechaEChequeComun,
                                    FechaVto = default,
                                    Importe = cheque.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.Librado,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = string.Empty,
                                    CUITFirmante = string.Empty,
                                    Endosante1 = string.Empty,
                                    Endosante2 = string.Empty,
                                    Endosante3 = string.Empty,
                                    CUITEndosante1 = string.Empty,
                                    CUITEndosante2 = string.Empty,
                                    CUITEndosante3 = string.Empty
                                }, tran);

                                await _saldosInicioBancosChequesRepository.Insert(new SaldoInicioBancoCheque
                                {
                                    IdSaldoInicioBanco = (int)id,
                                    IdTipoSdoInicio = (int)TipoSaldoInicioBancos.Cheque,
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    IdTipoCheque = (int)TipoCheque.E_ChequeComun,
                                    NumeroCheque = cheque.NumeroEChequeComun,
                                    Fecha = cheque.FechaEChequeComun.Value,
                                    FechaDiferido = default,
                                    Importe = cheque.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);

                                break;

                            case (int)TipoCheque.Diferido:

                                banco = await _bancosRepository.GetById<Banco>(cheque.IdBancoChequeDiferido.Value);

                                idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.Diferido,
                                    NroCheque = cheque.NumeroChequeDiferido,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    FechaEmision = cheque.FechaChequeDiferido,
                                    FechaVto = cheque.FechaDiferidoChequeDiferido,
                                    Importe = cheque.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.Librado,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = string.Empty,
                                    CUITFirmante = string.Empty,
                                    Endosante1 = string.Empty,
                                    Endosante2 = string.Empty,
                                    Endosante3 = string.Empty,
                                    CUITEndosante1 = string.Empty,
                                    CUITEndosante2 = string.Empty,
                                    CUITEndosante3 = string.Empty
                                }, tran);

                                await _saldosInicioBancosChequesRepository.Insert(new SaldoInicioBancoCheque
                                {
                                    IdSaldoInicioBanco = (int)id,
                                    IdTipoSdoInicio = (int)TipoSaldoInicioBancos.Cheque,
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    IdTipoCheque = (int)TipoCheque.Diferido,
                                    NumeroCheque = cheque.NumeroChequeDiferido,
                                    Fecha = cheque.FechaChequeDiferido.Value,
                                    FechaDiferido = cheque.FechaDiferidoChequeDiferido,
                                    Importe = cheque.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);

                                break;

                            case (int)TipoCheque.E_ChequeDiferido:

                                banco = await _bancosRepository.GetById<Banco>(cheque.IdBancoEChequeDiferido.Value);

                                idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.E_ChequeDiferido,
                                    NroCheque = cheque.NumeroEChequeDiferido,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    FechaEmision = cheque.FechaEChequeDiferido,
                                    FechaVto = cheque.FechaDiferidoEChequeDiferido,
                                    Importe = cheque.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.Librado,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = string.Empty,
                                    CUITFirmante = string.Empty,
                                    Endosante1 = string.Empty,
                                    Endosante2 = string.Empty,
                                    Endosante3 = string.Empty,
                                    CUITEndosante1 = string.Empty,
                                    CUITEndosante2 = string.Empty,
                                    CUITEndosante3 = string.Empty
                                }, tran);

                                await _saldosInicioBancosChequesRepository.Insert(new SaldoInicioBancoCheque
                                {
                                    IdSaldoInicioBanco = (int)id,
                                    IdTipoSdoInicio = (int)TipoSaldoInicioBancos.Cheque,
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdBanco = banco.IdBanco,
                                    Banco = banco.Descripcion,
                                    IdTipoCheque = (int)TipoCheque.E_ChequeDiferido,
                                    NumeroCheque = cheque.NumeroEChequeDiferido,
                                    Fecha = cheque.FechaEChequeDiferido.Value,
                                    FechaDiferido = cheque.FechaDiferidoEChequeDiferido,
                                    Importe = cheque.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);

                                break;

                            case (int)TipoCheque.Terceros:

                                idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.Terceros,
                                    NroCheque = cheque.NumeroChequeTerceros,
                                    IdBanco = default,
                                    Banco = cheque.BancoChequeTerceros.ToUpper().Trim(),
                                    FechaEmision = cheque.FechaChequeTerceros,
                                    FechaVto = cheque.FechaDiferidoChequeTerceros,
                                    Importe = cheque.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.EnCartera,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = string.Empty,
                                    CUITFirmante = string.Empty,
                                    Endosante1 = string.Empty,
                                    Endosante2 = string.Empty,
                                    Endosante3 = string.Empty,
                                    CUITEndosante1 = string.Empty,
                                    CUITEndosante2 = string.Empty,
                                    CUITEndosante3 = string.Empty
                                }, tran);

                                await _saldosInicioBancosChequesRepository.Insert(new SaldoInicioBancoCheque
                                {
                                    IdSaldoInicioBanco = (int)id,
                                    IdTipoSdoInicio = (int)TipoSaldoInicioBancos.Cheque,
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdBanco = default,
                                    Banco = cheque.BancoChequeTerceros.ToUpper().Trim(),
                                    IdTipoCheque = (int)TipoCheque.Terceros,
                                    NumeroCheque = cheque.NumeroChequeTerceros,
                                    Fecha = cheque.FechaChequeTerceros.Value,
                                    FechaDiferido = cheque.FechaDiferidoChequeTerceros,
                                    Importe = cheque.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);
                                break;
                        }
                    }
                }

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

        public async Task Remove(int idSaldoInicioBanco)
        {
            var saldoInicio = await _saldosInicioBancosRepository.GetById<SaldoInicioBanco>(idSaldoInicioBanco);

            if (saldoInicio == null)
            {
                throw new BusinessException("Número de Saldo Inicio Inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var anticiposClientes = await _saldosInicioBancosAnticiposRepository.GetAllByIdSaldoInicioBanco(idSaldoInicioBanco, (int)TipoSaldoInicioBancos.AnticiposClientes, tran);
                var anticiposProveedores = await _saldosInicioBancosAnticiposRepository.GetAllByIdSaldoInicioBanco(idSaldoInicioBanco, (int)TipoSaldoInicioBancos.AnticiposProveedores, tran);
                var cheques = await _saldosInicioBancosChequesRepository.GetAllByIdSaldoInicioBanco(idSaldoInicioBanco, tran);

                foreach (var anticipo in anticiposClientes)
                {
                    var reciboAnticipo = await _recibosAnticiposRepository.GetByIdAnticipo(anticipo.IdRecibo ?? 0, tran);

                    if(reciboAnticipo != default)
                        throw new BusinessException($"El Anticipo Nro. {anticipo.IdRecibo} se encuentra Imputado en un Recibo. No se puede eliminar el Saldo Inicio.");

                    await _recibosDetalleRepository.DeleteByIdRecibo(anticipo.IdRecibo ?? 0, tran);
                    var recibo = await _recibosRepository.GetById<Recibo>(anticipo.IdRecibo ?? 0, tran);

                    recibo.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _recibosRepository.Update(recibo, tran);

                }

                foreach (var anticipo in anticiposProveedores)
                {
                    var ordenPagoAnticipo = await _ordenesPagoAnticiposRepository.GetByIdAnticipo(anticipo.IdOrdenPago ?? 0, tran);

                    if (ordenPagoAnticipo != default)
                        throw new BusinessException($"El Anticipo Nro. {anticipo.IdOrdenPago} se encuentra Imputado en una Orden de Pago. No se puede eliminar el Saldo Inicio.");

                    await _ordenesPagoDetalleRepository.DeleteByIdOrdenPago(anticipo.IdOrdenPago ?? 0, tran);
                    var ordenPago = await _ordenesPagoRepository.GetById<OrdenPago>(anticipo.IdOrdenPago ?? 0, tran);

                    ordenPago.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _ordenesPagoRepository.Update(ordenPago, tran);

                }

                foreach (var cheque in cheques)
                {
                    var asiento = await _chequesDebitosAsientoRepository.GetByIdCheque(cheque.IdCheque, tran);
                    if (asiento != default)
                        throw new BusinessException($"El Cheque Nro. {cheque.NumeroCheque} se encuentra Debitado. No se puede eliminar el Saldo Inicio.");
                    var chq = await _chequesRepository.GetById<Cheque>(cheque.IdCheque, tran);
                    if (chq.IdTipoCheque == (int)TipoCheque.Terceros)
                    {
                        if (chq.IdEstadoCheque == (int)EstadoCheque.Entregado || chq.IdEstadoCheque == (int)EstadoCheque.Debitado_Depositado)
                            throw new BusinessException($"El Cheque Nro. {cheque.NumeroCheque} se encuentra Librado/Entregado. No se puede eliminar el Saldo Inicio.");
                    }
                    chq.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _chequesRepository.Update(chq, tran);
                }

                saldoInicio.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _saldosInicioBancosRepository.Update(saldoInicio, tran);

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

        private void Validate(SaldoInicioBancoViewModel saldoInicio)
        {
            if (saldoInicio.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una descripción para el Saldo Inicio");
            }

            if ((saldoInicio.Cheques == null || saldoInicio.Cheques.Count == 0) && (saldoInicio.AnticiposClientes == null || saldoInicio.AnticiposClientes.Count == 0) && (saldoInicio.AnticiposProveedores == null || saldoInicio.AnticiposProveedores.Count == 0))
            {
                throw new BusinessException("Ingrese al menos un Cheque, Anticipo Clientes o Proveedores para el Saldo Inicio.");
            }

            var ejercicio = (_ejerciciosRepository.GetAllByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult()).FirstOrDefault();

            if (ejercicio != default)
            {
                if (!saldoInicio.AnticiposClientes.IsNullOrEmpty())
                {
                    foreach (var anticipo in saldoInicio.AnticiposClientes)
                    {
                        if (anticipo.Fecha.CompareTo(ejercicio.FechaInicio) >= 0)
                        {
                            throw new BusinessException("Error en la Fecha del Anticipo a Clientes. La fecha debe ser Menor a la del Comienzo del Ejercicio.");
                        }
                    }
                }

                if (!saldoInicio.AnticiposProveedores.IsNullOrEmpty())
                {
                    foreach (var anticipo in saldoInicio.AnticiposProveedores)
                    {
                        if (anticipo.Fecha.CompareTo(ejercicio.FechaInicio) >= 0)
                        {
                            throw new BusinessException("Error en la Fecha del Anticipo a Proveedores. La fecha debe ser Menor a la del Comienzo del Ejercicio.");
                        }
                    }
                }
            }
        }

        public async Task<Custom.SaldoInicioBanco> ObtenerDetalleSaldoInicio(int id)
        {
            var saldoInicio = await _saldosInicioBancosRepository.GetById<SaldoInicioBanco>(id);

            var cheques = await _saldosInicioBancosChequesRepository.GetAllByIdSaldoInicioBanco(id);
            var anticiposClientess = await _saldosInicioBancosAnticiposRepository.GetAllByIdSaldoInicioBanco(id, (int)TipoSaldoInicioBancos.AnticiposClientes);
            var anticiposProveedores = await _saldosInicioBancosAnticiposRepository.GetAllByIdSaldoInicioBanco(id, (int)TipoSaldoInicioBancos.AnticiposProveedores  );

            var detalle = _mapper.Map<Custom.SaldoInicioBanco>(saldoInicio);
            detalle.Cheques = _mapper.Map<List<SaldoInicioBancoCheque>>(cheques);
            detalle.AnticiposClientes = _mapper.Map<List<SaldoInicioBancoAnticipo>>(anticiposClientess);
            detalle.AnticiposProveedores = _mapper.Map<List<SaldoInicioBancoAnticipo>>(anticiposProveedores);

            return detalle;
        }
    }
}

