using Dapper.DataTables.Models;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPedidosLaboratoriosHistorialRepository
    {
        Task<long> Insert<PedidosLaboratoriosHistorial>(PedidosLaboratoriosHistorial entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        DataTablesCustomQuery GetHistorial(int idPedidoLaboratorio);
    }
}
