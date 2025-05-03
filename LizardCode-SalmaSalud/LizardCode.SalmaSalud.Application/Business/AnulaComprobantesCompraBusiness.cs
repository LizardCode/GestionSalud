using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.AnulaComprobantesCompra;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class AnulaComprobantesCompraBusiness : BaseBusiness, IAnulaComprobantesCompraBusiness
    {
        private readonly ILogger<AnulaComprobantesCompraBusiness> _logger;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;
        private readonly IComprobantesComprasItemRepository _comprobantesComprasItemRepository;
        private readonly IComprobantesComprasAsientoRepository _comprobantesComprasAsientoRepository;
        private readonly IComprobantesComprasTotalesRepository _comprobantesComprasTotalesRepository;
        private readonly IComprobantesComprasAFIPRepository _comprobantesComprasAFIPRepository;
        private readonly IOrdenesPagoComprobantesRepository _ordenesPagoComprobantesRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;

        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public AnulaComprobantesCompraBusiness(
            ILogger<AnulaComprobantesCompraBusiness> logger,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            IComprobantesComprasRepository comprobantesComprasRepository,
            IComprobantesComprasItemRepository comprobantesComprasItemRepository,
            IComprobantesComprasAsientoRepository comprobantesComprasAsientoRepository,
            IComprobantesComprasAFIPRepository comprobantesComprasAFIPRepository,
            IOrdenesPagoComprobantesRepository ordenesPagoComprobantesRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IAfipAuthRepository afipAuthRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository)
        {
            _logger = logger;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _comprobantesComprasRepository = comprobantesComprasRepository;
            _comprobantesComprasItemRepository = comprobantesComprasItemRepository;
            _comprobantesComprasAsientoRepository = comprobantesComprasAsientoRepository;
            _comprobantesComprasTotalesRepository = comprobantesComprasTotalesRepository;
            _comprobantesComprasAFIPRepository = comprobantesComprasAFIPRepository;
            _ordenesPagoComprobantesRepository = ordenesPagoComprobantesRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _afipAuthRepository = afipAuthRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
        }

        public async Task<AnulaComprobantesCompraViewModel> Get(int idComprobanteCompra)
        {
            var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteVenta>(idComprobanteCompra);

            if (comprobante == null)
                return null;

            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompra(idComprobanteCompra);

            var model = _mapper.Map<AnulaComprobantesCompraViewModel>(comprobante);
            model.Items = _mapper.Map<List<AnulaComprobantesCompraDetalle>>(items);

            return model;
        }

        public async Task<Custom.ComprobanteCompra> GetCustom(int idComprobanteCompra)
        {
            var comprobante = await _comprobantesComprasRepository.GetByIdCustom(idComprobanteCompra);
            var items = await _comprobantesComprasItemRepository.GetAllByIdComprobanteCompra(idComprobanteCompra);

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

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");
            builder.Append($"AND IdTipoComprobante = {(int)TipoComprobante.AutomaticaAnulaFacturaProveedores}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.ComprobanteCompra>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(AnulaComprobantesCompraViewModel model)
        {
            var comprobante = _mapper.Map<ComprobanteCompra>(model);
            var items = _mapper.Map<List<ComprobanteCompraItem>>(model.Items);

            Validate(model);

            var tran = _uow.BeginTransaction();

            try
            {
                var sucursalAnular = model.NumeroComprobanteAnular.Substring(0, 5);
                var numeroAnular = model.NumeroComprobanteAnular.Substring(6, 8);

                var comprobanteAnular = await _comprobantesComprasRepository.GetComprobanteBySucNro(model.IdComprobanteAnular, sucursalAnular, numeroAnular, model.IdProveedor, _permissionsBusiness.Value.User.IdEmpresa, tran);

                if (comprobanteAnular == default)
                    throw new BusinessException($"No Existe el Comprobante de Compras Nro. {sucursalAnular}-{numeroAnular} para el Proveedor Seleccionado. Verifique.");

                var tipoComprobante = await _comprobantesComprasRepository.GetComprobanteCreditoBySucNro(model.IdComprobanteAnular, model.NumeroComprobanteAnular, model.IdProveedor, _permissionsBusiness.Value.User.IdEmpresa, tran);

                comprobante.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                comprobante.IdComprobante = tipoComprobante.IdComprobante;
                comprobante.Sucursal = model.NumeroComprobante.Substring(0, 5);
                comprobante.Numero = model.NumeroComprobante.Substring(6, 8);
                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                comprobante.IdUsuario = _permissionsBusiness.Value.User.Id;
                comprobante.FechaIngreso = DateTime.Now;
                comprobante.Subtotal = items.Sum(i => i.Importe);
                comprobante.Total = items.Sum(i => i.Importe * (1 + (i.Alicuota / 100)));
                comprobante.IdTipoComprobante = (int)TipoComprobante.AutomaticaAnulaFacturaProveedores;
                comprobante.Moneda = comprobanteAnular.Moneda;
                comprobante.Cotizacion = comprobanteAnular.Cotizacion;
                comprobante.CAE = model.CAE;
                comprobante.VencimientoCAE = model.VenciminetoCAE;
                comprobante.IdEstadoAFIP = (int)EstadoAFIP.Inicial;
                comprobante.FechaVto = model.Vto;

                var id = await _comprobantesComprasRepository.Insert(comprobante, tran);
                var iItem = 1;

                foreach (var item in items)
                {
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

                #region Asiento Contable

                var asientoComprobanteAnular = await _comprobantesComprasAsientoRepository.GetByIdComprobanteCompra(comprobanteAnular.IdComprobanteCompra);

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = comprobante.IdEjercicio,
                    Descripcion = string.Concat(tipoComprobante.Descripcion, comprobante.Sucursal, " -", comprobante.Numero),
                    Fecha = comprobante.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var asientoDetalle = await _asientosDetalleRepository.GetAllByIdAsiento(asientoComprobanteAnular.IdAsiento);

                var indItem = 1;
                foreach (var item in asientoDetalle)
                {
                    await _asientosDetalleRepository.Insert(new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        Item = indItem++,
                        IdCuentaContable = item.IdCuentaContable,
                        Detalle = item.Detalle,
                        Debitos = item.Creditos,
                        Creditos = item.Debitos
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

        public async Task Remove(int idComprobanteCompra)
        {
            var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(idComprobanteCompra);

            if (comprobante == null)
            {
                throw new BusinessException("Comprobante de Compra inexistente");
            }

            if (comprobante.IdTipoComprobante != (int)TipoComprobante.AutomaticaAnulaFacturaProveedores)
            {
                throw new BusinessException("El Tipo de Comprobante no es una N/Crédito Anula Factura. No se puede Eliminar.");
            }

            var pago = await _ordenesPagoComprobantesRepository.GetByIdComprobanteCompra(idComprobanteCompra);
            if (pago != default)
            {
                throw new BusinessException($"El Comprobante se encuentra Imputado en la Orden de Pago Número {pago.IdOrdenPago}. No se puede Eliminar.");
            }

            if (_cierreMesRepository.MesCerrado(comprobante.IdEjercicio.Value, comprobante.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Comprobante se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var comprobanteAsiento = await _comprobantesComprasAsientoRepository.GetByIdComprobanteCompra(idComprobanteCompra, tran);
                var asiento = await _asientosRepository.GetById<Asiento>(comprobanteAsiento.IdAsiento, tran);

                //Borro el Asiento de la Factura
                await _asientosDetalleRepository.DeleteByIdAsiento(comprobanteAsiento.IdAsiento, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(idComprobanteCompra, tran);
                await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(idComprobanteCompra, tran);

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

        private void Validate(AnulaComprobantesCompraViewModel comptobanteCpa)
        {
            if (comptobanteCpa.NumeroComprobanteAnular.IsNull())
            {
                throw new BusinessException("Ingrese el Numero de Comprobante a Anular");
            }

            if (comptobanteCpa.NumeroComprobante.Length != 14)
            {
                throw new BusinessException("El formato del Numero de Comprobante es Incorrecto");
            }


            if (comptobanteCpa.NumeroComprobanteAnular.Length != 14)
            {
                throw new BusinessException("El formato del Numero de Comprobante a Anular es Incorrecto");
            }

            var Sucursal = comptobanteCpa.NumeroComprobanteAnular.Substring(0, 5);
            var Numero = comptobanteCpa.NumeroComprobanteAnular.Substring(6, 8);
            if (!int.TryParse(Sucursal, out _))
            {
                throw new BusinessException("La Sucursal del Comprobante a Anular es Incorrecto");
            }

            if (!int.TryParse(Numero, out _))
            {
                throw new BusinessException("La Número del Comprobante a Anular es Incorrecto");
            }

            Sucursal = comptobanteCpa.NumeroComprobante.Substring(0, 5);
            Numero = comptobanteCpa.NumeroComprobante.Substring(6, 8);
            if (!int.TryParse(Sucursal, out _))
            {
                throw new BusinessException("La Sucursal del Comprobante a Anular es Incorrecto");
            }

            if (!int.TryParse(Numero, out _))
            {
                throw new BusinessException("La Número del Comprobante a Anular es Incorrecto");
            }

            if (_ejerciciosRepository.EjercicioCerrado(comptobanteCpa.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(comptobanteCpa.IdEjercicio, comptobanteCpa.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Comprobante no es Valida para el Ejercicio seleccionado. Verifique.");
            }


            if (_cierreMesRepository.MesCerrado(comptobanteCpa.IdEjercicio, comptobanteCpa.Fecha, Modulos.Proveedores.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha del Comprobante esta Cerrado. Verifique.");
            }

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
                    if (!string.IsNullOrEmpty(afip.Error))
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

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {

            var asiento = await _comprobantesComprasAsientoRepository.GetByIdComprobanteCompra(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
            
        }

        public async Task<List<Custom.ComprobanteCompraItem>> GetItemsNCAnulaByFactura(int IdComprobante, string numero, int idProveedor)
            => (await _comprobantesComprasItemRepository.GetItemsNCAnulaByFactura(IdComprobante, numero, idProveedor, _permissionsBusiness.Value.User.IdEmpresa))
                    .Select(s => new Custom.ComprobanteCompraItem
                    {
                        Seleccion = s.Seleccion,
                        Item = s.Item,
                        Descripcion = string.Concat("NOTA DE CREDITO ANULA FACTURA NUMERO: ", s.Descripcion),
                        Alicuota = s.Alicuota,
                        IdMoneda = s.IdMoneda,
                        Moneda = s.Moneda,
                        Importe = s.Importe
                    }).ToList();
    }
}
