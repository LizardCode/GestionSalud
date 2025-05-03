using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.AnulaComprobantesVenta;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class AnulaComprobantesVentaBusiness: BaseBusiness, IAnulaComprobantesVentaBusiness
    {
        private readonly ILogger<AnulaComprobantesVentaBusiness> _logger;        
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;
        private readonly IComprobantesVentasItemRepository _comprobantesVentasItemRepository;
        private readonly IComprobantesVentasAsientoRepository _comprobantesVentasAsientoRepository;
        private readonly IComprobantesVentasAnulacionesRepository _comprobantesVentasAnulacionesRepository;
        private readonly IComprobantesVentasTotalesRepository _comprobantesVentasTotalesRepository;
        private readonly IComprobantesVentasAFIPRepository _comprobantesVentasAFIPRepository;
        private readonly IRecibosComprobantesRepository _recibosComprobantesRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly ISucursalesRepository _sucursalesRepository;
        private readonly ISucursalesNumeracionRepository _sucursalesNumeracionRepository;

        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public AnulaComprobantesVentaBusiness(
            ILogger<AnulaComprobantesVentaBusiness> logger,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            IComprobantesVentasRepository comprobantesVentasRepository,
            IComprobantesVentasItemRepository comprobantesVentasItemRepository,
            IComprobantesVentasAsientoRepository comprobantesVentasAsientoRepository,
            IComprobantesVentasAnulacionesRepository comprobantesVentasAnulacionesRepository,
            IComprobantesVentasAFIPRepository comprobantesVentasAFIPRepository,
            IBancosRepository bancosRepository,
            IRecibosComprobantesRepository recibosComprobantesRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IAfipAuthRepository afipAuthRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            ISucursalesNumeracionRepository sucursalesNumeracionRepository,
            ISucursalesRepository sucursalesRepository,
            IComprobantesVentasTotalesRepository comprobantesVentasTotalesRepository)
        {
            _logger = logger;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _comprobantesVentasRepository = comprobantesVentasRepository;
            _comprobantesVentasItemRepository = comprobantesVentasItemRepository;
            _comprobantesVentasAsientoRepository = comprobantesVentasAsientoRepository;
            _comprobantesVentasTotalesRepository = comprobantesVentasTotalesRepository;
            _comprobantesVentasAnulacionesRepository = comprobantesVentasAnulacionesRepository;
            _comprobantesVentasAFIPRepository = comprobantesVentasAFIPRepository;
            _recibosComprobantesRepository = recibosComprobantesRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _sucursalesNumeracionRepository = sucursalesNumeracionRepository;
            _sucursalesRepository = sucursalesRepository;
            _afipAuthRepository = afipAuthRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _bancosRepository = bancosRepository;
        }

        public async Task<AnulaComprobantesVentaViewModel> Get(int idComprobanteVta)
        {
            var comprobante = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVta);

            if (comprobante == null)
                return null;

            var items = await _comprobantesVentasItemRepository.GetAllByIdComprobanteVenta(idComprobanteVta);

            var model = _mapper.Map<AnulaComprobantesVentaViewModel>(comprobante);
            model.Items = _mapper.Map<List<AnulaComprobantesVentaDetalle>>(items);

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
            builder.Append($"AND IdTipoComprobante = {(int)TipoComprobante.AutomaticaAnulaFacturaClientes}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.ComprobanteVenta>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(AnulaComprobantesVentaViewModel model)
        {
            var comprobante = _mapper.Map<ComprobanteVenta>(model);
            var items = _mapper.Map<List<ComprobanteVentaItem>>(model.Items);

            Validate(model);

            var tran = _uow.BeginTransaction();

            try
            {
                var sucursalAnular = await _sucursalesRepository.GetById<Sucursal>(model.IdSucursalAnular, tran);
                var comprobanteAnular = await _comprobantesVentasRepository.GetComprobanteBySucNro(model.IdComprobanteAnular, sucursalAnular.CodigoSucursal, model.NumeroComprobanteAnular, _permissionsBusiness.Value.User.IdEmpresa, tran);
                var tipoComprobante = await _comprobantesVentasRepository.GetComprobanteCreditoBySucNro(model.IdComprobanteAnular, sucursalAnular.CodigoSucursal, model.NumeroComprobanteAnular, _permissionsBusiness.Value.User.IdEmpresa, tran);

                var sucNumero = await _sucursalesNumeracionRepository.GetLastNumeroByComprobante(tipoComprobante.IdComprobante, model.IdSucursalAnular, _permissionsBusiness.Value.User.IdEmpresa, tran);
                if (sucNumero == default)
                    throw new BusinessException("No Existe Numerador para el Tipo de Comprobante Utilizado. Verifique en Menu Sucursales");

                var numero = int.Parse(sucNumero.Numerador) + 1;

                comprobante.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                comprobante.IdComprobante = tipoComprobante.IdComprobante;
                comprobante.IdCliente = comprobanteAnular.IdCliente;
                comprobante.IdSucursal = model.IdSucursalAnular;
                comprobante.Sucursal = sucursalAnular.CodigoSucursal;
                comprobante.Numero = numero.ToString().PadLeft(8, '0');
                comprobante.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                comprobante.IdUsuario = _permissionsBusiness.Value.User.Id;
                comprobante.FechaIngreso = DateTime.Now;
                comprobante.Subtotal = items.Sum(i => i.Importe);
                comprobante.Total = items.Sum(i => i.Importe * (1 + (i.Alicuota / 100)));
                comprobante.IdTipoComprobante = (int)TipoComprobante.AutomaticaAnulaFacturaClientes;
                comprobante.Moneda = comprobanteAnular.Moneda;
                comprobante.Cotizacion = comprobanteAnular.Cotizacion;
                comprobante.CAE = string.Empty;
                comprobante.IdEstadoAFIP = (int)EstadoAFIP.Inicial;
                comprobante.FechaVto = model.Vto;

                var id = await _comprobantesVentasRepository.Insert(comprobante, tran);
                var iItem = 1;

                foreach (var item in items)
                {
                    await _comprobantesVentasItemRepository.Insert(new ComprobanteVentaItem
                    {
                        IdComprobanteVenta = (int)id,
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
                    await _comprobantesVentasTotalesRepository.Insert(
                        new ComprobanteVentaTotales
                        {
                            IdComprobanteVenta = (int)id,
                            Item = iItem++,
                            ImporteAlicuota = total.ImporteAlicuota,
                            Neto = total.Neto,
                            Alicuota = total.Alicuota,
                            IdTipoAlicuota = (int)TipoAlicuota.IVA
                        });
                }

                await _comprobantesVentasAnulacionesRepository.Insert(new ComprobanteVentaAnulacion
                {
                    IdComprobanteVenta = (int)id,
                    IdComprobanteVentaAnulado = comprobanteAnular.IdComprobanteVenta
                }, tran);

                #region Asiento Contable

                var asientoComprobanteAnular = await _comprobantesVentasAsientoRepository.GetByIdComprobanteVenta(comprobanteAnular.IdComprobanteVenta);

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

                await _comprobantesVentasAsientoRepository.Insert(new ComprobanteVentaAsiento { IdComprobanteVenta = (int)id, IdAsiento = (int)idAsiento }, tran);

                #endregion

                //Actualizo el Ultimo Nro de Comprobante en la Sucursal
                sucNumero.Numerador = comprobante.Numero;
                await _sucursalesNumeracionRepository.Update(sucNumero, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Remove(int idComprobanteVta)
        {
            var comprobante = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVta);

            if (comprobante == null)
            {
                throw new BusinessException("Comprobante de Venta inexistente");
            }

            if (comprobante.IdTipoComprobante != (int)TipoComprobante.AutomaticaAnulaFacturaClientes)
            {
                throw new BusinessException("El Tipo de Comprobante no es una N/Crédito Automática. No se puede Eliminar.");
            }

            if (comprobante.CAE.IsNotNull())
            {
                throw new BusinessException("El Comprobante presenta un Número de CAE. No se puede Eliminar.");
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

        private void Validate(AnulaComprobantesVentaViewModel comptobanteVta)
        {
            if (comptobanteVta.NumeroComprobanteAnular.IsNull())
            {
                throw new BusinessException("Ingrese el Numero de Comprobante a Anular");
            }

            if (!int.TryParse(comptobanteVta.NumeroComprobanteAnular, out int numero))
            {
                throw new BusinessException("El Numero de Comprobante a Anular es Incorrecto");
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

                var tranComprobante = _uow.BeginTransaction();

                Custom.DatosComprobanteVentaAFIP afip;

                try
                {
                    var comprobanteVta = await _comprobantesVentasRepository.GetComprobanteVentaAFIP(idComprobanteVta, tranComprobante);
                    var comprobanteAnular = await _comprobantesVentasAnulacionesRepository.GetByIdComprobanteVenta(idComprobanteVta, tranComprobante);
                    var comprobanteAnulado = await _comprobantesVentasRepository.GetComprobanteVentaAFIP(comprobanteAnular.IdComprobanteVentaAnulado, tranComprobante);
                    var bancos = (await _bancosRepository.GetAll<Banco>(tranComprobante)).Where(b => b.IdEmpresa == _permissionsBusiness.Value.User.IdEmpresa).ToList();
                    var primerDiaMes = new DateTime(comprobanteVta.Fecha.Year, comprobanteVta.Fecha.Month, 1);
                    var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

                    comprobanteVta.FchServDesde = primerDiaMes;
                    comprobanteVta.FchServHasta = ultimoDiaMes;
                    comprobanteVta.Concepto = (int)ConceptosComprobantesAFIP.COMPROBANTE_SERVICIOS;

                    if (comprobanteVta.EsCredito)
                    {
                        comprobanteVta.ComprobanteVentaCbtAsociadosAFIP.Add(new Custom.ComprobanteVentaCbtAsociadoAFIP
                        {
                            Tipo = comprobanteAnulado.CbteTipo,
                            CbteFch = comprobanteAnulado.Fecha,
                            Cuit = empresa.CUIT.Replace("-", string.Empty),
                            PtoVta = int.Parse(comprobanteAnulado.Sucursal),
                            Nro = int.Parse(comprobanteAnulado.Numero)
                        });

                        
                        if (comprobanteVta.EsMiPymes)
                            comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "22", Valor = "N" });
                    }

                    if (comprobanteVta.EsMiPymes && !comprobanteVta.EsCredito)
                    {
                        var banco = bancos.Where(b => b.EsDefault).FirstOrDefault() ?? bancos.FirstOrDefault();
                        comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "2101", Valor = banco?.CBU ?? string.Empty });
                        comprobanteVta.ComprobanteVentaOpcionalesAFIP.Add(new Custom.ComprobanteVentaOpcionalAFIP { Id = "27", Valor = "ADC" });
                    }

                    afip = await _empresasRepository.ValidComprobanteVentas(afipAuth, empresa.CUIT.Replace("-", string.Empty), comprobanteVta, empresa.EnableProdAFIP);

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

                    //Traigo la Entidad para hacer el UPDATE
                    var comprobanteVenta = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobanteVta, tranComprobante);
                    if (afip.Resultado == "A")
                    {
                        comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Autorizado;
                        comprobanteVenta.CAE = afip.CAE;
                        comprobanteVenta.VencimientoCAE = DateTime.ParseExact(afip.VencimientoCAE, "yyyyMMdd", null);

                        await _comprobantesVentasRepository.Update(comprobanteVenta, tranComprobante);

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(afip.Error))
                            comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Error;
                        if (!string.IsNullOrEmpty(afip.Observacion))
                            comprobanteVenta.IdEstadoAFIP = (int)EstadoAFIP.Observado;

                        await _comprobantesVentasRepository.Update(comprobanteVenta);
                    }

                    tranComprobante.Commit();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                    tranComprobante.Rollback();
                    throw new BusinessException(ex.Message);
                }

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

        public async Task<List<Custom.ComprobanteVentaItem>> GetItemsNCAnulaByFactura(int IdComprobante, string sucursal, string numero)
            => (await _comprobantesVentasItemRepository.GetItemsNCAnulaByFactura(IdComprobante, sucursal, numero, _permissionsBusiness.Value.User.IdEmpresa))
                    .Select(s => new Custom.ComprobanteVentaItem
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
