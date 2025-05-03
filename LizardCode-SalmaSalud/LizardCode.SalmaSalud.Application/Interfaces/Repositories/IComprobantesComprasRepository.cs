using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesComprasRepository
    {
        Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<IList<TComprobanteCompra>> GetAll<TComprobanteCompra>(IDbTransaction transaction = null);

        Task<TComprobanteCompra> GetById<TComprobanteCompra>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TComprobanteCompra>(TComprobanteCompra entity, IDbTransaction transaction = null);

        Task<bool> Update<TComprobanteCompra>(TComprobanteCompra entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.ComprobanteCompra> GetByIdCustom(int idComprobanteVenta, IDbTransaction transaction = null);

        Task<List<Custom.ComprobanteCompraSubdiario>> GetAllSubdiarioCustomQuery(Dictionary<string, object> filters);

        Task<List<Custom.CitiCompra>> GetCITICompras(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.CitiCompraAlicuota>> GetCITIComprasAli(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<Comprobante> GetComprobanteCreditoBySucNro(int idComprobanteAnular, string numeroComprobanteAnular, int idProveedor, int idEmpresa, IDbTransaction tran);
        
        Task<List<Custom.ResumenCtaCtePro>> GetResumenCtaCtePro(Dictionary<string, object> filters);

        Task<List<Custom.ResumenCtaCteProDetalle>> GetCtaCteDetalle(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<double> GetCtaCteDetalleSdoInicio(int idProveedor, DateTime? fechaDesde, int idEmpresa);

        Task<List<Custom.ResumenCtaCteProPendiente>> GetCtasPagar(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<double> GetCtaCteProPendienteSdoInicio(int idProveedor, DateTime? fechaDesde, int idEmpresa);

        Task<Custom.ComprobanteCompra> GetComprobanteBySucNro(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular, int idProveedor, int idEmpresa, IDbTransaction transaction = null);

        Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null);

        Task<List<Custom.SubdiarioCompras>> GetSubdiarioCompras(Dictionary<string, object> filters);

        Task<List<Custom.SubdiarioComprasDetalle>> GetSubdiarioComprasDetalle(Dictionary<string, object> filters);

        Task<DateTime?> GetMinFechaComprobanteCompra(int idEmpresa);

        Task<DateTime?> GetMaxFechaComprobanteCompra(int idEmpresa);

        Task<ComprobanteCompra> GetComprobanteByProveedor(int idComprobante, string numeroComprobante, int idProveedor, IDbTransaction transaction = null);

        Task<Custom.ComprobanteCompraManual> GetManualByIdCustom(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<List<Custom.ComprobanteCompra>> GetComprobantesProveedor(Dictionary<string, object> filters);

        Task<List<Custom.DetalleAlicuota>> GetDetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor, int idEmpresa);

        Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor, int idEmpresa);

		Task<List<Custom.ResumenCtaCteProPendienteGeneral>> GetDetalleGeneralCtasPagar(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.ComprobanteCompraManualPercepcion>> GetPercepcionesByIdComprobanteCompra(int idComprobanteCpa);
    }
}