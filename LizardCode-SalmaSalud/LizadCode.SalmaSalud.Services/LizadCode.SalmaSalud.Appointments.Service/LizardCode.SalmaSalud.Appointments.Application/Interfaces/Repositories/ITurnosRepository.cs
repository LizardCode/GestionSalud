using System.Data;

namespace LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories
{
    public interface ITurnosRepository
    {
        Task<TTurno> GetById<TTurno>(int id, IDbTransaction transaction = null);
        Task<IList<Domain.Entities.Turno>> GetTurnosAusentes();
        Task<long> Insert<TTurno>(TTurno entity, IDbTransaction transaction = null);
        Task<bool> Update<TTurno>(TTurno entity, IDbTransaction transaction = null);
    }
}
