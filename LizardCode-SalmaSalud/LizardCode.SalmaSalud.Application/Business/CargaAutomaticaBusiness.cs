using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CargaAutomatica;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CargaAutomaticaBusiness: BaseBusiness, ICargaAutomaticaBusiness
    {
        private readonly ILogger<CargaAutomaticaBusiness> _logger;
        private readonly IMonedasRepository _monedasRepository;
        private readonly IMonedasFechasCambioRepository _monedasFechasCambioRepository;
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

        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public CargaAutomaticaBusiness(
            ILogger<CargaAutomaticaBusiness> logger,
            IMonedasRepository monedasRepository,
            IMonedasFechasCambioRepository monedasFechasCambioRepository,
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
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository)
        {
            _logger = logger;
            _monedasRepository = monedasRepository;
            _monedasFechasCambioRepository = monedasFechasCambioRepository;
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
        }

        public async Task<CargaAutomaticaViewModel> Get(int idComprobanteCpa)
        {
            var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(idComprobanteCpa);

            if (comprobante == null)
                return null;

            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompra(idComprobanteCpa);

            var model = _mapper.Map<CargaAutomaticaViewModel>(comprobante);
            model.Items = _mapper.Map<List<CargaAutomaticaDetalle>>(items);

            return model;
        }

        public async Task<Custom.ComprobanteCompra> GetCustom(int idComprobanteCpa)
        {
            var comprobante = await _comprobantesComprasRepository.GetByIdCustom(idComprobanteCpa);
            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompra(idComprobanteCpa);

            comprobante.Items = items;

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

            builder.Append($"AND IdTipoComprobante = {(int)TipoComprobante.AutomaticaProveedores}");
            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.ComprobanteCompra>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(CargaAutomaticaViewModel model)
        {

            var comprobante = _mapper.Map<ComprobanteCompra>(model);
            var items = _mapper.Map<List<ComprobanteCompraItem>>(model.Items);

            Validate(model);

            model.Cotizacion = model.Cotizacion == 0 ? 1 : model.Cotizacion;

            var tran = _uow.BeginTransaction();

            try
            {
                _ = int.TryParse(model.AnnoMesDesde.Split('/')[0], out int mesDesde);
                _ = int.TryParse(model.AnnoMesDesde.Split('/')[1], out int annoDesde);

                _ = int.TryParse(model.AnnoMesHasta.Split('/')[0], out int mesHasta);
                _ = int.TryParse(model.AnnoMesHasta.Split('/')[1], out int annoHasta);

                var tipoComprobante = await _comprobantesRepository.GetById<Comprobante>(comprobante.IdComprobante, tran);

                comprobante.Sucursal = model.NumeroComprobante.Substring(0, 5);
                comprobante.Numero = model.NumeroComprobante.Substring(6, 8);
                comprobante.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                comprobante.IdUsuario = _permissionsBusiness.Value.User.Id;
                comprobante.FechaIngreso = DateTime.Now;
                comprobante.Percepciones = model.ListaPercepciones?.Sum(i => i.Importe) ?? 0;
                comprobante.Subtotal = items.Sum(i => i.Importe);
                comprobante.Total = items.Sum(i => i.Importe * (1 + (i.Alicuota / 100))) + comprobante.Percepciones;
                comprobante.IdTipoComprobante = (int)TipoComprobante.AutomaticaProveedores;
                comprobante.CAE = model.CAE;
                comprobante.VencimientoCAE = model.VenciminetoCAE;
                comprobante.IdEstadoAFIP = (int)EstadoAFIP.Inicial;
                comprobante.IdCentroCosto = model.IdCentroCosto;
                comprobante.FechaVto = model.Vto;

                var id = await _comprobantesComprasRepository.Insert(comprobante, tran);
                var iItem = 1;

                for (int iLoop = 0; iLoop < items.Count; iLoop++)
                {
                    var item = items[iLoop];

                    //Guardo el Item del Estimado
                    await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                    {
                        IdComprobanteCompra = (int)id,
                        Item = iItem,
                        Impuestos = item.Importe * (item.Alicuota / 100),
                        Descripcion = item.Descripcion.ToUpper().Trim(),
                        Alicuota = item.Alicuota,
                        Bonificacion = item.Bonificacion,
                        Cantidad = item.Cantidad,
                        Importe = item.Importe,
                        Precio = item.Precio
                    }, tran);

                    iItem++;
                }

                var totales = items.GroupBy(t => t.Alicuota)
                    .Select(g => new
                    {
                        g.FirstOrDefault().Alicuota,
                        Neto = g.Sum(t => t.Importe),
                        ImporteAlicuota = g.Sum(t => t.Importe * (t.Alicuota / 100))
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
                            Alicuota = total.Alicuota
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
                        Debitos = tipoComprobante.EsCredito ? items.Sum(dc => dc.Importe * (1 + (dc.Alicuota / 100)) * model.Cotizacion) + comprobante.Percepciones : 0,
                        Creditos = tipoComprobante.EsCredito ? 0 : items.Sum(dc => dc.Importe * (1 + (dc.Alicuota / 100)) * model.Cotizacion) + comprobante.Percepciones
                    }, tran);
                }
                else
                    throw new BusinessException("No se ecnuentra la Cuenta Contable Integradora de Proveedores. Verifique.");
                
                var impuesto = items.Sum(dc => dc.Importe * (dc.Alicuota / 100)) * model.Cotizacion;
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

            if (comprobante == null)
            {
                throw new BusinessException("Comprobante de Compra inexistente");
            }

            if (comprobante.IdTipoComprobante != (int)TipoComprobante.AutomaticaProveedores)
            {
                throw new BusinessException("El Tipo de Comprobante es Manual o de Gastos. No se puede Eliminar.");
            }

            if (_cierreMesRepository.MesCerrado(comprobante.IdEjercicio.Value, comprobante.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Comprobante se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var pago = await _ordenesPagoComprobantesRepository.GetByIdComprobanteCompra(idComprobanteCpa);
            if (pago != default)
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
                await _comprobantesComprasPercepcionesRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);
                await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);
                await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(idComprobanteCpa, tran);

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

        private void Validate(CargaAutomaticaViewModel comprobanteCpa)
        {
            if (comprobanteCpa.NumeroComprobante.Length != 14)
            {
                throw new BusinessException("El Número del Comprobante es Incorrecto");
            }
            if (comprobanteCpa.AnnoMesDesde.IsNull())
            {
                throw new BusinessException("Ingrese Mes/Año para el Estimado de Producció");
            }

            if (comprobanteCpa.CAE.IsNull())
            {
                if (comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_A && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_B && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_C)
                    throw new BusinessException("Ingrese el C.A.E para el Comprobante a Ingresar.");
            }

            if (!comprobanteCpa.VenciminetoCAE.HasValue)
            {
                if (comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_A && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_B && comprobanteCpa.IdComprobante != (int)Comprobantes.TICKET_C)
                    throw new BusinessException("Ingrese el Vencimiento del C.A.E para el Comprobante a Ingresar.");
            }

            if (!int.TryParse(comprobanteCpa.AnnoMesDesde.Split('/')[0], out int mesDesde))
            {
                throw new BusinessException("El Mes Ingresado es Incorrecto");
            }
            if (mesDesde < 1 && mesDesde > 12)
            {
                throw new BusinessException("El Mes Ingresado es Incorrecto");
            }

            if (!int.TryParse(comprobanteCpa.AnnoMesDesde.Split('/')[1], out int annoDesde))
            {
                throw new BusinessException("El Año Ingresado es Incorrecto");
            }

            if (annoDesde < 2000 && annoDesde > 2999)
            {
                throw new BusinessException("El Año Ingresado es Incorrecto");
            }

            if (comprobanteCpa.AnnoMesHasta.IsNull())
            {
                throw new BusinessException("Ingrese Mes/Año para el Estimado de Producció");
            }

            if (!int.TryParse(comprobanteCpa.AnnoMesHasta.Split('/')[0], out int mesHasta))
            {
                throw new BusinessException("El Mes Ingresado es Incorrecto");
            }
            if (mesHasta < 1 && mesHasta > 12)
            {
                throw new BusinessException("El Mes Ingresado es Incorrecto");
            }

            if (!int.TryParse(comprobanteCpa.AnnoMesHasta.Split('/')[1], out int annoHasta))
            {
                throw new BusinessException("El Año Ingresado es Incorrecto");
            }

            if (annoHasta < 2000 && annoHasta > 2999)
            {
                throw new BusinessException("El Año Ingresado es Incorrecto");
            }

            if(comprobanteCpa.NumeroComprobante.Length != 14)
            {
                throw new BusinessException("El Número del Comprobante es Incorrecto");
            }

            if(comprobanteCpa.Items?.Count == 0)
            {
                throw new BusinessException("Seleccione algún Item para el Comprobante");
            }

            var compCompra = _comprobantesComprasRepository.GetComprobanteByProveedor(comprobanteCpa.IdComprobante, comprobanteCpa.NumeroComprobante, comprobanteCpa.IdProveedor).GetAwaiter().GetResult();
            if(compCompra != default)
            {
                throw new BusinessException("Existe un Comprobante con el mismo Número para el Proveedor Seleccionado. Verifique.");
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

    }
}
