using LizardCode.Framework.Helpers.ITextSharp;
using LizardCode.Framework.Helpers.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using LizardCode.Framework.Application.Common.Enums;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Impresiones;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using QRCoder;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ImpresionesBusiness : IImpresionesBusiness
    {
        private readonly bool _drawRulers;

        private readonly IPlantillasRepository _plantillasRpository;
        private readonly ILookupsBusiness _lookupsBusiness;
        private readonly IPermisosBusiness _permissionsBusiness;

        private readonly IEmpresasRepository _empresasRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly IProveedoresRepository _proveedoresRepository;

        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;
        private readonly IComprobantesVentasItemRepository _comprobantesVentasItemRepository;
        private readonly IComprobantesVentasTotalesRepository _comprobantesVentasTotalesRepository;

        private readonly IRecibosRepository _recibosRepository;
        private readonly IRecibosComprobantesRepository _recibosComprobantesRepository;
        private readonly IRecibosDetalleRepository _recibosDetalleRepository;
        private readonly IRecibosRetencionesRepository _recibosRetencionesRepository;

        private readonly IOrdenesPagoRepository _ordenesPagoRepository;
        private readonly IOrdenesPagoComprobantesRepository _ordenesPagoComprobantesRepository;
        private readonly IOrdenesPagoDetalleRepository _ordenesPagoDetalleRepository;
        private readonly IOrdenesPagoRetencionesRepository _ordenesPagoRetencionesRepository;
        private readonly IOrdenesPagoAnticiposRepository _ordenesPagoAnticiposRepository;
        private readonly IOrdenesPagoPlanillaGastosRepository _ordenesPagoPlanillaGastosRepository;
        private readonly IPlanillaGastosRepository _planillaGastosRepository;
        private readonly IPlanillaGastosItemsRepository _planillaGastosItemsRepository;

        private readonly IEvolucionesRecetasRepository _evolucionesRecetasRepository;
        private readonly IEvolucionesRepository _evolucionesRepository;
        private readonly IProfesionalesRepository _profesionalesRepository;
        private readonly IPacientesRepository _pacientesRepository;

        private readonly IFinanciadoresRepository _financiadoresRepository;
        private readonly IFinanciadoresPlanesRepository _financiadoresPlanesRepository;
        private readonly IArchivosRepository _archivosRepository;

        public ImpresionesBusiness(IPlantillasRepository plantillasRpository,
                                    ILookupsBusiness lookupsBusiness,
                                    IPermisosBusiness permissionsBusiness,
                                    IEmpresasRepository empresasRepository,
                                    IClientesRepository clientesRepository,
                                    IProveedoresRepository proveedoresRepository,

                                    IComprobantesVentasRepository comprobantesVentasRepository,
                                    IComprobantesVentasItemRepository comprobantesVentasItemRepository,
                                    IComprobantesVentasTotalesRepository comprobantesVentasTotalesRepository,

                                    IRecibosRepository recibosRepository,
                                    IRecibosComprobantesRepository recibosComprobantesRepository,
                                    IRecibosDetalleRepository recibosDetalleRepository,
                                    IRecibosRetencionesRepository recibosRetencionesRepository,

                                    IOrdenesPagoRepository ordenesPagoRepository,
                                    IOrdenesPagoComprobantesRepository ordenesPagoComprobantesRepository,
                                    IOrdenesPagoDetalleRepository ordenesPagoDetalleRepository,
                                    IOrdenesPagoRetencionesRepository ordenesPagoRetencionesRepository,
                                    IOrdenesPagoAnticiposRepository ordenesPagoAnticiposRepository,
                                    IOrdenesPagoPlanillaGastosRepository ordenesPagoPlanillaGastosRepository,
                                    IPlanillaGastosRepository planillaGastosRepository,
                                    IPlanillaGastosItemsRepository planillaGastosItemsRepository,

                                    IEvolucionesRecetasRepository evolucionesRecetasRepository,
                                    IEvolucionesRepository evolucionesRepository,
                                    IProfesionalesRepository profesionalesRepository,
                                    IPacientesRepository pacientesRepository,
                                    IFinanciadoresRepository financiadoresRepository,
                                    IFinanciadoresPlanesRepository financiadoresPlanesRepository,
                                    IArchivosRepository archivosRepository)
        {
            _drawRulers = "Impresiones:DrawRulers".FromAppSettings(false);

            _plantillasRpository = plantillasRpository;
            _lookupsBusiness = lookupsBusiness;
            _permissionsBusiness = permissionsBusiness;

            _empresasRepository = empresasRepository;
            _clientesRepository = clientesRepository;
            _proveedoresRepository = proveedoresRepository;

            _comprobantesVentasRepository = comprobantesVentasRepository;
            _comprobantesVentasItemRepository = comprobantesVentasItemRepository;
            _comprobantesVentasTotalesRepository = comprobantesVentasTotalesRepository;

            _recibosRepository = recibosRepository;
            _recibosComprobantesRepository = recibosComprobantesRepository;
            _recibosDetalleRepository = recibosDetalleRepository;
            _recibosRetencionesRepository = recibosRetencionesRepository;

            _ordenesPagoRepository = ordenesPagoRepository;
            _ordenesPagoComprobantesRepository = ordenesPagoComprobantesRepository;
            _ordenesPagoDetalleRepository = ordenesPagoDetalleRepository;
            _ordenesPagoRetencionesRepository = ordenesPagoRetencionesRepository;
            _ordenesPagoAnticiposRepository = ordenesPagoAnticiposRepository;
            _ordenesPagoPlanillaGastosRepository = ordenesPagoPlanillaGastosRepository;
            _planillaGastosRepository = planillaGastosRepository;
            _planillaGastosItemsRepository = planillaGastosItemsRepository;

            _evolucionesRecetasRepository = evolucionesRecetasRepository;
            _evolucionesRepository = evolucionesRepository;
            _profesionalesRepository = profesionalesRepository;
            _pacientesRepository = pacientesRepository;

            _financiadoresRepository = financiadoresRepository;
            _financiadoresPlanesRepository = financiadoresPlanesRepository;
            _archivosRepository = archivosRepository;
        }

        public async Task<PDF> GenerarImpresionFactura(int idComprobante)
        {
            await ValidarTipoUsuarioAutorizado(_permissionsBusiness.User.IdTipoUsuario,
                                                new List<TipoUsuario> {
                                                                    TipoUsuario.Administrador,
                                                                    TipoUsuario.Administracion
                                                });

            var factura = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(idComprobante);

            switch (factura.IdTipoComprobante)
            {
                case (int)TipoComprobante.ManualArticulos:
                    return await GenerarImpresionFacturaArticulos(factura);

                case (int)TipoComprobante.AutomaticaAnulaFacturaClientes:
                case (int)TipoComprobante.AutomaticaClientes:
                    return await GenerarImpresionFacturaOtros(factura, (int)TiposPlantilla.ComprobanteVentasAutomatico);

                case (int)TipoComprobante.ManualClientes:
                    return await GenerarImpresionFacturaOtros(factura, (int)TiposPlantilla.ComprobanteVentasManual);

                default:
                    return await GenerarImpresionFacturaOtros(factura, (int)TiposPlantilla.ComprobanteVentasAutomatico);
            }
        }

        private async Task<PDF> GenerarImpresionFacturaArticulos(ComprobanteVenta factura)
        {
            if (factura.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
                throw new UnauthorizedAccessException("No autorizado!!!");

            var empresa = await _empresasRepository.GetById<Empresa>(factura.IdEmpresa);
            var cliente = await _clientesRepository.GetById<Cliente>(factura.IdCliente);

            var items = await _comprobantesVentasItemRepository.GetAllByIdComprobanteVenta(factura.IdComprobanteVenta);
            var totales = await _comprobantesVentasTotalesRepository.GetAllByIdComprobanteVenta(factura.IdComprobanteVenta);

            var tipoIva = (TipoIVA)cliente.IdTipoIVA;
            var comprobantes = await _lookupsBusiness.GetAllComprobantes();
            var comprobante = comprobantes.FirstOrDefault(f => f.IdComprobante == factura.IdComprobante);
            var comprobanteB = comprobante.Letra == "B";

            var impuestos = string.Empty;
            var dImpuestos = 0d;
            foreach (var total in totales.Where(t => t.Alicuota != 0.5 && t.Alicuota != 1.3))
            {
                dImpuestos += total.ImporteAlicuota;
                impuestos += string.Format("{0}({1}{2}) {3} ", ((TipoAlicuota)total.IdTipoAlicuota).Description(), "%", total.Alicuota.ToString(), total.ImporteAlicuota.ToCurrency());
            }

            var cae = string.Empty;
            if (!string.IsNullOrEmpty(factura.CAE))
                cae = string.Format("C.A.E. N°: {0} - Vencimiento: {1}", factura.CAE, factura.VencimientoCAE.HasValue ? factura.VencimientoCAE.Value.ToString("dd/MM/yyyy") : string.Empty);

            var iibb_CABA = string.Empty;
            if (totales.Any(t => t.Alicuota == 0.5))
                iibb_CABA = "Percep. IIBB CABA:" + totales.FirstOrDefault(t => t.Alicuota == 0.5)?.ImporteAlicuota.ToCurrency(false, true);

            var iibb_BSAS = string.Empty;
            if (totales.Any(t => t.Alicuota == 1.3))
                iibb_CABA = "Percep. IIBB BSAS:" + totales.FirstOrDefault(t => t.Alicuota == 1.3)?.ImporteAlicuota.ToCurrency(false, true);

            string jsonContent = string.Empty;
            byte[] pdfBytes = null;
            var plantillas = await _plantillasRpository.GetPlantillasByTipo(_permissionsBusiness.User.IdEmpresa, (int)TiposPlantilla.ComprobanteVentasArticulos);
            if (plantillas != null && plantillas.Count > 0)
            {
                var plantilla = plantillas.FirstOrDefault();
                jsonContent = System.Text.Encoding.UTF8.GetString(plantilla.JSON);
                pdfBytes = plantilla.PDF;
            }
            else
            {
                //Si no logra leer la plantilla... hace el fallback al default de settings...
                var jsonPath = "Impresiones:Facturas:JsonPath".FromAppSettings<string>(notFoundException: true);
                jsonContent = Utils.GetJsonContent(jsonPath);
            }

            var leyendaUSD = factura.Moneda == Monedas.MonedaLocal.Description() ? string.Empty :
                                                "Tipo de cambio: u$s 1 = " + factura.Cotizacion.ToCurrency(true) + ".-";

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@LETRA]", comprobante.Letra.ToUpperInvariant())
                                .ReplaceLabel("[@CODIGO]", "Código N° " + comprobante.Codigo.ToString().ToUpperInvariant())
                                .ReplaceLabel("[@TIPO_COMPROBANTE]", comprobante.TipoComprobante.ToUpperInvariant())
                                .ReplaceLabel("[@COMPROBANTE]", string.Format("{0}-{1}", factura.Sucursal, factura.Numero))
                                .ReplaceLabel("[@FECHA]", factura.Fecha.ToString("dd/MM/yyyy"))
                                .ReplaceLabel("[@CUIT]", empresa.CUIT)
                                .ReplaceLabel("[@IIBB]", empresa.NroIBr)
                                .ReplaceLabel("[@FECHA_INICIO_ACTIVIDADES]", empresa.FechaInicioActividades.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@CLIENTE]", string.Format("{0}-{1}", cliente.IdCliente, cliente.NombreFantasia))
                                .ReplaceLabel("[@CLIENTE_DOMICILIO]", cliente.Direccion)
                                .ReplaceLabel("[@CLIENTE_LOCALIDAD]", cliente.Localidad)
                                .ReplaceLabel("[@CLIENTE_IVA]", tipoIva.Description())
                                .ReplaceLabel("[@CLIENTE_CUIT]", cliente.CUIT)
                                .ReplaceLabel("[@CONDICION_VENTA]", string.Empty)
                                .ReplaceLabel("[@VENCIMIENTO]", factura.FechaVto.HasValue ? "VENCIMIENTO:   " + factura.FechaVto.Value.ToString("dd/MM/yyyy") : "")

                                .ReplaceLabel("[@SUB_TOTAL]", comprobanteB ? (factura.Subtotal + dImpuestos).ToCurrency() : factura.Subtotal.ToCurrency())
                                //.ReplaceLabel("[@NO_GRAVADO]", "[VER...]")
                                //.ReplaceLabel("[@SUB_TOTAL]", "[VER...]")
                                .ReplaceLabel("[@IMPUESTOS]", comprobanteB ? string.Empty : impuestos)
                                .ReplaceLabel("[@TOTAL]", factura.Total.ToCurrency())

                                .ReplaceLabel("[@IIBB_CABA]", iibb_CABA)
                                .ReplaceLabel("[@IIBB_BSAS]", iibb_BSAS)

                                .ReplaceLabel("[@OBSERVACIONES]", string.Empty)
                                .ReplaceLabel("[@CAE]", cae)
                                .ReplaceLabel("[@REFERENCIA_COMERCIAL]", string.IsNullOrEmpty(factura.ReferenciaComercial) ? string.Empty : "Ref. Comercial: " + factura.ReferenciaComercial)
                                .ReplaceLabel("[@IMPORTE_LETRAS]", Convert.ToDecimal(factura.Total).NumeroALetras())
                                .ReplaceLabel("[@LEYENDA_DOLARES]", leyendaUSD); ;

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            //QR AFIP
            var qrElement = await ObtenerQRVentas(factura, comprobante, empresa, cliente);
            if (qrElement != null)
                jsonDocument.Elements.Add(qrElement);

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region ITEMS

            //var cntCols = 5;
            var table = new PdfPTable(new[] { 15f, 85f, 30f, 30f });
            var cellFont = ITFont.Helvetica.Size(10f).Font();


            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    var cellText = string.Empty;
                    var cellPaddingTop = 5f;
                    var cellPaddingLeft = 0f;
                    var cellBorderColorTop = BaseColor.LIGHT_GRAY;

                    //Cantidad
                    cellText = item.Cantidad.ToString();
                    var cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Center;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Descripcion
                    cellText = item.Descripcion.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Precio
                    cellText = item.Precio.ToCurrency(false, true);
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Total
                    var importe = comprobanteB ? (item.Cantidad * item.Precio) + item.Impuestos : (item.Cantidad * item.Precio);
                    cellText = importe.ToCurrency(false, true);
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            var filename = string.Format("Fac_{0}-{1}", factura.Sucursal, factura.Numero);
            return new PDF
            {
                Filename = $"{filename}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        private async Task<PDF> GenerarImpresionFacturaOtros(ComprobanteVenta factura, int tipoFactura)
        {
            if (factura.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
                throw new UnauthorizedAccessException("No autorizado!!!");

            var empresa = await _empresasRepository.GetById<Empresa>(factura.IdEmpresa);
            var cliente = await _clientesRepository.GetById<Cliente>(factura.IdCliente);

            var items = await _comprobantesVentasItemRepository.GetAllByIdComprobanteVenta(factura.IdComprobanteVenta);
            var totales = await _comprobantesVentasTotalesRepository.GetAllByIdComprobanteVenta(factura.IdComprobanteVenta);

            var tipoIva = (TipoIVA)cliente.IdTipoIVA;
            var comprobantes = await _lookupsBusiness.GetAllComprobantes();
            var comprobante = comprobantes.FirstOrDefault(f => f.IdComprobante == factura.IdComprobante);
            var comprobanteB = comprobante.Letra == "B";

            var impuestos = string.Empty;
            var dImpuestos = 0d;
            foreach (var total in totales.Where(t => t.Alicuota != 0.5 && t.Alicuota != 1.3))
            {
                dImpuestos += total.ImporteAlicuota;
                impuestos += string.Format("{0}({1}{2}) {3} ", ((TipoAlicuota)total.IdTipoAlicuota).Description(), "%", total.Alicuota.ToString(), total.ImporteAlicuota.ToCurrency());
            }

            var cae = string.Empty;
            if (!string.IsNullOrEmpty(factura.CAE))
                cae = string.Format("C.A.E. N°: {0} - Vencimiento: {1}", factura.CAE, factura.VencimientoCAE.HasValue ? factura.VencimientoCAE.Value.ToString("dd/MM/yyyy") : string.Empty);

            var iibb_CABA = string.Empty;
            if (totales.Any(t => t.Alicuota == 0.5))
                iibb_CABA = "Percep. IIBB CABA:" + totales.FirstOrDefault(t => t.Alicuota == 0.5)?.ImporteAlicuota.ToCurrency(false, true);

            var iibb_BSAS = string.Empty;
            if (totales.Any(t => t.Alicuota == 1.3))
                iibb_CABA = "Percep. IIBB BSAS:" + totales.FirstOrDefault(t => t.Alicuota == 1.3)?.ImporteAlicuota.ToCurrency(false, true);

            string jsonContent = string.Empty;
            byte[] pdfBytes = null;

            var plantillas = await _plantillasRpository.GetPlantillasByTipo(_permissionsBusiness.User.IdEmpresa, tipoFactura);
            if (plantillas != null && plantillas.Count > 0)
            {
                var plantilla = plantillas.FirstOrDefault();
                jsonContent = System.Text.Encoding.UTF8.GetString(plantilla.JSON);

                pdfBytes = plantilla.PDF;
            }
            else
            {
                //Si no logra leer la plantilla... hace el fallback al default de settings...
                var jsonPath = "Impresiones:Facturas:JsonPathOtros".FromAppSettings<string>(notFoundException: true);
                jsonContent = Utils.GetJsonContent(jsonPath);
            }

            var leyendaUSD = factura.Moneda == Monedas.MonedaLocal.Description() ? string.Empty : 
                                                "Tipo de cambio: u$s 1 = " + factura.Cotizacion.ToCurrency(true) + ".-";

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@LETRA]", comprobante.Letra.ToUpperInvariant())
                                .ReplaceLabel("[@CODIGO]", "Código N° " + comprobante.Codigo.ToString().ToUpperInvariant())
                                .ReplaceLabel("[@TIPO_COMPROBANTE]", comprobante.TipoComprobante.ToUpperInvariant())
                                .ReplaceLabel("[@COMPROBANTE]", string.Format("{0}-{1}", factura.Sucursal, factura.Numero))
                                .ReplaceLabel("[@FECHA]", factura.Fecha.ToString("dd/MM/yyyy"))
                                .ReplaceLabel("[@CUIT]", empresa.CUIT)
                                .ReplaceLabel("[@IIBB]", empresa.NroIBr)
                                .ReplaceLabel("[@FECHA_INICIO_ACTIVIDADES]", empresa.FechaInicioActividades.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@CLIENTE]", string.Format("{0}-{1}", cliente.IdCliente, cliente.NombreFantasia))
                                .ReplaceLabel("[@CLIENTE_DOMICILIO]", cliente.Direccion ?? string.Empty)
                                .ReplaceLabel("[@CLIENTE_LOCALIDAD]", cliente.Localidad ?? string.Empty)
                                .ReplaceLabel("[@CLIENTE_IVA]", tipoIva.Description())
                                .ReplaceLabel("[@CLIENTE_CUIT]", cliente.CUIT ?? cliente.Documento)
                                .ReplaceLabel("[@CONDICION_VENTA]", string.Empty)
                                .ReplaceLabel("[@VENCIMIENTO]", factura.FechaVto.HasValue ? "VENCIMIENTO:   " + factura.FechaVto.Value.ToString("dd/MM/yyyy") : "")

                                .ReplaceLabel("[@SUB_TOTAL]", comprobanteB ? (factura.Subtotal + dImpuestos).ToCurrency() : factura.Subtotal.ToCurrency())
                                //.ReplaceLabel("[@NO_GRAVADO]", "[VER...]")
                                //.ReplaceLabel("[@SUB_TOTAL]", "[VER...]")
                                .ReplaceLabel("[@IMPUESTOS]", comprobanteB ? string.Empty : impuestos)
                                .ReplaceLabel("[@TOTAL]", factura.Total.ToCurrency())

                                .ReplaceLabel("[@IIBB_CABA]", iibb_CABA)
                                .ReplaceLabel("[@IIBB_BSAS]", iibb_BSAS)

                                .ReplaceLabel("[@OBSERVACIONES]", string.Empty)
                                .ReplaceLabel("[@CAE]", cae)
                                .ReplaceLabel("[@REFERENCIA_COMERCIAL]", string.IsNullOrEmpty(factura.ReferenciaComercial) ? string.Empty : "Ref. Comercial: " + factura.ReferenciaComercial)
                                .ReplaceLabel("[@IMPORTE_LETRAS]", Convert.ToDecimal(factura.Total).NumeroALetras())
                                .ReplaceLabel("[@LEYENDA_DOLARES]", leyendaUSD);

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            //QR AFIP
            var qrElement = await ObtenerQRVentas(factura, comprobante, empresa, cliente);
            if (qrElement != null)
                jsonDocument.Elements.Add(qrElement);

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region ITEMS

            //var cntCols = 5;
            var table = new PdfPTable(new[] { 85f, 15f });
            var cellFont = ITFont.Helvetica.Size(10f).Font();

            if ((items != null && items.Count > 0) || !string.IsNullOrEmpty(factura.DescripcionUnica))
            {
                var cellText = string.Empty;
                var cellPaddingTop = 5f;
                var cellPaddingLeft = 0f;
                var cellBorderColorTop = BaseColor.LIGHT_GRAY;

                if (!string.IsNullOrEmpty(factura.DescripcionUnica))
                {
                    //Descripcion
                    cellText = factura.DescripcionUnica;
                    var cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Total
                    cellText = factura.Subtotal.ToCurrency(false, true);
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
                else
                { 
                    foreach (var item in items)
                    {
                        //Descripcion
                        cellText = item.Descripcion.ToString();
                        var cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Left;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        table.AddCell(cell);

                        //Total
                        var importe = comprobanteB ? item.Importe + item.Impuestos : item.Importe;
                        cellText = importe.ToCurrency(false, true);
                        cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Right;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        table.AddCell(cell);
                    }
                }
            }

            #endregion

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            var filename = string.Format("Fac_{0}-{1}", factura.Sucursal, factura.Numero);
            return new PDF
            {
                Filename = $"{filename}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }
        //public async Task<PDF> GenerarImpresionOrdenPago(int idOrdenPago)
        //{
        //    var factura = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(171);
        //    return await GenerarImpresionFacturaArticulos(factura);
        //}

        public async Task<PDF> GenerarImpresionOrdenPago(int idOrdenPago)
        {
            await ValidarTipoUsuarioAutorizado(_permissionsBusiness.User.IdTipoUsuario,
                                                new List<TipoUsuario> {
                                                                    TipoUsuario.Administrador,
                                                                    TipoUsuario.Tesoreria,
                                                                    TipoUsuario.CuentasPorPagar,
                                                                    TipoUsuario.Administracion
                                                                    //TipoUsuario.Proveedor
                                                });

            var ordenPago = await _ordenesPagoRepository.GetById<OrdenPago>(idOrdenPago);
            if (ordenPago.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
                throw new UnauthorizedAccessException("No autorizado!!!");

            //if (_permissionsBusiness.User.IdTipoUsuario == (int)TipoUsuario.Proveedor 
            //    && _permissionsBusiness.User.IdProveedor != ordenPago.IdProveedor)
            //    throw new UnauthorizedAccessException("No autorizado!!!");

            if (ordenPago.IdTipoOrdenPago == (int)TipoOrdenPago.Gastos)
            {
                return await GenerarImpresionOrdenPagoGastos(ordenPago);
            }
            else
            {
                return await GenerarImpresionOrdenPagoGeneral(ordenPago);
            }
        }

        public async Task<PDF> GenerarImpresionOrdenPagoGeneral(OrdenPago ordenPago)
        {
            var empresa = await _empresasRepository.GetById<Empresa>(ordenPago.IdEmpresa);

            var items = await _ordenesPagoDetalleRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);
            var ordenPagoComprobantes = await _ordenesPagoComprobantesRepository.GetComprobantesByIdOrdenPago(ordenPago.IdOrdenPago);
            var ordenPagoRetenciones = await _ordenesPagoRetencionesRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);

            //Anticipos
            List<OrdenPago> ordenPagoAnticipos = new List<OrdenPago>();
            //Si la OP es de tipo anticipo, va a taer solo uno... sino, va a traer los N anticipos...
            if (ordenPago.IdTipoOrdenPago == (int)TipoOrdenPago.Anticipo)
            {
                ordenPagoAnticipos.Add(ordenPago);
            }
            else
            {
                var anticipos = await _ordenesPagoAnticiposRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);
                if (anticipos != null && anticipos.Count > 0)
                {
                    foreach (var anticipo in anticipos)
                    {
                        var op = await _ordenesPagoRepository.GetById<OrdenPago>(anticipo.IdOrdenPago);
                        op.IdOrdenPago = anticipo.IdOrdenPagoAnticipo;
                        op.Importe = anticipo.Importe;
                        ordenPagoAnticipos.Add(op);
                    }

                }
            }

            var proveedor = await _proveedoresRepository.GetById<Proveedor>(ordenPago.IdProveedor.Value);
            var tipoIva = (TipoIVA)proveedor.IdTipoIVA;

            string jsonContent = string.Empty;
            byte[] pdfBytes = null;
            var plantillas = await _plantillasRpository.GetPlantillasByTipo(_permissionsBusiness.User.IdEmpresa, (int)TiposPlantilla.OrdenPago);
            if (plantillas != null && plantillas.Count > 0)
            {
                var plantilla = plantillas.FirstOrDefault();
                jsonContent = System.Text.Encoding.UTF8.GetString(plantilla.JSON);

                pdfBytes = plantilla.PDF;
            }
            else
            {
                //Si no logra leer la plantilla... hace el fallback al default de settings...
                var jsonPath = "Impresiones:OrdenesPago:JsonPath".FromAppSettings<string>(notFoundException: true);
                jsonContent = Utils.GetJsonContent(jsonPath);
            }

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@COMPROBANTE]", string.Format("Orden de Pago Nro.: {0}", ordenPago.IdOrdenPago))
                                .ReplaceLabel("[@FECHA]", ordenPago.Fecha.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@PROVEEDOR]", proveedor.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@PROVEEDOR_DIRECCION]", proveedor.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@PROVEEDOR_LOCALIDAD]", proveedor.Localidad.ToUpperInvariant())
                                .ReplaceLabel("[@PROVEEDOR_IVA]", tipoIva.Description())
                                .ReplaceLabel("[@PROVEEDOR_CUIT]", proveedor.CUIT);

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region COMPROBANTES

            var cellText = string.Empty;
            var cellPaddingTop = 5f;
            var cellPaddingLeft = 0f;
            var cellBorderColorTop = BaseColor.LIGHT_GRAY;

            var table = new PdfPTable(new[] { 20f, 50f, 30f });
            var cellFont = ITFont.Helvetica.Size(8f).Font();

            if (ordenPagoComprobantes != null && ordenPagoComprobantes.Count > 0)
            {
                double totalComprobantes = 0;

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "COMPROBANTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var comprobante in ordenPagoComprobantes)
                {
                    //Fecha
                    cellText = comprobante.Fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Descripcion
                    cellText = comprobante.NumeroComprobante.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = comprobante.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    totalComprobantes += comprobante.Importe;
                }

                //total
                var cellTotalText = totalComprobantes.ToCurrency();
                var cellTotal = new PdfPCell(new Phrase("TOTAL: " + cellTotalText, ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 3;
                table.AddCell(cellTotal);
            }

            #endregion

            #region ANTICIPOS

            if (ordenPagoAnticipos != null && ordenPagoAnticipos.Count > 0)
            {
                double totalActicipos = 0;

                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "DESCRIPCIÓN ANTICIPO";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var ordenPagoAnticipo in ordenPagoAnticipos)
                {
                    var descripcion = string.Empty;

                    var fecha = ordenPago.Fecha;

                    //Fecha
                    cellText = fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Anticipos
                    var descripcionA = string.Empty;
                    if (ordenPago.IdTipoOrdenPago != (int)TipoOrdenPago.Anticipo)
                        descripcionA = string.Format("ID: {0} - {1}", ordenPagoAnticipo.IdOrdenPago, ordenPagoAnticipo.Descripcion);
                    else
                        descripcionA = ordenPagoAnticipo.Descripcion;
                    cellText = descripcionA;
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = ordenPagoAnticipo.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    totalActicipos += ordenPagoAnticipo.Importe;
                }

                //total
                var cellTotalText = totalActicipos.ToCurrency();
                var cellTotal = new PdfPCell(new Phrase("TOTAL: " + cellTotalText, ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 3;
                table.AddCell(cellTotal);
            }

            #endregion

            #region PAGOS

            if (items != null && items.Count > 0)
            {
                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "MEDIO DE PAGO";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var item in items)
                {
                    var descripcion = string.Empty;

                    var fecha = ordenPago.Fecha;

                    if (item.FechaTransferencia.HasValue)
                        fecha = item.FechaTransferencia.Value;

                    if (item.FechaVto.HasValue)
                        fecha = item.FechaVto.Value;

                    //Fecha
                    cellText = fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Comprobante
                    cellText = item.Descripcion.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = item.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            #region RETENCIONES

            if (ordenPagoRetenciones != null && ordenPagoRetenciones.Count > 0)
            {
                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "RETENCIÓN";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var retencion in ordenPagoRetenciones)
                {
                    var tipoRetencion = (TipoRetencion)retencion.IdTipoRetencion;
                    var descripcion = string.Empty;

                    //Fecha
                    cellText = retencion.Fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Comprobante
                    cellText = tipoRetencion.Description();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = retencion.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            var filename = string.Format("Ord_{0}", ordenPago.IdOrdenPago);
            return new PDF
            {
                Filename = $"{filename}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        public async Task<PDF> GenerarImpresionOrdenPagoGastos(OrdenPago ordenPago)
        {
            var empresa = await _empresasRepository.GetById<Empresa>(ordenPago.IdEmpresa);

            var planillas = await _ordenesPagoPlanillaGastosRepository.GetPlanillasGastosByIdOrdenPago(ordenPago.IdOrdenPago);

            var items = await _ordenesPagoDetalleRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);
            //var ordenPagoComprobantes = await _ordenesPagoComprobantesRepository.GetComprobantesByIdOrdenPago(ordenPago.IdOrdenPago);
            var ordenPagoRetenciones = await _ordenesPagoRetencionesRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);

            //Anticipos
            List<OrdenPago> ordenPagoAnticipos = new List<OrdenPago>();
            //Si la OP es de tipo anticipo, va a taer solo uno... sino, va a traer los N anticipos...
            if (ordenPago.IdTipoOrdenPago == (int)TipoOrdenPago.Anticipo)
            {
                ordenPagoAnticipos.Add(ordenPago);
            }
            else
            {
                var anticipos = await _ordenesPagoAnticiposRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);
                if (anticipos != null && anticipos.Count > 0)
                {
                    foreach (var anticipo in anticipos)
                    {
                        var op = await _ordenesPagoRepository.GetById<OrdenPago>(anticipo.IdOrdenPago);
                        op.IdOrdenPago = anticipo.IdOrdenPagoAnticipo;
                        op.Importe = anticipo.Importe;
                        ordenPagoAnticipos.Add(op);
                    }

                }
            }

            //var proveedor = await _proveedoresRepository.GetById<Proveedor>(ordenPago.IdProveedor.Value);
            //var tipoIva = (TipoIVA)proveedor.IdTipoIVA;

            string jsonContent = string.Empty;
            byte[] pdfBytes = null;
            var plantillas = await _plantillasRpository.GetPlantillasByTipo(_permissionsBusiness.User.IdEmpresa, (int)TiposPlantilla.OrdenPago);
            if (plantillas != null && plantillas.Count > 0)
            {
                var plantilla = plantillas.FirstOrDefault();
                jsonContent = System.Text.Encoding.UTF8.GetString(plantilla.JSON);

                pdfBytes = plantilla.PDF;
            }
            else
            {
                //Si no logra leer la plantilla... hace el fallback al default de settings...
                var jsonPath = "Impresiones:OrdenesPago:JsonPath".FromAppSettings<string>(notFoundException: true);
                jsonContent = Utils.GetJsonContent(jsonPath);
            }

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@COMPROBANTE]", string.Format("Orden de Pago Nro.: {0}", ordenPago.IdOrdenPago))
                                .ReplaceLabel("[@FECHA]", ordenPago.Fecha.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@PROVEEDOR]", "GASTOS")
                                .ReplaceLabel("[@PROVEEDOR_DIRECCION]", string.Empty)
                                .ReplaceLabel("[@PROVEEDOR_LOCALIDAD]", string.Empty)
                                .ReplaceLabel("[@PROVEEDOR_IVA]", string.Empty)
                                .ReplaceLabel("[@PROVEEDOR_CUIT]", string.Empty);

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region COMPROBANTES

            var cellText = string.Empty;
            var cellPaddingTop = 5f;
            var cellPaddingLeft = 0f;
            var cellBorderColorTop = BaseColor.LIGHT_GRAY;

            var table = new PdfPTable(new[] { 20f, 20f, 30f, 30f });
            var cellFont = ITFont.Helvetica.Size(8f).Font();

            if (planillas != null && planillas.Count > 0)
            {

                double totalGastos = 0;

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "COMPROBANTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "PROVEEDOR";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var planilla in planillas)
                {
                    var gastos = await _planillaGastosItemsRepository.GetByIdPlanillaGastos(planilla.IdPlanillaGastos);

                    foreach (var gasto in gastos)
                    {
                        //Fecha
                        cellText = gasto.Fecha.ToString("dd/MM/yyyy");
                        cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Left;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        table.AddCell(cell);

                        //Descripcion
                        cellText = gasto.NumeroComprobante.ToString();
                        cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Left;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        table.AddCell(cell);

                        //Proveedor
                        cellText = gasto.Proveedor.ToString();
                        cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Left;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        table.AddCell(cell);

                        //Importe
                        cellText = gasto.Total.ToCurrency();
                        cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Right;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        table.AddCell(cell);

                        totalGastos += gasto.Total;
                    }
                }

                //total
                var cellTotalText = totalGastos.ToCurrency();
                var cellTotal = new PdfPCell(new Phrase("TOTAL: " + cellTotalText, ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 4;
                table.AddCell(cellTotal);
            }

            #endregion

            #region ANTICIPOS

            if (ordenPagoAnticipos != null && ordenPagoAnticipos.Count > 0)
            {
                double totalActicipos = 0;

                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "DESCRIPCIÓN ANTICIPO";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var ordenPagoAnticipo in ordenPagoAnticipos)
                {
                    var descripcion = string.Empty;

                    var fecha = ordenPago.Fecha;

                    //Fecha
                    cellText = fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Anticipos
                    var descripcionA = string.Empty;
                    if (ordenPago.IdTipoOrdenPago != (int)TipoOrdenPago.Anticipo)
                        descripcionA = string.Format("ID: {0} - {1}", ordenPagoAnticipo.IdOrdenPago, ordenPagoAnticipo.Descripcion);
                    else
                        descripcionA = ordenPagoAnticipo.Descripcion;
                    cellText = descripcionA;
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = ordenPagoAnticipo.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    totalActicipos += ordenPagoAnticipo.Importe;
                }

                //total
                var cellTotalText = totalActicipos.ToCurrency();
                var cellTotal = new PdfPCell(new Phrase("TOTAL: " + cellTotalText, ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 3;
                table.AddCell(cellTotal);
            }

            #endregion

            #region PAGOS

            if (items != null && items.Count > 0)
            {
                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "MEDIO DE PAGO";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var item in items)
                {
                    var descripcion = string.Empty;

                    var fecha = ordenPago.Fecha;

                    if (item.FechaTransferencia.HasValue)
                        fecha = item.FechaTransferencia.Value;

                    if (item.FechaEmision.HasValue)
                        fecha = item.FechaEmision.Value;

                    //Fecha
                    cellText = fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Comprobante
                    cellText = item.Descripcion.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = item.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            #region RETENCIONES

            if (ordenPagoRetenciones != null && ordenPagoRetenciones.Count > 0)
            {
                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "RETENCIÓN";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var retencion in ordenPagoRetenciones)
                {
                    var tipoRetencion = (TipoRetencion)retencion.IdTipoRetencion;
                    var descripcion = string.Empty;

                    //Fecha
                    cellText = retencion.Fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Comprobante
                    cellText = tipoRetencion.Description();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = retencion.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            var filename = string.Format("Ord_{0}", ordenPago.IdOrdenPago);
            return new PDF
            {
                Filename = $"{filename}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        //public async Task<PDF> GenerarImpresionRecibo(int idRecibo)
        //{
        //    var factura = await _comprobantesVentasRepository.GetById<ComprobanteVenta>(70);
        //    return await GenerarImpresionFacturaOtros(factura);
        //}

        public async Task<PDF> GenerarImpresionRecibo(int idRecibo)
        {
            await ValidarTipoUsuarioAutorizado(_permissionsBusiness.User.IdTipoUsuario,
                                                new List<TipoUsuario> {
                                                                    TipoUsuario.Administrador,
                                                                    TipoUsuario.Tesoreria,
                                                                    TipoUsuario.CuentasPorCobrar,
                                                                    TipoUsuario.Administracion
                                                });

            var recibo = await _recibosRepository.GetById<Recibo>(idRecibo);
            if (recibo.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
                throw new UnauthorizedAccessException("No autorizado!!!");

            var empresa = await _empresasRepository.GetById<Empresa>(recibo.IdEmpresa);

            //var reciboComprobantes = await _recibosComprobantesRepository.(idRecibo);
            var items = await _recibosDetalleRepository.GetAllByIdRecibo(recibo.IdRecibo);
            var reciboComprobantes = await _recibosComprobantesRepository.GetComprobantesByIdRecibo(recibo.IdRecibo);
            var reciboRetenciones = await _recibosRetencionesRepository.GetAllByIdRecibo(recibo.IdRecibo);

            var cliente = await _clientesRepository.GetById<Cliente>(recibo.IdCliente);
            var tipoIva = (TipoIVA)cliente.IdTipoIVA;

            string jsonContent = string.Empty;
            byte[] pdfBytes = null;
            var plantillas = await _plantillasRpository.GetPlantillasByTipo(_permissionsBusiness.User.IdEmpresa, (int)TiposPlantilla.Recibo);
            if (plantillas != null && plantillas.Count > 0)
            {
                var plantilla = plantillas.FirstOrDefault();
                jsonContent = System.Text.Encoding.UTF8.GetString(plantilla.JSON);

                pdfBytes = plantilla.PDF;
            }
            else
            {
                //Si no logra leer la plantilla... hace el fallback al default de settings...
                var jsonPath = "Impresiones:Recibos:JsonPath".FromAppSettings<string>(notFoundException: true);
                jsonContent = Utils.GetJsonContent(jsonPath);
            }

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@COMPROBANTE]", string.Format("Recibo Nro.: {0}", recibo.IdRecibo))
                                .ReplaceLabel("[@FECHA]", recibo.Fecha.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@CLIENTE]", cliente.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@CLIENTE_DIRECCION]", cliente.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@CLIENTE_LOCALIDAD]", cliente.Localidad.ToUpperInvariant())
                                .ReplaceLabel("[@CLIENTE_IVA]", tipoIva.Description())
                                .ReplaceLabel("[@CLIENTE_CUIT]", cliente.CUIT);

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region COMPROBANTES

            var cellText = string.Empty;
            var cellPaddingTop = 5f;
            var cellPaddingLeft = 0f;
            var cellBorderColorTop = BaseColor.LIGHT_GRAY;

            var table = new PdfPTable(new[] { 20f, 50f, 30f });
            var cellFont = ITFont.Helvetica.Size(8f).Font();

            if (reciboComprobantes != null && reciboComprobantes.Count > 0)
            {
                double totalComprobantes = 0;

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "COMPROBANTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var comprobante in reciboComprobantes)
                {
                    var esNotaCredito = false;
                    var comprobanteVenta = await _comprobantesVentasRepository.GetByIdCustom(comprobante.IdComprobanteVenta);
                    esNotaCredito = EsComprobante(comprobanteVenta.IdComprobante, new List<Comprobantes>
                                                    {
                                                        Comprobantes.NCREDITO_A,
                                                        Comprobantes.NCREDITO_B,
                                                        Comprobantes.NCREDITO_C,
                                                        Comprobantes.NCREDITO_E,
                                                        Comprobantes.NCREDITO_MIPYME_A,
                                                        Comprobantes.NCREDITO_MIPYME_B,
                                                        Comprobantes.NCREDITO_MIPYME_C,
                                                        Comprobantes.NCREDITO_M
                                                    });

                    //Fecha
                    cellText = comprobante.Fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Descripcion
                    cellText = comprobante.TipoComprobante + " " + comprobante.NumeroComprobante.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = (esNotaCredito ? "-" : string.Empty) + comprobante.Total.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    if (esNotaCredito)
                        totalComprobantes -= comprobante.Total;
                    else
                        totalComprobantes += comprobante.Total;
                }

                //total
                var cellTotalText = totalComprobantes.ToCurrency();
                var cellTotal = new PdfPCell(new Phrase("TOTAL: " + cellTotalText, ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 3;
                table.AddCell(cellTotal);
            }

            #endregion

            #region PAGOS

            if (items != null && items.Count > 0)
            {
                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "MEDIO DE PAGO";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var item in items)
                {
                    var descripcion = string.Empty;

                    var fecha = recibo.Fecha;

                    if (item.FechaTransferencia.HasValue)
                        fecha = item.FechaTransferencia.Value;

                    if (item.FechaEmision.HasValue)
                        fecha = item.FechaEmision.Value;

                    //Fecha
                    cellText = fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Comprobante
                    cellText = item.Descripcion.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = item.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            #region RETENCIONES

            if (reciboRetenciones != null && reciboRetenciones.Count > 0)
            {
                //Agregar fila vacía
                var cellEmpty = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(10f).Font()));
                cellEmpty.HorizontalAlignment = (int)Align.Left;
                cellEmpty.PaddingTop = 5f;
                cellEmpty.PaddingLeft = 0f;
                cellEmpty.Border = 0;
                cellEmpty.BorderWidthTop = 0;
                cellEmpty.Colspan = 3;
                table.AddCell(cellEmpty);

                //Agregar Títulos
                cellText = "FECHA";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "RETENCIÓN";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var retencion in reciboRetenciones)
                {
                    var descripcion = string.Empty;

                    //Fecha
                    cellText = retencion.Fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Comprobante
                    cellText = retencion.Categoria.ToUpperInvariant();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = retencion.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);
                }
            }

            #endregion

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            var filename = string.Format("Rec_{0}", recibo.IdRecibo);
            return new PDF
            {
                Filename = $"{filename}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        public async Task<PDF> GenerarImpresionRetenciones(int idOrdenPago, int idTipoRetencion)
        {
            await ValidarTipoUsuarioAutorizado(_permissionsBusiness.User.IdTipoUsuario,
                                                new List<TipoUsuario> {
                                                                    TipoUsuario.Administrador,
                                                                    TipoUsuario.Tesoreria,
                                                                    TipoUsuario.CuentasPorPagar,
                                                                    TipoUsuario.Administracion
                                                                    //TipoUsuario.Proveedor
                                                });

            var ordenPago = await _ordenesPagoRepository.GetById<OrdenPago>(idOrdenPago);
            if (ordenPago.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
                throw new UnauthorizedAccessException("No autorizado!!!");

            //if (_permissionsBusiness.User.IdTipoUsuario == (int)TipoUsuario.Proveedor 
            //    && _permissionsBusiness.User.IdProveedor != ordenPago.IdProveedor)
            //    throw new UnauthorizedAccessException("No autorizado!!!");

            var empresa = await _empresasRepository.GetById<Empresa>(ordenPago.IdEmpresa);

            var items = await _ordenesPagoDetalleRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);
            var ordenPagoComprobantes = await _ordenesPagoComprobantesRepository.GetComprobantesByIdOrdenPago(ordenPago.IdOrdenPago);
            var ordenPagoRetenciones = await _ordenesPagoRetencionesRepository.GetAllByIdOrdenPago(ordenPago.IdOrdenPago);
            if (ordenPagoRetenciones == null || ordenPagoRetenciones.Count == 0)
                throw new BusinessException("No existen retenciones asociadas a la Orden de Pago.");

            var ordenPagoRetencion = ordenPagoRetenciones.FirstOrDefault();
            if (ordenPagoRetencion == null)
                throw new BusinessException("No existe el tipo de retención indicada asociada a la Orden de Pago.");

            var proveedor = await _proveedoresRepository.GetById<Proveedor>(ordenPago.IdProveedor.Value);
            var tipoIva = (TipoIVA)proveedor.IdTipoIVA;


            //var regimen = "ENAJENACION DE BS MUEBLES Y BS DE CAMBIO.";
            var regimen = string.Empty;
            var codigosRetencion = await _proveedoresRepository.GetCodigosRetencionByIdProveedor(ordenPago.IdProveedor.Value);
            var codigoRetencion = codigosRetencion.FirstOrDefault(e => e.IdTipoRetencion == idTipoRetencion);
            if (codigoRetencion == null)
                throw new BusinessException("No existe el tipo de retención indicada asociada a la Orden de Pago.");

            regimen = string.Format("{0} - {1}", codigoRetencion.Regimen, codigoRetencion.Descripcion);

            string jsonContent = string.Empty;
            byte[] pdfBytes = null;
            var plantillas = await _plantillasRpository.GetPlantillasByTipo(_permissionsBusiness.User.IdEmpresa, (int)TiposPlantilla.Retenciones);
            if (plantillas != null && plantillas.Count > 0)
            {
                var plantilla = plantillas.FirstOrDefault();
                jsonContent = System.Text.Encoding.UTF8.GetString(plantilla.JSON);

                pdfBytes = plantilla.PDF;
            }
            else
            {
                //Si no logra leer la plantilla... hace el fallback al default de settings...
                var jsonPath = "Impresiones:Retenciones:JsonPath".FromAppSettings<string>(notFoundException: true);
                jsonContent = Utils.GetJsonContent(jsonPath);
            }

            //Textos default...
            var tipoRetencion = (TipoRetencion)idTipoRetencion;
            var titulo = "Impresiones:Retenciones:Titulo".FromAppSettings<string>(notFoundException: true);
            titulo += " " + tipoRetencion.Description().ToUpperInvariant();
            var texto = "Impresiones:Retenciones:Texto".FromAppSettings<string>(notFoundException: true);
            texto += "\r\n" + regimen;
            var footer = "Impresiones:Retenciones:Footer".FromAppSettings<string>(notFoundException: true);

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@COMPROBANTE]", string.Format("Retención Nro.: {0}", ordenPagoRetencion.IdOrdenPagoRetencion))
                                .ReplaceLabel("[@FECHA]", ordenPago.Fecha.ToString("dd/MM/yyyy"))
                                .ReplaceLabel("[@NRO_OP]", string.Format("OP Nro.: {0}", ordenPago.IdOrdenPago.ToString()))

                                .ReplaceLabel("[@PROVEEDOR]", proveedor.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@PROVEEDOR_DIRECCION]", proveedor.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@PROVEEDOR_LOCALIDAD]", proveedor.Localidad.ToUpperInvariant())
                                .ReplaceLabel("[@PROVEEDOR_IVA]", tipoIva.Description())
                                .ReplaceLabel("[@PROVEEDOR_CUIT]", proveedor.CUIT)

                                .ReplaceLabel("[@TITULO_RETENCION]", titulo)
                                .ReplaceLabel("[@TEXTO_RETENCION]", texto)
                                .ReplaceLabel("[@FOOTER_RETENCION]", footer);

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region COMPROBANTES

            var cellText = string.Empty;
            var cellPaddingTop = 5f;
            var cellPaddingLeft = 0f;
            var cellBorderColorTop = BaseColor.LIGHT_GRAY;

            var table = new PdfPTable(new[] { 20f, 50f, 30f });
            var cellFont = ITFont.Helvetica.Size(8f).Font();

            if (ordenPagoComprobantes != null && ordenPagoComprobantes.Count > 0)
            {
                double totalComprobantes = 0;

                //Agregar Títulos
                cellText = "COMPROBANTES QUE ORIGINAN LA RETENCIÓN:";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(12f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                cell.Colspan = 3;
                table.AddCell(cell);

                cell.Colspan = 0;
                cellText = "FECHA";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "COMPROBANTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                cellText = "IMPORTE";
                cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(10f).Font()));
                cell.HorizontalAlignment = (int)Align.Right;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var comprobante in ordenPagoComprobantes)
                {
                    //Fecha
                    cellText = comprobante.Fecha.ToString("dd/MM/yyyy");
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Descripcion
                    cellText = comprobante.NumeroComprobante.ToString();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Importe
                    cellText = comprobante.Importe.ToCurrency();
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Right;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    totalComprobantes += comprobante.Importe;
                }

                //total
                var cellTotalText = totalComprobantes.ToCurrency();
                var cellTotal = new PdfPCell(new Phrase("TOTAL: " + cellTotalText, ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 3;
                table.AddCell(cellTotal);

                //2 CELDAS VACIAS
                cellTotal = new PdfPCell(new Phrase(" ", ITFont.Helvetica.Size(12f).Font()));
                cellTotal.HorizontalAlignment = (int)Align.Right;
                cellTotal.PaddingTop = 5f;
                cellTotal.PaddingLeft = 0f;
                cellTotal.Border = 0;
                cellTotal.BorderWidthTop = 0;
                cellTotal.Colspan = 3;
                table.AddCell(cellTotal);
                table.AddCell(cellTotal);
            }

            #endregion

            //retencion
            //Agregar Títulos
            cellText = "DATOS DE LA RETENCIÓN PRACTICADA:";
            var cellR = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(12f).Font()));
            cellR.HorizontalAlignment = (int)Align.Left;
            cellR.PaddingTop = cellPaddingTop;
            cellR.PaddingLeft = cellPaddingLeft;
            cellR.Border = 0;
            cellR.Colspan = 3;
            table.AddCell(cellR);

            //regimen
            //var cellRetencionText = "Régimen";
            //var cellRetencion = new PdfPCell(new Phrase(cellRetencionText, ITFont.Helvetica.Size(12f).Font()));
            //cellRetencion.HorizontalAlignment = (int)Align.Left;
            //cellRetencion.PaddingTop = 5f;
            //cellRetencion.PaddingLeft = 0f;
            //cellRetencion.Border = 0;
            //cellRetencion.BorderWidthTop = 0;
            //cellRetencion.Colspan = 2;
            //table.AddCell(cellRetencion);

            //var cellRetencionTotalText = regimen;
            //var cellRetencionTotal = new PdfPCell(new Phrase(cellRetencionTotalText, ITFont.Helvetica.Size(12f).Font()));
            //cellRetencionTotal.HorizontalAlignment = (int)Align.Right;
            //cellRetencionTotal.PaddingTop = 5f;
            //cellRetencionTotal.PaddingLeft = 0f;
            //cellRetencionTotal.Border = 0;
            //cellRetencionTotal.BorderWidthTop = 0;
            //cellRetencionTotal.Colspan = 0;
            //table.AddCell(cellRetencionTotal);

            //base imponible
            var cellRetencionText = "Base Imponible en este certificado";
            var cellRetencion = new PdfPCell(new Phrase(cellRetencionText, ITFont.Helvetica.Size(12f).Font()));
            cellRetencion.HorizontalAlignment = (int)Align.Left;
            cellRetencion.PaddingTop = 5f;
            cellRetencion.PaddingLeft = 0f;
            cellRetencion.Border = 0;
            cellRetencion.BorderWidthTop = 0;
            cellRetencion.Colspan = 2;
            table.AddCell(cellRetencion);

            var cellRetencionTotalText = ordenPagoRetencion.BaseImponible.ToCurrency();
            var cellRetencionTotal = new PdfPCell(new Phrase(cellRetencionTotalText, ITFont.Helvetica.Size(12f).Font()));
            cellRetencionTotal.HorizontalAlignment = (int)Align.Right;
            cellRetencionTotal.PaddingTop = 5f;
            cellRetencionTotal.PaddingLeft = 0f;
            cellRetencionTotal.Border = 0;
            cellRetencionTotal.BorderWidthTop = 0;
            cellRetencionTotal.Colspan = 0;
            table.AddCell(cellRetencionTotal);

            //base imponible
            cellRetencionText = "Monto de la retención";
            cellRetencion = new PdfPCell(new Phrase(cellRetencionText, ITFont.Helvetica.Size(12f).Font()));
            cellRetencion.HorizontalAlignment = (int)Align.Left;
            cellRetencion.PaddingTop = 5f;
            cellRetencion.PaddingLeft = 0f;
            cellRetencion.Border = 0;
            cellRetencion.BorderWidthTop = 0;
            cellRetencion.Colspan = 2;
            table.AddCell(cellRetencion);

            cellRetencionTotalText = ordenPagoRetencion.Importe.ToCurrency();
            cellRetencionTotal = new PdfPCell(new Phrase(cellRetencionTotalText, ITFont.Helvetica.Size(12f).Font()));
            cellRetencionTotal.HorizontalAlignment = (int)Align.Right;
            cellRetencionTotal.PaddingTop = 5f;
            cellRetencionTotal.PaddingLeft = 0f;
            cellRetencionTotal.Border = 0;
            cellRetencionTotal.BorderWidthTop = 0;
            cellRetencionTotal.Colspan = 0;
            table.AddCell(cellRetencionTotal);

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            var filename = string.Format("Ret_{0}", ordenPago.IdOrdenPago);
            return new PDF
            {
                Filename = $"{filename}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        public async Task<PDF> GenerarImpresionReceta(int idEvolucionReceta, int idEvolucion)
        {
            await ValidarTipoUsuarioAutorizado(_permissionsBusiness.User.IdTipoUsuario,
                                                new List<TipoUsuario> {
                                                                    TipoUsuario.Administrador,
                                                                    TipoUsuario.Profesional,
                                                                    TipoUsuario.Paciente
                                                });

            var evolucion = await _evolucionesRepository.GetById<Evolucion>(idEvolucion);
            if (evolucion == null)
            {
                throw new BusinessException("No se encontró la evolución.");
            }

            if (_permissionsBusiness.User.IdPaciente > 0 && evolucion.IdPaciente != _permissionsBusiness.User.IdPaciente)
            {
                throw new UnauthorizedAccessException("Usuario no autorizado!!!");
            }

            if (_permissionsBusiness.User.IdTipoUsuario != (int)TipoUsuario.Administrador 
                && _permissionsBusiness.User.IdProfesional > 0 && evolucion.IdProfesional != _permissionsBusiness.User.IdProfesional)
            {
                throw new UnauthorizedAccessException("Usuario no autorizado!!!");
            }

            if (_permissionsBusiness.User.IdTipoUsuario != (int)TipoUsuario.Paciente
                && _permissionsBusiness.User.IdEmpresa > 0 && evolucion.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
            {
                throw new UnauthorizedAccessException("Usuario no autorizado!!!");
            }

            if (idEvolucionReceta > 0)
            {
                var evolucionReceta = await _evolucionesRecetasRepository.GetById<EvolucionReceta>(idEvolucionReceta);
                if (evolucionReceta == null)
                {
                    throw new BusinessException("No se encontró la receta.");
                }

                return await GenerarImpresionReceta(evolucion, evolucionReceta);
            }
            else
            {
                var evolucionesRecetas = await _evolucionesRecetasRepository.GetAllByIdEvolucion(idEvolucion);
                if (evolucionesRecetas == null || evolucionesRecetas.Count == 0)
                {
                    throw new BusinessException("No se encontraron indicaciones.");
                }

                return await GenerarImpresionRecetasIndicaciones(evolucion, evolucionesRecetas.ToList());
            }
        }

        private async Task<PDF> GenerarImpresionReceta(Evolucion evolucion, EvolucionReceta receta)
        {
            string jsonContent = string.Empty;
            byte[] pdfBytes = null;

            //Si no logra leer la plantilla... hace el fallback al default de settings...
            var jsonPath = "Impresiones:Recetas:JsonPath".FromAppSettings<string>(notFoundException: true);
            jsonContent = Utils.GetJsonContent(jsonPath);

            var empresa = await _empresasRepository.GetById<Empresa>(evolucion.IdEmpresa);
            var profesional = await _profesionalesRepository.GetById<Profesional>(evolucion.IdProfesional);
            var paciente = await _pacientesRepository.GetById<Paciente>(evolucion.IdPaciente);

            var sFinanciador = string.Empty;
            if (paciente.IdFinanciador.HasValue)
            { 
                var financiador = await _financiadoresRepository.GetById<Financiador>(paciente.IdFinanciador.Value);
                var financiadorPlan = await _financiadoresPlanesRepository.GetById<FinanciadorPlan>(paciente.IdFinanciadorPlan.Value);

                sFinanciador = string.Format("{0} {1}", financiador.Nombre.ToUpperInvariant(), financiadorPlan.Nombre.ToUpperInvariant());
            }

            var matriculas = string.Empty;
            if (!string.IsNullOrEmpty(profesional.Matricula))
            {
                matriculas += string.Format("Mat. Nac.: {0}", profesional.Matricula);
            }

            if (!string.IsNullOrEmpty(profesional.MatriculaProvincial))
            {                
                matriculas += string.Format("{0}Mat. Prov.: {1}", !string.IsNullOrEmpty(matriculas) ? ". " : "", profesional.MatriculaProvincial);
            }

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA_01]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION_01]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP_01]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_02]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION_02]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP_02]", empresa.CodigoPostal.ToUpperInvariant())

                                .ReplaceLabel("[@PROFESIONAL_01]", profesional.Nombre)
                                .ReplaceLabel("[@MATRICULAS_01]", matriculas)
                                .ReplaceLabel("[@PROFESIONAL_02]", profesional.Nombre)
                                .ReplaceLabel("[@MATRICULAS_02]", matriculas)

                                .ReplaceLabel("[@PACIENTE_01]", paciente.Nombre)
                                .ReplaceLabel("[@DOCUMENTO_01]", paciente.Documento)
                                .ReplaceLabel("[@FINANCIADOR_01]", paciente.IdFinanciador.HasValue ? sFinanciador : "[S/Cobertura]")
                                .ReplaceLabel("[@NRO_AFILIADO_01]", paciente.IdFinanciador.HasValue ? paciente.FinanciadorNro : "[S/Cobertura]")
                                .ReplaceLabel("[@PACIENTE_02]", paciente.Nombre)
                                .ReplaceLabel("[@DOCUMENTO_02]", paciente.Documento)
                                .ReplaceLabel("[@FINANCIADOR_02]", paciente.IdFinanciador.HasValue ? sFinanciador : "[S/Cobertura]")
                                .ReplaceLabel("[@NRO_AFILIADO_02]", paciente.IdFinanciador.HasValue ? paciente.FinanciadorNro : "[S/Cobertura]")

                                .ReplaceLabel("[@MEDICAMENTO_CANTIDAD]", string.Format("Cantidad: {0}", receta.Cantidad.ToString()))
                                .ReplaceLabel("[@MEDICAMENTO]", string.Format("{0}{1}", receta.Descripcion, receta.NoSustituir ? " - (NO SUSTITUIR)" : ""))
                                .ReplaceLabel("[@DIAGNOSTICO]", evolucion.Diagnostico ?? string.Empty)
                                .ReplaceLabel("[@INDICACIONES]", string.Format("Dosis: {0}. Frecuencia: {1}. {2}", receta.Dosis, receta.Frecuencia, receta.Indicaciones))

                                .ReplaceLabel("[@FECHA_01]", evolucion.Fecha.ToString("dd/MM/yyyy"))
                                .ReplaceLabel("[@FECHA_02]", evolucion.Fecha.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@CONTACTO_01]", string.Format("{0} - {1}", empresa.Email, empresa.Telefono))
                                .ReplaceLabel("[@CONTACTO_02]", string.Format("{0} - {1}", empresa.Email, empresa.Telefono))
                                ;

            var diasVencimiento = "Impresiones:Recetas:DiasVencimiento".FromAppSettings<int>(0);
            if (diasVencimiento > 0 && evolucion.Fecha.AddDays(diasVencimiento) < DateTime.Now.Date.AddDays(1))
            {
                jsonDocument.MergeWith = jsonDocument.MergeWith.Replace(".pdf", "_vencida.pdf");
            }

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region Imagenes

            if (empresa.IdArchivoRecetaLogo.HasValue)
            {
                var archivoLogo = await _archivosRepository.GetById<Archivo>(empresa.IdArchivoRecetaLogo.Value);
                pdfDocument.Image(archivoLogo.Contenido, 30, 30);
            }

            if (profesional.IdArchivoFirma.HasValue)
            {
                var archivoFrima = await _archivosRepository.GetById<Archivo>(profesional.IdArchivoFirma.Value);
                pdfDocument.Image(archivoFrima.Contenido, 190, 330);
            }

            #endregion

            var buffer = pdfDocument.CloseAndGetBuffer();

            return new PDF
            {
                Filename = $"{string.Format("{0}({1})", receta.PrincipioActivo, receta.Codigo)}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        private async Task<PDF> GenerarImpresionRecetasIndicaciones(Evolucion evolucion, List<EvolucionReceta> recetas)
        {           
            string jsonContent = string.Empty;
            byte[] pdfBytes = null;

            //Si no logra leer la plantilla... hace el fallback al default de settings...
            var jsonPath = "Impresiones:Recetas:JsonPathIndicaciones".FromAppSettings<string>(notFoundException: true);
            jsonContent = Utils.GetJsonContent(jsonPath);

            var empresa = await _empresasRepository.GetById<Empresa>(evolucion.IdEmpresa);
            var profesional = await _profesionalesRepository.GetById<Profesional>(evolucion.IdProfesional);
            var paciente = await _pacientesRepository.GetById<Paciente>(evolucion.IdPaciente);

            var sFinanciador = string.Empty;
            if (paciente.IdFinanciador.HasValue)
            {
                var financiador = await _financiadoresRepository.GetById<Financiador>(paciente.IdFinanciador.Value);
                var financiadorPlan = await _financiadoresPlanesRepository.GetById<FinanciadorPlan>(paciente.IdFinanciadorPlan.Value);

                sFinanciador = string.Format("{0} {1}", financiador.Nombre.ToUpperInvariant(), financiadorPlan.Nombre.ToUpperInvariant());
            }

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP]", empresa.CodigoPostal.ToUpperInvariant())
                                //.ReplaceLabel("[@EMPRESA_TELEFONO]", empresa.Telefono.ToUpperInvariant())

                                //.ReplaceLabel("[@EMPRESA_CUIT]", empresa.CUIT.ToUpperInvariant())

                                .ReplaceLabel("[@PROFESIONAL]", profesional.Nombre)
                                .ReplaceLabel("[@PROFESIONAL_01]", string.Empty)

                                .ReplaceLabel("[@FECHA]", evolucion.Fecha.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@PACIENTE]", paciente.Nombre)
                                .ReplaceLabel("[@DOCUMENTO]", paciente.Documento)
                                .ReplaceLabel("[@FINANCIADOR]", paciente.IdFinanciador.HasValue ? sFinanciador : "[S/Cobertura]")
                                .ReplaceLabel("[@NRO_AFILIADO]", paciente.IdFinanciador.HasValue ? paciente.FinanciadorNro : "[S/Cobertura]")

                                .ReplaceLabel("[@CONTACTO]", string.Format("{0} - {1}", empresa.Email, empresa.Telefono));

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            #region Imagenes

            if (empresa.IdArchivoRecetaLogo.HasValue)
            {
                var archivoLogo = await _archivosRepository.GetById<Archivo>(empresa.IdArchivoRecetaLogo.Value);
                pdfDocument.Image(archivoLogo.Contenido, 30, 30);
            }

            if (profesional.IdArchivoFirma.HasValue)
            {
                var archivoFrima = await _archivosRepository.GetById<Archivo>(profesional.IdArchivoFirma.Value);
                pdfDocument.Image(archivoFrima.Contenido, 350, 650);
            }

            #endregion

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region MEDICAMENTOS

            var cellText = string.Empty;
            var cellPaddingTop = 5f;
            var cellPaddingLeft = 0f;
            var cellBorderColorTop = BaseColor.LIGHT_GRAY;

            var table = new PdfPTable(new[] { 100f });
            var cellFont = ITFont.Helvetica.Size(8f).Font();

            if (recetas != null && recetas.Count > 0)
            {
                //Agregar Títulos
                cellText = "INDICACIONES:";
                var cell = new PdfPCell(new Phrase(cellText, ITFont.Helvetica.Size(12f).Font()));
                cell.HorizontalAlignment = (int)Align.Left;
                cell.PaddingTop = cellPaddingTop;
                cell.PaddingLeft = cellPaddingLeft;
                cell.Border = 0;
                table.AddCell(cell);

                foreach (var receta in recetas)
                {
                    //Medicamento
                    cellText = string.Format("{0} - Dosis: {1}. Frecuencia: {2}.", receta.Descripcion, receta.Dosis, receta.Frecuencia);
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    table.AddCell(cell);

                    //Indicaciones
                    if (!string.IsNullOrEmpty(receta.Indicaciones))
                    {
                        cellText = string.Format("Indicaciones adicionales: {0}.", receta.Indicaciones);
                        cell = new PdfPCell(new Phrase(cellText, cellFont));
                        cell.HorizontalAlignment = (int)Align.Left;
                        cell.PaddingTop = cellPaddingTop;
                        cell.PaddingLeft = cellPaddingLeft;
                        cell.Border = 0;
                        cell.Colspan = 3;
                        table.AddCell(cell);
                    }

                    //Fila vacía
                    cellText = string.Empty;
                    cell = new PdfPCell(new Phrase(cellText, cellFont));
                    cell.HorizontalAlignment = (int)Align.Left;
                    cell.PaddingTop = cellPaddingTop;
                    cell.PaddingLeft = cellPaddingLeft;
                    cell.Border = 0;
                    cell.Colspan = 3;
                    table.AddCell(cell);
                }
            }

            #endregion

            pdfDocument.Document.Add(table);

            var buffer = pdfDocument.CloseAndGetBuffer();

            return new PDF
            {
                Filename = $"INDICACIONES.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }

        public async Task<PDF> GenerarImpresionOrden(int idEvolucionOrden, int idEvolucion)
        {
            await ValidarTipoUsuarioAutorizado(_permissionsBusiness.User.IdTipoUsuario,
                                                new List<TipoUsuario> {
                                                                    TipoUsuario.Administrador,
                                                                    TipoUsuario.Profesional,
                                                                    TipoUsuario.Paciente
                                                });

            var evolucion = await _evolucionesRepository.GetById<Evolucion>(idEvolucion);
            if (evolucion == null)
            {
                throw new BusinessException("No se encontró la evolución.");
            }

            if (_permissionsBusiness.User.IdPaciente > 0 && evolucion.IdPaciente != _permissionsBusiness.User.IdPaciente)
            {
                throw new UnauthorizedAccessException("Usuario no autorizado!!!");
            }

            if (_permissionsBusiness.User.IdTipoUsuario != (int)TipoUsuario.Administrador
                && _permissionsBusiness.User.IdProfesional > 0 && evolucion.IdProfesional != _permissionsBusiness.User.IdProfesional)
            {
                throw new UnauthorizedAccessException("Usuario no autorizado!!!");
            }

            if (_permissionsBusiness.User.IdTipoUsuario != (int)TipoUsuario.Paciente
                && _permissionsBusiness.User.IdEmpresa > 0 && evolucion.IdEmpresa != _permissionsBusiness.User.IdEmpresa)
            {
                throw new UnauthorizedAccessException("Usuario no autorizado!!!");
            }

            var evolucionOrden = await _evolucionesRecetasRepository.GetById<EvolucionOrden>(idEvolucionOrden);
            if (evolucionOrden == null)
            {
                throw new BusinessException("No se encontró la orden.");
            }

            return await GenerarImpresionOrden(evolucion, evolucionOrden);
        }

        private async Task<PDF> GenerarImpresionOrden(Evolucion evolucion, EvolucionOrden orden)
        {
            string jsonContent = string.Empty;
            byte[] pdfBytes = null;

            //Si no logra leer la plantilla... hace el fallback al default de settings...
            var jsonPath = "Impresiones:Ordenes:JsonPath".FromAppSettings<string>(notFoundException: true);
            jsonContent = Utils.GetJsonContent(jsonPath);

            var empresa = await _empresasRepository.GetById<Empresa>(evolucion.IdEmpresa);
            var profesional = await _profesionalesRepository.GetById<Profesional>(evolucion.IdProfesional);
            var paciente = await _pacientesRepository.GetById<Paciente>(evolucion.IdPaciente);

            var sFinanciador = string.Empty;
            if (paciente.IdFinanciador.HasValue)
            {
                var financiador = await _financiadoresRepository.GetById<Financiador>(paciente.IdFinanciador.Value);
                var financiadorPlan = await _financiadoresPlanesRepository.GetById<FinanciadorPlan>(paciente.IdFinanciadorPlan.Value);

                sFinanciador = string.Format("{0} {1}", financiador.Nombre.ToUpperInvariant(), financiadorPlan.Nombre.ToUpperInvariant());
            }

            var matriculas = string.Empty;
            if (!string.IsNullOrEmpty(profesional.Matricula))
            {
                matriculas += string.Format("Mat. Nac.: {0}", profesional.Matricula);
            }

            if (!string.IsNullOrEmpty(profesional.MatriculaProvincial))
            {
                matriculas += string.Format("{0}Mat. Prov.: {1}", !string.IsNullOrEmpty(matriculas) ? ". " : "", profesional.MatriculaProvincial);
            }

            var jsonDocument = JsonDocumentHelper.ParseJson(jsonContent)
                                .ReplaceLabel("[@EMPRESA_01]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION_01]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP_01]", empresa.CodigoPostal.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_02]", empresa.RazonSocial.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_DIRECCION_02]", empresa.Direccion.ToUpperInvariant())
                                .ReplaceLabel("[@EMPRESA_CP_02]", empresa.CodigoPostal.ToUpperInvariant())

                                .ReplaceLabel("[@PROFESIONAL_01]", profesional.Nombre)
                                .ReplaceLabel("[@MATRICULAS_01]", matriculas)
                                .ReplaceLabel("[@PROFESIONAL_02]", profesional.Nombre)
                                .ReplaceLabel("[@MATRICULAS_02]", matriculas)

                                .ReplaceLabel("[@PACIENTE_01]", paciente.Nombre)
                                .ReplaceLabel("[@DOCUMENTO_01]", paciente.Documento)
                                .ReplaceLabel("[@FINANCIADOR_01]", paciente.IdFinanciador.HasValue ? sFinanciador : "[S/Cobertura]")
                                .ReplaceLabel("[@NRO_AFILIADO_01]", paciente.IdFinanciador.HasValue ? paciente.FinanciadorNro : "[S/Cobertura]")
                                .ReplaceLabel("[@PACIENTE_02]", paciente.Nombre)
                                .ReplaceLabel("[@DOCUMENTO_02]", paciente.Documento)
                                .ReplaceLabel("[@FINANCIADOR_02]", paciente.IdFinanciador.HasValue ? sFinanciador : "[S/Cobertura]")
                                .ReplaceLabel("[@NRO_AFILIADO_02]", paciente.IdFinanciador.HasValue ? paciente.FinanciadorNro : "[S/Cobertura]")

                                .ReplaceLabel("[@DESCRIPCION]", orden.Descripcion ?? "")
                                .ReplaceLabel("[@DIAGNOSTICO]", evolucion.Diagnostico ?? string.Empty)
                                .ReplaceLabel("[@INDICACIONES]", orden.Indicaciones ?? "")

                                .ReplaceLabel("[@FECHA_01]", evolucion.Fecha.ToString("dd/MM/yyyy"))
                                .ReplaceLabel("[@FECHA_02]", evolucion.Fecha.ToString("dd/MM/yyyy"))

                                .ReplaceLabel("[@CONTACTO_01]", string.Format("{0} - {1}", empresa.Email, empresa.Telefono))
                                .ReplaceLabel("[@CONTACTO_02]", string.Format("{0} - {1}", empresa.Email, empresa.Telefono))
                                ;

            var diasVencimiento = "Impresiones:Ordenes:DiasVencimiento".FromAppSettings<int>(0);
            if (diasVencimiento > 0 && evolucion.Fecha.AddDays(diasVencimiento) < DateTime.Now.Date.AddDays(1))
            {
                jsonDocument.MergeWith = jsonDocument.MergeWith.Replace(".pdf", "_vencida.pdf");
            }

            var pdfDocument = JsonDocumentHelper.Create(jsonDocument, pdfBytes);

            pdfDocument.OnEndingPage += (sender) =>
            {
                if (_drawRulers)
                    sender.DrawRulers();

                if (sender.Writer.PageNumber > 1)
                    JsonDocumentHelper.RenderNewPageElements(pdfDocument, jsonDocument);
            };

            JsonDocumentHelper.RenderElements(pdfDocument, jsonDocument);

            #region Imagenes

            if (empresa.IdArchivoRecetaLogo.HasValue)
            {
                var archivoLogo = await _archivosRepository.GetById<Archivo>(empresa.IdArchivoRecetaLogo.Value);
                pdfDocument.Image(archivoLogo.Contenido, 30, 30);
            }

            if (profesional.IdArchivoFirma.HasValue)
            {
                var archivoFrima = await _archivosRepository.GetById<Archivo>(profesional.IdArchivoFirma.Value);
                pdfDocument.Image(archivoFrima.Contenido, 190, 330);
            }

            #endregion

            var buffer = pdfDocument.CloseAndGetBuffer();

            return new PDF
            {
                Filename = $"{string.Format("{0}", orden.IdEvolucionOrden)}.pdf",
                Length = buffer.Length,
                Content = buffer
            };
        }
        private async Task<JsonElement> ObtenerQRVentas(ComprobanteVenta factura, Comprobante comprobante, Empresa empresa, Cliente cliente)
        {
            var enabled = "Impresiones:QR_AFIP:Enabled".FromAppSettings<Boolean>(notFoundException: false);
            if (!enabled)
                return null;

            var urlAfip = "Impresiones:QR_AFIP:Url".FromAppSettings<string>(notFoundException: true);
            var posX = "Impresiones:QR_AFIP:Position_X".FromAppSettings<int>(notFoundException: true);
            var posY = "Impresiones:QR_AFIP:Position_Y".FromAppSettings<int>(notFoundException: true);
            var width = "Impresiones:QR_AFIP:Width".FromAppSettings<int>(notFoundException: true);
            var height = "Impresiones:QR_AFIP:Height".FromAppSettings<int>(notFoundException: true);

            if (string.IsNullOrEmpty(factura.CAE))
            {
                return new JsonElement
                {
                    Type = ElementType.Text,
                    Text = "DOCUMENTO NO VÁLIDO COMO FACTURA",
                    Align = Align.Right,
                    Position = new System.Drawing.PointF(posX, posY),
                    Dimensions = new Dimensions { Height = height, Width = width },
                    VerticalAlign = VerticalAlign.Center,
                    Border = new Border { All = 0, Color = System.Drawing.Color.Black }
                };
            }

            //Reemplazos interactivos en URL
            var qrParams = new
            {
                ver = 1,
                fecha = factura.Fecha.ToString("yyyy-MM-dd"),
                cuit = long.Parse(empresa.CUIT.Replace("-", "")),
                ptoVta = int.Parse(factura.Sucursal),
                tipoCmp = comprobante.Codigo,
                nroCmp = int.Parse(factura.Numero),
                importe = factura.Total,
                moneda = "PES",
                ctz = 1,
                tipoDocRec = 80,
                nroDocRec = long.Parse(cliente.CUIT.Replace("-", "")),
                tipoCodAut = "E",
                codAut = long.Parse(factura.CAE)
            };

            var bJSON = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(qrParams));
            urlAfip += System.Convert.ToBase64String(bJSON);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(urlAfip, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] imgQR = qrCode.GetGraphic(20);

            return new JsonElement
            {
                Type = ElementType.Image,
                Bytes = imgQR,
                Position = new System.Drawing.PointF(posX, posY),
                Dimensions = new Dimensions { Height = height, Width = width },
                ImageScale = ImageScale.Auto,
                VerticalAlign = VerticalAlign.Center,
                Border = new Border { All = 0, Color = System.Drawing.Color.Black },
                Percent = 100
            };
        }

        private async Task ValidarTipoUsuarioAutorizado(int tipoUsuario, List<TipoUsuario> tiposPermitidos)
        {
            var autorizado = false;

            foreach (var tipo in tiposPermitidos)
            {
                if ((int)tipo == tipoUsuario)
                    autorizado = true;
            }

            if (!autorizado)
                throw new UnauthorizedAccessException("Tipo de Usuario no autorizado!!!");
        }

        private static bool EsComprobante(int iComprobante, List<Comprobantes> comprobantes)
        {
            var bReturn = false;

            foreach (var comprobante in comprobantes)
            {
                if ((int)comprobante == iComprobante)
                    bReturn = true;
            }

            return bReturn;
        }
    }
}
