using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace LizardCode.SalmaSalud.Application.Business
{
    public class SdoCtaCteCliBusiness: BaseBusiness, ISdoCtaCteCliBusiness
    {
        private readonly ILogger<SdoCtaCteCliBusiness> _logger;
        private readonly ISdoCtaCteCliRepository _sdoCtaCteCliRepository;
        private readonly ISdoCtaCteCliComprobantesVentasRepository _sdoCtaCteCliComprobantesVentasRepository;
        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;
        private readonly IComprobantesVentasTotalesRepository _comprobantesVentasTotalesRepository;
        private readonly ISucursalesRepository _sucursalesRepository;

        public SdoCtaCteCliBusiness(
            ISdoCtaCteCliRepository sdoCtaCteCliRepository,
            ISdoCtaCteCliComprobantesVentasRepository sdoCtaCteCliComprobantesVentasRepository,
            IComprobantesVentasRepository comprobantesVentasRepository,
            IComprobantesVentasTotalesRepository comprobantesVentasTotalesRepository,
            ISucursalesRepository sucursalesRepository,
            ILogger<SdoCtaCteCliBusiness> logger)
        {
            _sdoCtaCteCliRepository = sdoCtaCteCliRepository;
            _sdoCtaCteCliComprobantesVentasRepository = sdoCtaCteCliComprobantesVentasRepository;
            _comprobantesVentasRepository = comprobantesVentasRepository;
            _comprobantesVentasTotalesRepository = comprobantesVentasTotalesRepository;
            _sucursalesRepository = sucursalesRepository;
            _logger = logger;
        }


        public async Task New(SdoCtaCteCliViewModel model)
        {
            var sdoCtaCteCli = _mapper.Map<SdoCtaCteCli>(model);

            await Validate(sdoCtaCteCli);

            if (model.Items.Any(i => DateTime.Parse(i.Fecha) < sdoCtaCteCli.FechaDesde || DateTime.Parse(i.Fecha) > sdoCtaCteCli.FechaHasta))
            {
                throw new BusinessException($"Existen comprobantes con fecha fuera del rango descriptivo.");
            }

            var sucursales = (await _sucursalesRepository.GetAllSucursalesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa)).ToList();

            var tran = _uow.BeginTransaction();

            try
            {
                sdoCtaCteCli.Fecha = DateTime.Now;
                sdoCtaCteCli.Descripcion = sdoCtaCteCli.Descripcion.ToUpper().Trim();
                sdoCtaCteCli.IdUsuario = _permissionsBusiness.Value.User.Id;
                sdoCtaCteCli.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                sdoCtaCteCli.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                sdoCtaCteCli.Total = model.Items.Sum(s => s.Total);

                var idSdoCtaCteCli = await _sdoCtaCteCliRepository.Insert(sdoCtaCteCli, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {
                    var sucursal = item.Sucursal.PadLeft(5, '0');
                    var numero = item.Numero.PadLeft(8, '0');

                    var idSucursal = sucursales.FirstOrDefault(s => s.CodigoSucursal == sucursal)?.IdSucursal;
                    if (!idSucursal.HasValue)
                        throw new BusinessException($"Sucursal {sucursal} Inexistente");

                    var idComprobanteVenta = await _comprobantesVentasRepository.Insert(new ComprobanteVenta
                    {
                        IdCliente = item.IdCliente,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        Fecha = DateTime.Parse(item.Fecha),
                        FechaVto = DateTime.Parse(item.Vencimiento),
                        IdComprobante = item.IdComprobante,
                        IdSucursal = idSucursal.Value,
                        Sucursal = sucursal,
                        Numero = numero,
                        IdEjercicio = null,
                        IdTipoComprobante = (int)TipoComprobante.ManualClientes,
                        Moneda = Monedas.MonedaLocal.Description(),
                        Cotizacion = 1,
                        Total = item.Total,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        DescripcionUnica = string.Empty
                    }, tran);

                    await _comprobantesVentasTotalesRepository.Insert(new ComprobanteVentaTotales
                    {
                        IdComprobanteVenta = (int)idComprobanteVenta,
                        Item = iItem,
                        ImporteAlicuota = (item.Total - item.NetoGravado),
                        Neto = item.NetoGravado,
                        Alicuota = item.IdAlicuota,
                        IdTipoAlicuota = (int)TipoAlicuota.IVA
                    }, tran);

                    await _sdoCtaCteCliComprobantesVentasRepository.Insert(new SdoCtaCteCliComprobantesVentas { IdSaldoCtaCteCli = idSdoCtaCteCli, IdComprobanteVenta = idComprobanteVenta, Item = iItem }, tran);

                    iItem++;
                }

                tran.Commit();
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new InternalException();
            }
        }

        public async Task<SdoCtaCteCliViewModel> Get(int idSdoCtaCteCli)
        {
            var sdoCtaCteCli = await _sdoCtaCteCliRepository.GetById<SdoCtaCteCli>(idSdoCtaCteCli);

            if (sdoCtaCteCli == null)
                return null;

            var model = _mapper.Map<SdoCtaCteCliViewModel>(sdoCtaCteCli);

            return model;
        }

        public async Task<SdoCtaCteCliViewModel> GetCustom(int idSdoCtaCteCli)
        {
            var model = await Get(idSdoCtaCteCli);

            model.Items = _mapper.Map<List<SdoCtaCteCliDetalle>>(await _sdoCtaCteCliRepository.GetItemsByIdSaldoCtaCteCli(idSdoCtaCteCli));

            return model;
        }

        public async Task<DataTablesResponse<Domain.EntitiesCustom.SdoCtaCteCli>> GetAll(DataTablesRequest request)
        {
            var customQuery = _sdoCtaCteCliRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Domain.EntitiesCustom.SdoCtaCteCli >(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(SdoCtaCteCliViewModel model)
        {
            var sdoCtaCteCli = _mapper.Map<SdoCtaCteCli>(model);

            await Validate(sdoCtaCteCli);

            if (model.Items.Any(i => DateTime.Parse(i.Fecha) < sdoCtaCteCli.FechaDesde || DateTime.Parse(i.Fecha) > sdoCtaCteCli.FechaHasta))
            {
                throw new BusinessException($"Existen comprobantes con fecha fuera del rango descriptivo.");
            }

            var dbSdoCteCteCli = await _sdoCtaCteCliRepository.GetById<SdoCtaCteCli>(sdoCtaCteCli.IdSaldoCtaCteCli);

            if (dbSdoCteCteCli == null)
            {
                throw new BusinessException("Movimiento inexistente");
            }

            var sucursales = (await _sucursalesRepository.GetAllSucursalesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa)).ToList();

            var tran = _uow.BeginTransaction();

            try
            {
                dbSdoCteCteCli.Fecha = DateTime.Now;
                dbSdoCteCteCli.FechaDesde = model.FechaDesde;
                dbSdoCteCteCli.FechaHasta = model.FechaHasta;
                dbSdoCteCteCli.IdUsuario = _permissionsBusiness.Value.User.Id;
                dbSdoCteCteCli.Descripcion = sdoCtaCteCli.Descripcion.ToUpper().Trim();
                dbSdoCteCteCli.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbSdoCteCteCli.Total = model.Items.Sum(s => s.Total);

                var idSaldoCtaCteCli = dbSdoCteCteCli.IdSaldoCtaCteCli;

                //actualizo encabezado
                await _sdoCtaCteCliRepository.Update(dbSdoCteCteCli, tran);

                //Borro todas las asociaciones
                await _sdoCtaCteCliComprobantesVentasRepository.RemoveAllByIdSdoCtaCteCli(idSaldoCtaCteCli, tran);

                //Borro todos los TOTALES DE comprobantes ya generados...
                await _comprobantesVentasTotalesRepository.RemoveAllByIdSdoCtaCteCli(idSaldoCtaCteCli, tran);

                //Borro todos los comprobantes ya generados...
                await _comprobantesVentasRepository.RemoveAllByIdSdoCtaCteCli(idSaldoCtaCteCli, tran);

                var iItem = 1;
                foreach (var item in model.Items)
                {
                    var sucursal = item.Sucursal.PadLeft(5, '0');
                    var numero = item.Numero.PadLeft(8, '0');

                    var idSucursal = sucursales.FirstOrDefault(s => s.CodigoSucursal == sucursal)?.IdSucursal;
                    if (!idSucursal.HasValue)
                        throw new BusinessException($"Sucursal {sucursal} inexistente");

                    var idComprobanteVenta = await _comprobantesVentasRepository.Insert(new ComprobanteVenta
                    {
                        IdCliente = item.IdCliente,
                        IdUsuario = _permissionsBusiness.Value.User.Id,
                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                        Fecha = DateTime.Parse(item.Fecha),
                        FechaVto = DateTime.Parse(item.Vencimiento),
                        IdComprobante = item.IdComprobante,
                        IdSucursal = idSucursal.Value,
                        Sucursal = sucursal,
                        Numero = numero,
                        IdEjercicio = null,
                        IdTipoComprobante = (int)TipoComprobante.ManualClientes,
                        Moneda = Monedas.MonedaLocal.Description(),
                        Cotizacion = 1,
                        Total = item.Total,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        DescripcionUnica = string.Empty
                    }, tran);

                    await _comprobantesVentasTotalesRepository.Insert(new ComprobanteVentaTotales
                    {
                        IdComprobanteVenta = (int)idComprobanteVenta,
                        Item = iItem,
                        ImporteAlicuota = (item.Total - item.NetoGravado),
                        Neto = item.NetoGravado,
                        Alicuota = item.IdAlicuota, 
                        IdTipoAlicuota = (int)TipoAlicuota.IVA
                    }, tran);

                    await _sdoCtaCteCliComprobantesVentasRepository.Insert(new SdoCtaCteCliComprobantesVentas { IdSaldoCtaCteCli = idSaldoCtaCteCli, IdComprobanteVenta = idComprobanteVenta, Item = iItem }, tran);

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

        public async Task Remove(int idSdoCtaCteCli)
        {
            var sdoCtaCteCli = await _sdoCtaCteCliRepository.GetById<SdoCtaCteCli>(idSdoCtaCteCli);

            if (sdoCtaCteCli == null)
            {
                throw new BusinessException("Movimiento inexistente");
            }

            var idSaldoCtaCteCli = sdoCtaCteCli.IdSaldoCtaCteCli;

            var tran = _uow.BeginTransaction();

            try
            {
                sdoCtaCteCli.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                //Borro todas las asociaciones
                await _sdoCtaCteCliComprobantesVentasRepository.RemoveAllByIdSdoCtaCteCli(idSaldoCtaCteCli, tran);

                //Borro todos los TOTALES DE comprobantes ya generados...
                await _comprobantesVentasTotalesRepository.RemoveAllByIdSdoCtaCteCli(idSaldoCtaCteCli, tran);

                //Borro todos los comprobantes ya generados...
                await _comprobantesVentasRepository.RemoveAllByIdSdoCtaCteCli(idSaldoCtaCteCli, tran);

                await _sdoCtaCteCliRepository.Update(sdoCtaCteCli);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new InternalException();
            }
        }

        public async Task<List<SdoCtaCteCliDetalle>> ProcesarExcel(IFormFile file)
        {
            var resultados = new List<SdoCtaCteCliDetalle>();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<SdoCtaCteCliXLSViewModel>();

                if (excelRows.Count > 50)
                    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

                var clientes = await _lookupsBusiness.Value.GetAllClientesLookup();
                var comprobantes = (await _lookupsBusiness.Value.GetAllComprobantes()).ToList();

                var iItem = 1;
                foreach (var item in excelRows)
                {
                    var idCliente = clientes.FirstOrDefault(f => f.CUIT == item.CUIT.Trim())?.IdCliente;
                    var idComprobante = comprobantes.FirstOrDefault(f => f.Descripcion == item.Comprobante)?.IdComprobante;

                    var sucursal = item.Sucursal.PadLeft(5, '0');
                    var numero = item.Numero.PadLeft(8, '0');

                    resultados.Add(new SdoCtaCteCliDetalle
                    {
                        Item = iItem,
                        IdCliente = idCliente ?? 0,
                        Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                        Vencimiento = item.Vencimiento.ToString("dd/MM/yyyy"),
                        Sucursal = sucursal,
                        Numero = numero,
                        IdComprobante = idComprobante ?? 0,
                        IdAlicuota = item.Alicuota,
                        NetoGravado = item.Neto,
                        Total = item.Total
                    });
                    iItem++;
                }

                #region MOCK
                //MOCK
                //resultados.Add(new SdoCtaCteCliDetalle
                //{
                //    Item = 1,
                //    IdCliente = 1,
                //    Fecha = DateTime.Now,
                //    Vencimiento = DateTime.Now,
                //    IdComprobante = 1,
                //    Sucursal = "000001",
                //    Numero = "00000001",
                //    NetoGravado = 10000,
                //    IdAlicuota = 21,
                //    Total = 12100
                //});

                //resultados.Add(new SdoCtaCteCliDetalle
                //{
                //    Item = 2,
                //    IdCliente = 2,
                //    Fecha = DateTime.Now,
                //    Vencimiento = DateTime.Now,
                //    IdComprobante = 1,
                //    Sucursal = "000001",
                //    Numero = "00000002",
                //    NetoGravado = 100000,
                //    IdAlicuota = 21,
                //    Total = 121000
                //});
                #endregion

                if (resultados == null || resultados.Count == 0)
                    throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

                return resultados;
            }
            catch(BusinessException ex)
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

        private async Task Validate(SdoCtaCteCli sdoCtaCteCli)
        {
            if (sdoCtaCteCli.FechaDesde > sdoCtaCteCli.FechaHasta)
            {
                throw new BusinessException($"Rango de fechas inválido.");
            }

            if (sdoCtaCteCli.Descripcion.IsNull())
            {
                throw new BusinessException($"Ingrese una Descripción para el Comprobante.");
            }
        }

    }
}
