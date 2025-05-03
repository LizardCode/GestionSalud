using LizardCode.SalmaSalud.Appointments.Domain.Entities;

namespace LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business
{
    public interface ITurnosBusiness
    {
        Task<List<Turno>> GetTurnosAusentes();
        Task MarcarAusenteSinAviso(Turno turno);
    }
}
