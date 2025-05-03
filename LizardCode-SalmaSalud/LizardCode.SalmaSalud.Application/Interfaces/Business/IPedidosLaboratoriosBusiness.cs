using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPedidosLaboratoriosBusiness
    {
        Task<PedidoLaboratorioViewModel> Get(int idPedidoLaboratorio);
        Task<DataTablesResponse<Custom.PedidoLaboratorio>> GetAll(DataTablesRequest request);
        Task New(PedidoLaboratorioViewModel model);
        Task Remove(int idPedidoLaboratorio);
        //Task Enviar(int idPedidoLaboratorio);
        //Task Recibir(int idPedidoLaboratorio);
        Task Marcar(EnviarItemViewModel model, EstadoPedidoLaboratorio estado);
        Task<DataTablesResponse<Custom.PedidoLaboratorioHistorial>> GetHistorial(int idPedidoLaboratorio, DataTablesRequest request);
    }
}
