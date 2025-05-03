using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoRepository
    {
        Task<IList<TOrdenPago>> GetAll<TOrdenPago>(IDbTransaction transaction = null);

        Task<TOrdenPago> GetById<TOrdenPago>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TOrdenPago>(TOrdenPago entity, IDbTransaction transaction = null);

        Task<bool> Update<TOrdenPago>(TOrdenPago entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.OrdenPago> GetByIdCustom(int idOrdenPago, IDbTransaction transaction = null);

        Task<double?> GetTotalRetenido(int idTipoRetencion, DateTime fecha, int idProveedor, int idEmpresa, IDbTransaction transaction = null);

		Task<List<SubdiarioPagos>> GetSubdiarioPagos(Dictionary<string, object> filters);

		Task<List<SubdiarioPagosDetalle>> GetSubdiarioPagosDetalle(Dictionary<string, object> filters);

        Task<int> GetCantidadOrdenesPagoProveedor(int idProveedor, int idEmpresa);

        DataTablesCustomQuery GetUltimasOrdenesPagoProveedor(int idProveedor, int idEmpresa);

        Task<List<Custom.OrdenPago>> GetOrdenesPagoProveedor(Dictionary<string, object> filters);

        Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<dynamic>> GetOrdenesPagoByCheque(int idCheque);

		Task<DateTime?> GetMaxFechaOrdenPago(int idEmpresa);

		Task<DateTime?> GetMinFechaOrdenPago(int idEmpresa);
	}
}