using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.CodigosRetencion;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICodigosRetencionBusiness
    {
        Task<CodigosRetencionViewModel> Get(int idCodigoRetencion);
        Task<DataTablesResponse<Custom.CodigosRetencion>> GetAll(DataTablesRequest request);
        Task New(CodigosRetencionViewModel model);
        Task Remove(int idCodigoRetencion);
        Task Update(CodigosRetencionViewModel model);
        Task<Custom.CodigosRetencion> GetCustom(int idCodigoRetencion);
    }
}