using LizadCode.SalmaSalud.Notifications.Domain.Enums;
using System.Data;

namespace LizadCode.SalmaSalud.Notifications.Application.Interfaces.Business
{
    public interface IChatApiBusiness
    {
        Task SendMessage(EventosChatApi evento, string telefono, string mensaje, int idEmpresa, int? idPaciente = null, IDbTransaction transaction = null);
    }
}
