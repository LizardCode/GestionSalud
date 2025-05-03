using LizadCode.SalmaSalud.Notifications.Domain.EntitiesCustom;
using System.Data;

namespace LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories
{
    public interface ITurnosRepository
    {
        Task<TTurno> GetById<TTurno>(int id, IDbTransaction transaction = null);
        Task<Turno> GetByIdCustom(int idTurno, IDbTransaction transaction = null);
        Task<IList<Domain.Entities.Turno>> GetTurnosAConfirmar();
        Task<long> Insert<TTurno>(TTurno entity, IDbTransaction transaction = null);
        Task<bool> Update<TTurno>(TTurno entity, IDbTransaction transaction = null);
    }
}
