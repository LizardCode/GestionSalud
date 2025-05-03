using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CargosBanco;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CargosBancoBusiness: BaseBusiness, ICargosBancoBusiness
    {
        private readonly ILogger<AsientosBusiness> _logger;
        private readonly ICargosBancoRepository _cargosBancoRepository;
        private readonly ICargosBancoItemsRepository _cargosBancoItemsRepository;
        private readonly ICargosBancoAsientoRepository _cargosBancoAsientoRepository;
        private readonly ICargosBancoItemsComprobantesCompraRepository _cargosBancoItemsComprobantesCompraRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly IProveedoresRepository _proveedoresRepository;
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;
        private readonly IComprobantesComprasItemRepository _comprobantesComprasItemRepository;
        private readonly IComprobantesComprasTotalesRepository _comprobantesComprasTotalesRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public CargosBancoBusiness(
            ILogger<AsientosBusiness> logger,
            ICargosBancoRepository cargosBancoRepository,
            ICargosBancoItemsRepository cargosBancoItemsRepository,
            ICargosBancoAsientoRepository cargosBancoAsientoRepository,
            IAsientosRepository asientosRepository,
            IAsientosDetalleRepository asientosDetalleRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IBancosRepository bancosRepository,
            IProveedoresRepository proveedoresRepository,
            ICargosBancoItemsComprobantesCompraRepository cargosBancoItemsComprobantesCompraRepository,
            IComprobantesComprasRepository comprobantesComprasRepository,
            IComprobantesComprasItemRepository comprobantesComprasItemRepository,
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository,
            ICuentasContablesRepository cuentasContablesRepository)
        {
            _logger = logger;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cargosBancoRepository = cargosBancoRepository;
            _cargosBancoItemsRepository = cargosBancoItemsRepository;
            _cargosBancoAsientoRepository = cargosBancoAsientoRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _cierreMesRepository = cierreMesRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _bancosRepository = bancosRepository;
            _proveedoresRepository = proveedoresRepository;
            _cargosBancoItemsComprobantesCompraRepository = cargosBancoItemsComprobantesCompraRepository;
            _comprobantesComprasRepository = comprobantesComprasRepository;
            _comprobantesComprasItemRepository = comprobantesComprasItemRepository;
            _comprobantesComprasTotalesRepository = comprobantesComprasTotalesRepository;
        }

        public async Task<CargosBancoViewModel> Get(int idCargoBanco)
        {
            var cargoBanco = await _cargosBancoRepository.GetById<CargoBanco>(idCargoBanco);

            if (cargoBanco == null)
                return null;

            var items = await _cargosBancoItemsRepository.GetAllByIdCargoBanco(idCargoBanco);

            var model = _mapper.Map<CargosBancoViewModel>(cargoBanco);
            model.Items = _mapper.Map<List<CargosBancoItems>>(items);

            return model;
        }

        public async Task<Custom.CargoBanco> GetCustom(int idCargoBanco)
        {
            var cargoBanco = await _cargosBancoRepository.GetByIdCustom(idCargoBanco);
            var items = await _cargosBancoItemsRepository.GetAllByIdCargoBanco(idCargoBanco);

            cargoBanco.Items = items;

            return cargoBanco;
        }

        public async Task<DataTablesResponse<Custom.CargoBanco>> GetAll(DataTablesRequest request)
        {
            var customQuery = _cargosBancoRepository.GetAllCustomQuery();
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

            return await _dataTablesService.Resolve<Custom.CargoBanco>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(CargosBancoViewModel model)
        {
            var cargoBanco = _mapper.Map<CargoBanco>(model);

            Validate(cargoBanco, model.Items);

            var tran = _uow.BeginTransaction();
            var rnd = new Random();

            try
            {
                cargoBanco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                cargoBanco.Descripcion = cargoBanco.Descripcion.ToUpper().Trim();
                cargoBanco.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                cargoBanco.Total = model.Items.Sum(i => i.Total);
                cargoBanco.IdUsuario = _permissionsBusiness.Value.User.Id;
                cargoBanco.FechaIngreso = DateTime.Now;

                var idCargo = await _cargosBancoRepository.Insert(cargoBanco, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {
                    await _cargosBancoItemsRepository.Insert(new CargoBancoItem
                    {
                        IdCargoBanco = (int)idCargo,
                        Item = iItem++,
                        Detalle = item.Detalle.ToUpper().Trim(),
                        IdCuentaContable = item.IdCuentaContable,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        Importe = item.Importe,
                        Alicuota = item.IdAlicuota,
                        Total = item.Total
                    }, tran);
                    

                    #region Comprobantes de Compras

                    if (item.IdAlicuota > 0)
                    {
                        var bancoPro = await _bancosRepository.GetById<Banco>(cargoBanco.IdBanco, tran);
                        var proveedor = await _proveedoresRepository.GetById<Proveedor>(bancoPro.IdProveedor ?? 0, tran);

                        if(proveedor != default)
                        {
                            var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                            {
                                IdComprobante = (int)Comprobantes.OTROS_COMPROBANTES,
                                Sucursal = "00000",
                                Numero = rnd.Next(1, 99999999).ToString().PadLeft(8, '0'),
                                IdEjercicio = cargoBanco.IdEjercicio,
                                IdProveedor = proveedor.IdProveedor,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                Fecha = DateTime.Now,
                                FechaReal = cargoBanco.Fecha,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                IdUsuario = _permissionsBusiness.Value.User.Id,
                                FechaIngreso = DateTime.Now,
                                Subtotal = item.Importe,
                                Total = item.Importe * (1 + (item.IdAlicuota / 100)),
                                Percepciones = 0,
                                Moneda = Monedas.MonedaLocal.Description(),
                                Cotizacion = 1,
                                IdTipoComprobante = (int)TipoComprobante.CargosBancarios,
                                CAE = string.Empty,
                                VencimientoCAE = default,
                                IdEstadoAFIP = (int)EstadoAFIP.Inicial
                            }, tran);

                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 1,
                                Descripcion = item.Detalle.ToUpper().Trim(),
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = item.Importe,
                                Alicuota = item.IdAlicuota,
                                Impuestos = item.Importe * (item.IdAlicuota / 100)
                            }, tran);

                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 1,
                                Neto = item.Importe,
                                Alicuota = item.IdAlicuota,
                                ImporteAlicuota = item.Importe * (item.IdAlicuota / 100)
                            }, tran);

                            await _cargosBancoItemsComprobantesCompraRepository.Insert(new CargoBancoItemComprobanteCompra
                            {
                                IdCargoBanco = (int)idCargo,
                                Item = item.Item,
                                IdComprobanteCompra = (int)idComprobanteCompra
                            }, tran);
                            
                        }
                    }

                    #endregion

                }

                #region Asiento Contable

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = cargoBanco.IdEjercicio,
                    Descripcion = string.Concat("CARGO BANCARIO ", cargoBanco.Descripcion),
                    Fecha = cargoBanco.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var itemsContabilidad = model.Items.GroupBy(k => k.IdCuentaContable)
                    .Select(g => new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        IdCuentaContable = g.First().IdCuentaContable,
                        Detalle = g.First().Detalle.ToUpper().Trim(),
                        Debitos = g.Sum(i => i.Importe),
                        Creditos = 0
                    })
                    .ToList();
                
                var indItem = 1;
                foreach (var item in itemsContabilidad)
                {
                    idAsiento = (int)idAsiento;
                    item.Item = indItem++;
                    await _asientosDetalleRepository.Insert(item, tran);
                }

                var impuesto = model.Items.Sum(dc => dc.Importe * (dc.IdAlicuota / 100));
                if(impuesto > 0)
                {
                    var ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA, tran);
                    if (ctaCreditoFiscal != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)asiento.IdAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaCreditoFiscal.IdCuentaContable,
                            Detalle = ctaCreditoFiscal.Descripcion,
                            Debitos = impuesto,
                            Creditos = 0
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Credito Fiscal IVA. Verifique.");
                }

                var banco = await _bancosRepository.GetById<Banco>(cargoBanco.IdBanco);
                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)asiento.IdAsiento,
                    Item = indItem,
                    IdCuentaContable = banco.IdCuentaContable,
                    Detalle = cargoBanco.Descripcion.ToUpper().Trim(),
                    Debitos = 0,
                    Creditos = model.Items.Sum(dc => dc.Total)
                }, tran);

                await _cargosBancoAsientoRepository.Insert(new CargoBancoAsiento { IdCargoBanco = (int)idCargo, IdAsiento = (int)idAsiento }, tran);

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

        public async Task Remove(int idCargoBanco)
        {
            var cargoBanco = await _cargosBancoRepository.GetById<CargoBanco>(idCargoBanco);

            if (cargoBanco == null)
            {
                throw new BusinessException("Cargo de Banco inexistente");
            }

            if (_cierreMesRepository.MesCerrado(cargoBanco.IdEjercicio, cargoBanco.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Cargo Bancario se encuentra en un Mes Cerrado. No se puede Eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var cargoAsiento = await _cargosBancoAsientoRepository.GetByIdCargoBanco(cargoBanco.IdCargoBanco, tran);
                if (cargoAsiento != default)
                {
                    var asiento = await _asientosRepository.GetById<Asiento>(cargoAsiento.IdAsiento, tran);
                    await _asientosDetalleRepository.DeleteByIdAsiento(cargoAsiento.IdAsiento, tran);
                    await _cargosBancoAsientoRepository.DeleteByIdCargoBanco(cargoAsiento.IdCargoBanco, tran);

                    asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _asientosRepository.Update(asiento, tran);
                }

                var cargoComprobantesCompra = await _cargosBancoItemsComprobantesCompraRepository.GetByIdCargoBanco(idCargoBanco, tran);

                if(cargoComprobantesCompra.Count() > 0)
                {
                    foreach(var cargoComprobanteCompra in cargoComprobantesCompra)
                    {
                        var comprobante = await _comprobantesComprasRepository.GetById<ComprobanteCompra>(cargoComprobanteCompra.IdComprobanteCompra, tran);

                        await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(cargoComprobanteCompra.IdComprobanteCompra, tran);
                        await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(cargoComprobanteCompra.IdComprobanteCompra, tran);

                        comprobante.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                        await _comprobantesComprasRepository.Update(comprobante, tran);
                    }

                    await _cargosBancoItemsComprobantesCompraRepository.DeleteByIdCargoBanco(idCargoBanco, tran);
                }

                await _cargosBancoItemsRepository.DeleteByIdCargoBanco(idCargoBanco, tran);

                cargoBanco.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _cargosBancoRepository.Update(cargoBanco, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }


        public async Task Update(CargosBancoViewModel model)
        {
            var cargoBanco = _mapper.Map<CargoBanco>(model);

            Validate(cargoBanco, model.Items);

            var dbCargoBanco = await _asientosRepository.GetById<CargoBanco>(cargoBanco.IdCargoBanco);

            if (dbCargoBanco == null)
            {
                throw new BusinessException("Cargo de Banco inexistente");
            }

            var tran = _uow.BeginTransaction();
            var rnd = new Random();

            try
            {
                dbCargoBanco.IdEjercicio = cargoBanco.IdEjercicio;
                dbCargoBanco.Fecha = cargoBanco.Fecha;
                dbCargoBanco.Descripcion = cargoBanco.Descripcion.ToUpper().Trim();
                dbCargoBanco.FechaReal = cargoBanco.FechaReal;
                dbCargoBanco.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbCargoBanco.IdBanco = cargoBanco.IdBanco;
                dbCargoBanco.Total = model.Items.Sum(i => i.Total);
                dbCargoBanco.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbCargoBanco.IdUsuario = _permissionsBusiness.Value.User.Id;
                dbCargoBanco.FechaIngreso = DateTime.Now;

                await _cargosBancoRepository.Update(dbCargoBanco, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {
                    var cargoItem = await _cargosBancoItemsRepository.GetByIdCargoBancoAndItem(dbCargoBanco.IdCargoBanco, iItem, tran);

                    if (cargoItem == default)
                    {
                        await _cargosBancoItemsRepository.Insert(new CargoBancoItem
                        {
                            IdCargoBanco = dbCargoBanco.IdCargoBanco,
                            Item = iItem++,
                            Detalle = item.Detalle.ToUpper().Trim(),
                            IdCuentaContable = item.IdCuentaContable,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            Importe = item.Importe,
                            Alicuota = item.IdAlicuota,
                            Total = item.Total
                        }, tran);
                    }
                    else
                    {
                        cargoItem.IdCargoBanco = dbCargoBanco.IdCargoBanco;
                        cargoItem.Item = iItem++;
                        cargoItem.Detalle = item.Detalle.ToUpper().Trim();
                        cargoItem.IdCuentaContable = item.IdCuentaContable;
                        cargoItem.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                        cargoItem.Importe = item.Importe;
                        cargoItem.Alicuota = item.IdAlicuota;
                        cargoItem.Total = item.Total;

                        await _cargosBancoItemsRepository.Update(cargoItem, tran);
                    }

                    #region Comprobantes de Compras

                    var cargoComprobanteCompra = await _cargosBancoItemsComprobantesCompraRepository.GetByIdCargoBancoItem(dbCargoBanco.IdCargoBanco, item.Item, tran);

                    if (cargoComprobanteCompra != default)
                    {
                        await _cargosBancoItemsComprobantesCompraRepository.DeleteByIdComprobanteCompraItem(cargoComprobanteCompra.IdComprobanteCompra, cargoItem.Item, tran);
                        await _comprobantesComprasItemRepository.DeleteByIdComprobanteCompra(cargoComprobanteCompra.IdComprobanteCompra, tran);
                        await _comprobantesComprasTotalesRepository.DeleteByIdComprobanteCompra(cargoComprobanteCompra.IdComprobanteCompra, tran);
                        await _comprobantesComprasRepository.DeleteByIdComprobanteCompra(cargoComprobanteCompra.IdComprobanteCompra, tran);
                    }

                    if (item.IdAlicuota > 0)
                    {
                        var bancoPro = await _bancosRepository.GetById<Banco>(cargoBanco.IdBanco, tran);
                        var proveedor = await _proveedoresRepository.GetById<Proveedor>(bancoPro.IdProveedor ?? 0, tran);

                        if (proveedor != default)
                        {
                            var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                            {
                                IdComprobante = (int)Comprobantes.OTROS_COMPROBANTES,
                                Sucursal = "00000",
                                Numero = rnd.Next(1, 99999999).ToString().PadLeft(8, '0'),
                                IdEjercicio = cargoBanco.IdEjercicio,
                                IdProveedor = proveedor.IdProveedor,
                                IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                                Fecha = DateTime.Now,
                                FechaReal = cargoBanco.Fecha,
                                IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                                IdUsuario = _permissionsBusiness.Value.User.Id,
                                FechaIngreso = DateTime.Now,
                                Subtotal = item.Importe,
                                Total = item.Importe * (1 + (item.IdAlicuota / 100)),
                                Percepciones = 0,
                                Moneda = Monedas.MonedaLocal.Description(),
                                Cotizacion = 1,
                                IdTipoComprobante = (int)TipoComprobante.CargosBancarios,
                                CAE = string.Empty,
                                VencimientoCAE = default,
                                IdEstadoAFIP = (int)EstadoAFIP.Inicial
                            }, tran);

                            await _comprobantesComprasItemRepository.Insert(new ComprobanteCompraItem
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 1,
                                Descripcion = item.Detalle.ToUpper().Trim(),
                                Cantidad = 0,
                                Bonificacion = 0,
                                Precio = 0,
                                Importe = item.Importe,
                                Alicuota = item.IdAlicuota,
                                Impuestos = item.Importe * (item.IdAlicuota / 100)
                            }, tran);

                            await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                            {
                                IdComprobanteCompra = (int)idComprobanteCompra,
                                Item = 1,
                                Neto = item.Importe,
                                Alicuota = item.IdAlicuota,
                                ImporteAlicuota = item.Importe * (item.IdAlicuota / 100)
                            }, tran);

                            await _cargosBancoItemsComprobantesCompraRepository.Insert(new CargoBancoItemComprobanteCompra
                            {
                                IdCargoBanco = dbCargoBanco.IdCargoBanco,
                                Item = item.Item,
                                IdComprobanteCompra = (int)idComprobanteCompra
                            }, tran);
                        }
                    }

                    #endregion
                }

                #region Asiento Contable

                var cargoAsiento = await _cargosBancoAsientoRepository.GetByIdCargoBanco(dbCargoBanco.IdCargoBanco, tran);
                if(cargoAsiento != null)
                {
                    var oldAsiento = await _asientosRepository.GetById<Asiento>(cargoAsiento.IdAsiento, tran);
                    await _asientosDetalleRepository.DeleteByIdAsiento(cargoAsiento.IdAsiento, tran);
                    await _cargosBancoAsientoRepository.DeleteByIdCargoBanco(dbCargoBanco.IdCargoBanco, tran);

                    oldAsiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _asientosRepository.Update(oldAsiento, tran);
                }

                var asiento = new Asiento
                {
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdEjercicio = cargoBanco.IdEjercicio,
                    Descripcion = string.Concat("CARGO BANCARIO ", cargoBanco.Descripcion),
                    Fecha = cargoBanco.Fecha,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    FechaIngreso = DateTime.Now
                };

                var idAsiento = await _asientosRepository.Insert(asiento, tran);

                var itemsContabilidad = model.Items.GroupBy(k => k.IdCuentaContable)
                    .Select(g => new AsientoDetalle
                    {
                        IdAsiento = (int)idAsiento,
                        IdCuentaContable = g.First().IdCuentaContable,
                        Detalle = g.First().Detalle.ToUpper().Trim(),
                        Debitos = g.Sum(i => i.Importe),
                        Creditos = 0
                    })
                    .ToList();

                var indItem = 1;
                foreach (var item in itemsContabilidad)
                {
                    item.Item = indItem++;
                    await _asientosDetalleRepository.Insert(item, tran);
                }

                var impuesto = model.Items.Sum(dc => dc.Importe * (dc.IdAlicuota / 100));
                if (impuesto > 0)
                {
                    var ctaCreditoFiscal = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CREDITO_FISCAL_IVA);
                    if (ctaCreditoFiscal != null)
                    {
                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = ctaCreditoFiscal.IdCuentaContable,
                            Detalle = ctaCreditoFiscal.Descripcion,
                            Debitos = impuesto,
                            Creditos = 0
                        }, tran);
                    }
                    else
                        throw new BusinessException("No se ecnuentra la Cuenta Contable de Credito Fiscal IVA. Verifique.");
                }

                var banco = await _bancosRepository.GetById<Banco>(cargoBanco.IdBanco);
                await _asientosDetalleRepository.Insert(new AsientoDetalle
                {
                    IdAsiento = (int)idAsiento,
                    Item = indItem++,
                    IdCuentaContable = banco.IdCuentaContable,
                    Detalle = cargoBanco.Descripcion.ToUpper().Trim(),
                    Debitos = 0,
                    Creditos = model.Items.Sum(dc => dc.Total)
                }, tran);

                await _cargosBancoAsientoRepository.Insert(new CargoBancoAsiento { IdCargoBanco = dbCargoBanco.IdCargoBanco, IdAsiento = (int)idAsiento }, tran);

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

        private void Validate(CargoBanco cargoBanco, List<CargosBancoItems> items)
        {
            if (cargoBanco.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(cargoBanco.Descripcion));
            }

            if (_ejerciciosRepository.EjercicioCerrado(cargoBanco.IdEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(cargoBanco.IdEjercicio, cargoBanco.Fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha del Cargo Bancario no es Valida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(cargoBanco.IdEjercicio, cargoBanco.Fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para la Fecha del Comprobante esta Cerrado. Verifique.");
            }

            if (items == null || items.Count == 0)
            {
                throw new BusinessException("Ingrese al menos un Item para el Cargo Bancario.");
            }
        }

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {

            var asiento = await _cargosBancoAsientoRepository.GetByIdCargoBanco(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);

        }
    }
}
