using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesVentasRepository
    {
        Task<IList<TComprobanteVenta>> GetAll<TComprobanteVenta>(IDbTransaction transaction = null);

        Task<TComprobanteVenta> GetById<TComprobanteVenta>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TComprobanteVenta>(TComprobanteVenta entity, IDbTransaction transaction = null);

        Task<bool> Update<TComprobanteVenta>(TComprobanteVenta entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.ComprobanteVenta> GetByIdCustom(int idComprobanteVenta, IDbTransaction transaction = null);
        
        Task<int?> GetLastNumeroByComprobante(int idComprobante, int idSucursal, int idEmpresa, IDbTransaction transaction = null);

        Task<Comprobante> GetComprobanteCreditoBySucNro(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular, int idEmpresa, IDbTransaction transaction = null);

        Task<Custom.ComprobanteVentaAFIP> GetComprobanteVentaAFIP(int idComprobanteVta, IDbTransaction transaction = null);

        Task<List<Custom.ComprobanteVentaSubdiario>> GetAllSubdiarioCustomQuery(Dictionary<string, object> filters);

        Task<List<Custom.CitiVenta>> GetCITIVentas(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.CitiVentaAlicuota>> GetCITIVentasAli(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.ResumenCtaCteCli>> GetResumenCtaCteCli(Dictionary<string, object> filters);

        Task<List<Custom.ResumenCtaCteCliDetalle>> GetCtaCteDetalle(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<double> GetCtaCteDetalleSdoInicio(int idCliente, DateTime? fechaDesde, int idEmpresa);

        Task<List<Custom.ResumenCtaCteCliPendiente>> GetCtasCobrar(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<double> GetCtaCteCliPendienteSdoInicio(int idCliente, DateTime? fechaDesde, int idEmpresa);
        
        Task<ComprobanteVenta> GetComprobanteBySucNro(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular, int idEmpresa, IDbTransaction transaction = null);

        Task<double> GetResumenCtaCteCliDashboard(int idEmpresa);

        Task<bool> RemoveAllByIdSdoCtaCteCli(int idSdoCtaCteCli, IDbTransaction transaction = null);

        Task<List<Custom.SubdiarioVentas>> GetSubdiarioVentas(Dictionary<string, object> filters);

        Task<List<Custom.SubdiarioVentasDetalle>> GetSubdiarioVentasDetalle(Dictionary<string, object> filters);

        Task<DateTime?> GetMinFechaComprobanteVenta(int idEmpresa);

        Task<DateTime?> GetMaxFechaComprobanteVenta(int idEmpresa);

        Task<List<Custom.DetalleAlicuota>> GetDetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente, int idEmpresa);

        Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente, int idEmpresa);

		Task<List<Custom.ResumenCtaCteCliPendienteGeneral>> GetDetalleGeneralCtasCobrar(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);
	}
}