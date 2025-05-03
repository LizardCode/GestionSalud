using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Recibos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class RecibosBusiness: BaseBusiness, IRecibosBusiness
    {
        private readonly ILogger<RecibosBusiness> _logger;
        private readonly IRecibosAsientoRepository _recibosAsientoRepository;
        private readonly IRecibosRepository _recibosRepository;
        private readonly IRecibosAnticiposRepository _recibosAnticiposRepository;
        private readonly IRecibosComprobantesRepository _recibosComprobantesRepository;
        private readonly IRecibosDetalleRepository _recibosDetalleRepository;
        private readonly IRecibosRetencionesRepository _recibosRetencionesRepository;
        private readonly IChequesRepository _chequesRepository;
        private readonly ITransferenciasRepository _transferenciasRepository;
        private readonly IDocumentosRepository _documentosRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public RecibosBusiness(
            ILogger<RecibosBusiness> logger,
            IRecibosAsientoRepository recibosAsientoRepository,
            IRecibosRepository recibosRepository,
            IRecibosAnticiposRepository recibosAnticiposRepository,
            IRecibosComprobantesRepository recibosComprobantesRepository,
            IRecibosDetalleRepository recibosDetalleRepository,
            IRecibosRetencionesRepository recibosRetencionesRepository,
            IChequesRepository chequesRepository,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            ITransferenciasRepository transferenciasRepository,
            IDocumentosRepository documentosRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IBancosRepository bancosRepository)
        {
            _logger = logger;
            _recibosRepository = recibosRepository;
            _recibosAsientoRepository = recibosAsientoRepository;
            _recibosAnticiposRepository = recibosAnticiposRepository;
            _recibosComprobantesRepository = recibosComprobantesRepository;
            _recibosDetalleRepository = recibosDetalleRepository;
            _recibosRetencionesRepository = recibosRetencionesRepository;
            _chequesRepository = chequesRepository;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _bancosRepository = bancosRepository;
            _transferenciasRepository = transferenciasRepository;
            _documentosRepository = documentosRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;

        }

        public async Task<RecibosViewModel> Get(int idRecibo)
        {
            var recibo = await _recibosRepository.GetById<Recibo>(idRecibo);

            if (recibo == null)
                return null;

            var items = await _recibosDetalleRepository.GetAllByIdRecibo(idRecibo);
            var retenciones = await _recibosRetencionesRepository.GetAllByIdRecibo(idRecibo);
            var anticipos = await _recibosAnticiposRepository.GetByIdRecibo(idRecibo, recibo.IdCliente);

            var model = _mapper.Map<RecibosViewModel>(recibo);
            model.Items = _mapper.Map<List<RecibosDetalle>>(items);
            model.Retenciones = _mapper.Map<List<RecibosRetencion>>(retenciones);
            model.Anticipos = _mapper.Map<List<RecibosAnticipo>>(anticipos);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Recibo>> GetAll(DataTablesRequest request)
        {
            var customQuery = _recibosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEstadoRecibo"))
                builder.Append($"AND IdEstadoRecibo = {filters["IdEstadoRecibo"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdCliente"))
                builder.Append($"AND IdCliente = {filters["IdCliente"]}");

            if (filters.ContainsKey("NumeroRecibo"))
                builder.Append($"AND IdRecibo = {filters["NumeroRecibo"]}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Recibo>(request, customQuery.Sql, customQuery.Parameters, true,staticWhere: builder.Sql);
        }

        public async Task New(RecibosViewModel model)
        {
            var recibo = _mapper.Map<Recibo>(model);

            Validate(model);

            model.Cotizacion = model.Cotizacion == 0 ? 1 : model.Cotizacion;

            var tran = _uow.BeginTransaction();

            try
            {
                recibo.IdEstadoRecibo = (int)EstadoRecibo.Ingresado;
                recibo.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                recibo.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                recibo.Importe = model.Items?.Sum(i => i.Importe) ?? 0 + (model.Anticipos?.Where(a => a.Importe > 0)?.Sum(a => a.Importe) ?? 0);
                recibo.Total = recibo.Importe + (model.Retenciones?.Sum(r => r.Importe) ?? 0);
                recibo.IdUsuario = _permissionsBusiness.Value.User.Id;
                recibo.Moneda = model.IdMoneda;
                recibo.MonedaCobro = model.IdMonedaCobro;
                recibo.Cotizacion = model.Cotizacion;
                recibo.FechaIngreso = DateTime.Now;

                var id = await _recibosRepository.Insert(recibo, tran);

                if (model.Items?.Count > 0)
                {
                    foreach (var item in model.Items)
                    {
                        switch (item.IdTipoCobro)
                        {
                            case (int)TipoCobro.Efectivo:
                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = (int)id,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion.ToUpper().Trim(),
                                    Importe = item.Importe
                                }, tran);
                                break;

                            case (int)TipoCobro.Cheque:
                                var idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.Terceros,
                                    NroCheque = item.NroCheque,
                                    Banco = item.BancoCheque?.ToUpper().Trim(),
                                    FechaEmision = item.FechaEmision,
                                    FechaVto = item.FechaVto,
                                    Importe = item.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.EnCartera,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = item.FirmanteCheque?.ToUpper().Trim(),
                                    CUITFirmante = item.CUITFirmante,
                                    Endosante1 = item.Endosante1Cheque?.ToUpper().Trim(),
                                    CUITEndosante1 = item.CUITEndosante1,
                                    Endosante2 = item.Endosante2Cheque?.ToUpper().Trim(),
                                    CUITEndosante2 = item.CUITEndosante2,
                                    Endosante3 = item.Endosante3Cheque?.ToUpper().Trim(),
                                    CUITEndosante3 = item.CUITEndosante3
                                }, tran);

                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = (int)id,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion.ToUpper().Trim(),
                                    Importe = item.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);
                                break;

                            case (int)TipoCobro.Transferencia:
                                var idTransferencia = await _transferenciasRepository.Insert(new Transferencia
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    Fecha = item.FechaTransferencia.Value,
                                    NroTransferencia = item.NroTransferencia ?? string.Empty,
                                    IdBanco = item.IdBancoTranferencia,
                                    BancoOrigen = item.BancoOrigen?.ToUpper().Trim() ?? string.Empty,
                                    Importe = item.Importe
                                }, tran);

                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = (int)id,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion?.ToUpper().Trim(),
                                    Importe = item.Importe,
                                    IdTransferencia = (int)idTransferencia
                                }, tran);
                                break;

                            case (int)TipoCobro.Documento:
                                var idDocumento = await _documentosRepository.Insert(new Documento
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    FechaEmision = item.FechaDocumento.Value,
                                    FechaVto = item.FechaVto.Value,
                                    NroDocumento = item.NroDocumento,
                                    Firmante = item.FirmanteDocumento?.ToUpper().Trim(),
                                    CUITFirmante = item.CUITFirmanteDocumento,
                                    Importe = item.Importe
                                });

                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = (int)id,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion?.ToUpper().Trim(),
                                    Importe = item.Importe,
                                    IdDocumento = (int)idDocumento
                                }, tran);
                                break;
                        }
                    }
                }

                if (model.Retenciones?.Count > 0)
                {
                    foreach (var retencion in model.Retenciones)
                    {
                        var cuentaRetencion = retencion.IdCategoria switch
                        {
                            (int)CategoriaRetencion.Ganancias => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_GANANCIAS_CLIENTES, tran),
                            (int)CategoriaRetencion.IVA => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_IVA_CLIENTES, tran),
                            (int)CategoriaRetencion.SUSS => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_SUSS_CLIENTES, tran),
                            (int)CategoriaRetencion.IngresosBrutos => await _cuentasContablesRepository.GetById<CuentaContable>(retencion.IdCuentaContable),
                            _ => throw new BusinessException($"Categoria de Retención {retencion.Categoria} Invalida. Verifique.")
                        };

                        if (cuentaRetencion == default)
                            throw new BusinessException($"No Existe Cuenta Contable para la Categoria de Retención {retencion.Categoria}. Verifique.");

                        retencion.IdCuentaContable = cuentaRetencion.IdCuentaContable;

                        await _recibosRetencionesRepository.Insert(new ReciboRetencion
                        {
                            IdRecibo = (int)id,
                            Fecha = retencion.Fecha,
                            IdCategoria = retencion.IdCategoria,
                            Categoria = ((CategoriaRetencion)retencion.IdCategoria).Description(),
                            NroRetencion = retencion.NroRetencion,
                            BaseImponible = (retencion.BaseImponible ?? 0),
                            Importe = retencion.Importe,
                            IdCuentaContable = cuentaRetencion.IdCuentaContable
                        }, tran);
                    }
                }

                if (model.Anticipos?.Count > 0)
                {
                    foreach (var anticipo in model.Anticipos)
                    {
                        if (anticipo.Importe > 0)
                        {
                            await _recibosAnticiposRepository.Insert(new ReciboAnticipo
                            {
                                IdRecibo = (int)id,
                                IdAnticipo = anticipo.IdRecibo,
                                Importe = anticipo.Importe
                            }, tran);
                        }
                    }
                }

                #region Asiento Contable

                if (model.Items?.Sum(i => i.Importe) > 0)
                {
                    var asiento = new Asiento
                    {
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        IdEjercicio = recibo.IdEjercicio,
                        Descripcion = recibo.Descripcion.ToUpper().Trim(),
                        Fecha = recibo.Fecha,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        FechaIngreso = DateTime.Now
                    };

                    var idAsiento = await _asientosRepository.Insert(asiento, tran);

                    var indItem = 1;
                    CuentaContable cuentaContable;
                    foreach (var item in model.Items)
                    {
                        switch (item.IdTipoCobro)
                        {
                            case (int)TipoCobro.Efectivo:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                                break;
                            case (int)TipoCobro.Cheque:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.VALORES_A_DEPOSITAR, tran);
                                break;
                            case (int)TipoCobro.Transferencia:
                                var banco = await _bancosRepository.GetById<Banco>(item.IdBancoTranferencia.Value, tran);
                                cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(banco.IdCuentaContable, tran);
                                break;
                            case (int)TipoCobro.Documento:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.DOCUMENTOS_EN_CARTERA, tran);
                                break;

                            default:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                                break;
                        }

                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable para la forma de Cobro Seleccionada. Verifique.");


                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = item.Importe * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion),
                            Creditos = 0
                        }, tran);
                    }

                    if (model.Anticipos?.Count > 0)
                    {
                        cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.ANTICIPOS_CLIENTES, tran);

                        foreach (var anticipo in model.Anticipos)
                        {
                            if (anticipo.Importe > 0)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = cuentaContable.IdCuentaContable,
                                    Detalle = cuentaContable.Descripcion,
                                    Debitos = anticipo.Importe * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion),
                                    Creditos = 0
                                }, tran);
                            }
                        }
                    }

                    if (model.Retenciones?.Count > 0)
                    {
                        foreach (var retencion in model.Retenciones)
                        {
                            cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(retencion.IdCuentaContable);

                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = cuentaContable.IdCuentaContable,
                                Detalle = cuentaContable.Descripcion,
                                Debitos = retencion.Importe * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion),
                                Creditos = 0
                            }, tran);
                        }
                    }

                    cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_CLIENTES, tran);

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = 0,
                        Creditos = (model.Items.Sum(i => i.Importe) + (model.Anticipos?.Sum(a => a.Importe) ?? 0) + (model.Retenciones?.Sum(r => r.Importe) ?? 0)) * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                    }, tran);

                    await _recibosAsientoRepository.Insert(new ReciboAsiento
                    {
                        IdAsiento = (int)idAsiento,
                        IdRecibo = (int)id
                    }, tran);
                }

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

        public async Task Remove(int idRecibo)
        {
            var recibo = await _recibosRepository.GetById<Recibo>(idRecibo);

            if (recibo == null)
            {
                throw new BusinessException("Número de Recibo Inexistente");
            }

            if (_cierreMesRepository.MesCerrado(recibo.IdEjercicio.Value, recibo.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Recibo se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                switch(recibo.IdEstadoRecibo)
                {
                    case (int)EstadoRecibo.Finalizado:
                        await _recibosComprobantesRepository.DeleteByIdRecibo(idRecibo, tran);

                        recibo.IdEstadoRecibo = (int)EstadoRecibo.Ingresado;
                        recibo.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                        await _recibosRepository.Update(recibo, tran);
                        break;

                    case (int)EstadoRecibo.Ingresado:
                        var reciboAsiento = await _recibosAsientoRepository.GetByIdRecibo(idRecibo, tran);
                        var asiento = await _asientosRepository.GetById<Asiento>(reciboAsiento?.IdAsiento ?? 0, tran);

                        //Borro el Asiento del Recibo
                        await _asientosDetalleRepository.DeleteByIdAsiento(reciboAsiento?.IdAsiento ?? 0, tran);
                        await _recibosAsientoRepository.DeleteByIdRecibo(idRecibo, tran);

                        if (asiento != null)
                        {
                            asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                            await _asientosRepository.Update(asiento, tran);
                        }

                        //Borro Detalle Previo y Analizar Detalle
                        var reciboDetalle = await _recibosDetalleRepository.GetAllByIdRecibo(idRecibo, tran);
                        await _recibosDetalleRepository.DeleteByIdRecibo(idRecibo, tran);
                        foreach (var detalle in reciboDetalle)
                        {
                            if (detalle.IdCheque.HasValue)
                            {
                                var cheque = await _chequesRepository.GetById<Cheque>(detalle.IdCheque.Value, tran);
                                cheque.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                                await _chequesRepository.Update(cheque, tran);
                            }
                            if (detalle.IdTransferencia.HasValue)
                            {
                                await _transferenciasRepository.DeleteById(detalle.IdTransferencia.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                            }
                            if (detalle.IdDocumento.HasValue)
                            {
                                await _documentosRepository.DeleteById(detalle.IdDocumento.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                            }
                        }

                        await _recibosAnticiposRepository.DeleteByIdRecibo(idRecibo, tran);
                        await _recibosRetencionesRepository.DeleteByIdRecibo(idRecibo, tran);

                        recibo.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                        await _recibosRepository.Update(recibo, tran);
                        break;
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Update(RecibosViewModel model)
        {
            var recibo = _mapper.Map<Recibo>(model);

            var dbRecibo = await _recibosRepository.GetById<Recibo>(recibo.IdRecibo);

            if (dbRecibo == null)
            {
                throw new BusinessException("Número de Recibo Inexistente");
            }

            if (dbRecibo.IdEstadoRecibo == (int)EstadoRecibo.Finalizado)
            {
                throw new BusinessException("El Recibo ya se encuentra Finalizado. No se puede Editar.");
            }

            model.IdEjercicio = dbRecibo.IdEjercicio.Value;
            Validate(model);

            model.Cotizacion = model.Cotizacion == 0 ? 1 : model.Cotizacion;

            var tran = _uow.BeginTransaction();

            try
            {
                var reciboAsiento = await _recibosAsientoRepository.GetByIdRecibo(dbRecibo.IdRecibo, tran);
                var asiento = await _asientosRepository.GetById<Domain.Entities.Asiento>(reciboAsiento?.IdAsiento ?? 0, tran);

                //Borro el Asiento del Recibo
                await _asientosDetalleRepository.DeleteByIdAsiento(reciboAsiento?.IdAsiento ?? 0, tran);
                await _recibosAsientoRepository.DeleteByIdRecibo(dbRecibo.IdRecibo, tran);

                if (asiento != null)
                {
                    asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _asientosRepository.Update(asiento, tran);
                }

                dbRecibo.Fecha = recibo.Fecha;
                dbRecibo.Descripcion = recibo.Descripcion.ToUpper().Trim();
                dbRecibo.Importe = model.Items?.Sum(i => i.Importe) ?? 0 + (model.Anticipos?.Where(a => a.Importe > 0)?.Sum(a => a.Importe) ?? 0);
                dbRecibo.Total = dbRecibo.Importe + (model.Retenciones?.Sum(r => r.Importe) ?? 0);
                dbRecibo.IdEstadoRecibo = (int)EstadoRecibo.Ingresado;
                dbRecibo.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbRecibo.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                var id = await _recibosRepository.Update(dbRecibo, tran);

                //Borro Detalle Previo y Analizar Detalle
                var reciboDetalle = await _recibosDetalleRepository.GetAllByIdRecibo(dbRecibo.IdRecibo, tran);

                await _recibosDetalleRepository.DeleteByIdRecibo(dbRecibo.IdRecibo, tran);

                foreach (var detalle in reciboDetalle)
                {
                    if (detalle.IdCheque.HasValue)
                    {
                        var cheque = await _chequesRepository.GetById<Cheque>(detalle.IdCheque.Value, tran);
                        cheque.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                        await _chequesRepository.Update(cheque, tran);
                    }
                    if (detalle.IdTransferencia.HasValue)
                    {
                        await _transferenciasRepository.DeleteById(detalle.IdTransferencia.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                    }
                    if (detalle.IdDocumento.HasValue)
                    {
                        await _documentosRepository.DeleteById(detalle.IdDocumento.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                    }
                }

                await _recibosAnticiposRepository.DeleteByIdRecibo(dbRecibo.IdRecibo, tran);
                await _recibosRetencionesRepository.DeleteByIdRecibo(dbRecibo.IdRecibo, tran);

                if (model.Items?.Count > 0)
                {
                    foreach (var item in model.Items)
                    {
                        switch (item.IdTipoCobro)
                        {
                            case (int)TipoCobro.Efectivo:
                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = dbRecibo.IdRecibo,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion.ToUpper().Trim(),
                                    Importe = item.Importe
                                }, tran);
                                break;

                            case (int)TipoCobro.Cheque:
                                var idCheque = await _chequesRepository.Insert(new Cheque
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdTipoCheque = (int)TipoCheque.Terceros,
                                    NroCheque = item.NroCheque,
                                    Banco = item.BancoCheque?.ToUpper().Trim(),
                                    FechaEmision = item.FechaEmision,
                                    FechaVto = item.FechaVto,
                                    Importe = item.Importe,
                                    IdEstadoCheque = (int)EstadoCheque.EnCartera,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                    Firmante = item.FirmanteCheque?.ToUpper().Trim(),
                                    CUITFirmante = item.CUITFirmante,
                                    Endosante1 = item.Endosante1Cheque?.ToUpper().Trim(),
                                    CUITEndosante1 = item.CUITEndosante1,
                                    Endosante2 = item.Endosante2Cheque?.ToUpper().Trim(),
                                    CUITEndosante2 = item.CUITEndosante2,
                                    Endosante3 = item.Endosante3Cheque?.ToUpper().Trim(),
                                    CUITEndosante3 = item.CUITEndosante3
                                }, tran);

                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = dbRecibo.IdRecibo,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion.ToUpper().Trim(),
                                    Importe = item.Importe,
                                    IdCheque = (int)idCheque
                                }, tran);
                                break;

                            case (int)TipoCobro.Transferencia:
                                var idTransferencia = await _transferenciasRepository.Insert(new Transferencia
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    Fecha = item.FechaTransferencia.Value,
                                    NroTransferencia = item.NroTransferencia,
                                    IdBanco = item.IdBancoTranferencia,
                                    BancoOrigen = item.BancoOrigen.ToUpper().Trim(),
                                    Importe = item.Importe
                                }, tran);

                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = dbRecibo.IdRecibo,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion.ToUpper().Trim(),
                                    Importe = item.Importe,
                                    IdTransferencia = (int)idTransferencia
                                }, tran);
                                break;

                            case (int)TipoCobro.Documento:
                                var idDocumento = await _documentosRepository.Insert(new Documento
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    FechaEmision = item.FechaDocumento.Value,
                                    FechaVto = item.FechaVto.Value,
                                    NroDocumento = item.NroDocumento,
                                    Firmante = item.FirmanteDocumento.ToUpper().Trim(),
                                    CUITFirmante = item.CUITFirmanteDocumento,
                                    Importe = item.Importe
                                });

                                await _recibosDetalleRepository.Insert(new ReciboDetalle
                                {
                                    IdRecibo = dbRecibo.IdRecibo,
                                    IdTipoCobro = item.IdTipoCobro,
                                    Descripcion = item.Descripcion.ToUpper().Trim(),
                                    Importe = item.Importe,
                                    IdDocumento = (int)idDocumento
                                }, tran);
                                break;
                        }
                    }
                }

                if (model.Retenciones?.Count > 0)
                {
                    foreach (var retencion in model.Retenciones)
                    {
                        var cuentaRetencion = retencion.IdCategoria switch
                        {
                            (int)CategoriaRetencion.Ganancias => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_GANANCIAS_CLIENTES, tran),
                            (int)CategoriaRetencion.IVA => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_IVA_CLIENTES, tran),
                            (int)CategoriaRetencion.SUSS => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_SUSS_CLIENTES, tran),
                            (int)CategoriaRetencion.IngresosBrutos => await _cuentasContablesRepository.GetById<CuentaContable>(retencion.IdCuentaContable),
                            _ => throw new BusinessException($"Categoria de Retención {retencion.Categoria} Invalida. Verifique.")
                        };

                        if (cuentaRetencion == default)
                            throw new BusinessException($"No Existe Cuenta Contable para la Categoria de Retención {retencion.Categoria}. Verifique.");

                        retencion.IdCuentaContable = cuentaRetencion.IdCuentaContable;

                        await _recibosRetencionesRepository.Insert(new ReciboRetencion
                        {
                            IdRecibo = dbRecibo.IdRecibo,
                            Fecha = retencion.Fecha,
                            IdCategoria = retencion.IdCategoria,
                            Categoria = ((CategoriaRetencion)retencion.IdCategoria).Description(),
                            NroRetencion = retencion.NroRetencion,
                            BaseImponible = (retencion.BaseImponible ?? 0),
                            Importe = retencion.Importe,
                            IdCuentaContable = cuentaRetencion.IdCuentaContable
                        }, tran);
                    }
                }

                if (model.Anticipos?.Count > 0)
                {
                    foreach (var anticipo in model.Anticipos)
                    {
                        if (anticipo.Importe > 0)
                        {
                            await _recibosAnticiposRepository.Insert(new ReciboAnticipo
                            {
                                IdRecibo = dbRecibo.IdRecibo,
                                IdAnticipo = anticipo.IdRecibo,
                                Importe = anticipo.Importe
                            }, tran);
                        }
                    }
                }

                #region Asiento Contable

                if (model.Items.Sum(i => i.Importe) > 0)
                {
                    asiento = new Asiento
                    {
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        IdEjercicio = dbRecibo.IdEjercicio,
                        Descripcion = dbRecibo.Descripcion,
                        Fecha = recibo.Fecha,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        FechaIngreso = DateTime.Now
                    };

                    var idAsiento = await _asientosRepository.Insert(asiento, tran);

                    var indItem = 1;
                    CuentaContable cuentaContable;
                    foreach (var item in model.Items)
                    {
                        switch (item.IdTipoCobro)
                        {
                            case (int)TipoCobro.Efectivo:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                                break;
                            case (int)TipoCobro.Cheque:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.VALORES_A_DEPOSITAR, tran);
                                break;
                            case (int)TipoCobro.Transferencia:
                                var banco = await _bancosRepository.GetById<Banco>(item.IdBancoTranferencia.Value, tran);
                                cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(banco.IdCuentaContable, tran);
                                break;
                            case (int)TipoCobro.Documento:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.DOCUMENTOS_EN_CARTERA, tran);
                                break;

                            default:
                                cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran);
                                break;
                        }

                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable que hace Referencia al Tipo de Cobro. Verifique los Códigos de Observación.");

                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = item.Importe * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion),
                            Creditos = 0
                        }, tran);
                    }

                    if (model.Anticipos?.Count > 0)
                    {
                        cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.ANTICIPOS_CLIENTES, tran);

                        foreach (var anticipo in model.Anticipos)
                        {
                            if (anticipo.Importe > 0)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = cuentaContable.IdCuentaContable,
                                    Detalle = cuentaContable.Descripcion,
                                    Debitos = anticipo.Importe * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion),
                                    Creditos = 0
                                }, tran);
                            }
                        }
                    }

                    if (model.Retenciones?.Count > 0)
                    {
                        foreach (var retencion in model.Retenciones)
                        {
                            cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(retencion.IdCuentaContable);

                            if (cuentaContable == default)
                                throw new BusinessException("No Existe la Cuenta Contable que hace Referencia a las Retenciones. Verifique los Códigos de Observación.");

                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = cuentaContable.IdCuentaContable,
                                Detalle = cuentaContable.Descripcion,
                                Debitos = retencion.Importe * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion),
                                Creditos = 0
                            }, tran);
                        }
                    }

                    cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_CLIENTES, tran);

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = 0,
                        Creditos = (model.Items.Sum(i => i.Importe) + (model.Anticipos?.Sum(a => a.Importe) ?? 0) + (model.Retenciones?.Sum(r => r.Importe) ?? 0)) * (model.IdMonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                    }, tran);

                    await _recibosAsientoRepository.Insert(new ReciboAsiento
                    {
                        IdAsiento = (int)idAsiento,
                        IdRecibo = dbRecibo.IdRecibo
                    }, tran);
                }

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

        private void Validate(RecibosViewModel recibo)
        {
            if (recibo.IdMoneda != recibo.IdMonedaCobro)
            {
                if (recibo.IdMonedaCobro != Monedas.MonedaLocal.Description())
                {
                    throw new BusinessException("Error en la composición de Monedas. Verifique las Monedas de los Comprobantes y del Recibo.");
                }
            }

            if (recibo.Items?.Count > 0 && recibo.Items?.Sum(i => i.Importe) < 0)
            {
                throw new BusinessException("El Importe del Recibo tiene que ser Mayor o Igual a 0.");
            }

            if (_ejerciciosRepository.EjercicioCerrado(recibo.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(recibo.IdEjercicio, recibo.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Recibo no es Válida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(recibo.IdEjercicio, recibo.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha del Recibo esta Cerrado. Verifique.");
            }
        }

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {
            var asiento = await _recibosAsientoRepository.GetByIdRecibo(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
        }

        public async Task<IList<Custom.ReciboComprobante>> GetComprobantesImputar(int idRecibo)
        {
            var recibo = await _recibosRepository.GetById<Recibo>(idRecibo);
            if (recibo == null)
                return new List<Custom.ReciboComprobante>();

            var cotizacion = recibo.Cotizacion;
            if (recibo.Moneda.Equals(recibo.MonedaCobro))
                cotizacion = 1;

            var comprobantes = (await _recibosComprobantesRepository.GetComprobantesImputar(recibo.IdCliente, recibo.Moneda, _permissionsBusiness.Value.User.IdEmpresa))
                .Select(cv => new Custom.ReciboComprobante
                {
                    Seleccionar = cv.Seleccionar,
                    IdComprobanteVenta = cv.IdComprobanteVenta,
                    Fecha = cv.Fecha,
                    TipoComprobante = cv.TipoComprobante,
                    NumeroComprobante = cv.NumeroComprobante,
                    Importe = cv.EsCredito ? cv.Importe * recibo.Cotizacion * -1 : cv.Importe * recibo.Cotizacion,
                    Saldo = cv.EsCredito ? cv.Saldo * recibo.Cotizacion * -1 : cv.Saldo * recibo.Cotizacion,
                    Total = cv.EsCredito ? cv.Total * recibo.Cotizacion * -1 : cv.Total * recibo.Cotizacion,
                    Cotizacion = recibo.MonedaCobro.Equals(Monedas.MonedaLocal.Description()) ? 1 : cv.Cotizacion
                })
                .ToList();

            return comprobantes;
        }

        public async Task AddImputaciones(RecibosViewModel model)
        {
            var dbRecibo = await _recibosRepository.GetById<Recibo>(model.IdRecibo);

            if (dbRecibo == null)
            {
                throw new BusinessException("Número de Recibo Inexistente");
            }

            if (dbRecibo.IdEstadoRecibo == (int)EstadoRecibo.Finalizado)
            {
                throw new BusinessException("El Recibo ya se encuentra Finalizado. No se puede Editar.");
            }

            if (model.Redondeo > 100)
            {
                throw new BusinessException("El Importe de Redondeo no puede superar los 100.");
            }

            if (Math.Round(model.Imputaciones.Sum(i => i.Importe), 2) != Math.Round(dbRecibo.Total, 2) + (model.Redondeo ?? 0))
            {
                throw new BusinessException("La suma de Comprobantes Difiere con el Total del Recibo. Verifique.");
            }

            foreach (var imputacion in model.Imputaciones)
            {
                if(Math.Abs(imputacion.Saldo) < Math.Abs(imputacion.Total))
                {
                    throw new BusinessException("Existen comprobantes Imputados de forma Incorrecta. Verifique.");
                }
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var imputaciones = model.Imputaciones.Where(imp => Math.Abs(imp.Importe) > 0);
                foreach (var imputacion in imputaciones)
                {
                    await _recibosComprobantesRepository.Insert(new ReciboComprobante
                    {
                        IdRecibo = dbRecibo.IdRecibo,
                        IdComprobanteVenta = imputacion.IdComprobanteVenta,
                        Importe = imputacion.Importe,
                        Cotizacion = imputacion.Cotizacion
                    }, tran);
                }

                dbRecibo.IdEstadoRecibo = (int)EstadoRecibo.Finalizado;

                await _recibosRepository.Update(dbRecibo, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task<IList<Custom.ReciboAnticipo>> GetAnticiposImputar(int idCliente, string idMoneda) =>
            await _recibosAnticiposRepository.GetAnticiposImputar(idCliente, idMoneda, _permissionsBusiness.Value.User.IdEmpresa);

    }
}

