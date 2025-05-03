using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RetencionesPercepcionesRepository : IRetencionesPercepcionesRepository
    {
        private readonly IDbContext _context;

        public RetencionesPercepcionesRepository(IDbContext context)
        {
            _context = context;
        }

        public RetencionesPercepcionesRepository()
        {
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionGananciasProveedores(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        opr.IdOrdenPagoRetencion Id, 
                        opr.Fecha, 
                        'ORDEN DE PAGO' Comprobante, 
                        p.RazonSocial, 
                        p.CUIT, 
                        RIGHT('000000' + opr.IdOrdenPagoRetencion, 6) NumeroComprobante, 
                        opr.BaseImponible, 
                        opr.Importe
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"opr.IdTipoRetencion IN ({(int)TipoRetencion.Ganancias}, {(int)Domain.Enums.TipoRetencion.GananciasMonotributo})");
            builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionIngresosBrutosProveedores(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        opr.IdOrdenPagoRetencion Id, 
                        opr.Fecha, 
                        'ORDEN DE PAGO' Comprobante, 
                        p.RazonSocial, 
                        p.CUIT, 
                        RIGHT('000000' + opr.IdOrdenPagoRetencion, 6) NumeroComprobante, 
                        opr.BaseImponible, 
                        opr.Importe
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"opr.IdTipoRetencion = {(int)TipoRetencion.IngresosBrutos}");
            builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionIVAProveedores(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        opr.IdOrdenPagoRetencion Id, 
                        opr.Fecha, 
                        'ORDEN DE PAGO' Comprobante, 
                        p.RazonSocial, 
                        p.CUIT, 
                        RIGHT('000000' + opr.IdOrdenPagoRetencion, 6) NumeroComprobante, 
                        opr.BaseImponible, 
                        opr.Importe
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"opr.IdTipoRetencion IN ({(int)TipoRetencion.IVA}, {(int)TipoRetencion.IVAMonotributo})");
            builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionSUSSProveedores(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        opr.IdOrdenPagoRetencion Id, 
                        opr.Fecha, 
                        'ORDEN DE PAGO' Comprobante, 
                        p.RazonSocial, 
                        p.CUIT, 
                        RIGHT('000000', opr.IdOrdenPagoRetencion, 6) NumeroComprobante, 
                        opr.BaseImponible, 
                        opr.Importe
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"opr.IdTipoRetencion = {(int)TipoRetencion.SUSS}");
            builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetPercepcionIngresosBrutosProveedores(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.IdComprobanteCompra Id, 
                        cc.Fecha, 
                        c.Descripcion Comprobante, 
                        p.RazonSocial, 
                        p.CUIT, 
                        (cc.Sucursal + '-' + cc.Numero) AS NumeroComprobante, 
                        cc.Subtotal BaseImponible, 
                        cc.Percepciones Importe 
                    FROM ComprobantesCompras cc
                        INNER JOIN Proveedores p ON cc.IdProveedor = p.IdProveedor
                        INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cc.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cc.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"cc.Percepciones <> {0D}");
            builder.Where($"cc.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionGananciasClientes(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        rr.IdReciboRetencion Id, 
                        rr.Fecha, 
                        'RECIBO' Comprobante, 
                        c.RazonSocial, 
                        c.CUIT, 
                        rr.NroRetencion NumeroComprobante, 
                        rr.BaseImponible, 
                        rr.Importe
                    FROM RecibosRetenciones rr
                        INNER JOIN Recibos r ON rr.IdRecibo = r.IdRecibo
                        INNER JOIN Clientes c ON r.IdCliente = c.IdCliente
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"rr.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"rr.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"rr.IdCategoria = {(int)CategoriaRetencion.Ganancias}");
            builder.Where($"r.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionIngresosBrutosClientes(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        rr.IdReciboRetencion Id, 
                        rr.Fecha, 
                        'RECIBO' Comprobante, 
                        c.RazonSocial, 
                        c.CUIT, 
                        rr.NroRetencion NumeroComprobante, 
                        rr.BaseImponible, 
                        rr.Importe
                    FROM RecibosRetenciones rr
                        INNER JOIN Recibos r ON rr.IdRecibo = r.IdRecibo
                        INNER JOIN Clientes c ON r.IdCliente = c.IdCliente
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"rr.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"rr.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"rr.IdCategoria = {(int)CategoriaRetencion.IngresosBrutos}");
            builder.Where($"r.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionIVAClientes(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        rr.IdReciboRetencion Id, 
                        rr.Fecha, 
                        'RECIBO' Comprobante, 
                        c.RazonSocial, 
                        c.CUIT, 
                        rr.NroRetencion NumeroComprobante, 
                        rr.BaseImponible, 
                        rr.Importe
                    FROM RecibosRetenciones rr
                        INNER JOIN Recibos r ON rr.IdRecibo = r.IdRecibo
                        INNER JOIN Clientes c ON r.IdCliente = c.IdCliente
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"rr.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"rr.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"rr.IdCategoria = {(int)CategoriaRetencion.IVA}");
            builder.Where($"r.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetRetencionSUSSClientes(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        rr.IdReciboRetencion Id, 
                        rr.Fecha, 
                        'RECIBO' Comprobante, 
                        c.RazonSocial, 
                        c.CUIT, 
                        rr.NroRetencion NumeroComprobante, 
                        rr.BaseImponible, 
                        rr.Importe
                    FROM RecibosRetenciones rr
                        INNER JOIN Recibos r ON rr.IdRecibo = r.IdRecibo
                        INNER JOIN Clientes c ON r.IdCliente = c.IdCliente
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"rr.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"rr.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"rr.IdCategoria = {(int)CategoriaRetencion.SUSS}");
            builder.Where($"r.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<Custom.RetencionPercepcion>> GetPercepcionIngresosBrutosClientes(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cv.IdComprobanteVenta Id, 
                        cv.Fecha, 
                        UPPER(c.Descripcion + ' - ' + ta.Descripcion) Comprobante, 
                        cl.RazonSocial, 
                        cl.CUIT, 
                        (cv.Sucursal + '-' + cv.Numero) NumeroComprobante, 
                        cvt.Neto BaseImponible, 
                        cvt.ImporteAlicuota Importe 
                    FROM ComprobantesVentasTotales cvt
                        INNER JOIN ComprobantesVentas cv ON cv.IdComprobanteVenta = cvt.IdComprobanteVenta
                        INNER JOIN Clientes cl ON cv.IdCliente = cl.IdCliente
                        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                        INNER JOIN TipoAlicuotas ta ON cvt.IdTipoAlicuota = ta.IdTipoAlicuota
                    /**where**/"
                );

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cv.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cv.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Where($"cvt.IdTipoAlicuota IN ({(int)TipoAlicuota.PercepcionAGIP},{(int)TipoAlicuota.PercepcionARBA})");
            builder.Where($"cv.IdEmpresa = {filters["IdEmpresa"]}");

            var result = await builder.QueryAsync<Custom.RetencionPercepcion>();

            return result.AsList();
        }

        public async Task<List<SicoreGananciasProveedores>> GetGananciasProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            DateTime fechaD = fechaDesde.HasValue ? fechaDesde.Value : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fechaH = fechaHasta.HasValue ? fechaHasta.Value : fechaD.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        '06' CodigoComprobante,
                        op.Fecha FechaEmisionComprobante,
                        RIGHT('0000000000000000' + op.IdOrdenPago, 16) NroComprobante,
                        opr.BaseImponible ImporteNetoGravado,
                        '217' CodImpuesto,
                        cr.Regimen,
                        '1' CodigoOperacion,
                        opr.BaseImponible,
                        op.Fecha FechaEmisionRetencion,
                        '01' CodigoCondicion,
                        '0' CodSujetosSuspendidos,
                        opr.Importe ImporteRetencion,
                        SPACE(6) PorcentajeExclusion,
                        SPACE(10) FechaEmisionBoletin,
                        '80' TipoDocumentoDelRetenido,
                        RIGHT(SPACE(20) + REPLACE(p.CUIT, '-', ''), 20) NroDocumentoDelRetenido,
                        RIGHT(SPACE(14) + '000000' + opr.IdOrdenPagoRetencion, 14) NroCertificadoOriginal
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                        INNER JOIN ProveedoresCodigosRetencion pcr ON p.IdProveedor = pcr.IdProveedor
                        INNER JOIN CodigosRetencion cr ON pcr.IdCodigoRetencion = cr.IdCodigoRetencion
                    /**where**/"
            );

            builder.Where($"cr.IdTipoRetencion IN ({TipoRetencion.Ganancias}, {TipoRetencion.GananciasMonotributo})");
            builder.Where($"op.Fecha >= {fechaD}");
            builder.Where($"op.Fecha <= {fechaH}");
            builder.Where($"opr.IdTipoRetencion IN ({(int)TipoRetencion.Ganancias}, {(int)TipoRetencion.GananciasMonotributo})");
            builder.Where($"op.IdEmpresa = {idEmpresa}");

            var result = await builder.QueryAsync<SicoreGananciasProveedores>();

            return result.AsList();
        }

        public async Task<List<SicoreIVAProveedores>> GetIVAProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            DateTime fechaD = fechaDesde.HasValue ? fechaDesde.Value : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fechaH = fechaHasta.HasValue ? fechaHasta.Value : fechaD.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        '06' CodigoComprobante,
                        op.Fecha FechaEmisionComprobante,
                        RIGHT('0000000000000000' + op.IdOrdenPago, 16) NroComprobante,
                        opr.BaseImponible ImporteNetoGravado,
                        '217' CodImpuesto,
                        cr.Regimen,
                        '1' CodigoOperacion,
                        opr.BaseImponible,
                        op.Fecha FechaEmisionRetencion,
                        '01' CodigoCondicion,
                        '0' CodSujetosSuspendidos,
                        opr.Importe ImporteRetencion,
                        SPACE(6) PorcentajeExclusion,
                        SPACE(10) FechaEmisionBoletin,
                        '80' TipoDocumentoDelRetenido,
                        RIGHT(SPACE(20) + REPLACE(p.CUIT, '-', ''), 20) NroDocumentoDelRetenido,
                        RIGHT(SPACE(14) + '000000' + opr.IdOrdenPagoRetencion, 14) NroCertificadoOriginal,
                        SPACE(53) Espacios
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                        INNER JOIN ProveedoresCodigosRetencion pcr ON p.IdProveedor = pcr.IdProveedor
                        INNER JOIN CodigosRetencion cr ON pcr.IdCodigoRetencion = cr.IdCodigoRetencion
                    /**where**/"
            );

            builder.Where($"cr.IdTipoRetencion IN ({TipoRetencion.IVA}, {TipoRetencion.IVAMonotributo})");
            builder.Where($"op.Fecha >= {fechaD}");
            builder.Where($"op.Fecha <= {fechaH}");
            builder.Where($"opr.IdTipoRetencion IN ({(int)TipoRetencion.IVA}, {(int)TipoRetencion.IVAMonotributo})");
            builder.Where($"op.IdEmpresa = {idEmpresa}");

            var result = await builder.QueryAsync<SicoreIVAProveedores>();

            return result.AsList();
        }

        public async Task<List<SicoreIngresosBrutosProveedores>> GetIngresosBrutosProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            DateTime fechaD = fechaDesde.HasValue ? fechaDesde.Value : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fechaH = fechaHasta.HasValue ? fechaHasta.Value : fechaD.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        RIGHT(SPACE(20) + REPLACE(p.CUIT, '-', ''), 20) CUIT,
                        op.Fecha FechaRetencion,
                        '0000' Sucursal,
                        RIGHT('00000000' + op.IdOrdenPago, 8) NroComprobante,
                        opr.Importe ImporteRetencion,
                        'A' CodigoOperacion
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                        INNER JOIN ProveedoresCodigosRetencion pcr ON p.IdProveedor = pcr.IdProveedor
                        INNER JOIN CodigosRetencion cr ON pcr.IdCodigoRetencion = cr.IdCodigoRetencion
                    /**where**/"
            );

            builder.Where($"cr.IdTipoRetencion = {TipoRetencion.IngresosBrutos}");
            builder.Where($"op.Fecha >= {fechaD}");
            builder.Where($"op.Fecha <= {fechaH}");
            builder.Where($"opr.IdTipoRetencion = {(int)TipoRetencion.IngresosBrutos}");
            builder.Where($"op.IdEmpresa = {idEmpresa}");

            var result = await builder.QueryAsync<SicoreIngresosBrutosProveedores>();

            return result.AsList();
        }

        public async Task<List<SicoreSUSSProveedores>> GetSUSSProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            DateTime fechaD = fechaDesde.HasValue ? fechaDesde.Value : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fechaH = fechaHasta.HasValue ? fechaHasta.Value : fechaD.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        RIGHT(SPACE(20) + REPLACE(p.CUIT, '-', ''), 20) CUIT,
                        op.Fecha FechaRetencion,
                        RIGHT('00000000' + op.IdOrdenPago, 8) NroRetencion,
                        opr.Importe ImporteRetencion
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                        INNER JOIN ProveedoresCodigosRetencion pcr ON p.IdProveedor = pcr.IdProveedor
                        INNER JOIN CodigosRetencion cr ON pcr.IdCodigoRetencion = cr.IdCodigoRetencion
                    /**where**/"
            );

            builder.Where($"cr.IdTipoRetencion = {TipoRetencion.SUSS}");
            builder.Where($"op.Fecha >= {fechaD}");
            builder.Where($"op.Fecha <= {fechaH}");
            builder.Where($"opr.IdTipoRetencion = {(int)TipoRetencion.SUSS}");
            builder.Where($"op.IdEmpresa = {idEmpresa}");

            var result = await builder.QueryAsync<SicoreSUSSProveedores>();

            return result.AsList();
        }

        public async Task<List<SicorePercepcionIngresosBrutosProveedores>> GetIngresosBrutosProveedoresPercepcion(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            DateTime fechaD = fechaDesde.HasValue ? fechaDesde.Value : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fechaH = fechaHasta.HasValue ? fechaHasta.Value : fechaD.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        RIGHT(SPACE(20) + REPLACE(p.CUIT, '-', ''), 20) CUIT,
                        op.Fecha FechaRetencion,
                        RIGHT('00000000' + op.IdOrdenPago, 8) NroRetencion,
                        opr.Importe ImporteRetencion
                    FROM OrdenesPagoRetenciones opr
                        INNER JOIN OrdenesPago op ON opr.idOrdenPago = op.idOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                        INNER JOIN ProveedoresCodigosRetencion pcr ON p.IdProveedor = pcr.IdProveedor
                        INNER JOIN CodigosRetencion cr ON pcr.IdCodigoRetencion = cr.IdCodigoRetencion
                    /**where**/"
            );

            builder.Where($"cr.IdTipoRetencion = {TipoRetencion.SUSS}");
            builder.Where($"op.Fecha >= {fechaD}");
            builder.Where($"op.Fecha <= {fechaH}");
            builder.Where($"opr.IdTipoRetencion = {(int)TipoRetencion.SUSS}");
            builder.Where($"op.IdEmpresa = {idEmpresa}");

            var result = await builder.QueryAsync<SicorePercepcionIngresosBrutosProveedores>();

            return result.AsList();
        }
    }
}
