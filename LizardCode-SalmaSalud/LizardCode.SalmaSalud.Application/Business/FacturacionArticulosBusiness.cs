using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.FacturacionArticulos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class FacturacionArticulosBusiness: BaseBusiness, IFacturacionArticulosBusiness
    {
        private readonly ILogger<FacturacionManualBusiness> _logger;
        private readonly ISucursalesNumeracionRepository _sucursalesNumeracionRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IComprobantesRepository _comprobantesRepository;
        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;
        private readonly IComprobantesVentasItemRepository _comprobantesVentasItemRepository;
        private readonly IComprobantesVentasAsientoRepository _comprobantesVentasAsientoRepository;
        private readonly IComprobantesVentasTotalesRepository _comprobantesVentasTotalesRepository;
        private readonly IComprobantesVentasAFIPRepository _comprobantesVentasAFIPRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IRecibosComprobantesRepository _recibosComprobantesRepository;
        private readonly IArticulosRepository _articulosRepository;
        private readonly IArticulosMovimientosStockRepository _articulosMovimientosStockRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly ISucursalesRepository _sucursalesRepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;
        private readonly IARBARepository _arbaRepository;
        private readonly IAGIPRepository _agipRepository;

        public FacturacionArticulosBusiness(
            ISucursalesNumeracionRepository sucursalesNumeracionRepository,
            ILogger<FacturacionManualBusiness> logger,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            IComprobantesRepository comprobantesRepository,
            IComprobantesVentasRepository comprobantesVentasRepository,
            IComprobantesVentasItemRepository comprobantesVentasItemRepository,
            IComprobantesVentasAsientoRepository comprobantesVentasAsientoRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            IEmpresasRepository empresasRepository,
            ISucursalesRepository sucursalesRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IComprobantesVentasAFIPRepository comprobantesVentasAFIPRepository,
            IArticulosMovimientosStockRepository articulosMovimientosStockRepository,
            IRecibosComprobantesRepository recibosComprobantesRepository,
            IArticulosRepository articulosRepository,
            IAfipAuthRepository afipAuthRepository,
            IBancosRepository bancosRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IClientesRepository clientesRepository,
            IARBARepository arbaRepository,
            IAGIPRepository agipRepository,
            IComprobantesVentasTotalesRepository comprobantesVentasTotalesRepository)
        {
            _logger = logger;
            _sucursalesNumeracionRepository = sucursalesNumeracionRepository;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _comprobantesRepository = comprobantesRepository;
            _comprobantesVentasRepository = comprobantesVentasRepository;
            _comprobantesVentasItemRepository = comprobantesVentasItemRepository;
            _comprobantesVentasAsientoRepository = comprobantesVentasAsientoRepository;
            _comprobantesVentasTotalesRepository = comprobantesVentasTotalesRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _comprobantesVentasAFIPRepository = comprobantesVentasAFIPRepository;
            _recibosComprobantesRepository = recibosComprobantesRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _afipAuthRepository = afipAuthRepository;
            _articulosMovimientosStockRepository = articulosMovimientosStockRepository;
            _articulosRepository = articulosRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _clientesRepository = clientesRepository;
            _sucursalesRepository = sucursalesRepository;
            _arbaRepository = arbaRepository;
            _agipRepository = agipRepository;
            _bancosRepository = bancosRepository;
        }

        public async Task<FacturacionArticulosViewModel> Get(int idComprobanteVta)
        {
            var comprobante = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVta);

            if (comprobante == null)
                return null;

            var items = await _comprobantesVentasItemRepository.GetAllByIdComprobanteVenta(idComprobanteVta);

            var model = _mapper.Map<FacturacionArticulosViewModel>(comprobante);
            model.Items = _mapper.Map<List<FacturacionArticulosDetalle>>(items);

            return model;
        }

        public async Task<Custom.ComprobanteVenta> GetCustom(int idComprobanteVta)
        {
            var comprobante = await _comprobantesVentasRepository.GetByIdCustom(idComprobanteVta);
            var items = await _comprobantesVentasItemRepository.GetAllByIdComprobanteVenta(idComprobanteVta);

            comprobante.Items = items;

            return comprobante;
        }

        public async Task<DataTablesResponse<Custom.ComprobanteVenta>> GetAll(DataTablesRequest request)
        {
            var customQuery = _comprobantesVentasRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdCliente"))
                builder.Append($"AND IdCliente = {filters["IdCliente"]}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");
            builder.Append($"AND IdTipoComprobante = {(int)TipoComprobante.ManualArticulos}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.ComprobanteVenta>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(FacturacionArticulosViewModel model)
        {
            var comprobante = _mapper.Map<ComprobanteVenta>(model);

            Validate(model);

            var alicuotas = (await _lookupsBusiness.Value.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA)
                .ToDictionary(a => a.IdAlicuota, a => a.Valor);

            model.Cotizacion = model.Cotizacion == 0 ? 1 : model.Cotizacion;

            var tran = _uow.BeginTransaction();

            try
            {
                var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                var cliente = await _clientesRepository.GetById<Cliente>(comprobante.IdCliente);

                var sucursal = await _sucursalesRepository.GetById<Sucursal>(comprobante.IdSucursal);
                var numeracion = await _sucursalesNumeracionRepository.GetLastNumeroByComprobante(comprobante.IdComprobante, comprobante.IdSucursal, _permissionsBusiness.Value.User.IdEmpresa, tran);
                var numero = int.Parse(numeracion.Numerador) + 1;

                var tipoComprobante = await _comprobantesRepository.GetById<Comprobante>(comprobante.IdComprobante, tran);

                comprobante.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                comprobante.Sucursal = sucursal.CodigoSucursal;
                comprobante.Numero = numero.ToString().PadLeft(8, '0');
                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                comprobante.IdUsuario = _permissionsBusiness.Value.User.Id;
                comprobante.FechaIngreso = DateTime.Now;
                comprobante.Subtotal = model.Items.Sum(i => i.Importe);
                comprobante.IdTipoComprobante = (int)TipoComprobante.ManualArticulos;
                comprobante.Moneda = model.IdMonedaComprobante;
                comprobante.CAE = string.Empty;
                comprobante.IdEstadoAFIP = (int)EstadoAFIP.Inicial;
                comprobante.ReferenciaComercial = model.ReferenciaComercial;

                var percepcionAGIP = 0D;
                AGIP agip = new();
                if (empresa.AgentePercepcionAGIP && !tipoComprobante.EsCredito)
                {
                    agip = await _agipRepository.GetByCUITFechaVig(cliente.CUIT.Replace("-", String.Empty), comprobante.Fecha, tran);
                    if (agip != default)
                        percepcionAGIP = comprobante.Subtotal * (agip.AliPercepcion / 100);
                }

                var percepcionARBA = 0D;
                ARBA arba = new();
                if (empresa.AgentePercepcionARBA && !tipoComprobante.EsCredito)
                {
                    arba = await _arbaRepository.GetByCUITFechaVig("P", cliente.CUIT.Replace("-", String.Empty), comprobante.Fecha, tran);
                    if (arba != default)
                        percepcionARBA = comprobante.Subtotal * (arba.Alicuota / 100);
                }

                comprobante.Total = model.Items.Sum(i => i.Importe * (1 + (alicuotas.GetValueOrDefault(i.IdAlicuota) / 100))) + percepcionAGIP + percepcionARBA;
                comprobante.FechaVto = model.Vto;

                var id = await _comprobantesVentasRepository.Insert(comprobante, tran);
                var iItem = 1;

                foreach (var item in model.Items)
                {
                    var alicuota = alicuotas.GetValueOrDefault(item.IdAlicuota);

                    //Guardo el Item
                    await _comprobantesVentasItemRepository.Insert(new ComprobanteVentaItem
                    {
                        IdComprobanteVenta = (int)id,
                        Item = iItem++,
                        IdArticulo = item.IdArticulo,
                        Impuestos = item.Importe * (alicuota / 100),
                        Descripcion = item.Descripcion.ToUpper().Trim(),
                        Alicuota = alicuota,
                        Bonificacion = 0,
                        Precio = item.PrecioUnitario,
                        Cantidad = item.Cantidad,
                        Importe = item.Importe,
                    }, tran);

                    await _articulosMovimientosStockRepository.Insert(new ArticuloMovimientoStock
                    {
                        IdArticulo = item.IdArticulo,
                        IdComprobanteVenta = (int)id,
                        Canitdad = -item.Cantidad
                    }, tran);

                    var articulo = await _articulosRepository.GetById<Articulo>(item.IdArticulo, tran);
                    articulo.Stock -= item.Cantidad;
                    await _articulosRepository.Update(articulo, tran);
                }

                var totales = model.Items.GroupBy(t => t.IdAlicuota)
                    .Select(g => new
                    {
                        g.FirstOrDefault().IdAlicuota,
                        Neto = g.Sum(t => t.Importe),
                        ImporteAlicuota = g.Sum(t => t.Importe * (alicuotas.GetValueOrDefault(t.IdAlicuota) / 100))
                    });

                iItem = 1;
                foreach (var total in totales)
                {
                    await _comprobantesVentasTotalesRepository.Insert(
                        new ComprobanteVentaTotales
                        {
                            IdComprobanteVenta = (int)id,
                            Item = iItem++,
                            ImporteAlicuota = total.ImporteAlicuota,
                            Neto = total.Neto,
                            Alicuota = alicuotas.GetValueOrDefault(total.IdAlicuota),
                            IdTipoAlicuota = (int)TipoAlicuota.IVA
                        });
                }

                if (percepcionAGIP > 0)
                {
                    await _comprobantesVentasTotalesRepository.Insert(
                        new ComprobanteVentaTotales
                        {
                            IdComprobanteVenta = (int)id,
                            Item = iItem++,
                            ImporteAlicuota = percepcionAGIP,
                            Neto = comprobante.Subtotal,
                            Alicuota = agip.AliPercepcion,
                            IdTipoAlicuota = (int)TipoAlicuota.PercepcionAGIP
                        }, tran);
                }

                if (percepcionARBA > 0)
                {
                    await _comprobantesVentasTotalesRepository.Insert(
                        new ComprobanteVentaTotales
                        {
                            IdComprobanteVenta = (int)id,
                            Item = iItem++,
                            ImporteAlicuota = percepcionARBA,
                            Neto = comprobante.Subtotal,
                            Alicuota = arba.Alicuota,
                            IdTipoAlicuota = (int)TipoAlicuota.PercepcionARBA
                        }, tran);
                }

                #region Asiento Contable

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = comprobante.IdEjercicio,
                    Descripcion = string.Concat(tipoComprobante.Descripcion, comprobante.Sucursal, "-", comprobante.Numero),
                    Fecha = comprobante.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var indItem = 1;
                var ctaVentaArticulos = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.INTEGRADORA_CLIENTES, tran);
                if (ctaVentaArticulos != null)
                {
                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = ctaVentaArticulos.IdCuentaContable,
                        Detalle = ctaVentaArticulos.Descripcion,
                        Debitos = tipoComprobante.EsCredito ? 0 : comprobante.Total * model.Cotizacion,
                        Creditos = tipoComprobante.EsCredito ? comprobante.Total * model.Cotizacion : 0
                    }, tran);
                }
                else
                    throw new BusinessException("No se ecnuentra la Cuenta Integradora de Clientes. Verifique.");

                var impuesto = model.Items.Sum(dc => dc.Importe * (alicuotas.GetValueOrDefault(dc.IdAlicuota) / 100)) * model.Cotizacion;
                if (impuesto > 0)
                {
                    var ctaDebitoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.DEBITO_FISCAL_IVA, tran);
                    if (ctaDebitoFiscal != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaDebitoFiscal.IdCuentaContable,
                            Detalle = ctaDebitoFiscal.Descripcion,
                            Debitos = tipoComprobante.EsCredito ? impuesto : 0,
                            Creditos = tipoComprobante.EsCredito ? 0 : impuesto
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Débito Fiscal IVA. Verifique.");
                }

                if (percepcionAGIP > 0)
                {
                    var ctaPercepcionCABA = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.PERCEPCION_ING_BRUTOS_CLIENTES_CABA, tran);
                    if (ctaPercepcionCABA != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaPercepcionCABA.IdCuentaContable,
                            Detalle = ctaPercepcionCABA.Descripcion,
                            Debitos = 0,
                            Creditos = percepcionAGIP * model.Cotizacion
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Percepción de Ingresos Brutos CABA. Verifique.");
                }

                if (percepcionARBA > 0)
                {
                    var ctaPercepcionGBA = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.PERCEP_ING_BRUTOS_CLIENTES, tran);
                    if (ctaPercepcionGBA != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaPercepcionGBA.IdCuentaContable,
                            Detalle = ctaPercepcionGBA.Descripcion,
                            Debitos = 0,
                            Creditos = percepcionARBA * model.Cotizacion
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Percepción de Ingresos Brutos GBA. Verifique.");
                }

                foreach (var item in model.Items)
                {
                    var articulo = await _articulosRepository.GetById<Articulo>(item.IdArticulo, tran);
                    var cuentaContable = await _cuentasContablesRepository.GetById<CuentaContable>(articulo.IdCuentaContableVentas, tran);

                    if (cuentaContable != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = cuentaContable.IdCuentaContable,
                            Detalle = cuentaContable.Descripcion,
                            Debitos = tipoComprobante.EsCredito ? item.Importe * model.Cotizacion : 0,
                            Creditos = tipoComprobante.EsCredito ? 0 : item.Importe * model.Cotizacion
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Ventas asociada al Articulo. Verifique.");
                }

                await _comprobantesVentasAsientoRepository.Insert(new ComprobanteVentaAsiento { IdComprobanteVenta = (int)id, IdAsiento = (int)idAsiento }, tran);

                #endregion

                numeracion.Numerador = comprobante.Numero;
                await _sucursalesNumeracionRepository.Update(numeracion, tran);

                tran.Commit();
            }
            catch(BusinessException ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw ex;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException(ex.Message);
            }
        }

        public async Task Remove(int idComprobanteVta)
        {
            var comprobante = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVta);

            if (comprobante == null)
            {
                throw new BusinessException("Comprobante de Venta inexistente");
            }

            if (comprobante.IdTipoComprobante != (int)TipoComprobante.ManualArticulos)
            {
                throw new BusinessException("El Tipo de Comprobante no es de Articulos. No se puede Eliminar.");
            }

            if (_cierreMesRepository.MesCerrado(comprobante.IdEjercicio.Value, comprobante.Fecha, Modulos.Clientes.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Comprobante se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var cobro = await _recibosComprobantesRepository.GetByIdComprobanteVenta(idComprobanteVta);
            if (cobro != default)
            {
                throw new BusinessException($"El Comprobante se encuentra Imputado en el Recibo Número {cobro.IdRecibo}. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var comprobanteAsiento = await _comprobantesVentasAsientoRepository.GetByIdComprobanteVenta(idComprobanteVta, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(comprobanteAsiento.IdAsiento, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(comprobanteAsiento.IdAsiento, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                await _comprobantesVentasItemRepository.DeleteByIdComprobanteVenta(idComprobanteVta, tran);
                await _comprobantesVentasTotalesRepository.DeleteByIdComprobanteVenta(idComprobanteVta, tran);

                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _comprobantesVentasRepository.Update(comprobante, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        private void Validate(FacturacionArticulosViewModel comptobanteVta)
        {
            if (comptobanteVta.Items?.Count == 0)
            {
                throw new BusinessException("Seleccione algún Item para el Comprobante");

            }

            if (_ejerciciosRepository.EjercicioCerrado(comptobanteVta.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(comptobanteVta.IdEjercicio, comptobanteVta.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Comprobante no es Valida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(comptobanteVta.IdEjercicio, comptobanteVta.Fecha, Modulos.Clientes.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha del Comprobante esta Cerrado. Verifique.");
            }
        }

        public async Task<Custom.DatosComprobanteVentaAFIP> ValidateComprobante(int idComprobanteVta)
        {
            try
            {

                var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                var afipAuth = await _afipAuthRepository.GetValidSignToken(_permissionsBusiness.Value.User.IdEmpresa, ServicioAFIP.WEB_SERVICE_FACTURA_ELECTRONICA.Description());

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
                        throw new BusinessException(ex.Message);
                    }
                }

                var comprobanteVta = await _comprobantesVentasRepository.GetComprobanteVentaAFIP(idComprobanteVta);
                var bancos = (await _bancosRepository.GetAll<Banco>()).Where(b => b.IdEmpresa == _permissionsBusiness.Value.User.IdEmpresa).ToList();
                var primerDiaMes = new DateTime(comprobanteVta.Fecha.Year, comprobanteVta.Fecha.Month, 1);
                var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
                comprobanteVta.FchServDesde = primerDiaMes;
                comprobanteVta.FchServHasta = ultimoDiaMes;
                comprobanteVta.Concepto = (int)ConceptosComprobantesAFIP.COMPROBANTE_SERVICIOS;

                if(comprobanteVta.EsCredito)
                {
                    if(comprobanteVta.CbteTipo == 203 || comprobanteVta.CbteTipo == 208)
                    {
                        comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "22", Valor = "N" });
                    }
                }

                if (comprobanteVta.EsMiPymes && !comprobanteVta.EsCredito)
                {
                    var banco = bancos.Where(b => b.EsDefault).FirstOrDefault() ?? bancos.FirstOrDefault();
                    comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "2101", Valor = banco?.CBU ?? string.Empty });
                    comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "27", Valor = "ADC" });
                }

                if (!string.IsNullOrEmpty(comprobanteVta.ReferenciaComercial))
                    comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "23", Valor = comprobanteVta.ReferenciaComercial });

                var comprobanteVenta = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVta);

                var afip = await _empresasRepository.ValidComprobanteVentas(afipAuth, empresa.CUIT.Replace("-", string.Empty), comprobanteVta, empresa.EnableProdAFIP);

                await _comprobantesVentasAFIPRepository.Insert(new ComprobanteVentaAFIP
                {
                    IdComprobanteVenta = idComprobanteVta,
                    Estado = afip.Resultado,
                    CAE = afip.CAE,
                    VencimientoCAE = afip.VencimientoCAE.IsNull() ? default : DateTime.ParseExact(afip.VencimientoCAE, "yyyyMMdd", null),
                    FechaRequest = DateTime.Now,
                    Request = afip.XMLRequest,
                    Response = afip.XMLResponse
                });

                if (afip.Resultado == "A")
                {
                    comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Autorizado;
                    comprobanteVenta.CAE = afip.CAE;
                    comprobanteVenta.VencimientoCAE = DateTime.ParseExact(afip.VencimientoCAE, "yyyyMMdd", null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(afip.Error))
                        comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Error;
                    if (!string.IsNullOrEmpty(afip.Observacion))
                        comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Observado;
                }

                await _comprobantesVentasRepository.Update(comprobanteVenta);

                await _comprobantesVentasAFIPRepository.Insert(new ComprobanteVentaAFIP
                {
                    IdComprobanteVenta = idComprobanteVta,
                    Estado = afip.Resultado,
                    CAE = afip.CAE,
                    VencimientoCAE = string.IsNullOrEmpty(afip.VencimientoCAE) ? DateTime.MinValue : DateTime.ParseExact(afip.VencimientoCAE, "yyyyMMdd", null),
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

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {

            var asiento = await _comprobantesVentasAsientoRepository.GetByIdComprobanteVenta(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
            
        }

        public async Task<Custom.Percepcion> GetPercepcionesByCliente(int idCliente, int idComprobante, DateTime fecha, int idEmpresa)
        {
            var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
            var cliente = await _clientesRepository.GetById<Cliente>(idCliente);
            if (idComprobante == 0)
            {
                switch (cliente.IdTipoIVA)
                {
                    case (int)TipoIVA.ConsumidorFinal:
                        idComprobante = (int)Comprobantes.FACTURA_B;
                        break;
                    case (int)TipoIVA.Monotributo:
                        idComprobante = (int)Comprobantes.FACTURA_C;
                        break;
                    case (int)TipoIVA.ResponsableInscripto:
                        idComprobante = (int)Comprobantes.FACTURA_A;
                        break;
                    default:
                        idComprobante = (int)Comprobantes.FACTURA_B;
                        break;
                }
            }

            var comprobante = await _comprobantesRepository.GetById<Comprobante>(idComprobante);

            AGIP agip = new();
            ARBA arba = new();

            if (empresa.AgentePercepcionAGIP && !comprobante.EsCredito)
            {
                agip = await _agipRepository.GetByCUITFechaVig(cliente.CUIT.Replace("-", String.Empty), fecha);
            }

            if (empresa.AgentePercepcionARBA && !comprobante.EsCredito)
            {
                arba = await _arbaRepository.GetByCUITFechaVig("P", cliente.CUIT.Replace("-", String.Empty), fecha);
            }

            return new Custom.Percepcion
            {
                PercepcionAGIP = agip?.AliPercepcion ?? 0,
                PercepcionARBA = arba?.Alicuota ?? 0
            };

        }

        public async Task<Custom.DatosComprobanteVentaAFIP> ObtenerCAEComprobantesById(int idComprobanteVenta)
        {
            try
            {
                var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                var afipAuth = await _afipAuthRepository.GetValidSignToken(_permissionsBusiness.Value.User.IdEmpresa, ServicioAFIP.WEB_SERVICE_FACTURA_ELECTRONICA.Description());

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
                        throw new BusinessException(ex.Message);
                    }
                }

                var comprobanteVta = await _comprobantesVentasRepository.GetComprobanteVentaAFIP(idComprobanteVenta);

                var afip = await _empresasRepository.GetConsultaComprobanteAFIP(afipAuth, empresa.CUIT.Replace("-", string.Empty), comprobanteVta, empresa.EnableProdAFIP);

                var comprobanteVenta = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVenta);
                if (afip.Resultado == "A")
                {
                    comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Autorizado;
                    comprobanteVenta.CAE = afip.CAE;
                    comprobanteVenta.VencimientoCAE = DateTime.ParseExact(afip.VencimientoCAE, "yyyyMMdd", null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(afip.Error))
                        comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Error;
                    if (!string.IsNullOrEmpty(afip.Observacion))
                        comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Observado;
                }

                await _comprobantesVentasRepository.Update(comprobanteVenta);

                return afip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new BusinessException(ex.Message);
            }
        }
        public async Task<double> GetPrecio(int idArticulo, int idEmpresa)
            => 0D;
        
            
        
    }
}
