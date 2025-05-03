using LizadCode.SalmaSalud.Notifications.Domain.Entities;

namespace LizadCode.SalmaSalud.Notifications.Application.Interfaces.Business
{
    public interface ITurnosBusiness
    {
        Task<List<Turno>> GetTurnosAConfirmar();
        Task EnviarRecordatorio(Turno turno);
    }
}
