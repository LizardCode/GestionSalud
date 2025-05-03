using System.Data;

namespace LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories
{
    public interface ITurnosHistorialRepository
    {
        Task<long> Insert<TTurnoHistorial>(TTurnoHistorial entity, IDbTransaction transaction = null);
    }
}
