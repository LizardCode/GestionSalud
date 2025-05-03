using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Recibos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IRecibosBusiness
    {
        Task<RecibosViewModel> Get(int idRecibo);
        Task<DataTablesResponse<Custom.Recibo>> GetAll(DataTablesRequest request);
        Task New(RecibosViewModel model);
        Task Remove(int idRecibo);
        Task Update(RecibosViewModel model);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<IList<Custom.ReciboComprobante>> GetComprobantesImputar(int idRecibo);
        Task AddImputaciones(RecibosViewModel model);
        Task<IList<Custom.ReciboAnticipo>> GetAnticiposImputar(int idCliente, string idMoneda);
    }
}