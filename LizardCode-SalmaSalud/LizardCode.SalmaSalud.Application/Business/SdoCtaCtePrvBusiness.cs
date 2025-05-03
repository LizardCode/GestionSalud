using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCtePrv;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace LizardCode.SalmaSalud.Application.Business
{
    public class SdoCtaCtePrvBusiness : BaseBusiness, ISdoCtaCtePrvBusiness
    {
        private readonly ILogger<SdoCtaCtePrvBusiness> _logger;
        private readonly ISdoCtaCtePrvRepository _sdoCtaCtePrvRepository;
        private readonly ISdoCtaCtePrvComprobantesComprasRepository _sdoCtaCtePrvComprobantesComprasRepository;
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;
        private readonly IComprobantesComprasTotalesRepository _comprobantesComprasTotalesRepository;

        public SdoCtaCtePrvBusiness(
            ISdoCtaCtePrvRepository sdoCtaCtePrvRepository,
            ISdoCtaCtePrvComprobantesComprasRepository sdoCtaCtePrvComprobantesComprasRepository,
            IComprobantesComprasRepository comprobantesComprasRepository,
            IComprobantesComprasTotalesRepository comprobantesComprasTotalesRepository,
            ILogger<SdoCtaCtePrvBusiness> logger)
        {
            _sdoCtaCtePrvRepository = sdoCtaCtePrvRepository;
            _sdoCtaCtePrvComprobantesComprasRepository = sdoCtaCtePrvComprobantesComprasRepository;
            _comprobantesComprasRepository = comprobantesComprasRepository;
            _comprobantesComprasTotalesRepository = comprobantesComprasTotalesRepository;
            _logger = logger;
        }


        public async Task New(SdoCtaCtePrvViewModel model)
        {
            var sdoCtaCtePrv = _mapper.Map<SdoCtaCtePrv>(model);

            await Validate(sdoCtaCtePrv);
                
            if (model.Items.Any(i => DateTime.Parse(i.Fecha) < sdoCtaCtePrv.FechaDesde || DateTime.Parse(i.Fecha) > sdoCtaCtePrv.FechaHasta))
            {
                throw new BusinessException($"Existen comprobantes con fecha fuera del rango descriptivo.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                sdoCtaCtePrv.Fecha = DateTime.Now;
                sdoCtaCtePrv.Descripcion = sdoCtaCtePrv.Descripcion.ToUpper().Trim();
                sdoCtaCtePrv.IdUsuario = _permissionsBusiness.Value.User.Id;
                sdoCtaCtePrv.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                sdoCtaCtePrv.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                sdoCtaCtePrv.Total = model.Items.Sum(s => s.Total);

                var idSdoCtaCtePrv = await _sdoCtaCtePrvRepository.Insert(sdoCtaCtePrv, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {
                    var sucursal = item.Sucursal.PadLeft(5, '0');
                    var numero = item.Numero.PadLeft(8, '0');

                    var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                    {
                        IdProveedor = item.IdProveedor,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        Fecha = DateTime.Parse(item.Fecha),
                        FechaVto = DateTime.Parse(item.Vencimiento),
                        FechaReal = DateTime.Parse(item.Fecha),
                        IdComprobante = item.IdComprobante,
                        Sucursal = sucursal,
                        Numero = numero,
                        IdEjercicio = null,
                        IdTipoComprobante = (int)TipoComprobante.ManualProveedores,
                        Moneda = Monedas.MonedaLocal.Description(),
                        Cotizacion = 1,
                        Subtotal= item.NetoGravado,
                        Percepciones = item.Percepciones,
                        Total = item.Total,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo
                    }, tran);

                    await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                    {
                        IdComprobanteCompra = (int)idComprobanteCompra,
                        Item = iItem,
                        ImporteAlicuota = (item.Total - item.NetoGravado),
                        Neto = item.NetoGravado,
                        Alicuota = item.IdAlicuota,
                    }, tran);

                    await _sdoCtaCtePrvComprobantesComprasRepository.Insert(new SdoCtaCtePrvComprobantesCompras { IdSaldoCtaCtePrv = idSdoCtaCtePrv, IdComprobanteCompra = idComprobanteCompra, Item = iItem }, tran);

                    iItem++;
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new InternalException();
            }
        }

        public async Task<SdoCtaCtePrvViewModel> Get(int idSdoCtaCtePrv)
        {
            var sdoCtaCtePrv = await _sdoCtaCtePrvRepository.GetById<SdoCtaCtePrv>(idSdoCtaCtePrv);

            if (sdoCtaCtePrv == null)
                return null;

            var model = _mapper.Map<SdoCtaCtePrvViewModel>(sdoCtaCtePrv);

            return model;
        }

        public async Task<SdoCtaCtePrvViewModel> GetCustom(int idSdoCtaCtePrv)
        {
            var model = await Get(idSdoCtaCtePrv);

            var items = await _sdoCtaCtePrvRepository.GetItemsByIdSaldoCtaCtePrv(idSdoCtaCtePrv);
            model.Items = _mapper.Map<List<SdoCtaCtePrvDetalle>>(items);

            return model;
        }

        public async Task<DataTablesResponse<Domain.EntitiesCustom.SdoCtaCtePrv>> GetAll(DataTablesRequest request)
        {
            var customQuery = _sdoCtaCtePrvRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Domain.EntitiesCustom.SdoCtaCtePrv>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(SdoCtaCtePrvViewModel model)
        {
            var sdoCtaCtePrv = _mapper.Map<SdoCtaCtePrv>(model);

            await Validate(sdoCtaCtePrv);

            if (model.Items.Any(i => DateTime.Parse(i.Fecha) < sdoCtaCtePrv.FechaDesde || DateTime.Parse(i.Fecha) > sdoCtaCtePrv.FechaHasta))
            {
                throw new BusinessException($"Existen comprobantes con fecha fuera del rango descriptivo.");
            }

            var dbSdoCteCtePrv = await _sdoCtaCtePrvRepository.GetById<SdoCtaCtePrv>(sdoCtaCtePrv.IdSaldoCtaCtePrv);

            if (dbSdoCteCtePrv == null)
            {
                throw new BusinessException("Movimiento inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbSdoCteCtePrv.Fecha = DateTime.Now;
                dbSdoCteCtePrv.FechaDesde = model.FechaDesde;
                dbSdoCteCtePrv.FechaHasta = model.FechaHasta;
                dbSdoCteCtePrv.IdUsuario = _permissionsBusiness.Value.User.Id;
                dbSdoCteCtePrv.Descripcion = sdoCtaCtePrv.Descripcion.ToUpper().Trim();
                dbSdoCteCtePrv.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbSdoCteCtePrv.Total = model.Items.Sum(s => s.Total);

                var idSaldoCtaCtePrv = dbSdoCteCtePrv.IdSaldoCtaCtePrv;

                //actualizo encabezado
                await _sdoCtaCtePrvRepository.Update(dbSdoCteCtePrv, tran);

                //Borro todas las asociaciones
                await _sdoCtaCtePrvComprobantesComprasRepository.RemoveAllByIdSdoCtaCtePrv(idSaldoCtaCtePrv, tran);

                //Borro todos los TOTALES de comprobantes ya generados...
                await _comprobantesComprasTotalesRepository.RemoveAllByIdSdoCtaCtePrv(idSaldoCtaCtePrv, tran);

                //Borro todos los comprobantes ya generados...
                await _comprobantesComprasRepository.RemoveAllByIdSdoCtaCtePrv(idSaldoCtaCtePrv, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {
                    var sucursal = item.Sucursal.PadLeft(5, '0');
                    var numero = item.Numero.PadLeft(8, '0');

                    var idComprobanteCompra = await _comprobantesComprasRepository.Insert(new ComprobanteCompra
                    {
                        IdProveedor = item.IdProveedor,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        Fecha = DateTime.Parse(item.Fecha),
                        FechaVto = DateTime.Parse(item.Vencimiento),
                        FechaReal = DateTime.Parse(item.Fecha),
                        IdComprobante = item.IdComprobante,
                        Sucursal = sucursal,
                        Numero = numero,
                        IdEjercicio = null,
                        IdTipoComprobante = (int)TipoComprobante.ManualProveedores,
                        Moneda = Monedas.MonedaLocal.Description(),
                        Cotizacion = 1,
                        Subtotal = item.NetoGravado,
                        Percepciones = item.Percepciones,
                        Total = item.Total,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo
                    }, tran);

                    await _comprobantesComprasTotalesRepository.Insert(new ComprobanteCompraTotales
                    {
                        IdComprobanteCompra = (int)idComprobanteCompra,
                        Item = iItem,
                        ImporteAlicuota = (item.Total - item.NetoGravado),
                        Neto = item.NetoGravado,
                        Alicuota = item.IdAlicuota
                    }, tran);

                    await _sdoCtaCtePrvComprobantesComprasRepository.Insert(new SdoCtaCtePrvComprobantesCompras { IdSaldoCtaCtePrv = idSaldoCtaCtePrv, IdComprobanteCompra = idComprobanteCompra, Item = iItem }, tran);

                    iItem++;
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new InternalException();
            }
        }

        public async Task Remove(int idSdoCtaCtePrv)
        {
            var sdoCtaCtePrv = await _sdoCtaCtePrvRepository.GetById<SdoCtaCtePrv>(idSdoCtaCtePrv);

            if (sdoCtaCtePrv == null)
            {
                throw new BusinessException("Movimiento inexistente");
            }

            var idSaldoCtaCtePrv = sdoCtaCtePrv.IdSaldoCtaCtePrv;

            var tran = _uow.BeginTransaction();

            try
            {
                sdoCtaCtePrv.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                //Borro todas las asociaciones
                await _sdoCtaCtePrvComprobantesComprasRepository.RemoveAllByIdSdoCtaCtePrv(idSaldoCtaCtePrv, tran);

                //Borro todos los TOTALES de comprobantes ya generados...
                await _comprobantesComprasTotalesRepository.RemoveAllByIdSdoCtaCtePrv(idSaldoCtaCtePrv, tran);

                //Borro todos los comprobantes ya generados...
                await _comprobantesComprasRepository.RemoveAllByIdSdoCtaCtePrv(idSaldoCtaCtePrv, tran);

                await _sdoCtaCtePrvRepository.Update(sdoCtaCtePrv);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new InternalException();
            }
        }

        public async Task<List<SdoCtaCtePrvDetalle>> ProcesarExcel(IFormFile file)
        {
            var resultados = new List<SdoCtaCtePrvDetalle>();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<SdoCtaCtePrvXLSViewModel>();

                if (excelRows.Count > 50)
                    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

                var proveedores = await _lookupsBusiness.Value.GetAllProveedoresByIdEmpresaLookup(_permissionsBusiness.Value.User.IdEmpresa);
                var comprobantes = (await _lookupsBusiness.Value.GetAllComprobantes()).ToList();

                var iItem = 1;
                foreach (var item in excelRows)
                {
                    var idProveedor = proveedores.FirstOrDefault(f => f.CUIT == item.CUIT.Trim())?.IdProveedor;
                    var idComprobante = comprobantes.FirstOrDefault(f => f.Descripcion == item.Comprobante)?.IdComprobante;

                    var sucursal = item.Sucursal.PadLeft(5, '0');
                    var numero = item.Numero.PadLeft(8, '0');

                    resultados.Add(new SdoCtaCtePrvDetalle
                    {
                        Item = iItem,
                        IdProveedor = idProveedor ?? 0,
                        Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                        Vencimiento = item.Vencimiento.ToString("dd/MM/yyyy"),
                        Sucursal = sucursal,
                        Numero = numero,
                        IdComprobante = idComprobante ?? 0,
                        IdAlicuota = item.Alicuota,
                        NetoGravado = item.Neto,
                        Percepciones = item.Percepciones,
                        Total = item.Total
                    });
                    iItem++;
                }

                #region MOCK
                //MOCK
                //resultados.Add(new SdoCtaCtePrvDetalle
                //{
                //    Item = 1,
                //    IdProveedor = 1,
                //    Fecha = DateTime.Now,
                //    Vencimiento = DateTime.Now,
                //    IdComprobante = 1,
                //    Sucursal = "000001",
                //    Numero = "00000001",
                //    NetoGravado = 10000,
                //    IdAlicuota = 21,
                //    Total = 12100
                //});

                //resultados.Add(new SdoCtaCtePrvDetalle
                //{
                //    Item = 2,
                //    IdProveedor = 2,
                //    Fecha = DateTime.Now,
                //    Vencimiento = DateTime.Now,
                //    IdComprobante = 1,
                //    Sucursal = "000001",
                //    Numero = "00000002",
                //    NetoGravado = 100000,
                //    IdAlicuota = 21,
                //    Percepciones = 9000,
                //    Total = 130000
                //});
                #endregion

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
        private async Task Validate(SdoCtaCtePrv sdoCtaCtePrv)
        {
            if (sdoCtaCtePrv.FechaDesde > sdoCtaCtePrv.FechaHasta)
            {
                throw new BusinessException($"Rango de fechas inválido.");
            }

            if (sdoCtaCtePrv.Descripcion.IsNull())
            {
                throw new BusinessException($"Ingrese una Descripción para el movimiento.");
            }
        }

    }
}
