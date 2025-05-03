using System.Data;

namespace LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories
{
    public interface ITurnosHistorialRepository
    {
        Task<long> Insert<TTurnoHistorial>(TTurnoHistorial entity, IDbTransaction transaction = null);
    }
}
