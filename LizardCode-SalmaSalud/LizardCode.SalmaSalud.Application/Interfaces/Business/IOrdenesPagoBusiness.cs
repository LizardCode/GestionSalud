using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.OrdenesPago;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IOrdenesPagoBusiness
    {
        Task<OrdenesPagoViewModel> Get(int idRecibo);

        Task<DataTablesResponse<Custom.OrdenPago>> GetAll(DataTablesRequest request);

        Task New(OrdenesPagoViewModel model);

        Task Remove(int idOrdenPago);

        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);

        Task<IList<Custom.OrdenPagoComprobanteImputar>> GetComprobantesImputar(int idOrdenPago, string idMoneda, string idMonedaPago, double cotizacion);

        Task<IList<Custom.OrdenPagoPlanillaGasto>> GetPlanillasImputar(string idMoneda, string idMonedaPago, double cotizacion);

        Task AddPagar(OrdenesPagoViewModel model);

        Task<IList<Custom.OrdenPagoDetalle>> ObtenerDetallePago(int id);

        Task<bool> VerificarChequeDisponible(int idBanco, int idTipoCheque, string nroCheque);

        Task<IList<Custom.OrdenPagoAnticipo>> GetAnticiposImputar(int idProveedor, string idMoneda);
    }
}