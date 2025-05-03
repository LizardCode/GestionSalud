using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Clientes;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IClientesBusiness
    {
        Task<ClienteViewModel> Get(int idClient);
        Task<DataTablesResponse<Custom.Cliente>> GetAll(DataTablesRequest request);
        Task New(ClienteViewModel model);
        Task Remove(int idClient);
        Task Update(ClienteViewModel model);
        Task<string> ValidarNroCUIT(string cuit, int? idCliente, int? idTipoDocumento);
        Task<Custom.Contribuyente> GetPadron(string cuit);
        Task<string> ValidarNroDocumento(string documento, int? idCliente);
    }
}