using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.PlanillaGastos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PlanillaGastosBusiness: BaseBusiness, IPlanillaGastosBusiness
    {
        private readonly ILogger<AsientosBusiness> _logger;
        private readonly IMonedasFechasCambioRepository _monedasFechasCambioRepository;
        private readonly IPlanillaGastosRepository _planillaGastosRepository;
        private readonly IPlanillaGastosItemsRepository _planillaGastosItemsRepository;
        private readonly IPlanillaGastosAsientoRepository _planillaGastosAsientoRepository;
        private readonly IPlanillaGastosComprobantesComprasRepository _planillaGastosComprobantesComprasRepository;
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;
        private readonly IComprobantesComprasItemRepository _comprobantesComprasItemRepository;
        private readonly IComprobantesComprasTotalesRepository _comprobantesComprasTotalesRepository;
        private readonly IComprobantesComprasPercepcionesRepository _comprobantesComprasPercepcionesRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IProveedoresRepository _proveedoresRepository;
        private readonly IProveedoresEmpresasRepository _proveedoresEmpresasRepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public PlanillaGastosBusiness(
            ILogger<AsientosBusiness> logger,
            IPlanillaGastosRepository planillaGastosRepository,
            IPlanillaGastosItemsRepository planillaGastosItemsRepository,
            IPlanillaGastosComprobantesComprasRepository planillaGastosComprobantesComprasRepository,
            IPlanillaGastosAsientoRepository planillaGastosAsientoRepository,
            IComprobantesComprasRepository comprobantesComprasRepository,
            IComprobantesComprasItemRepository comprobantesComprasItemRepository,
            IComprobantesComprasPercepcionesRepository comprobantesComprasPercepcionesRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            IProveedoresRepository proveedoresRepository,
            IProveedoresEmpresasRepository proveedoresEmpresasRepository,
            ILookupsBusiness lookupsBusiness,
            IAsientosRepository asientosRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository,
            IMonedasFechasCambioRepository monedasFechasCambioRepository)
        {
            _logger = logger;
            _planillaGastosRepository = planillaGastosRepository;
            _planillaGastosItemsRepository = planillaGastosItemsRepository;
            _planillaGastosComprobantesComprasRepository = planillaGastosComprobantesComprasRepository;
            _planillaGastosAsientoRepository = planillaGastosAsientoRepository;
            _comprobantesComprasRepository = comprobantesComprasRepository;
            _comprobantesComprasItemRepository = comprobantesComprasItemRepository;
            _comprobantesComprasTotalesRepository = comprobantesComprasTotalesRepository;
            _comprobantesComprasPercepcionesRepository = comprobantesComprasPercepcionesRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _proveedoresRepository = proveedoresRepository;
            _proveedoresEmpresasRepository = proveedoresEmpresasRepository;
            _asientosRepository = asientosRepository;
            _monedasFechasCambioRepository = monedasFechasCambioRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
        }

        public async Task<PlanillaGastosViewModel> Get(int idPlanillaGastos)
        {
            var planilla = await _planillaGastosRepository.GetById<PlanillaGastos>(idPlanillaGastos);

            if (planilla == null)
                return null;

            var items = await _planillaGastosItemsRepository.GetByIdPlanillaGastos(idPlanillaGastos);

            var model = _mapper.Map<PlanillaGastosViewModel>(planilla);
            model.Items = _mapper.Map<List<PlanillaGastosDetalle>>(items);

            return model;
        }

        public async Task<Custom.PlanillaGastos> GetCustom(int idPlanillaGastos)
        {
            var planilla = await _planillaGastosRepository.GetCustomByIdPlanillaGastos(idPlanillaGastos);
            var items = await _planillaGastosItemsRepository.GetByIdPlanillaGastos(idPlanillaGastos);

            planilla.Items = items;

            return planilla;
        }

        public async Task<DataTablesResponse<Custom.PlanillaGastos>> GetAll(DataTablesRequest request)
        {
            var customQuery = _planillaGastosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEjercicio"))
                builder.Append($"AND IdEjercicio = {filters["IdEjercicio"]}");

            if (filters.ContainsKey("Descripcion"))
                builder.Append($"AND Descripcion LIKE {string.Concat("%", filters["Descripcion"], "%")}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.PlanillaGastos>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(PlanillaGastosViewModel model)
        {
            var planilla = _mapper.Map<PlanillaGastos>(model);
            var items = _mapper.Map<List<PlanillaGastoItem>>(model.Items);

            Validate(planilla, items);

            //Maestro de Alicutoas
            var alicuotas = (await _lookupsBusiness.Value.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA)
                .ToDictionary(a => a.IdAlicuota, a => a.Valor);

            var tran = _uow.BeginTransaction();

            try
            {
                planilla.IdCuentaContable = model.IdCuentaContable;
                planilla.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                planilla.Descripcion = planilla.Descripcion.ToUpper().Trim();
                planilla.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                planilla.IdUsuario = _permissionsBusiness.Value.User.Id;
                planilla.Moneda = model.IdMoneda;
                planilla.FechaIngreso = DateTime.Now;
                planilla.IdEstadoPlanilla = (int)EstadoPlanillaGastos.Ingresada;
                planilla.ImporteTotal = items?.Sum(i => i.Total) ?? 0;

                var id = await _planillaGastosRepository.Insert(planilla, tran);

                foreach (var item in model.Items)
                {
                    item.Alicuota = alicuotas.GetValueOrDefault(item.IdAlicuota);
                    if(item.IdAlicuota2 != default)
                        item.Alicuota2 = alicuotas.GetValueOrDefault(item.IdAlicuota2.Value);

                    await _planillaGastosItemsRepository.Insert(new PlanillaGastoItem
                    {
                        IdPlanillaGastos = (int)id,
                        Item = item.Item,
                        CUIT = string.IsNullOrEmpty(item.CUIT) ? string.Empty : item.CUIT,
                        Proveedor = item.Proveedor.ToUpper().Trim(),
                        Fecha = DateTime.Parse(item.Fecha),
                        IdComprobante = item.IdComprobante,
                        NumeroComprobante = item.NumeroComprobante,
                        Detalle = item.Detalle.ToUpper().Trim(),
                        SubtotalNoGravado = item.NoGravado,
                        Subtotal = item.Subtotal,
                        IdAlicuota = item.IdAlicuota,
                        Alicuota = item.Alicuota,
                        Subtotal2 = item.Subtotal2,
                        IdAlicuota2 = item.IdAlicuota2,
                        Alicuota2 = item.Alicuota2,
                        IdCuentaContablePercepcion = item.IdCuentaContablePercepcion,
                        Percepcion = item.Percepcion,
						IdCuentaContablePercepcion2 = item.IdCuentaContablePercepcion2,
						Percepcion2 = item.Percepcion2,
                        ImpuestosInternos = item.ImpuestosInternos,
                        Total = item.Total,
                        CAE = item.CAE ?? string.Empty,
                        VencimientoCAE = string.IsNullOrEmpty(item.VencimientoCAE) ? default : item.VencimientoCAE.ToDate("dd/MM/yyyy")
                    }, tran); 
                }

                #region Comprobantes de Compras

                double? cotizacion = 1D;
                if (planilla.Moneda != Monedas.MonedaLocal.Description())
                    cotizacion = await _monedasFechasCambioRepository.GetFechaCambioByCodigo(planilla.Moneda, planilla.Fecha, _permissionsBusiness.Value.User.IdEmpresa, tran);

                foreach (var item in model.Items)
                {
                    if (!string.IsNullOrEmpty(item.CUIT))
                    {
                        long idProveedor;
                        var proveedor = await _proveedoresRepository.GetByCUIT(item.CUIT, _permissionsBusiness.Value.User.IdEmpresa, tran);
                        if (proveedor == default)
                        {
                            var idTipoIVA = await _lookupsBusiness.Value.GetTipoIVAByComprobante(item.IdComprobante);
                            idProveedor = await _proveedoresRepository.Insert(new Proveedor
                            {
                                RazonSocial = item.Proveedor.ToUpper().Trim(),
                                NombreFantasia = item.Proveedor.ToUpper().Trim(),
                                CUIT = item.CUIT.ToUpper().Trim(),
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

                            var proveedorEmpresa = _proveedoresEmpresasRepository.GetByIdProveedorAndEmpresa((int)idProveedor, _permissionsBusiness.Value.User.IdEmpresa, tran);
                            if (proveedorEmpresa == default)
                            {
                                await _proveedoresEmpresasRepository.Insert<ProveedorEmpresa>(new ProveedorEmpresa
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdProveedor = (int)idProveedor
                                }, tran);
                            }
                        }

                        var comprobanteCpas = await _comprobantesComprasRepository.GetComprobanteByProveedor(item.IdComprobante, item.NumeroComprobante, (int)idProveedor);
                        if (comprobanteCpas != default)
                            throw new BusinessException($"El Comprobante {item.NumeroComprobante} del item {item.Item} para el Proveedor {proveedor.RazonSocial} ya se Encuentra Cargado en el Sistema. Verifique.");

                        var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                        {
                            IdComprobante = item.IdComprobante,
                            Sucursal = item.NumeroComprobante.Substring(0, 5),
                            Numero = item.NumeroComprobante.Substring(6, 8),
                            IdEjercicio = planilla.IdEjercicio,
                            IdProveedor = (int)idProveedor,
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            Fecha = DateTime.Now,
                            FechaReal = DateTime.Parse(item.Fecha),
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            IdUsuario = _permissionsBusiness.Value.User.Id,
                            FechaIngreso = DateTime.Now,
                            Subtotal = item.Subtotal + (item.Subtotal2 ?? 0D) + (item.NoGravado ?? 0D),
                            Total = item.Total,
                            Percepciones = (item.Percepcion ?? 0D) + (item.Percepcion2 ?? 0D),
                            Moneda = planilla.Moneda,
                            Cotizacion = cotizacion.HasValue ? cotizacion.Value : 0,
                            IdTipoComprobante = (int)TipoComprobante.GastosProveedores,
                            CAE = item.CAE ?? string.Empty,
                            VencimientoCAE = string.IsNullOrEmpty(item.VencimientoCAE) == default ? default : item.VencimientoCAE.ToDate("dd/MM/yyyy"),
                            IdEstadoAFIP = string.IsNullOrEmpty(item.CAE) ? (int)EstadoAFIP.DocumentoSinCAE : (int)EstadoAFIP.Inicial
                        }, tran);

                        await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                        {
                            IdComprobanteCompra = (int)idComprobanteCompra,
                            Item = 1,
                            Descripcion = string.Concat(planilla.Descripcion.ToUpper().Trim(), proveedor.RazonSocial),
                            Cantidad = 0,
                            Bonificacion = 0,
                            Precio = 0,
                            Importe = item.Subtotal,
                            Alicuota = item.Alicuota,
                            Impuestos = item.Subtotal * (item.Alicuota / 100)
                        }, tran);

                        if (item.IdAlicuota2.HasValue && (item.Subtotal2 ?? 0) != 0)
                        {
                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 2,
                                Descripcion = string.Concat(planilla.Descripcion.ToUpper().Trim(), proveedor.RazonSocial),
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = item.Subtotal,
                                Alicuota = item.Alicuota2.Value,
                                Impuestos = item.Subtotal2.Value * (item.Alicuota2.Value / 100)
                            }, tran);
                        }

                        if (item.NoGravado.HasValue && item.NoGravado.Value != 0)
                        {
                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 3,
                                Descripcion = string.Concat(planilla.Descripcion.ToUpper().Trim(), proveedor.RazonSocial),
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = item.NoGravado.Value,
                                Alicuota = 0,
                                Impuestos = 0
                            }, tran);
                        }

                        if (item.Percepcion.HasValue && item.Percepcion.Value != 0)
                        {
                            if (item.IdCuentaContablePercepcion.HasValue)
                            {
                                await _comprobantesComprasPercepcionesRepository.Insert(new ComprobanteCompraPercepcion
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = item.IdCuentaContablePercepcion.Value,
                                    Importe = item.Percepcion.Value
                                });
                            }
                            else
                                throw new BusinessException($"Ingrese la Cuenta Contable para la Percepcion del Comprobante de {item.NumeroComprobante}.");
                        }

                        if (item.Percepcion2.HasValue && item.Percepcion2.Value != 0)
                        {
                            if (item.IdCuentaContablePercepcion2.HasValue)
                            {
                                await _comprobantesComprasPercepcionesRepository.Insert(new ComprobanteCompraPercepcion
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = item.IdCuentaContablePercepcion2.Value,
                                    Importe = item.Percepcion2.Value
                                });
                            }
                            else
                                throw new BusinessException($"Ingrese la Cuenta Contable para la Percepcion del Comprobante de {item.NumeroComprobante}.");
                        }

                        if (item.ImpuestosInternos.HasValue && item.ImpuestosInternos.Value != 0)
                        {
                            var cuentaContableImpInterno = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.IMPUESTOS_INTERNOS, tran);
                            if (cuentaContableImpInterno != default)
                            {
                                await _comprobantesComprasPercepcionesRepository.Insert(new ComprobanteCompraPercepcion
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = cuentaContableImpInterno.IdCuentaContable,
                                    Importe = item.ImpuestosInternos.Value
                                });
                            }
                            else
                                throw new BusinessException("No se ecnuentra la Cuenta Contable de Impuestos Internos. Verifique.");
                        }

                        await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                        {
                            IdComprobanteCompra = (int)idComprobanteCompra,
                            Item = 1,
                            Neto = item.Subtotal,
                            Alicuota = item.Alicuota,
                            ImporteAlicuota = item.Subtotal * (item.Alicuota / 100)
                        }, tran);

                        if (item.Subtotal2.HasValue && item.Subtotal2.Value != 0)
                        {
                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 2,
                                Neto = item.Subtotal2.Value,
                                Alicuota = item.Alicuota2.Value,
                                ImporteAlicuota = item.Subtotal2.Value * (item.Alicuota2.Value / 100)
                            }, tran);
                        }

                        if (item.NoGravado.HasValue)
                        {
                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 3,
                                Neto = item.NoGravado.Value,
                                Alicuota = 0,
                                ImporteAlicuota = 0
                            }, tran);
                        }

                        await _planillaGastosComprobantesComprasRepository.Insert(new PlanillaGastoComprobanteCompra
                        {
                            IdComprobanteCompra = (int)idComprobanteCompra,
                            IdPlanillaGastos = (int)id
                        }, tran);
                    }
                }

                #endregion

                #region Asiento Contable

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = planilla.IdEjercicio,
                    Descripcion = string.Concat(string.Concat("ASIENTO ", planilla.Descripcion)).ToUpper().Trim(),
                    Fecha = planilla.Fecha,
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
                        Debitos = model.Items.Sum(dc => dc.Total),
                        Creditos = 0
                    }, tran);
                }
                else
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Compras. Verifique.");

                var impuesto = model.Items.Sum(dc => dc.Subtotal * (dc.IdAlicuota / 100)) + model.Items.Sum(dc => (dc.Subtotal2 ?? 0) * ((dc.IdAlicuota2 ?? 0D) / 100));
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
                            Debitos = 0,
                            Creditos = impuesto
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");
                }

				var percepciones = model.Items.GroupBy(pe => pe.IdCuentaContablePercepcion)
					.Select(g => new
					{
						g.FirstOrDefault().IdCuentaContablePercepcion,
						Importe = g.Sum(t => t.Percepcion ?? 0)
					});

				foreach (var percepcion in percepciones)
				{
                    if (percepcion.IdCuentaContablePercepcion != default)
                    {
                        var ctaPercepcion = await _cuentasContablesRepository.GetById<CuentaContable>(percepcion.IdCuentaContablePercepcion ?? 0, tran);

                        if (ctaPercepcion != default)
                        {
                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = ctaPercepcion.IdCuentaContable,
                                Detalle = ctaPercepcion.Descripcion,
                                Debitos = 0,
                                Creditos = percepcion.Importe
                            }, tran);
                        }
                        else
                            throw new BusinessException("No se ecnuentra la Cuenta Contable de Percepción. Verifique.");
                    }
				}

				var percepciones2 = model.Items.GroupBy(pe => pe.IdCuentaContablePercepcion2)
					.Select(g => new
					{
						g.FirstOrDefault().IdCuentaContablePercepcion2,
						Importe = g.Sum(t => t.Percepcion2 ?? 0)
					});

				foreach (var percepcion in percepciones2)
				{
                    if (percepcion.IdCuentaContablePercepcion2 != default)
                    {
                        var ctaPercepcion = await _cuentasContablesRepository.GetById<CuentaContable>(percepcion.IdCuentaContablePercepcion2 ?? 0, tran);

                        if (ctaPercepcion != default)
                        {
                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = ctaPercepcion.IdCuentaContable,
                                Detalle = ctaPercepcion.Descripcion,
                                Debitos = 0,
                                Creditos = percepcion.Importe
                            }, tran);
                        }
                        else
                            throw new BusinessException("No se ecnuentra la Cuenta Contable de Percepción. Verifique.");
                    }
				}

                var impuestosInternos = model.Items.Sum(ii => ii.ImpuestosInternos);
                if (impuestosInternos.HasValue && impuestosInternos > 0)
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
                            Debitos = 0,
                            Creditos = impuestosInternos.Value
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Impuestos Internos. Verifique.");
                }
                
                var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(planilla.IdCuentaContable, tran);
                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)idAsiento,
                    Item = indItem++,
                    IdCuentaContable = cuentaContable.IdCuentaContable,
                    Detalle = cuentaContable.Descripcion,
                    Debitos = 0,
                    Creditos = model.Items.Sum(pe => pe.Subtotal) + model.Items.Sum(pe => (pe.Subtotal2 ?? 0D)) + model.Items.Sum(pe => (pe.NoGravado ?? 0D))
                }, tran);

                await _planillaGastosAsientoRepository.Insert(new PlanillaGastoAsiento { IdPlanillaGastos = (int)id, IdAsiento = (int)idAsiento }, tran);

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

        public async Task Remove(int idPlanillaGastos)
        {
            var planilla = await _planillaGastosRepository.GetById<PlanillaGastos>(idPlanillaGastos);

            if (planilla == null)
            {
                throw new BusinessException("Planilla de Gastos Inexistente");
            }

            if (planilla.IdEstadoPlanilla == (int)EstadoPlanillaGastos.Pagada)
            {
                throw new BusinessException("La Planilla de Gastos esta Pagada. No se puede Eliminar.");
            }

            if (_cierreMesRepository.MesCerrado(planilla.IdEjercicio, planilla.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("La Planilla de Gastos se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var planillaAsiento = await _planillaGastosAsientoRepository.GetByIdPlanillaGastos(idPlanillaGastos, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(planillaAsiento.IdAsiento, tran);

                //Borro el Asiento de la Planilla
                await _planillaGastosAsientoRepository.DeleteByIdPlanillaGastos(idPlanillaGastos, tran);
                await _asientosDetalleRepository.DeleteByIdAsiento(planillaAsiento.IdAsiento, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                var comprobantePlanilla = await _planillaGastosComprobantesComprasRepository.GetByIdPlanillaGastos(idPlanillaGastos);

                foreach (var comprobanteCompra in comprobantePlanilla)
                {
                    var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(comprobanteCompra.IdComprobanteCompra, tran);

                    await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(comprobanteCompra.IdComprobanteCompra, tran);
                    await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(comprobanteCompra.IdComprobanteCompra, tran);
                    await _comprobantesComprasPercepcionesRepository.DeleteByIdComprobanteCompra(comprobanteCompra.IdComprobanteCompra, tran);

                    comprobante.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                    await _comprobantesComprasRepository.Update(comprobante, tran);
                }

                planilla.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _planillaGastosComprobantesComprasRepository.DeleteByIdPlanillaGastos(idPlanillaGastos, tran);
                await _planillaGastosAsientoRepository.DeleteByIdPlanillaGastos(idPlanillaGastos, tran);
                await _planillaGastosItemsRepository.DeleteByIdPlanillaGastos(idPlanillaGastos, tran);

                await _planillaGastosRepository.Update(planilla, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Update(PlanillaGastosViewModel model)
        {
            var planilla = _mapper.Map<PlanillaGastos>(model);
            var items = _mapper.Map<List<PlanillaGastoItem>>(model.Items);

            var dbPlanilla = await _planillaGastosRepository.GetById<PlanillaGastos>(planilla.IdPlanillaGastos);

            Validate(planilla, items);

            if (dbPlanilla == null)
            {
                throw new BusinessException("Planilla de Gastos inexistente");
            }

            if (dbPlanilla.IdEstadoPlanilla == (int)EstadoPlanillaGastos.Pagada)
            {
                throw new BusinessException("La Planilla de Gastos esta Pagada. No se puede Modificar.");
            }

            var alicuotas = (await _lookupsBusiness.Value.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA)
                .ToDictionary(a => a.IdAlicuota, a => a.Valor);

            var tran = _uow.BeginTransaction();

            try
            {
                var planillaAsiento = await _planillaGastosAsientoRepository.GetByIdPlanillaGastos(dbPlanilla.IdPlanillaGastos, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(planillaAsiento.IdAsiento, tran);

                //Borro el Asiento de la Planilla
                await _planillaGastosAsientoRepository.DeleteByIdPlanillaGastos(dbPlanilla.IdPlanillaGastos, tran);
                await _asientosDetalleRepository.DeleteByIdAsiento(planillaAsiento.IdAsiento, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                //Borro el Asiento de la Planilla
                await _asientosDetalleRepository.DeleteByIdAsiento(planillaAsiento.IdAsiento, tran);

                dbPlanilla.Descripcion = planilla.Descripcion.ToUpper().Trim();
                dbPlanilla.IdUsuario = _permissionsBusiness.Value.User.Id;
                dbPlanilla.Fecha = planilla.Fecha;
                dbPlanilla.IdCuentaContable = planilla.IdCuentaContable;
                dbPlanilla.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbPlanilla.ImporteTotal = model.Items?.Sum(i => i.Total) ?? 0;

                await _planillaGastosRepository.Update(dbPlanilla, tran);

                var comprobantePlanilla = await _planillaGastosComprobantesComprasRepository.GetByIdPlanillaGastos(dbPlanilla.IdPlanillaGastos);

                foreach (var comprobanteCompra in comprobantePlanilla)
                {
                    var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(comprobanteCompra.IdComprobanteCompra, tran);

                    await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(comprobanteCompra.IdComprobanteCompra, tran);
                    await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(comprobanteCompra.IdComprobanteCompra, tran);

                    comprobante.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                    await _comprobantesComprasRepository.Update(comprobante, tran);
                }

                await _planillaGastosComprobantesComprasRepository.DeleteByIdPlanillaGastos(dbPlanilla.IdPlanillaGastos, tran);
                await _planillaGastosItemsRepository.DeleteByIdPlanillaGastos(dbPlanilla.IdPlanillaGastos, tran);

                foreach (var item in model.Items)
                {
                    item.Alicuota = alicuotas.GetValueOrDefault(item.IdAlicuota);
                    if (item.IdAlicuota2 != default)
                        item.Alicuota2 = alicuotas.GetValueOrDefault(item.IdAlicuota2.Value);

                    await _planillaGastosItemsRepository.Insert(new PlanillaGastoItem
                    {
                        IdPlanillaGastos = dbPlanilla.IdPlanillaGastos,
                        Item = item.Item,
                        CUIT = string.IsNullOrEmpty(item.CUIT) ? string.Empty : item.CUIT,
                        Proveedor = item.Proveedor.ToUpper().Trim(),
                        Fecha = DateTime.Parse(item.Fecha),
                        IdComprobante = item.IdComprobante,
                        NumeroComprobante = item.NumeroComprobante,
                        Detalle = item.Detalle.ToUpper().Trim(),
                        SubtotalNoGravado = item.NoGravado,
                        Subtotal = item.Subtotal,
                        IdAlicuota = item.IdAlicuota,
                        Alicuota = item.Alicuota,
                        Subtotal2 = item.Subtotal2,
                        IdAlicuota2 = item.IdAlicuota2,
                        Alicuota2 = item.Alicuota2,
						IdCuentaContablePercepcion = item.IdCuentaContablePercepcion,
						Percepcion = item.Percepcion,
						IdCuentaContablePercepcion2 = item.IdCuentaContablePercepcion2,
						Percepcion2 = item.Percepcion2,
                        ImpuestosInternos = item.ImpuestosInternos,
                        Total = item.Total,
                        CAE = item.CAE ?? string.Empty,
                        VencimientoCAE = string.IsNullOrEmpty(item.VencimientoCAE) ? default : item.VencimientoCAE.ToDate("dd/MM/yyyy")
                    }, tran);
                }

                #region Comprobantes de Compras

                double? cotizacion = 1D;
                if (planilla.Moneda != Monedas.MonedaLocal.Description())
                    cotizacion = await _monedasFechasCambioRepository.GetFechaCambioByCodigo(planilla.Moneda, planilla.Fecha, _permissionsBusiness.Value.User.IdEmpresa, tran);

                foreach (var item in model.Items)
                {
                    if (!string.IsNullOrEmpty(item.CUIT))
                    {
                        long idProveedor;
                        var proveedor = await _proveedoresRepository.GetByCUIT(item.CUIT, _permissionsBusiness.Value.User.IdEmpresa, tran);
                        if (proveedor == default)
                        {
                            var idTipoIVA = await _lookupsBusiness.Value.GetTipoIVAByComprobante(item.IdComprobante);
                            idProveedor = await _proveedoresRepository.Insert(new Proveedor
                            {
                                RazonSocial = item.Proveedor.ToUpper().Trim(),
                                NombreFantasia = item.Proveedor.ToUpper().Trim(),
                                CUIT = item.CUIT.ToUpper().Trim(),
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

                            var proveedorEmpresa = _proveedoresEmpresasRepository.GetByIdProveedorAndEmpresa((int)idProveedor, _permissionsBusiness.Value.User.IdEmpresa, tran);
                            if (proveedorEmpresa == default)
                            {
                                await _proveedoresEmpresasRepository.Insert<ProveedorEmpresa>(new ProveedorEmpresa
                                {
                                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                    IdProveedor = (int)idProveedor
                                }, tran);
                            }
                        }

                        var comprobanteCpas = await _comprobantesComprasRepository.GetComprobanteByProveedor(item.IdComprobante, item.NumeroComprobante, (int)idProveedor);
                        if(comprobanteCpas != default)
                            throw new BusinessException($"El Comprobante {item.NumeroComprobante} del item {item.Item} para el Proveedor {proveedor.RazonSocial} ya se Encuentra Cargado en el Sistema. Verifique.");

                        var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                        {
                            IdComprobante = item.IdComprobante,
                            Sucursal = item.NumeroComprobante.Substring(0, 5),
                            Numero = item.NumeroComprobante.Substring(6, 8),
                            IdEjercicio = planilla.IdEjercicio,
                            IdProveedor = (int)idProveedor,
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            Fecha = DateTime.Now,
                            FechaReal = DateTime.Parse(item.Fecha),
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            IdUsuario = _permissionsBusiness.Value.User.Id,
                            FechaIngreso = DateTime.Now,
                            Subtotal = item.Subtotal + (item.Subtotal2 ?? 0D) + (item.NoGravado ?? 0D),
                            Total = item.Total,
                            Percepciones = (item.Percepcion ?? 0D) + (item.Percepcion2 ?? 0D),
                            Moneda = dbPlanilla.Moneda,
                            Cotizacion = cotizacion.HasValue ? cotizacion.Value : 0,
                            IdTipoComprobante = (int)TipoComprobante.GastosProveedores,
                            CAE = item.CAE ?? string.Empty,
                            VencimientoCAE = string.IsNullOrEmpty(item.VencimientoCAE) ? default : item.VencimientoCAE.ToDate("dd/MM/yyyy"),
                            IdEstadoAFIP = string.IsNullOrEmpty(item.CAE) ? (int)EstadoAFIP.DocumentoSinCAE : (int)EstadoAFIP.Inicial
                        }, tran);

                        await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                        {
                            IdComprobanteCompra = (int)idComprobanteCompra,
                            Item = 1,
                            Descripcion = string.Concat(dbPlanilla.Descripcion.ToUpper().Trim() + proveedor.RazonSocial),
                            Cantidad = 0,
                            Bonificacion = 0,
                            Precio = 0,
                            Importe = item.Subtotal,
                            Alicuota = item.Alicuota,
                            Impuestos = item.Subtotal * (item.Alicuota / 100)
                        }, tran);

                        if (item.IdAlicuota2.HasValue && item.Subtotal2.Value != 0)
                        {
                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 2,
                                Descripcion = string.Concat(dbPlanilla.Descripcion.ToUpper().Trim() + proveedor.RazonSocial),
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = item.Subtotal2.Value,
                                Alicuota = item.Alicuota2.Value,
                                Impuestos = item.Subtotal2.Value * (item.Alicuota2.Value / 100)
                            }, tran);
                        }

                        if (item.NoGravado.HasValue && item.NoGravado.Value != 0)
                        {
                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 3,
                                Descripcion = string.Concat(dbPlanilla.Descripcion.ToUpper().Trim() + proveedor.RazonSocial),
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = item.NoGravado.Value,
                                Alicuota = 0,
                                Impuestos = 0
                            }, tran);
                        }


                        if (item.Percepcion.HasValue && item.Percepcion.Value != 0)
                        {
                            if (item.IdCuentaContablePercepcion.HasValue)
                            {
                                await _comprobantesComprasPercepcionesRepository.Insert(new ComprobanteCompraPercepcion
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = item.IdCuentaContablePercepcion.Value,
                                    Importe = item.Percepcion.Value
                                });
                            }
                            else
                                throw new BusinessException($"Ingrese la Cuenta Contable para la Percepcion del Comprobante de {item.NumeroComprobante}.");
                        }

                        if (item.Percepcion2.HasValue && item.Percepcion2.Value != 0)
                        {
                            if (item.IdCuentaContablePercepcion2.HasValue)
                            {
                                await _comprobantesComprasPercepcionesRepository.Insert(new ComprobanteCompraPercepcion
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = item.IdCuentaContablePercepcion2.Value,
                                    Importe = item.Percepcion2.Value
                                });
                            }
                            else
                                throw new BusinessException($"Ingrese la Cuenta Contable para la Percepcion del Comprobante de {item.NumeroComprobante}.");
                        }

                        if (item.ImpuestosInternos.HasValue && item.ImpuestosInternos.Value != 0)
                        {
                            var cuentaContableImpInterno = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.IMPUESTOS_INTERNOS, tran);
                            if (cuentaContableImpInterno != default)
                            {
                                await _comprobantesComprasPercepcionesRepository.Insert(new ComprobanteCompraPercepcion
                                {
                                    IdComprobanteCompra = (int)idComprobanteCompra,
                                    IdCuentaContable = cuentaContableImpInterno.IdCuentaContable,
                                    Importe = item.ImpuestosInternos.Value
                                });
                            }
                            else
                                throw new BusinessException("No se ecnuentra la Cuenta Contable de Impuestos Internos. Verifique.");
                        }

                        await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                        {
                            IdComprobanteCompra = (int)idComprobanteCompra,
                            Item = 1,
                            Neto = item.Subtotal,
                            Alicuota = item.IdAlicuota,
                            ImporteAlicuota = item.Subtotal * (item.IdAlicuota / 100)
                        }, tran);

                        if (item.Subtotal2.HasValue && item.Subtotal2.Value != 0)
                        {
                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 2,
                                Neto = item.Subtotal2.Value,
                                Alicuota = item.IdAlicuota2.Value,
                                ImporteAlicuota = item.Subtotal2.Value * (item.IdAlicuota2.Value / 100)
                            }, tran);
                        }

                        if (item.NoGravado.HasValue && item.NoGravado.Value != 0)
                        {
                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 3,
                                Neto = item.NoGravado.Value,
                                Alicuota = 0,
                                ImporteAlicuota = 0
                            }, tran);
                        }

                        await _planillaGastosComprobantesComprasRepository.Insert(new PlanillaGastoComprobanteCompra
                        {
                            IdComprobanteCompra = (int)idComprobanteCompra,
                            IdPlanillaGastos = dbPlanilla.IdPlanillaGastos
                        }, tran);
                    }
                }

                #endregion

                #region Asiento Contable

                asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = planilla.IdEjercicio,
                    Descripcion = string.Concat(string.Concat("ASIENTO ", planilla.Descripcion)).ToUpper().Trim(),
                    Fecha = planilla.Fecha,
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
                        Debitos = model.Items.Sum(dc => dc.Total),
                        Creditos = 0
                    }, tran);
                }
                else
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Compras. Verifique.");

                var impuesto = model.Items.Sum(dc => dc.Subtotal * (dc.Alicuota / 100)) + model.Items.Sum(dc => (dc.Subtotal2 ?? 0) * ((dc.Alicuota2 ?? 0D) / 100));
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
                            Debitos = 0,
                            Creditos = impuesto
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Crédito Fiscal IVA. Verifique.");
                }

				var percepciones = model.Items.GroupBy(pe => pe.IdCuentaContablePercepcion)
					.Select(g => new
					{
						g.FirstOrDefault().IdCuentaContablePercepcion,
						Importe = g.Sum(t => t.Percepcion ?? 0)
					});

                foreach (var percepcion in percepciones)
                {
                    if (percepcion.IdCuentaContablePercepcion != default)
                    {
                        var ctaPercepcion = await _cuentasContablesRepository.GetById<CuentaContable>(percepcion.IdCuentaContablePercepcion ?? 0, tran);

                        if (ctaPercepcion != default)
                        {
                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = ctaPercepcion.IdCuentaContable,
                                Detalle = ctaPercepcion.Descripcion,
                                Debitos = 0,
                                Creditos = percepcion.Importe
                            }, tran);
                        }
                        else
                            throw new BusinessException("No se ecnuentra la Cuenta Contable de Percepción. Verifique.");
                    }
				}

				var percepciones2 = model.Items.GroupBy(pe => pe.IdCuentaContablePercepcion2)
					.Select(g => new
					{
						g.FirstOrDefault().IdCuentaContablePercepcion2,
						Importe = g.Sum(t => t.Percepcion2 ?? 0)
					});

				foreach (var percepcion in percepciones2)
				{
                    if (percepcion.IdCuentaContablePercepcion2 != default)
                    {
                        var ctaPercepcion = await _cuentasContablesRepository.GetById<CuentaContable>(percepcion.IdCuentaContablePercepcion2 ?? 0, tran);

                        if (ctaPercepcion != default)
                        {
                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = ctaPercepcion.IdCuentaContable,
                                Detalle = ctaPercepcion.Descripcion,
                                Debitos = 0,
                                Creditos = percepcion.Importe
                            }, tran);
                        }
                        else
                            throw new BusinessException("No se ecnuentra la Cuenta Contable de Percepción. Verifique.");
                    }
				}

                var impuestosInternos = model.Items.Sum(ii => ii.ImpuestosInternos);
                if (impuestosInternos.HasValue && impuestosInternos > 0)
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
                            Debitos = 0,
                            Creditos = impuestosInternos.Value
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Impuestos Internos. Verifique.");
                }

                var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(dbPlanilla.IdCuentaContable, tran);
                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)idAsiento,
                    Item = indItem++,
                    IdCuentaContable = cuentaContable.IdCuentaContable,
                    Detalle = cuentaContable.Descripcion,
                    Debitos = 0,
                    Creditos = model.Items.Sum(pe => pe.Subtotal) + model.Items.Sum(pe => (pe.Subtotal2 ?? 0D)) + model.Items.Sum(pe => (pe.NoGravado ?? 0D))
                }, tran);

                await _planillaGastosAsientoRepository.Insert(new PlanillaGastoAsiento { IdPlanillaGastos = dbPlanilla.IdPlanillaGastos, IdAsiento = (int)idAsiento }, tran);

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

        private void Validate(PlanillaGastos planilla, List<PlanillaGastoItem> items)
        {
            if (planilla.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Descripción para la Planilla de Gastos.");
            }

            if (planilla.IdCuentaContable == 0)
            {
                throw new BusinessException("Seleccione una Cuenta Contable de Gastos.");
            }

            if (_ejerciciosRepository.EjercicioCerrado(planilla.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if(items.Count == 0)
            {
                throw new BusinessException("Ingrese al menos un Item de Gasto para la Planilla.");
            }

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.CUIT) && !item.CUIT.ValidarCUIT())
                {
                    throw new BusinessException($"El CUIT del Proveedor {item.Proveedor} es Incorrecto. Verifique.");
                }

                if(!string.IsNullOrEmpty(item.CAE) && !item.VencimientoCAE.HasValue)
                {
                    throw new BusinessException($"Ingrese una Fecha de Venticimiento de CAE para el Comprobante {item.NumeroComprobante}.");
                }

                if(item.IdCuentaContablePercepcion.HasValue)
                {
                    if(!item.Percepcion.HasValue)
                        throw new BusinessException($"Ingrese una Importe para la Percepción del Comprobante {item.NumeroComprobante}.");
                }

                if (item.IdCuentaContablePercepcion2.HasValue)
                {
                    if (!item.Percepcion2.HasValue)
                        throw new BusinessException($"Ingrese una Importe para la Percepción 2 del Comprobante {item.NumeroComprobante}.");
                }

                if (_ejerciciosRepository.ValidateFechaEjercicio(planilla.IdEjercicio, item.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
                {
                    throw new BusinessException($"La fecha del Comprobante {item.NumeroComprobante} no corresponde con el Ejercicio Seleccionado. Verifique.");
                }

                if (_cierreMesRepository.MesCerrado(planilla.IdEjercicio, item.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
                {
                    throw new BusinessException("El Mes seleccionado para la Fecha del Comprobante esta Cerrado. Verifique.");
                }
            }
        }

        public async Task<List<Custom.Select2Custom>> GetItemsGastos(string annoMes, int numero, string moneda)
        {
            int.TryParse(annoMes.Split('/')[0], out int mes);
            int.TryParse(annoMes.Split('/')[1], out int anno);

            return await _planillaGastosRepository.GetItemsGastos(anno, mes, numero, moneda, _permissionsBusiness.Value.User.IdEmpresa);
        }

        //public async Task<double> GetSaldoItemGastos(string annoMes, int numero, int item)
        //{
        //    int.TryParse(annoMes.Split('/')[0], out int mes);
        //    int.TryParse(annoMes.Split('/')[1], out int anno);

        //    return await _planillaGastosRepository.GetSaldoItemGastos(anno, mes, numero, item, _permissionsBusiness.Value.User.IdEmpresa);
        //}

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {

            var asiento = await _planillaGastosAsientoRepository.GetByIdPlanillaGastos(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);

        }

        public async Task<List<PlanillaGastosDetalle>> ProcesarExcel(IFormFile file)
        {
            var resultados = new List<PlanillaGastosDetalle>();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<PlanillaGastosXLSViewModel>();

                var comprobantes = (await _lookupsBusiness.Value.GetAllComprobantes()).ToList();
                var alicuotas = (await _lookupsBusiness.Value.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA)
                    .ToDictionary(a => a.Valor, a => a.IdAlicuota);

                var iItem = 1;
                foreach (var item in excelRows)
                {
                    var proveedor = await _lookupsBusiness.Value.GetProveedorByCUIT(item.CUIT, _permissionsBusiness.Value.User.IdEmpresa);
                    var idComprobante = comprobantes.FirstOrDefault(f => f.Descripcion == item.Comprobante)?.IdComprobante;

                    var numero = string.Empty;
                    try
                    {
                        var splittedNumero = item.Numero.Split("-");
                        numero = string.Format("{0}-{1}", splittedNumero[0].PadLeft(5, '0'), splittedNumero[1].PadLeft(8, '0'));
                    }
                    catch (Exception ex)
                    {
                        //Nada...
                    }

                    var idAlicuota = alicuotas.GetValueOrDefault(item.Alicuota);
                    var idAlicuota2 = alicuotas.GetValueOrDefault(item.Alicuota2);

                    resultados.Add(new PlanillaGastosDetalle
                    {
                        Item = iItem,
                        CUIT = proveedor?.CUIT ?? item.CUIT,
                        Proveedor = proveedor?.NombreFantasia,
                        Fecha = item.Fecha.ToDateString("yyyy-MM-dd"),
                        IdComprobante = idComprobante ?? 0,
                        NumeroComprobante = numero,
                        Detalle = item.Detalle,
                        NoGravado = item.NoGravado,
                        Subtotal = item.Subtotal,
                        IdAlicuota = idAlicuota,
                        Alicuota = item.Alicuota,
                        Subtotal2 = item.Subtotal2,
                        IdAlicuota2 = idAlicuota2,
                        Alicuota2 = item.Alicuota2,
                        IdCuentaContablePercepcion = default,
                        Percepcion = item.Percepcion,
                        IdCuentaContablePercepcion2 = default,
                        Percepcion2 = item.Percepcion2,
                        Total = item.Total,
                        CAE = item.CAE,
                        VencimientoCAE = item.VencimientoCAE == DateTime.MinValue ? string.Empty : item.VencimientoCAE.ToDateString("yyyy-MM-dd")
                    });
                    iItem++;
                }

                if (resultados == null || resultados.Count == 0)
                    throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

                return resultados;
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