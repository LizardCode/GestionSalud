using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.OrdenesPago;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class OrdenesPagoBusiness: BaseBusiness, IOrdenesPagoBusiness
    {
        private readonly ILogger<OrdenesPagoBusiness> _logger;
        private readonly IOrdenesPagoAsientoRepository _ordenesPagoAsientoRepository;
        private readonly IOrdenesPagoRepository _ordenesPagoRepository;
        private readonly IOrdenesPagoComprobantesRepository _ordenesPagoComprobantesRepository;
        private readonly IOrdenesPagoDetalleRepository _ordenesPagoDetalleRepository;
        private readonly IOrdenesPagoPlanillaGastosRepository _ordenesPagoPlanillaGastosRepository;
        private readonly IOrdenesPagoRetencionesRepository _ordenesPagoRetencionesRepository;
        private readonly IOrdenesPagoAnticiposRepository _ordenesPagoAnticiposRepository;
        private readonly IPlanillaGastosRepository _planillaGastosRepository;
        private readonly IChequesRepository _chequesRepository;
        private readonly ITransferenciasRepository _transferenciasRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IProveedoresRepository _proveedoresRepository;
        private readonly ICodigosRetencionGananciasRepository _codigosRetencionGananciasRepository;
        private readonly ICodigosRetencionGananciasItemsRepository _codigosRetencionGananciasItemsRepository;
        private readonly ICodigosRetencionIVARepository _codigosRetencionIVARepository;
        private readonly ICodigosRetencionIngresosBrutosRepository _codigosRetencionIngresosBrutosRepository;
        private readonly ICodigosRetencionSussRepository _codigosRetencionSussRepository;
        private readonly ICodigosRetencionMonotributoRepository _codigosRetencionMonotributoRepository;
        private readonly IComprobantesComprasTotalesRepository _comprobantesComprasTotalesRepository;
        private readonly IAGIPRepository _AGIPRepository;
        private readonly IARBARepository _ARBARepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public OrdenesPagoBusiness(
            ILogger<OrdenesPagoBusiness> logger,
            IOrdenesPagoAsientoRepository ordenesPagoAsientoRepository,
            IOrdenesPagoRepository ordenesPagoRepository,
            IOrdenesPagoComprobantesRepository ordenesPagoComprobantesRepository,
            IOrdenesPagoDetalleRepository ordenesPagoDetalleRepository,
            IOrdenesPagoRetencionesRepository ordenesPagoRetencionesRepository,
            IOrdenesPagoPlanillaGastosRepository ordenesPagoPlanillaGastosRepository,
            IOrdenesPagoAnticiposRepository ordenesPagoAnticiposRepository,
            IPlanillaGastosRepository planillaGastosRepository,
            IChequesRepository chequesRepository,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            ITransferenciasRepository transferenciasRepository,
            IProveedoresRepository proveedoresRepository,
            IBancosRepository bancosRepository,
            ICodigosRetencionGananciasRepository codigosRetencionGananciasRepository,
            ICodigosRetencionGananciasItemsRepository codigosRetencionGananciasItemsRepository,
            ICodigosRetencionIVARepository codigosRetencionIVARepository,
            ICodigosRetencionIngresosBrutosRepository codigosRetencionIngresosBrutosRepository,
            ICodigosRetencionSussRepository codigosRetencionSussRepository,
            ICodigosRetencionMonotributoRepository codigosRetencionMonotributoRepository,
            IAGIPRepository AGIPRepository,
            IARBARepository ARBARepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository
            )
        {
            _logger = logger;
            _ordenesPagoRepository = ordenesPagoRepository;
            _ordenesPagoAsientoRepository = ordenesPagoAsientoRepository;
            _ordenesPagoComprobantesRepository = ordenesPagoComprobantesRepository;
            _ordenesPagoDetalleRepository = ordenesPagoDetalleRepository;
            _ordenesPagoRetencionesRepository = ordenesPagoRetencionesRepository;
            _ordenesPagoAnticiposRepository = ordenesPagoAnticiposRepository;
            _ordenesPagoPlanillaGastosRepository = ordenesPagoPlanillaGastosRepository;
            _planillaGastosRepository = planillaGastosRepository;
            _chequesRepository = chequesRepository;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _bancosRepository = bancosRepository;
            _transferenciasRepository = transferenciasRepository;
            _proveedoresRepository = proveedoresRepository;
            _codigosRetencionGananciasRepository = codigosRetencionGananciasRepository;
            _codigosRetencionGananciasItemsRepository = codigosRetencionGananciasItemsRepository;
            _codigosRetencionIVARepository = codigosRetencionIVARepository;
            _codigosRetencionIngresosBrutosRepository = codigosRetencionIngresosBrutosRepository;
            _codigosRetencionSussRepository = codigosRetencionSussRepository;
            _codigosRetencionMonotributoRepository = codigosRetencionMonotributoRepository;
            _comprobantesComprasTotalesRepository = comprobantesComprasTotalesRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _AGIPRepository = AGIPRepository;
            _ARBARepository = ARBARepository;
        }

        public async Task<OrdenesPagoViewModel> Get(int idOrdenPago)
        {
            var ordenPago = await _ordenesPagoRepository.GetById<OrdenPago>(idOrdenPago);

            if (ordenPago == null)
                return null;

            var items = await _ordenesPagoDetalleRepository.GetAllByIdOrdenPago(idOrdenPago);
            var comprobantes = await _ordenesPagoComprobantesRepository.GetComprobantesByIdOrdenPago(idOrdenPago);
            var retenciones = await _ordenesPagoRetencionesRepository.GetCustomByIdOrdenPago(idOrdenPago);
            var anticipos = await _ordenesPagoAnticiposRepository.GetByIdOrdenPago(idOrdenPago, ordenPago.IdProveedor ?? 0);

            var model = _mapper.Map<OrdenesPagoViewModel>(ordenPago);
            model.Imputaciones = _mapper.Map<List<OrdenesPagoImputacion>>(comprobantes);
            model.Retenciones = _mapper.Map<List<OrdenesPagoRetencion>>(retenciones);
            if (anticipos != default)
            {
                model.Anticipos = _mapper.Map<List<OrdenesPagoAnticipo>>(anticipos);
                model.ImporteAnticipo = model.Importe;
            }
            model.Items = items.Select(item => new OrdenesPagoDetalle
            {
                IdBancoChequeComun = item.IdCheque,
                BancoChequeComun = item.BancoCheque,
                FechaChequeComun = item.FechaEmision,
                NumeroChequeComun = item.NumeroCheque,
                IdBancoEChequeComun = item.IdCheque,
                BancoEChequeComun = item.BancoCheque,
                FechaEChequeComun = item.FechaEmision,
                NumeroEChequeComun = item.NumeroCheque,
                IdBancoChequeDiferido = item.IdCheque,
                BancoChequeDiferido = item.BancoCheque,
                FechaChequeDiferido = item.FechaEmision,
                FechaDiferidoChequeDiferido = item.FechaVto,
                NumeroChequeDiferido = item.NumeroCheque,
                IdBancoEChequeDiferido = item.IdCheque,
                BancoEChequeDiferido = item.BancoCheque,
                FechaEChequeDiferido = item.FechaEmision,
                FechaDiferidoEChequeDiferido = item.FechaVto,
                NumeroEChequeDiferido = item.NumeroCheque,
                IdBancoTranferencia = item.IdCheque,
                BancoTranferencia = item.BancoCheque,
                FechaTransferencia = item.FechaEmision,
                NumeroTransferencia = item.NumeroTransferencia,
                Importe = item.Importe
            }).ToList();

            return model;
        }

        public async Task<DataTablesResponse<Custom.OrdenPago>> GetAll(DataTablesRequest request)
        {
            var customQuery = _ordenesPagoRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEstadoOrdenPago"))
                builder.Append($"AND IdEstadoOrdenPago = {filters["IdEstadoOrdenPago"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdProveedor"))
                builder.Append($"AND IdProveedor = {filters["IdProveedor"]}");

            if (filters.ContainsKey("NumeroOrdenPago"))
                builder.Append($"AND IdOrdenPago = {filters["NumeroOrdenPago"]}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.OrdenPago>(request, customQuery.Sql, customQuery.Parameters, true,staticWhere: builder.Sql);
        }

        public async Task New(OrdenesPagoViewModel model)
        {
            var ordenPago = _mapper.Map<OrdenPago>(model);

            Validate(model);

            model.Cotizacion = model.Cotizacion == 0 ? 1 : model.Cotizacion;

            var tran = _uow.BeginTransaction();

            try
            {
                ordenPago.Descripcion = ordenPago.Descripcion.ToUpper().Trim();
                ordenPago.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                ordenPago.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                switch((int)ordenPago.IdTipoOrdenPago)
                {
                    case (int)TipoOrdenPago.Proveedores:
                        ordenPago.Importe = model.Imputaciones.Sum(i => i.Importe) - (model.Anticipos?.Sum(a => a.Importe) ?? 0);
                        break;
                    case (int)TipoOrdenPago.Gastos:
                        ordenPago.Importe = model.PlanillasGastos.Where(p => p.Seleccionar).Sum(i => i.Importe);
                        break;
                    case (int)TipoOrdenPago.Anticipo:
                        ordenPago.Importe = model.ImporteAnticipo;
                        break;
                    case (int)TipoOrdenPago.Varios:
                        ordenPago.IdCuentaContable = model.IdCuentaContable;
                        ordenPago.Importe = model.ImporteVarios;
                        break;
                }
                ordenPago.Moneda = model.IdMoneda;
                ordenPago.MonedaPago = model.IdMonedaPago;
                ordenPago.Cotizacion = model.Cotizacion;
                ordenPago.IdUsuario = _permissionsBusiness.Value.User.Id;
                ordenPago.FechaIngreso = DateTime.Now;
                ordenPago.IdEstadoOrdenPago = (int)EstadoOrdenPago.Ingresado;
                if (ordenPago.Importe == 0)
                    ordenPago.IdEstadoOrdenPago = (int)EstadoOrdenPago.Pagado;
                ordenPago.Total = ordenPago.Importe;

                var id = await _ordenesPagoRepository.Insert(ordenPago, tran);

                switch (ordenPago.IdTipoOrdenPago)
                {
                    case (int)TipoOrdenPago.Varios:
                    case (int)TipoOrdenPago.Anticipo:
                        break;
                    case (int)TipoOrdenPago.Proveedores:

                        var imputaciones = model.Imputaciones.Where(imp => Math.Abs(imp.Importe) > 0);
                        foreach (var imputacion in imputaciones)
                        {
                            if (imputacion.Importe != 0)
                            {
                                await _ordenesPagoComprobantesRepository.Insert(new OrdenPagoComprobante
                                {
                                    IdOrdenPago = (int)id,
                                    IdComprobanteCompra = imputacion.IdComprobanteCompra,
                                    Importe = imputacion.Importe,
                                    Cotizacion = imputacion.Cotizacion
                                }, tran);
                            }
                        }

                        if (model.Anticipos?.Count > 0)
                        {
                            foreach (var anticipo in model.Anticipos)
                            {
                                if (anticipo.Importe > 0)
                                {
                                    await _ordenesPagoAnticiposRepository.Insert(new OrdenPagoAnticipo
                                    {
                                        IdOrdenPago = (int)id,
                                        IdAnticipo = anticipo.IdOrdenPago,
                                        Importe = anticipo.Importe
                                    }, tran);
                                }
                            }
                        }

                        #region Calculo de Retenciones

                        var totalImputaciones = model.Imputaciones.Sum(i => i.Importe);

                        if (totalImputaciones > 0)
                        {
                            totalImputaciones *= (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion);
                            var codigosRentecion = await _proveedoresRepository.GetCodigosRetencionByIdProveedor(ordenPago.IdProveedor.Value, tran);

                            foreach (var retencion in codigosRentecion)
                            {
                                switch (retencion.IdTipoRetencion)
                                {
                                    case (int)TipoRetencion.Ganancias:
                                        var ganancias = await _codigosRetencionGananciasRepository.GetById<CodigosRetencionGanancias>(retencion.IdCodigoRetencion, tran);
                                        List<Custom.ComprobanteCompraTotales> comprobantesGan;
                                        double? totalRetenidoMes;
                                        if (ganancias.AcumulaPagos)
                                        {
                                            comprobantesGan = await _comprobantesComprasTotalesRepository.GetTotalesByFechaPriUlt(ordenPago.Fecha, _permissionsBusiness.Value.User.IdEmpresa, ordenPago.IdProveedor ?? 0, tran);
                                            totalRetenidoMes = (await _ordenesPagoRepository.GetTotalRetenido((int)TipoRetencion.Ganancias, ordenPago.Fecha, ordenPago.IdProveedor.Value, _permissionsBusiness.Value.User.IdEmpresa, tran)) ?? 0;
                                        }
                                        else
                                        {
                                            comprobantesGan = await _comprobantesComprasTotalesRepository.GetTotalesByIdOrdenPago(_permissionsBusiness.Value.User.IdEmpresa, (int)id, tran);
                                            totalRetenidoMes = 0D;
                                        }
                                        var baseRetencionGanancias = 0D;

                                        foreach (var comprobante in comprobantesGan)
                                        {
                                            if (Math.Round(comprobante.Neto + comprobante.Impuestos, 2) != Math.Abs(Math.Round(comprobante.ImportePagoTotal, 2)))
                                            {
                                                if (comprobante.EsCredito)
                                                    baseRetencionGanancias -= Math.Abs(Math.Round(comprobante.ImportePagoTotal, 2));
                                                else
                                                    baseRetencionGanancias += Math.Round(comprobante.ImportePagoTotal, 2);
                                            }
                                            else
                                            {
                                                if (comprobante.EsCredito)
                                                    baseRetencionGanancias -= Math.Round(comprobante.Neto, 2);
                                                else
                                                    baseRetencionGanancias += Math.Round(comprobante.Neto, 2);
                                            }
                                        }

                                        if (baseRetencionGanancias > ganancias.ImporteNoSujeto)
                                        {
                                            baseRetencionGanancias -= ganancias.ImporteNoSujeto;

                                            var gananciasItem = await _codigosRetencionGananciasItemsRepository.GetByImporteDesdeHasta(ganancias.IdCodigoRetencion, baseRetencionGanancias, tran);

                                            if (gananciasItem != null)
                                            {
                                                var importeRetencionGanancias = gananciasItem.ImporteRetencion + (((baseRetencionGanancias - gananciasItem.SobreExcedente) * gananciasItem.MasPorcentaje) / 100) - (totalRetenidoMes ?? 0D);
                                                if (importeRetencionGanancias > ganancias.ImporteMinimoRetencion)
                                                {
                                                    await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                                    {
                                                        IdOrdenPago = ordenPago.IdOrdenPago,
                                                        BaseImponible = baseRetencionGanancias,
                                                        Fecha = DateTime.Now,
                                                        IdTipoRetencion = (int)TipoRetencion.Ganancias,
                                                        Importe = importeRetencionGanancias / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                                    });
                                                }
                                            }
                                        }
                                        break;
                                    case (int)TipoRetencion.IVA:
                                        var iva = await _codigosRetencionIVARepository.GetById<CodigosRetencionIVA>(retencion.IdCodigoRetencion, tran);
                                        var comprobantesIva = await _comprobantesComprasTotalesRepository.GetTotalesByIdOrdenPago(_permissionsBusiness.Value.User.IdEmpresa, (int)id, tran);
                                        var baseRetencionIva = 0D;

                                        foreach (var comprobante in comprobantesIva)
                                        {
                                            if (comprobante.EsCredito)
                                                baseRetencionIva -= comprobante.Impuestos;
                                            else
                                                baseRetencionIva += comprobante.Impuestos;
                                        }

                                        if (baseRetencionIva > iva.ImporteNoSujeto)
                                        {
                                            await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                            {
                                                IdOrdenPago = ordenPago.IdOrdenPago,
                                                BaseImponible = baseRetencionIva,
                                                Fecha = DateTime.Now,
                                                IdTipoRetencion = (int)TipoRetencion.IVA,
                                                Importe = (baseRetencionIva * iva.PorcentajeRetencion / 100) / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                            });
                                        }

                                        break;
                                    case (int)TipoRetencion.IngresosBrutos:
                                        var iibr = await _codigosRetencionIngresosBrutosRepository.GetById<CodigosRetencionIngresosBrutos>(retencion.IdCodigoRetencion, tran);
                                        var comprobantesIIbr = await _comprobantesComprasTotalesRepository.GetTotalesByIdOrdenPago(_permissionsBusiness.Value.User.IdEmpresa, (int)id, tran);
                                        var baseRetencionIIbr = 0D;

                                        foreach (var comprobante in comprobantesIIbr)
                                        {
                                            if (comprobante.EsCredito)
                                                baseRetencionIIbr -= comprobante.Neto;
                                            else
                                                baseRetencionIIbr += comprobante.Neto;
                                        }

                                        if (iibr.PadronRetencionARBA || iibr.PadronRetencionAGIP)
                                        {
                                            var proveedor = await _proveedoresRepository.GetById<Proveedor>(ordenPago.IdProveedor ?? 0, tran);

                                            if (iibr.PadronRetencionARBA)
                                            {
                                                if (proveedor != default) 
                                                {
                                                    var alicuotaReten = await _ARBARepository.GetByCUITFechaVig("R", proveedor.CUIT.Replace("-", string.Empty), ordenPago.Fecha);
                                                    if(alicuotaReten != default)
                                                    {
                                                        await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                                        {
                                                            IdOrdenPago = ordenPago.IdOrdenPago,
                                                            BaseImponible = baseRetencionIIbr,
                                                            Fecha = DateTime.Now,
                                                            IdTipoRetencion = (int)TipoRetencion.IngresosBrutos,
                                                            Importe = (baseRetencionIIbr * alicuotaReten.Alicuota / 100) / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                                        });
                                                    }
                                                }
                                            }

                                            if (iibr.PadronRetencionAGIP)
                                            {
                                                if (proveedor != default)
                                                {
                                                    var alicuotaReten = await _AGIPRepository.GetByCUITFechaVig(proveedor.CUIT.Replace("-", string.Empty), ordenPago.Fecha);
                                                    if (alicuotaReten != default)
                                                    {
                                                        await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                                        {
                                                            IdOrdenPago = ordenPago.IdOrdenPago,
                                                            BaseImponible = baseRetencionIIbr,
                                                            Fecha = DateTime.Now,
                                                            IdTipoRetencion = (int)TipoRetencion.IngresosBrutos,
                                                            Importe = (baseRetencionIIbr * alicuotaReten.AliRetencion / 100) / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (baseRetencionIIbr > iibr.ImporteNoSujeto)
                                            {
                                                await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                                {
                                                    IdOrdenPago = ordenPago.IdOrdenPago,
                                                    BaseImponible = baseRetencionIIbr,
                                                    Fecha = DateTime.Now,
                                                    IdTipoRetencion = (int)TipoRetencion.IngresosBrutos,
                                                    Importe = (baseRetencionIIbr * iibr.PorcentajeRetencion / 100) / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                                });
                                            }
                                        }
                                        break;
                                    case (int)TipoRetencion.SUSS:
                                        var suss = await _codigosRetencionSussRepository.GetById<CodigosRetencionSUSS>(retencion.IdCodigoRetencion, tran);
                                        var comprobantesSuss = await _comprobantesComprasTotalesRepository.GetTotalesByIdOrdenPago(_permissionsBusiness.Value.User.IdEmpresa, (int)id, tran);
                                        var baseRetencionSuss = 0D;

                                        foreach (var comprobante in comprobantesSuss)
                                        {
                                            if (comprobante.EsCredito)
                                                baseRetencionSuss -= comprobante.Neto;
                                            else
                                                baseRetencionSuss += comprobante.Neto;
                                        }

                                        if (baseRetencionSuss > suss.ImporteNoSujeto)
                                        {
                                            await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                            {
                                                IdOrdenPago = ordenPago.IdOrdenPago,
                                                BaseImponible = baseRetencionSuss,
                                                Fecha = DateTime.Now,
                                                IdTipoRetencion = (int)TipoRetencion.IVA,
                                                Importe = (baseRetencionSuss * suss.PorcentajeRetencion / 100) / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                            });
                                        }
                                        break;
                                    case (int)TipoRetencion.IVAMonotributo:
                                    case (int)TipoRetencion.GananciasMonotributo:
                                        var mono = await _codigosRetencionSussRepository.GetById<CodigosRetencionMonotributo>(retencion.IdCodigoRetencion, tran);
                                        var comprobantesMono = await _comprobantesComprasTotalesRepository.GetTotalesByFechaMonotributo(DateTime.Now, mono.CantidadMeses, _permissionsBusiness.Value.User.IdEmpresa, ordenPago.IdProveedor.Value, tran);
                                        var baseRetencionMono = 0D;

                                        foreach (var comprobante in comprobantesMono)
                                        {
                                            if (comprobante.EsCredito)
                                                baseRetencionMono -= comprobante.Neto + comprobante.Impuestos;
                                            else
                                                baseRetencionMono += comprobante.Neto + comprobante.Impuestos;
                                        }

                                        if (baseRetencionMono > mono.ImporteNoSujeto)
                                        {
                                            await _ordenesPagoRetencionesRepository.Insert(new OrdenPagoRetencion
                                            {
                                                IdOrdenPago = ordenPago.IdOrdenPago,
                                                BaseImponible = baseRetencionMono,
                                                Fecha = DateTime.Now,
                                                IdTipoRetencion = retencion.IdTipoRetencion,
                                                Importe = (baseRetencionMono * mono.PorcentajeRetencion / 100) / (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                                            });
                                        }
                                        break;
                                }
                            }

                            var retenciones = await _ordenesPagoRetencionesRepository.GetAllByIdOrdenPago((int)id, tran);
                            if (retenciones.Count > 0)
                            {
                                ordenPago.IdOrdenPago = (int)id;
                                ordenPago.Total = ordenPago.Importe;
                                ordenPago.Importe = ordenPago.Importe - (retenciones?.Sum(r => r.Importe) ?? 0);

                                if (ordenPago.Importe < 0)
                                    throw new BusinessException("Error en la composición del Pago y las Retenciones. El monto de la Retención Supera el Importe de la Orden de Pago. Verifique.");

                                await _ordenesPagoRepository.Update(ordenPago, tran);
                            }
                        }

                        #endregion

                        break;

                    case (int)TipoOrdenPago.Gastos:
                        foreach (var planilla in model.PlanillasGastos)
                        {
                            if (planilla.Seleccionar)
                            {
                                await _ordenesPagoPlanillaGastosRepository.Insert(new OrdenPagoPlanillaGasto
                                {
                                    IdOrdenPago = (int)id,
                                    IdPlanillaGastos = planilla.IdPlanillaGastos
                                }, tran);

                                var planillaGastos = await _planillaGastosRepository.GetById<PlanillaGastos>(planilla.IdPlanillaGastos);
                                planillaGastos.IdEstadoPlanilla = (int)EstadoPlanillaGastos.Pagada;

                                await _planillaGastosRepository.Update(planillaGastos, tran);
                            }
                        }
                        break;
                }

                #region Asiento Contable - Solo para Ordenes de Pago con Anticipo o N/Credito en 0

                if (ordenPago.Importe == 0 && model.Anticipos?.Count > 0)
                {
                    var asiento = new Asiento
                    {
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        IdEjercicio = ordenPago.IdEjercicio,
                        Descripcion = ordenPago.Descripcion,
                        Fecha = ordenPago.Fecha,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        FechaIngreso = DateTime.Now
                    };

                    var idAsiento = await _asientosRepository.Insert(asiento, tran);

                    var indItem = 1;
                    CuentaContable cuentaContable;
                    cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES, tran);
                    if (cuentaContable == default)
                        throw new BusinessException("No Existe la Cuenta Contable Integradora de Proveedores. Verifique.");

                    var imputacionesNotasCredito = model.Imputaciones.Where(i => i.Importe < 0).ToList();
                    var imputaciones = model.Imputaciones.Where(i => i.Importe > 0);

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = (imputaciones.Sum(i => i.Importe) - imputacionesNotasCredito.Sum(i => Math.Abs(i.Importe)) * (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)),
                        Creditos = 0
                    }, tran);

                    if (model.Anticipos?.Count > 0)
                    {
                        cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.ANTICIPOS_PROVEEDORES, tran);
                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable Anticipo a Proveedores. Verifique.");

                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = 0,
                            Creditos = model.Anticipos.Sum(a => a.Importe) * (model.IdMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : model.Cotizacion)
                        }, tran);
                        
                    }

                    await _ordenesPagoAsientoRepository.Insert(new OrdenPagoAsiento
                    {
                        IdAsiento = (int)idAsiento,
                        IdOrdenPago = ordenPago.IdOrdenPago
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

        public async Task Remove(int idOrdenPago)
        {
            var ordenPago = await _ordenesPagoRepository.GetById<OrdenPago>(idOrdenPago);

            if (ordenPago == null)
            {
                throw new BusinessException("Número de Orden de Pago Inexistente");
            }

            if (_cierreMesRepository.MesCerrado(ordenPago.IdEjercicio ?? 0, ordenPago.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("La Orden de Pago se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var ordenPagoAsiento = await _ordenesPagoAsientoRepository.GetByIdOrdenPago(idOrdenPago, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(ordenPagoAsiento?.IdAsiento ?? 0, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(ordenPagoAsiento?.IdAsiento ?? 0, tran);
                await _ordenesPagoAsientoRepository.DeleteByIdOrdenPago(idOrdenPago, tran);

                if (asiento != null)
                {
                    asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _asientosRepository.Update(asiento, tran);
                }

                if (ordenPago.Importe == 0)
                    ordenPago.IdEstadoOrdenPago = (int)EstadoOrdenPago.Ingresado;

                switch (ordenPago.IdEstadoOrdenPago)
                {
                    case (int)EstadoOrdenPago.Pagado:

                        var ordenPagoDetalle = await _ordenesPagoDetalleRepository.GetAllByIdOrdenPago(idOrdenPago, tran);
                        await _ordenesPagoDetalleRepository.DeleteByIdOrdenPago(idOrdenPago, tran);
                        foreach (var detalle in ordenPagoDetalle)
                        {
                            if (detalle.IdCheque.HasValue)
                            {
                                var cheque = await _chequesRepository.GetById<Cheque>(detalle.IdCheque.Value, tran);
                                if(cheque.IdTipoCheque == (int)TipoCheque.Terceros)
                                {
                                    cheque.IdEstadoCheque = (int)EstadoCheque.EnCartera;
                                    cheque.Firmante = "";
                                    cheque.CUITFirmante = "";
                                    cheque.Endosante1 = "";
                                    cheque.CUITEndosante1 = "";
                                    cheque.Endosante2 = "";
                                    cheque.CUITEndosante2 = "";
                                    cheque.Endosante3 = "";
                                    cheque.CUITEndosante3 = "";
                                }
                                else
                                {
                                    cheque.IdEstadoCheque = (int)EstadoCheque.SinLibrar;
                                    cheque.Importe = 0D;
                                }

                                cheque.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                                cheque.FechaEmision = default;
                                cheque.FechaVto = default;

                                await _chequesRepository.Update(cheque, tran);
                            }
                            if (detalle.IdTransferencia.HasValue)
                            {
                                await _transferenciasRepository.DeleteById(detalle.IdTransferencia.Value, _permissionsBusiness.Value.User.IdEmpresa, tran);
                            }
                        }

                        ordenPago.IdEstadoOrdenPago = (int)EstadoOrdenPago.Ingresado;
                        await _ordenesPagoRepository.Update(ordenPago, tran);

                        break;

                    case (int)EstadoOrdenPago.Ingresado:

                        var planillasGastos = await _ordenesPagoPlanillaGastosRepository.GetPlanillasGastosByIdOrdenPago(idOrdenPago, tran);

                        foreach(var planilla in planillasGastos)
                        {
                            var planillaGastos = await _planillaGastosRepository.GetById<PlanillaGastos>(planilla.IdPlanillaGastos);
                            planillaGastos.IdEstadoPlanilla = (int)EstadoPlanillaGastos.Ingresada;

                            await _planillaGastosRepository.Update(planillaGastos, tran);
                        }

                        //Borro Detalle Previo y Analizar Detalle
                        await _ordenesPagoAnticiposRepository.DeleteByIdOrdenPago(idOrdenPago, tran);
                        await _ordenesPagoComprobantesRepository.DeleteByIdOrdenPago(idOrdenPago, tran);
                        await _ordenesPagoPlanillaGastosRepository.DeleteByIdOrdenPago(idOrdenPago, tran);
                        await _ordenesPagoRetencionesRepository.DeleteByIdOrdenPago(idOrdenPago, tran);

                        ordenPago.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                        await _ordenesPagoRepository.Update(ordenPago, tran);

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

        private void Validate(OrdenesPagoViewModel ordenPago)
        {
            if (ordenPago.IdMoneda != ordenPago.IdMonedaPago)
            {
                if (ordenPago.IdMonedaPago != Monedas.MonedaLocal.Description())
                {
                    throw new BusinessException("Error en la composición de Monedas. Verifique las Monedas de los Comprobantes y la Orden de Pago.");
                }
            }

            switch(ordenPago.IdTipoOrdenPago)
            {
                case (int)TipoOrdenPago.Varios:
                    if (ordenPago.ImporteVarios <= 0)
                    {
                        throw new BusinessException("La Orden de Pago Varios tiene que tener Mayor a 0 (Cero).");
                    }

                    if (!ordenPago.IdCuentaContable.HasValue)
                    {
                        throw new BusinessException("Ingrese una Cuenta Contable para la Orden de Pago Varios.");
                    }
                    break;
                case (int)TipoOrdenPago.Anticipo:
                    if (ordenPago.ImporteAnticipo < 0)
                    {
                        throw new BusinessException("La Orden de Pago de Anticipo tiene que tener Mayor a 0 (Cero).");
                    }
                    break;
                case (int)TipoOrdenPago.Proveedores:
                    if (ordenPago.Imputaciones == null || ordenPago.Imputaciones.Count == 0)
                    {
                        throw new BusinessException("Seleccione al menos un Comprobante para la Orden de Pago.");
                    }

                    if(ordenPago.Imputaciones.Sum(i => i.Importe) - (ordenPago.Anticipos?.Sum(a => a.Importe) ?? 0) < 0)
                    {
                        throw new BusinessException("La Orden de Pago tiene que tener Saldo Positivo.");
                    }

                    if (ordenPago.Anticipos?.Count > 0)
                    {
                        foreach (var anticipo in ordenPago.Anticipos)
                        {
                            if (anticipo.Importe > anticipo.Saldo)
                                throw new BusinessException("Error en la Imputación de Saldos de Anticipos. Verifique.");
                        }
                    }
                    break;

                case (int)TipoOrdenPago.Gastos:
                    if (ordenPago.PlanillasGastos == null || ordenPago.PlanillasGastos.Count == 0 || !ordenPago.PlanillasGastos.Any(s=> s.Seleccionar))
                    {
                        throw new BusinessException("Seleccione al menos una Planilla de Gastos para la Orden de Pago.");
                    }
                    break;
            }

            if (_ejerciciosRepository.EjercicioCerrado(ordenPago.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(ordenPago.IdEjercicio, ordenPago.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha de la Orden de Pago no es Válida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(ordenPago.IdEjercicio, ordenPago.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha de la Orden de Pago esta Cerrado. Verifique.");
            }
        }

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {
            var asiento = await _ordenesPagoAsientoRepository.GetByIdOrdenPago(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
        }

        public async Task<IList<Custom.OrdenPagoDetalle>> ObtenerDetallePago(int id)
            => await _ordenesPagoDetalleRepository.GetAllByIdOrdenPago(id);
        

        public async Task<IList<Custom.OrdenPagoComprobanteImputar>> GetComprobantesImputar(int idProveedor, string idMoneda, string idMonedaPago, double cotizacion)
        {
            if(idMoneda != idMonedaPago && idMonedaPago != Monedas.MonedaLocal.Description())
                throw new BusinessException("La Moneda de Pago debe ser Igual que la Moneda Origen o Moneda Local.");

            if (idMoneda.Equals(idMonedaPago))
                cotizacion = 1;

            var comprobantes = (await _ordenesPagoComprobantesRepository.GetComprobantesImputar(idProveedor, idMoneda, _permissionsBusiness.Value.User.IdEmpresa))
                .Select(cc => new Custom.OrdenPagoComprobanteImputar
                {
                    Seleccionar = cc.Seleccionar,
                    IdComprobanteCompra = cc.IdComprobanteCompra,
                    Fecha = cc.Fecha.ToDateString(),
                    TipoComprobante = cc.TipoComprobante,
                    NumeroComprobante = cc.NumeroComprobante,
                    Importe = Math.Round(cc.EsCredito ? cc.Importe * cotizacion * -1 : cc.Importe * cotizacion, 2, MidpointRounding.AwayFromZero),
                    Saldo = Math.Round(cc.EsCredito ? cc.Saldo * cotizacion * -1 : cc.Saldo * cotizacion, 2, MidpointRounding.AwayFromZero),
                    Total = Math.Round(cc.EsCredito ? cc.Total * cotizacion * -1 : cc.Total * cotizacion, 2, MidpointRounding.AwayFromZero),
                    Cotizacion = idMonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : cc.Cotizacion
                })
                .ToList();

            return comprobantes;
        }

        public async Task<IList<Custom.OrdenPagoPlanillaGasto>> GetPlanillasImputar(string idMoneda, string idMonedaPago, double cotizacion)
        {
            if (idMoneda != idMonedaPago && idMonedaPago != Monedas.MonedaLocal.Description())
                throw new BusinessException("La Moneda de Pago debe ser Igual que la Moneda Origen o Moneda Local.");

            if (idMoneda.Equals(idMonedaPago))
                cotizacion = 1;

            var planillas = (await _ordenesPagoComprobantesRepository.GetPlanillasImputar(idMoneda, _permissionsBusiness.Value.User.IdEmpresa))
                .Select(pl => new Custom.OrdenPagoPlanillaGasto
                {
                    IdPlanillaGastos = pl.IdPlanillaGastos,
                    Seleccionar = pl.Seleccionar,
                    Fecha = pl.Fecha,
                    Descripcion = pl.Descripcion,
                    Importe = pl.Importe * cotizacion
                })
                .ToList();

            return planillas;
        }

        public async Task AddPagar(OrdenesPagoViewModel model)
        {
            var dbOrdenPago = await _ordenesPagoRepository.GetById<OrdenPago>(model.IdOrdenPago);
            var dbOrdenPagoReten = await _ordenesPagoRetencionesRepository.GetCustomByIdOrdenPago(model.IdOrdenPago);

            if (dbOrdenPago == null)
            {
                throw new BusinessException("Número de Orden de Pago Inexistente");
            }

            if (dbOrdenPago.IdEstadoOrdenPago == (int)EstadoOrdenPago.Pagado)
            {
                throw new BusinessException("La Orden de Pago ya se encuentra Pagada.");
            }

            var importeOrdenPago = Math.Truncate(Math.Round(dbOrdenPago.Importe, 2, MidpointRounding.AwayFromZero) * 100) / 100;
            var importeItems = Math.Truncate(Math.Round(model.Items.Sum(i => i.Importe), 2,
                                                                MidpointRounding.AwayFromZero) * 100) / 100;

            if (Math.Round(importeOrdenPago - importeItems, 2) > 0.01)
            {
                throw new BusinessException("La suma de Comprobantes Difiere con el Total de la Orden de Pago. Verifique.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbOrdenPago.IdEstadoOrdenPago = (int)EstadoOrdenPago.Pagado;

                await _ordenesPagoRepository.Update(dbOrdenPago, tran);

                #region Items de Forma de Pago

                foreach (var item in model.Items)
                {
                    switch (item.IdTipoPago)
                    {
                        case (int)TipoPago.Efectivo:
                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe
                            }, tran);
                            break;

                        case (int)TipoPago.ChequeComun:
                            var chequeComun = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeComun, item.IdBancoChequeComun.Value, (int)TipoCheque.Comun, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            chequeComun.FechaEmision = item.FechaChequeComun;
                            chequeComun.Importe = item.Importe;
                            chequeComun.IdEstadoCheque = (int)EstadoCheque.Librado;

                            await _chequesRepository.Update(chequeComun, tran);

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeComun.IdCheque
                            }, tran);
                            break;

                        case (int)TipoPago.EChequeComun:
                            var eChequeComun = await _chequesRepository.GetByNumeroCheque(item.NumeroEChequeComun, item.IdBancoEChequeComun.Value, (int)TipoCheque.E_ChequeComun, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            eChequeComun.FechaEmision = item.FechaChequeComun;
                            eChequeComun.Importe = item.Importe;
                            eChequeComun.IdEstadoCheque = (int)EstadoCheque.Librado;

                            await _chequesRepository.Update(eChequeComun, tran);

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = eChequeComun.IdCheque
                            }, tran);
                            break;

                        case (int)TipoPago.ChequeDiferido:
                            var chequeDiferido = await _chequesRepository.GetByNumeroCheque(item.NumeroChequeDiferido, item.IdBancoChequeDiferido.Value, (int)TipoCheque.Diferido, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            chequeDiferido.FechaEmision = item.FechaChequeDiferido;
                            chequeDiferido.FechaVto = item.FechaDiferidoChequeDiferido;
                            chequeDiferido.Importe = item.Importe;
                            chequeDiferido.IdEstadoCheque = (int)EstadoCheque.Librado;

                            await _chequesRepository.Update(chequeDiferido, tran);

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = chequeDiferido.IdCheque
                            }, tran);
                            break;

                        case (int)TipoPago.EChequeDiferido:
                            var eChequeDiferido = await _chequesRepository.GetByNumeroCheque(item.NumeroEChequeDiferido, item.IdBancoEChequeDiferido.Value, (int)TipoCheque.E_ChequeDiferido, _permissionsBusiness.Value.User.IdEmpresa, tran);

                            eChequeDiferido.FechaEmision = item.FechaEChequeDiferido;
                            eChequeDiferido.FechaVto = item.FechaDiferidoEChequeDiferido;
                            eChequeDiferido.Importe = item.Importe;
                            eChequeDiferido.IdEstadoCheque = (int)EstadoCheque.Librado;

                            await _chequesRepository.Update(eChequeDiferido, tran);

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCheque = eChequeDiferido.IdCheque
                            }, tran);
                            break;

                        case (int)TipoPago.ChequeTerceros:
                            var chequeTerceros = await _chequesRepository.GetById<Cheque>(item.IdChequeTerceros.Value, tran);

                            chequeTerceros.IdEstadoCheque = (int)EstadoCheque.Entregado;

                            await _chequesRepository.Update(chequeTerceros, tran);

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
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

                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdTransferencia = (int)idTransferencia
                            }, tran);
                            break;

                        case (int)TipoPago.CuentaContable:
                            await _ordenesPagoDetalleRepository.Insert(new OrdenPagoDetalle
                            {
                                IdOrdenPago = dbOrdenPago.IdOrdenPago,
                                IdTipoPago = item.IdTipoPago,
                                Descripcion = item.Descripcion.ToUpper().Trim(),
                                Importe = item.Importe,
                                IdCuentaContable = (int)item.IdCuentaContable
                            }, tran);
                            break;
                    }
                }

                #endregion

                #region Asiento Contable

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = dbOrdenPago.IdEjercicio,
                    Descripcion = dbOrdenPago.Descripcion,
                    Fecha = dbOrdenPago.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var retenciones = await _ordenesPagoRetencionesRepository.GetAllByIdOrdenPago(dbOrdenPago.IdOrdenPago, tran);
                var anticipos = await _ordenesPagoAnticiposRepository.GetAllByIdOrdenPago(dbOrdenPago.IdOrdenPago, tran);

                var indItem = 1;
                CuentaContable cuentaContable;
                switch (dbOrdenPago.IdTipoOrdenPago)
                {
                    case (int)TipoOrdenPago.Anticipo:
                        cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.ANTICIPOS_PROVEEDORES, tran);
                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable Anticipo a Proveedores. Verifique.");

                        break;
                    case (int)TipoOrdenPago.Varios:
                        cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(dbOrdenPago.IdCuentaContable ?? 0, tran);
                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable Anticipo a Proveedores. Verifique.");

                        break;

                    default:
                        cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_PROVEEDORES, tran);
                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable Integradora de Proveedores. Verifique.");
                        break;
                }

                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)idAsiento,
                    Item = indItem++,
                    IdCuentaContable = cuentaContable.IdCuentaContable,
                    Detalle = cuentaContable.Descripcion,
                    Debitos = ((model.Items.Sum(i => i.Importe) + (retenciones?.Sum(i => i.Importe) ?? 0) + (anticipos?.Sum(a => a.Importe) ?? 0)) * (dbOrdenPago.MonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : dbOrdenPago.Cotizacion)),
                    Creditos = 0
                }, tran);

                if (anticipos?.Count > 0)
                {
                    cuentaContable = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.ANTICIPOS_PROVEEDORES, tran);

                    foreach (var anticipo in anticipos)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = 0,
                            Creditos = anticipo.Importe * (dbOrdenPago.MonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : dbOrdenPago.Cotizacion)
                        }, tran);
                    }
                }


                foreach (var item in model.Items)
                {
                    cuentaContable = item.IdTipoPago switch
                    {
                        (int)TipoPago.Efectivo => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CAJA, tran),
                        (int)TipoPago.ChequeComun => await _cuentasContablesRepository.GetByIdBanco(item.IdBancoChequeComun.Value, _permissionsBusiness.Value.User.IdEmpresa, tran),
                        (int)TipoPago.EChequeComun => await _cuentasContablesRepository.GetByIdBanco(item.IdBancoEChequeComun.Value, _permissionsBusiness.Value.User.IdEmpresa, tran),
                        (int)TipoPago.ChequeDiferido => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CHEQUES_DIFERIDOS, tran),
                        (int)TipoPago.EChequeDiferido => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CHEQUES_DIFERIDOS, tran),
                        (int)TipoPago.Transferencia => await _cuentasContablesRepository.GetByIdBanco(item.IdBancoTranferencia.Value, _permissionsBusiness.Value.User.IdEmpresa, tran),
                        (int)TipoPago.ChequeTerceros => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.VALORES_A_DEPOSITAR, tran),
                        (int)TipoPago.CuentaContable => await _cuentasContablesRepository.GetById<CuentaContable>(item.IdCuentaContable.Value, tran),
                        _ => throw new BusinessException("No Existe la Cuenta Contable para el Tipo de Pago Seleccionado. Verifique.")
                    };

                    if (cuentaContable == default)
                        throw new BusinessException("No Existe la Cuenta Contable para el Tipo de Pago Seleccionado. Verifique.");

                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = 0,
                        Creditos = item.Importe * (dbOrdenPago.MonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : dbOrdenPago.Cotizacion)
                    }, tran);
                }

                if (retenciones.Count > 0) {
                    foreach (var retencion in retenciones)
                    {
                        cuentaContable = retencion.IdTipoRetencion switch
                        {
                            (int)TipoRetencion.Ganancias => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_GANANCIAS, tran),
                            (int)TipoRetencion.IVA => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_IVA, tran),
                            (int)TipoRetencion.IngresosBrutos => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_ING_BRUTOS, tran),
                            (int)TipoRetencion.SUSS => await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.RETENCION_SUSS, tran),
                            _ => throw new BusinessException("No Existe la Cuenta Contable para la Retención Seleccionada. Verifique.")
                        };

                        if (cuentaContable == default)
                            throw new BusinessException("No Existe la Cuenta Contable para la Retención Seleccionada. Verifique.");
    
                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = cuentaContable.IdCuentaContable,
                        Detalle = cuentaContable.Descripcion,
                        Debitos = 0,
                        Creditos = retencion.Importe * (dbOrdenPago.MonedaPago.Equals(Monedas.MonedaLocal.Description()) ? 1 : dbOrdenPago.Cotizacion)
                    }, tran);
                    }
                }

                await _ordenesPagoAsientoRepository.Insert(new OrdenPagoAsiento
                {
                    IdAsiento = (int)idAsiento,
                    IdOrdenPago = dbOrdenPago.IdOrdenPago
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

        public async Task<bool> VerificarChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
            => ((await _chequesRepository.GetByNumeroCheque(nroCheque, idBanco, idTipoCheque, _permissionsBusiness.Value.User.IdEmpresa)) != default);

        public async Task<IList<Custom.OrdenPagoAnticipo>> GetAnticiposImputar(int idProveedor, string idMoneda) =>
            await _ordenesPagoAnticiposRepository.GetAnticiposImputar(idProveedor, idMoneda, _permissionsBusiness.Value.User.IdEmpresa);
    }
}

