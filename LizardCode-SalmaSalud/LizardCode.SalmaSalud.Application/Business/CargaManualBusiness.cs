using CsvHelper;
using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CargaManual;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.Framework.Helpers.Excel;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CargaManualBusiness: BaseBusiness, ICargaManualBusiness
    {
        private readonly ILogger<CargaManualBusiness> _logger;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IComprobantesRepository _comprobantesRepository;
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;
        private readonly IComprobantesComprasItemRepository _comprobantesComprasItemRepository;
        private readonly IComprobantesComprasAsientoRepository _comprobantesComprasAsientoRepository;
        private readonly IComprobantesComprasTotalesRepository _comprobantesComprasTotalesRepository;
        private readonly IComprobantesComprasAFIPRepository _comprobantesComprasAFIPRepository;
        private readonly IComprobantesComprasPercepcionesRepository _comprobantesComprasPercepcionesRepository;
        private readonly IOrdenesPagoComprobantesRepository _ordenesPagoComprobantesRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IProveedoresRepository _proveedoresRepository;
        private readonly IProveedoresEmpresasRepository _proveedoresEmpresasRepository;
        private readonly IMonedasRepository _monedasRepository;

        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public CargaManualBusiness(
            ILogger<CargaManualBusiness> logger,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            IComprobantesRepository comprobantesRepository,
            IComprobantesComprasRepository comprobantesComprasRepository,
            IComprobantesComprasItemRepository comprobantesComprasItemRepository,
            IComprobantesComprasAsientoRepository comprobantesComprasAsientoRepository,
            IComprobantesComprasAFIPRepository comprobantesComprasAFIPRepository,
            IComprobantesComprasPercepcionesRepository comprobantesComprasPercepcionesRepository,
            IOrdenesPagoComprobantesRepository ordenesPagoComprobantesRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IAfipAuthRepository afipAuthRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IProveedoresRepository proveedoresRepository,
            IProveedoresEmpresasRepository proveedoresEmpresasRepository,
            IMonedasRepository monedasRepository,
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository)
        {
            _logger = logger;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _comprobantesRepository = comprobantesRepository;
            _comprobantesComprasRepository = comprobantesComprasRepository;
            _comprobantesComprasItemRepository = comprobantesComprasItemRepository;
            _comprobantesComprasAsientoRepository = comprobantesComprasAsientoRepository;
            _comprobantesComprasTotalesRepository = comprobantesComprasTotalesRepository;
            _comprobantesComprasAFIPRepository = comprobantesComprasAFIPRepository;
            _comprobantesComprasPercepcionesRepository = comprobantesComprasPercepcionesRepository;
            _ordenesPagoComprobantesRepository = ordenesPagoComprobantesRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _afipAuthRepository = afipAuthRepository;
            _proveedoresRepository = proveedoresRepository;
            _proveedoresEmpresasRepository = proveedoresEmpresasRepository;
            _monedasRepository = monedasRepository;
        }

        public async Task<CargaManualViewModel> Get(int idComprobanteCpa)
        {
            var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(idComprobanteCpa);

            if (comprobante == null)
                return null;

            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompra(idComprobanteCpa);

            var model = _mapper.Map<CargaManualViewModel>(comprobante);
            model.Items = _mapper.Map<List<CargaManualDetalle>>(items);

            return model;
        }

        public async Task<Custom.ComprobanteCompraManual> GetCustom(int idComprobanteCpa)
        {
            var comprobante = await _comprobantesComprasRepository.GetManualByIdCustom(idComprobanteCpa);
            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompraManual(idComprobanteCpa);

            comprobante.Items = items;

            return comprobante;
        }

        public async Task<Custom.ComprobanteCompraManual> GetItemsPagosCustom(int idComprobanteCpa)
        {
            var comprobante = await _comprobantesComprasRepository.GetManualByIdCustom(idComprobanteCpa);
            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompraManual(idComprobanteCpa);
            var percepciones = await _comprobantesComprasRepository.GetPercepcionesByIdComprobanteCompra(idComprobanteCpa);
            var pagos = await _ordenesPagoComprobantesRepository.GetOrdenPagoByIdComprobanteCompra(idComprobanteCpa);

            comprobante.Items = items;
            comprobante.Pagos = pagos;
            comprobante.ListaPercepciones = percepciones;

            return comprobante;
        }

        public async Task<DataTablesResponse<Custom.ComprobanteCompra>> GetAll(DataTablesRequest request)
        {
            var customQuery = _comprobantesComprasRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdProveedor"))
                builder.Append($"AND IdProveedor = {filters["IdProveedor"]}");

            if (filters.ContainsKey("IdCentroCosto"))
                builder.Append($"AND IdCentroCosto = {filters["IdCentroCosto"]}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");
            builder.Append($"AND IdTipoComprobante IN ({(int)TipoComprobante.ManualProveedores}, {(int)TipoComprobante.GastosProveedores}, {(int)TipoComprobante.Interfaz})");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.ComprobanteCompra>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(CargaManualViewModel model)
        {

            var comprobante = _mapper.Map<ComprobanteCompra>(model);

            Validate(model);

            var alicuotas = (await _lookupsBusiness.Value.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA)
                .ToDictionary(a => a.IdAlicuota, a => a.Valor);

            var tran = _uow.BeginTransaction();

            try
            {
                var tipoComprobante = await _comprobantesRepository.GetById<Comprobante>(comprobante.IdComprobante, tran);

                comprobante.Sucursal = model.NumeroComprobante.Substring(0, 5);
                comprobante.Numero = model.NumeroComprobante.Substring(6, 8);
                comprobante.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                comprobante.IdUsuario = _permissionsBusiness.Value.User.Id;
                comprobante.FechaIngreso = DateTime.Now;
                comprobante.Percepciones = model.ListaPercepciones?.Sum(i => i.Importe) ?? 0;
                comprobante.Subtotal = model.Items.Sum(i => i.Importe);
                comprobante.Total = model.Items.Sum(i => i.Importe * (1 + (alicuotas.GetValueOrDefault(i.IdAlicuota) / 100))) + comprobante.Percepciones;
                comprobante.IdTipoComprobante = (int)TipoComprobante.ManualProveedores;
                comprobante.CAE = model.CAE;
                comprobante.VencimientoCAE = model.VenciminetoCAE;
                comprobante.IdEstadoAFIP = (int)EstadoAFIP.Inicial;
                comprobante.IdCentroCosto = model.IdCentroCosto;
                comprobante.FechaVto = model.Vto;

                var id = await _comprobantesComprasRepository.Insert(comprobante, tran);
                var iItem = 1;

                for (int iLoop = 0; iLoop < model.Items.Count; iLoop++)
                {
                    var item = model.Items[iLoop];
                    var alicuota = alicuotas.GetValueOrDefault(item.IdAlicuota);

                    await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                    {
                        IdComprobanteCompra = (int)id,
                        Item = iItem++,
                        Impuestos = item.Importe * (alicuota / 100),
                        Descripcion = item.Descripcion.ToUpper().Trim(),
                        IdCuentaContable = item.IdCuentaContable,
                        Alicuota = alicuota,
                        Bonificacion = 0,
                        Cantidad = 0,
                        Importe = item.Importe,
                        Precio = 0
                    }, tran);
                }

                var totales = model.Items.GroupBy(t => t.Alicuota)
                    .Select(g => new
                    {
                        g.FirstOrDefault().IdAlicuota,
                        Neto = g.Sum(t => t.Importe),
                        ImporteAlicuota = g.Sum(t => t.Importe * (alicuotas.GetValueOrDefault(t.IdAlicuota) / 100))
                    });

                iItem = 1;
                foreach (var total in totales)
                {
                    await _comprobantesComprasTotalesRepository.Insert(
                        new ComprobanteCompraTotales
                        {
                            IdComprobanteCompra = (int)id,
                            Item = iItem++,
                            ImporteAlicuota = total.ImporteAlicuota,
                            Neto = total.Neto,
                            Alicuota = alicuotas.GetValueOrDefault(total.IdAlicuota)
                        }, tran);
                }

                if (model.ListaPercepciones != default)
                {
                    foreach (var percepcion in model.ListaPercepciones)
                    {
                        await _comprobantesComprasPercepcionesRepository.Insert(
                            new ComprobanteCompraPercepcion
                            {
                                IdComprobanteCompra = (int)id,
                                IdCuentaContable = percepcion.IdCuentaContable,
                                Importe = percepcion.Importe
                            }, tran);
                    }
                }

                #region Asiento Contable

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = comprobante.IdEjercicio,
                    Descripcion = string.Concat(string.Concat("COMPRA ", tipoComprobante.Descripcion), comprobante.Sucursal, "-", comprobante.Numero),
                    Fecha = comprobante.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);
                var indItem = 1;
                var ctaCompraServicios = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES, tran);
                if (ctaCompraServicios != null)
                {
                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = ctaCompraServicios.IdCuentaContable,
                        Detalle = ctaCompraServicios.Descripcion,
                        Debitos = tipoComprobante.EsCredito ? model.Items.Sum(dc => dc.Importe * (1 + (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion) + comprobante.Percepciones : 0,
                        Creditos = tipoComprobante.EsCredito ? 0 : model.Items.Sum(dc => dc.Importe * (1 + (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion) + comprobante.Percepciones
                    }, tran);
                }
                else
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Proveedores. Verifique.");

                var impuesto = model.Items.Sum(dc => dc.Importe * (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion;
                if (impuesto > 0)
                {
                    var ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA, tran);
                    if (ctaCreditoFiscal != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaCreditoFiscal.IdCuentaContable,
                            Detalle = ctaCreditoFiscal.Descripcion,
                            Debitos = tipoComprobante.EsCredito ? 0 : impuesto,
                            Creditos = tipoComprobante.EsCredito ? impuesto : 0
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");
                }

                if (model.ListaPercepciones != default)
                {
                    foreach (var percepcion in model.ListaPercepciones)
                    {
                        var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(percepcion.IdCuentaContable, tran);

                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = tipoComprobante.EsCredito ? 0 : percepcion.Importe * model.Cotizacion,
                            Creditos = tipoComprobante.EsCredito ? percepcion.Importe * model.Cotizacion : 0
                        }, tran);
                    }
                }

                for (int iLoop = 0; iLoop < model.Items.Count; iLoop++)
                {
                    var item = model.Items[iLoop];

                    var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(item.IdCuentaContable, tran);

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = tipoComprobante.EsCredito ? 0 : item.Importe * model.Cotizacion,
                        Creditos = tipoComprobante.EsCredito ? item.Importe * model.Cotizacion : 0
                    }, tran);
                }

                await _comprobantesComprasAsientoRepository.Insert(new ComprobanteCompraAsiento { IdComprobanteCompra = (int)id, IdAsiento = (int)idAsiento }, tran);

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

        public async Task Remove(int idComprobanteCpa)
        {
            var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(idComprobanteCpa);

            if (comprobante == default)
            {
                throw new BusinessException("Comprobante de Compra inexistente");
            }

            if(comprobante.IdTipoComprobante != (int)TipoComprobante.ManualProveedores && comprobante.IdTipoComprobante != (int)TipoComprobante.Interfaz)
            {
                throw new BusinessException("El Tipo de Comprobante es Automático o de Gastos. No se puede Eliminar.");
            }

            if (_cierreMesRepository.MesCerrado(comprobante.IdEjercicio.Value, comprobante.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Comprobante se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var pago = await _ordenesPagoComprobantesRepository.GetByIdComprobanteCompra(idComprobanteCpa);
            if(pago != default)
            {
                throw new BusinessException($"El Comprobante se encuentra Imputado en la Orden de Pago Número {pago.IdOrdenPago}. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var comprobanteAsiento = await _comprobantesComprasAsientoRepository.GetByIdComprobanteCompra(idComprobanteCpa, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(comprobanteAsiento.IdAsiento, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(comprobanteAsiento.IdAsiento, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                await _comprobantesComprasAsientoRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);
                await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);
                await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);
                await _comprobantesComprasPercepcionesRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);

                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _comprobantesComprasRepository.Update(comprobante, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }


        public async Task Update(CargaManualViewModel model)
        {
            Validate(model);

            var dbComprobante = await _comprobantesRepository.GetById<ComprobanteCompra>(model.IdComprobanteCompra);

            if (dbComprobante == null)
            {
                throw new BusinessException("Comprobante de Compra inexistente");
            }

            var alicuotas = (await _lookupsBusiness.Value.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA)
                .ToDictionary(a => a.IdAlicuota, a => a.Valor);

            model.Cotizacion = model.Cotizacion == 0 ? 1 : model.Cotizacion;

            var tran = _uow.BeginTransaction();

            try
            {
                var tipoComprobante = await _comprobantesRepository.GetById<Comprobante>(dbComprobante.IdComprobante, tran);

                dbComprobante.Sucursal = model.NumeroComprobante.Substring(0, 5);
                dbComprobante.Numero = model.NumeroComprobante.Substring(6, 8);
                dbComprobante.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbComprobante.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                dbComprobante.IdUsuario = _permissionsBusiness.Value.User.Id;
                dbComprobante.FechaReal = model.FechaReal;
                dbComprobante.FechaIngreso = DateTime.Now;
                dbComprobante.Percepciones = model.ListaPercepciones?.Sum(i => i.Importe) ?? 0;
                dbComprobante.Subtotal = model.Items.Sum(i => i.Importe);
                dbComprobante.Total = model.Items.Sum(i => i.Importe * (1 + (alicuotas.GetValueOrDefault(i.IdAlicuota) / 100))) + dbComprobante.Percepciones;
                dbComprobante.CAE = model.CAE;
                dbComprobante.VencimientoCAE = model.VenciminetoCAE;
                dbComprobante.IdEstadoAFIP = (int)EstadoAFIP.Inicial;
                dbComprobante.IdCentroCosto = model.IdCentroCosto;
                dbComprobante.IdCondicion = model.IdCondicion;
                dbComprobante.FechaVto = model.Vto;

                await _comprobantesComprasRepository.Update(dbComprobante, tran);

                await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(dbComprobante.IdComprobanteCompra, tran);

                var iItem = 1;

                for (int iLoop = 0; iLoop < model.Items.Count; iLoop++)
                {
                    var item = model.Items[iLoop];
                    var alicuota = alicuotas.GetValueOrDefault(item.IdAlicuota);

                    await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                    {
                        IdComprobanteCompra = dbComprobante.IdComprobanteCompra,
                        Item = iItem++,
                        IdCuentaContable = item.IdCuentaContable,
                        Impuestos = item.Importe * (alicuota / 100),
                        Descripcion = item.Descripcion.ToUpper().Trim(),
                        Alicuota = alicuota,
                        Bonificacion = 0,
                        Cantidad = 0,
                        Importe = item.Importe,
                        Precio = 0
                    }, tran);
                }

                await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(dbComprobante.IdComprobanteCompra, tran);

                var totales = model.Items.GroupBy(t => t.Alicuota)
                    .Select(g => new
                    {
                        g.FirstOrDefault().IdAlicuota,
                        Neto = g.Sum(t => t.Importe),
                        ImporteAlicuota = g.Sum(t => t.Importe * (alicuotas.GetValueOrDefault(t.IdAlicuota) / 100))
                    });

                iItem = 1;
                foreach (var total in totales)
                {
                    await _comprobantesComprasTotalesRepository.Insert(
                        new ComprobanteCompraTotales
                        {
                            IdComprobanteCompra = dbComprobante.IdComprobanteCompra,
                            Item = iItem++,
                            ImporteAlicuota = total.ImporteAlicuota,
                            Neto = total.Neto,
                            Alicuota = alicuotas.GetValueOrDefault(total.IdAlicuota)
                        }, tran);
                }

                await _comprobantesComprasPercepcionesRepository.DeleteByIdComprobanteCompra(dbComprobante.IdComprobanteCompra, tran);

                if (model.ListaPercepciones != default)
                {
                    foreach (var percepcion in model.ListaPercepciones)
                    {
                        await _comprobantesComprasPercepcionesRepository.Insert(
                            new ComprobanteCompraPercepcion
                            {
                                IdComprobanteCompra = dbComprobante.IdComprobanteCompra,
                                IdCuentaContable = percepcion.IdCuentaContable,
                                Importe = percepcion.Importe
                            }, tran);
                    }
                }

                #region Asiento Contable

                var comprobanteAsiento = await _comprobantesComprasAsientoRepository.GetByIdComprobanteCompra(dbComprobante.IdComprobanteCompra, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(comprobanteAsiento.IdAsiento, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(comprobanteAsiento.IdAsiento, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                await _comprobantesComprasAsientoRepository.DeleteByIdComprobanteCompra(dbComprobante.IdComprobanteCompra, tran);

                asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = dbComprobante.IdEjercicio,
                    Descripcion = string.Concat(string.Concat("COMPRA ", tipoComprobante.Descripcion), dbComprobante.Sucursal, "-", dbComprobante.Numero),
                    Fecha = dbComprobante.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);
                var indItem = 1;
                var ctaCompraServicios = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES, tran);
                if (ctaCompraServicios != null)
                {
                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = ctaCompraServicios.IdCuentaContable,
                        Detalle = ctaCompraServicios.Descripcion,
                        Debitos = tipoComprobante.EsCredito ? model.Items.Sum(dc => dc.Importe * (1 + (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion) + dbComprobante.Percepciones : 0,
                        Creditos = tipoComprobante.EsCredito ? 0 : model.Items.Sum(dc => dc.Importe * (1 + (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion) + dbComprobante.Percepciones
                    }, tran);
                }
                else
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Proveedores. Verifique.");

                var impuesto = model.Items.Sum(dc => dc.Importe * (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion;
                if (impuesto > 0)
                {
                    var ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA, tran);
                    if (ctaCreditoFiscal != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaCreditoFiscal.IdCuentaContable,
                            Detalle = ctaCreditoFiscal.Descripcion,
                            Debitos = tipoComprobante.EsCredito ? 0 : impuesto,
                            Creditos = tipoComprobante.EsCredito ? impuesto : 0
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");
                }

                if (model.ListaPercepciones != default)
                {
                    foreach (var percepcion in model.ListaPercepciones)
                    {
                        var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(percepcion.IdCuentaContable, tran);

                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = tipoComprobante.EsCredito ? 0 : percepcion.Importe * model.Cotizacion,
                            Creditos = tipoComprobante.EsCredito ? percepcion.Importe * model.Cotizacion : 0
                        }, tran);
                    }
                }

                for (int iLoop = 0; iLoop < model.Items.Count; iLoop++)
                {
                    var item = model.Items[iLoop];

                    var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(item.IdCuentaContable, tran);

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = tipoComprobante.EsCredito ? 0 : item.Importe * model.Cotizacion,
                        Creditos = tipoComprobante.EsCredito ? item.Importe * model.Cotizacion : 0
                    }, tran);
                }

                await _comprobantesComprasAsientoRepository.Insert(new ComprobanteCompraAsiento { IdComprobanteCompra = dbComprobante.IdComprobanteCompra, IdAsiento = (int)idAsiento }, tran);

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

        private void Validate(CargaManualViewModel comprobanteCpa)
        {
            if(comprobanteCpa.NumeroComprobante.Length != 14)
            {
                throw new BusinessException("El Número del Comprobante es Incorrecto");
            }

            var compCompra = _comprobantesComprasRepository.GetComprobanteByProveedor(comprobanteCpa.IdComprobante, comprobanteCpa.NumeroComprobante, comprobanteCpa.IdProveedor).GetAwaiter().GetResult();
            if (compCompra != default)
            {
                throw new BusinessException("Existe un Comprobante con el mismo Número para el Proveedor Seleccionado. Verifique.");
            }

            if (comprobanteCpa.CAE.IsNull())
            {
                if(comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_A && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_B && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_C)
                    throw new BusinessException("Ingrese el C.A.E para el Comprobante a Ingresar.");
            }

            if (!comprobanteCpa.VenciminetoCAE.HasValue)
            {
                if (comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_A && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_B && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_C)
                    throw new BusinessException("Ingrese el Vencimiento del C.A.E para el Comprobante a Ingresar.");
            }

            if (_ejerciciosRepository.EjercicioCerrado(comprobanteCpa.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(comprobanteCpa.IdEjercicio, comprobanteCpa.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Comprobante no es Valida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(comprobanteCpa.IdEjercicio, comprobanteCpa.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha del Comprobante esta Cerrado. Verifique.");
            }

            if (comprobanteCpa.NumeroComprobante.Length != 14)
            {
                throw new BusinessException("El Número del Comprobante es Incorrecto");
            }

            if (comprobanteCpa.Items?.Count == 0)
            {
                throw new BusinessException("Seleccione algún Item para el Comprobante");
            }

        }

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {

            var asiento = await _comprobantesComprasAsientoRepository.GetByIdComprobanteCompra(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
            
        }

        public async Task<Custom.DatosComprobanteCompraAFIP> ValidateComprobante(int idComprobanteCpa)
        {
            try
            {
                var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                var afipAuth = await _afipAuthRepository.GetValidSignToken(_permissionsBusiness.Value.User.IdEmpresa, ServicioAFIP.WEB_SERVICE_CONSULTA_COMPROBANTES.Description());

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

                        afipAuth = await _afipAuthRepository.NewSignToken(_permissionsBusiness.Value.User.IdEmpresa, crt.CRT, empresa.PrivateKey, empresa.CUIT.Replace("-", string.Empty), ServicioAFIP.WEB_SERVICE_CONSULTA_COMPROBANTES.Description(), empresa.EnableProdAFIP);

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
                var afip = await _empresasRepository.ValidComprobanteCompras(afipAuth, empresa.CUIT.Replace("-", string.Empty), idComprobanteCpa, empresa.EnableProdAFIP);

                var comprobanteCompra = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(idComprobanteCpa);

                if (afip.Resultado == "A")
                {    
                    comprobanteCompra.IdEstadoAFIP = (int)EstadoAFIP.Autorizado;
                }
                else
                {
                    if(!string.IsNullOrEmpty(afip.Error))
                        comprobanteCompra.IdEstadoAFIP = (int)EstadoAFIP.Error;
                    if (!string.IsNullOrEmpty(afip.Observacion))
                        comprobanteCompra.IdEstadoAFIP = (int)EstadoAFIP.Observado;
                }

                await _comprobantesComprasRepository.Update(comprobanteCompra);

                await _comprobantesComprasAFIPRepository.Insert(new ComprobanteCompraAFIP
                {
                    IdComprobanteCompra = idComprobanteCpa,
                    Estado = afip.Resultado,
                    Request = afip.XMLRequest,
                    Response = afip.XMLResponse,
                    FechaRequest = DateTime.Now
                });

                return afip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new BusinessException(ex.Message);
            }
        }

        public async Task<List<string>> ProcesarCSV(IFormFile file, DateTime fechaInterfaz, int idEjercicioInterfaz, int idCuentaCotable)
        {
            var memoryStream = new MemoryStream();

            try
            {
                var ctaCompraServicios = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES);
                if(ctaCompraServicios == default)
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Proveedores. Verifique.");

                var ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA);
                if (ctaCreditoFiscal == default)
                    throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");

                var ctaGasto = await _cuentasContablesRepository.GetById<CuentaContable>(idCuentaCotable);
                if (ctaGasto == default)
                    throw new BusinessException("No se ecnuentra la Cuenta Contable de Gastos Seleccionada. Verifique.");

                //Verifico Ejercicio segun Fecha
                var ejercicio = await _ejerciciosRepository.GetCurrentByFechaIdEmpresa(fechaInterfaz, _permissionsBusiness.Value.User.IdEmpresa);
                if (ejercicio == default)
                {
                    throw new BusinessException($"La Fecha Contable para los Comprobantes no pertenece a ningún Ejercicio Seleccionado. Verifique.");
                }

                if (ejercicio?.Cerrado == Commons.Si.Description())
                {
                    throw new BusinessException($"La Fecha Contable Para los Comprobantes pertenece a un Ejercicio Cerrado. Verifique.");
                }

                List<string> resultados = new();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var reader = new StreamReader(memoryStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var comprobantes = csv.GetRecords<CargaManualCSV>();

                    foreach(var (comp, iLoop) in comprobantes.Select((value, iLoop) => (value, iLoop)))
                    {
                        var tran = _uow.BeginTransaction();

                        try
                        {
                            #region Validaciones Negocio

                            var modena = (await _monedasRepository.GetAll<Moneda>()).FirstOrDefault(m => m.Simbolo == comp.Moneda);
                            if(modena == default)
                            {
                                throw new BusinessException($"No Existe Modena Configurada en el Sistema par ael Comprobante {comp.NumeroDesde.ToString().PadLeft(8, '0')} en la Linea {iLoop}. Verifique.");
                            }

                            #endregion

                            #region Proveedor

                            long idProveedor;
                            Comprobante tipo = (await _comprobantesRepository.GetAll<Comprobante>(tran)).AsList().Where(c => c.Codigo == int.Parse(comp.Tipo.Split("-").First().Trim())).FirstOrDefault();

                            var proveedor = await _proveedoresRepository.GetByCUIT(Convert.ToInt64(comp.NroDocEmisor).ToString("##-########-#"), _permissionsBusiness.Value.User.IdEmpresa, tran);
                            if (proveedor == default)
                            {
                                var idTipoIVA = await _lookupsBusiness.Value.GetTipoIVAByComprobante(tipo.IdComprobante);
                                idProveedor = await _proveedoresRepository.Insert(new Proveedor
                                {
                                    RazonSocial = comp.DenominacionEmisor.ToUpper().Trim(),
                                    NombreFantasia = comp.DenominacionEmisor.ToUpper().Trim(),
                                    CUIT = Convert.ToInt64(comp.NroDocEmisor).ToString("##-########-#").ToUpper().Trim(),
                                    IdTipoTelefono = (int)TipoTelefono.Otro,
                                    Telefono = String.Empty,
                                    IdTipoIVA = idTipoIVA,
                                    NroIBr = string.Empty,
                                    Email = string.Empty,
                                    Direccion = string.Empty,
                                    CodigoPostal = string.Empty,
                                    Piso = string.Empty,
                                    Departamento = string.Empty,
                                    Localidad = string.Empty,
                                    Provincia = string.Empty,
                                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo
                                }, tran);

                                await _proveedoresEmpresasRepository.Insert<ProveedorEmpresa>(new ProveedorEmpresa
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdProveedor = (int)idProveedor
                                }, tran);

                                proveedor = await _proveedoresRepository.GetById<Proveedor>((int)idProveedor);
                            }
                            else
                            {
                                idProveedor = proveedor.IdProveedor;

                                var proveedorEmpresa = await _proveedoresEmpresasRepository.GetByIdProveedorAndEmpresa((int)idProveedor, _permissionsBusiness.Value.User.IdEmpresa, tran);
                                if(proveedorEmpresa == default)
                                {
                                    await _proveedoresEmpresasRepository.Insert<ProveedorEmpresa>(new ProveedorEmpresa
                                    {
                                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                        IdProveedor = (int)idProveedor
                                    }, tran);
                                }
                            }

                            #endregion

                            #region Comprobante de Compras

                            var numeroCompCompleto = string.Concat(comp.PuntoVenta.ToString().PadLeft(5, '0'), "-", comp.NumeroDesde.ToString().PadLeft(8, '0'));
                            var comprobanteCpas = await _comprobantesComprasRepository.GetComprobanteByProveedor(tipo.IdComprobante, numeroCompCompleto, (int)idProveedor, tran);
                            if (comprobanteCpas != default)
                                throw new BusinessException($"El Comprobante {comp.NumeroDesde.ToString().PadLeft(8, '0')} en la Linea {iLoop} para el Proveedor {proveedor.RazonSocial} ya se Encuentra Cargado en el Sistema. Verifique.");

                            var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                            {
                                IdComprobante = tipo.IdComprobante,
                                Sucursal = comp.PuntoVenta.PadLeft(5, '0'),
                                Numero = comp.NumeroDesde.ToString().PadLeft(8, '0'),
                                IdEjercicio = idEjercicioInterfaz,
                                IdProveedor = (int)idProveedor,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                Fecha = fechaInterfaz,
                                FechaReal = comp.Fecha,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                IdUsuario = _permissionsBusiness.Value.User.Id,
                                FechaIngreso = DateTime.Now,
                                Subtotal = comp.NetoNoGravado + comp.NetoGravado + comp.ImpOpExentas,
                                Total = comp.Total,
                                Percepciones = 0,
                                Moneda = modena.Codigo,
                                Cotizacion = comp.TipoCambio,
                                IdTipoComprobante = (int)TipoComprobante.Interfaz,
                                CAE = comp.CAE,
                                VencimientoCAE = comp.Fecha.AddDays(10),
                                IdEstadoAFIP = (int)EstadoAFIP.Inicial
                            }, tran);

                            int iTotal = 1;
                            if (comp.NetoGravado != 0)
                            {
                                await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    Item = 1,
                                    IdCuentaContable = idCuentaCotable,
                                    Descripcion = string.Concat("GASTOS ", proveedor.RazonSocial),
                                    Cantidad = 0,
                                    Bonificacion = 0,
                                    Precio = 0,
                                    Importe = comp.NetoGravado,
                                    Alicuota = Math.Round((comp.IVA * 100) / comp.NetoGravado, 1),
                                    Impuestos = comp.IVA
                                }, tran);

                                await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    Item = iTotal++,
                                    Neto = comp.NetoGravado,
                                    Alicuota = Math.Round((comp.IVA * 100) / comp.NetoGravado, 1),
                                    ImporteAlicuota = comp.IVA
                                }, tran);
                            }

                            if (comp.NetoNoGravado != 0)
                            {
                                await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    Item = 2,
                                    IdCuentaContable = idCuentaCotable,
                                    Descripcion = string.Concat("GASTOS ", proveedor.RazonSocial),
                                    Cantidad = 0,
                                    Bonificacion = 0,
                                    Precio = 0,
                                    Importe = comp.NetoNoGravado,
                                    Alicuota = 0,
                                    Impuestos = 0
                                }, tran);

                                await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    Item = iTotal++,
                                    Neto = comp.NetoNoGravado,
                                    Alicuota = 0,
                                    ImporteAlicuota = 0
                                }, tran);
                            }

                            if(comp.NetoGravado == 0 && comp.NetoNoGravado == 0)
                            {
                                await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = idCuentaCotable,
                                    Item = 2,
                                    Descripcion = string.Concat("GASTOS ", proveedor.RazonSocial),
                                    Cantidad = 0,
                                    Bonificacion = 0,
                                    Precio = 0,
                                    Importe = comp.Total,
                                    Alicuota = 0,
                                    Impuestos = 0
                                }, tran);

                                await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    Item = iTotal++,
                                    Neto = comp.Total,
                                    Alicuota = 0,
                                    ImporteAlicuota = 0
                                }, tran);
                            }

                            #endregion

                            #region Asiento Contable

                            var asiento = new Asiento
                            {
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                IdEjercicio = ejercicio.IdEjercicio,
                                Descripcion = string.Concat(string.Concat("COMPRA ", tipo.Descripcion), comp.PuntoVenta.PadLeft(5, '0'), "-", comp.PuntoVenta.PadLeft(8, '0')),
                                Fecha = comp.Fecha,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                IdUsuario = _permissionsBusiness.Value.User.Id,
                                FechaIngreso = DateTime.Now
                            };

                            var idAsiento = await _asientosRepository.Insert(asiento, tran);
                            var indItem = 1;
                            ctaCompraServicios = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES, tran);
                            if (ctaCompraServicios != null)
                            {
                                var importeCtaServicios = (comp.NetoGravado + comp.NetoNoGravado + comp.ImpOpExentas) * comp.TipoCambio;
                                if (importeCtaServicios == 0)
                                    importeCtaServicios = comp.Total * comp.TipoCambio;

                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = ctaCompraServicios.IdCuentaContable,
                                    Detalle = ctaCompraServicios.Descripcion,
                                    Debitos = tipo.EsCredito ? importeCtaServicios : 0,
                                    Creditos = tipo.EsCredito ? 0 : importeCtaServicios
                                }, tran);
                            }
                            else
                                throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Proveedores. Verifique.");

                            if (comp.IVA > 0)
                            {
                                ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA, tran);
                                if (ctaCreditoFiscal != null)
                                {
                                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                                    {
                                        IdAsiento = (int)idAsiento,
                                        Item = indItem++,
                                        IdCuentaContable = ctaCreditoFiscal.IdCuentaContable,
                                        Detalle = ctaCreditoFiscal.Descripcion,
                                        Debitos = tipo.EsCredito ? 0 : comp.IVA * comp.TipoCambio,
                                        Creditos = tipo.EsCredito ? comp.IVA * comp.TipoCambio : 0
                                    }, tran);
                                }
                                else
                                    throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");
                            }

                            var cuentaContable = (await _cuentasContablesRepository.GetById<CuentaContable>(idCuentaCotable, tran));

                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = cuentaContable.IdCuentaContable,
                                Detalle = cuentaContable.Descripcion,
                                Debitos = tipo.EsCredito ? 0 : comp.Total * comp.TipoCambio,
                                Creditos = tipo.EsCredito ? comp.Total * comp.TipoCambio : 0
                            }, tran);

                            await _comprobantesComprasAsientoRepository.Insert(new ComprobanteCompraAsiento { IdComprobanteCompra = (int)idComprobanteCompra, IdAsiento = (int)idAsiento }, tran);

                            #endregion

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _logger.LogError(ex, null);
                            resultados.Add(ex.Message);
                        }
                    }
                }

                return resultados;
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, null);
                throw ex;
            }
            catch(CsvHelperException ex)
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

        public async Task<List<string>> ProcesarCustomDawa(IFormFile file, DateTime fechaInterfaz, int idEjercicioInterfaz)
        {
            var memoryStream = new MemoryStream();

            try
            {
                var ctaCompraServicios = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES);
                if (ctaCompraServicios == default)
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Proveedores. Verifique.");

                var ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA);
                if (ctaCreditoFiscal == default)
                    throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");


                List<string> resultados = new();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<CargaManualXLS>();

                foreach (var (comp, iLoop) in excelRows.Select((value, iLoop) => (value, iLoop)))
                {
                    var tran = _uow.BeginTransaction();

                    try
                    {
                        #region Validacion de Negocio

                        //Verifico Ejercicio segun Fecha
                        var ejercicio = await _ejerciciosRepository.GetCurrentByFechaIdEmpresa(comp.Fecha, _permissionsBusiness.Value.User.IdEmpresa, tran);
                        if (ejercicio == default)
                        {
                            throw new BusinessException($"La Fecha Contable para los Comprobantes no pertenece a ningún Ejercicio Seleccionado. Verifique.");
                        }

                        if (ejercicio?.Cerrado == Commons.Si.Description())
                        {
                            throw new BusinessException($"La Fecha Contable Para los Comprobantes pertenece a un Ejercicio Cerrado. Verifique.");
                        }

                        #endregion

                        #region Proveedor

                        long idProveedor;
                        Comprobante tipo = (await _comprobantesRepository.GetAll<Comprobante>(tran)).AsList().Where(c => c.Descripcion.Equals(comp.Comprobante)).FirstOrDefault();

                        if (tipo == default)
                            throw new BusinessException($"El Tipo de Comprobante {comp.Comprobante} en la Linea {iLoop} no se encuentra cargado en el Sistema. Verifique.");

                        if (comp.CUIT.IsNull())
                            throw new BusinessException($"El CUIT en la Linea {iLoop} no se encuentra cargado en el Excel. Verifique.");

                        var proveedor = await _proveedoresRepository.GetByCUIT(comp.CUIT.ToUpper().Trim(), _permissionsBusiness.Value.User.IdEmpresa, tran);
                        if (proveedor == default)
                        {
                            var idTipoIVA = await _lookupsBusiness.Value.GetTipoIVAByComprobante(tipo.IdComprobante);
                            idProveedor = await _proveedoresRepository.Insert(new Proveedor
                            {
                                RazonSocial = comp.Proveedor?.ToUpper().Trim() ?? string.Empty,
                                NombreFantasia = comp.Proveedor?.ToUpper().Trim() ?? string.Empty,
                                CUIT = comp.CUIT.ToUpper().Trim(),
                                IdTipoTelefono = (int)TipoTelefono.Otro,
                                Telefono = string.Empty,
                                IdTipoIVA = idTipoIVA,
                                NroIBr = comp.CUIT.ToUpper().Replace("-", string.Empty).Trim(),
                                Email = string.Empty,
                                Direccion = string.Empty,
                                CodigoPostal = string.Empty,
                                Piso = string.Empty,
                                Departamento = string.Empty,
                                Localidad = string.Empty,
                                Provincia = string.Empty,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
                            }, tran);

                            await _proveedoresEmpresasRepository.Insert<ProveedorEmpresa>(new ProveedorEmpresa
                            {
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                IdProveedor = (int)idProveedor
                            }, tran);

                            proveedor = await _proveedoresRepository.GetById<Proveedor>((int)idProveedor, tran);
                        }
                        else
                        {
                            idProveedor = proveedor.IdProveedor;

                            var proveedorEmpresa = await _proveedoresEmpresasRepository.GetByIdProveedorAndEmpresa((int)idProveedor, _permissionsBusiness.Value.User.IdEmpresa, tran);
                            if (proveedorEmpresa == default)
                            {
                                await _proveedoresEmpresasRepository.Insert(new ProveedorEmpresa
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdProveedor = (int)idProveedor
                                }, tran);
                            }
                        }

                        #endregion

                        #region Comprobante de Compras

                        if (!comp.Numero.Contains('-'))
                            throw new BusinessException($"El Comprobante {comp.Numero} en la Linea {iLoop} es Incorrecto. Verifique.");

                        var numeroCompCompleto = string.Concat(comp.Numero.Split("-").First().Trim().PadLeft(5, '0'), "-", comp.Numero.Split("-").Last().Trim().PadLeft(8, '0'));
                        var comprobanteCpas = await _comprobantesComprasRepository.GetComprobanteByProveedor(tipo.IdComprobante, numeroCompCompleto, (int)idProveedor, tran);
                        if (comprobanteCpas != default)
                            throw new BusinessException($"El Comprobante {comp.Comprobante.ToString().PadLeft(8, '0')} en la Linea {iLoop} para el Proveedor {proveedor.RazonSocial} ya se Encuentra Cargado en el Sistema. Verifique.");

                        var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                        {
                            IdComprobante = tipo.IdComprobante,
                            Sucursal = numeroCompCompleto.Split("-").First(),
                            Numero = numeroCompCompleto.Split("-").Last(),
                            IdEjercicio = idEjercicioInterfaz,
                            IdProveedor = (int)idProveedor,
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            Fecha = comp.Fecha,
                            FechaReal = comp.Fecha,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            IdUsuario = _permissionsBusiness.Value.User.Id,
                            FechaIngreso = DateTime.Now,
                            Subtotal = comp.Neto + comp.Neto2 + comp.NoGravado,
                            Total = comp.Total,
                            Percepciones = comp.Percepcion + comp.Percepcion2,
                            Moneda = Monedas.MonedaLocal.Description(),
                            Cotizacion = 1,
                            IdTipoComprobante = (int)TipoComprobante.Interfaz,
                            CAE = comp.CAE,
                            VencimientoCAE = comp.VencimientoCAE == DateTime.MinValue ? null : comp.VencimientoCAE,
                            IdEstadoAFIP = string.IsNullOrEmpty(comp.CAE) ? (int)EstadoAFIP.DocumentoSinCAE : (int)EstadoAFIP.Inicial
                        }, tran);

                        int iTotal = 1;
                        int iItem = 1;
                        if (comp.NoGravado != 0)
                        {
                            var ctaNoGravado = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta_NoGravado, tran);
                            if (ctaNoGravado == default)
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable para el Importe No Gravado en la Linea {iLoop}. Verifique");

                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = iItem++,
                                IdCuentaContable = ctaNoGravado.IdCuentaContable,
                                Descripcion = ctaNoGravado.Descripcion,
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = Math.Round(comp.NoGravado, 2),
                                Alicuota = 0,
                                Impuestos = 0
                            }, tran);

                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = iTotal++,
                                Neto = Math.Round(comp.NoGravado, 2),
                                Alicuota = 0,
                                ImporteAlicuota = 0
                            }, tran);
                        }

                        if (comp.Neto != 0)
                        {
                            var ctaNeto = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta, tran);
                            if (ctaNeto == default)
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable para el Importe Neto en la Linea {iLoop}. Verifique");

                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = iItem++,
                                IdCuentaContable = ctaNeto.IdCuentaContable,
                                Descripcion = ctaNeto.Descripcion,
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = Math.Round(comp.Neto, 2),
                                Alicuota = comp.Alicuota,
                                Impuestos = Math.Round(comp.Neto * (comp.Alicuota / 100), 2)
                            }, tran);

                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = iTotal++,
                                Neto = Math.Round(comp.Neto, 2),
                                Alicuota = comp.Alicuota,
                                ImporteAlicuota = Math.Round(comp.Neto * (comp.Alicuota / 100), 2)
                            }, tran);
                        }

                        if (comp.Neto2 != 0)
                        {
                            var ctaNeto2 = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta2, tran);
                            if (ctaNeto2 == default)
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable para el Importe Neto2 en la Linea {iLoop}. Verifique");

                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = iItem++,
                                IdCuentaContable = ctaNeto2.IdCuentaContable,
                                Descripcion = ctaNeto2.Descripcion,
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = Math.Round(comp.Neto2, 2),
                                Alicuota = comp.Alicuota2,
                                Impuestos = Math.Round(comp.Neto2 * (comp.Alicuota2 / 100), 2)
                            }, tran);

                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = iTotal++,
                                Neto = Math.Round(comp.Neto2, 2),
                                Alicuota = comp.Alicuota2,
                                ImporteAlicuota = Math.Round(comp.Neto2 * (comp.Alicuota2 / 100), 2)
                            }, tran);
                        }

                        #endregion

                        #region Asiento Contable

                        var asiento = new Asiento
                        {
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            IdEjercicio = ejercicio.IdEjercicio,
                            Descripcion = string.Concat("COMPRA ", tipo.Descripcion, " ", numeroCompCompleto),
                            Fecha = comp.Fecha,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            IdUsuario = _permissionsBusiness.Value.User.Id,
                            FechaIngreso = DateTime.Now
                        };

                        var idAsiento = await _asientosRepository.Insert(asiento, tran);
                        var indItem = 1;

                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaCompraServicios.IdCuentaContable,
                            Detalle = ctaCompraServicios.Descripcion,
                            Debitos = tipo.EsCredito ? Math.Round(comp.Total, 2) : 0,
                            Creditos = tipo.EsCredito ? 0 : Math.Round(comp.Total, 2)
                        }, tran);

                        if (comp.Neto > 0)
                        {
                            var ctaNeto = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta, tran);
                            if (ctaNeto != default)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = ctaNeto.IdCuentaContable,
                                    Detalle = ctaNeto.Descripcion,
                                    Debitos = tipo.EsCredito ? 0 : Math.Round(comp.Neto, 2),
                                    Creditos = tipo.EsCredito ? Math.Round(comp.Neto, 2) : 0
                                }, tran);
                            }
                            else
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable en la Linea {iLoop}. Verifique.");
                        }

                        if (comp.Neto2 > 0)
                        {
                            var ctaNeto2 = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta2, tran);
                            if (ctaNeto2 != default)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = ctaNeto2.IdCuentaContable,
                                    Detalle = ctaNeto2.Descripcion,
                                    Debitos = tipo.EsCredito ? 0 : Math.Round(comp.Neto2, 2),
                                    Creditos = tipo.EsCredito ? Math.Round(comp.Neto2, 2) : 0
                                }, tran);
                            }
                            else
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable 2 en la Linea {iLoop}. Verifique.");
                        }

                        if (comp.NoGravado > 0)
                        {
                            var ctaNoGravado = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta_NoGravado, tran);
                            if (ctaNoGravado != default)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = ctaNoGravado.IdCuentaContable,
                                    Detalle = ctaNoGravado.Descripcion,
                                    Debitos = tipo.EsCredito ? 0 : Math.Round(comp.NoGravado, 2),
                                    Creditos = tipo.EsCredito ? Math.Round(comp.NoGravado, 2) : 0
                                }, tran);
                            }
                            else
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable No Gravado en la Linea {iLoop}. Verifique.");
                        }

                        var impuesto = comp.Neto * (comp.Alicuota / 100) + (comp.Neto2 * (comp.Alicuota2 / 100));
                        if (impuesto > 0)
                        {
                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = ctaCreditoFiscal.IdCuentaContable,
                                Detalle = ctaCreditoFiscal.Descripcion,
                                Debitos = tipo.EsCredito ? 0 : Math.Round(impuesto, 2),
                                Creditos = tipo.EsCredito ? Math.Round(impuesto, 2) : 0
                            }, tran);
                        }

                        if (comp.Percepcion > 0)
                        {
                            var ctaPercepcion = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta_Percepcion, tran);
                            if (ctaPercepcion != default)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = ctaPercepcion.IdCuentaContable,
                                    Detalle = ctaPercepcion.Descripcion,
                                    Debitos = tipo.EsCredito ? 0 : Math.Round(comp.Percepcion, 2),
                                    Creditos = tipo.EsCredito ? Math.Round(comp.Percepcion, 2) : 0
                                }, tran);
                            }
                            else
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable de Percepción en la Linea {iLoop}. Verifique.");
                        }

                        if (comp.Percepcion2 > 0)
                        {
                            var ctaPercepcion = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndDescripcion(_permissionsBusiness.Value.User.IdEmpresa, comp.Cta_Percepcion2, tran);
                            if (ctaPercepcion != default)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = ctaPercepcion.IdCuentaContable,
                                    Detalle = ctaPercepcion.Descripcion,
                                    Debitos = tipo.EsCredito ? 0 : Math.Round(comp.Percepcion2, 2),
                                    Creditos = tipo.EsCredito ? Math.Round(comp.Percepcion2, 2) : 0
                                }, tran);
                            }
                            else
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable de Percepción en la Linea {iLoop}. Verifique.");
                        }

                        if (comp.ImpuestosInternos > 0)
                        {
                            var cuentaContableImpInterno = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.IMPUESTOS_INTERNOS, tran);
                            if (cuentaContableImpInterno != default)
                            {
                                await _asientosDetalleRepository.Insert(new AsientoDetalle
                                {
                                    IdAsiento = (int)idAsiento,
                                    Item = indItem++,
                                    IdCuentaContable = cuentaContableImpInterno.IdCuentaContable,
                                    Detalle = cuentaContableImpInterno.Descripcion,
                                    Debitos = tipo.EsCredito ? 0 : Math.Round(comp.ImpuestosInternos, 2),
                                    Creditos = tipo.EsCredito ? Math.Round(comp.ImpuestosInternos, 2) : 0
                                }, tran);
                            }
                            else
                                throw new BusinessException($"No se ecnuentra la Cuenta Contable de Impuestos Internos en la Linea {iLoop}. Verifique.");
                        }

                        await _comprobantesComprasAsientoRepository.Insert(new ComprobanteCompraAsiento { IdComprobanteCompra = (int)idComprobanteCompra, IdAsiento = (int)idAsiento }, tran);

                        #endregion

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        _logger.LogError(ex, null);
                        resultados.Add(ex.Message);
                    }
                }

                return resultados;
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, null);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

    }
}
