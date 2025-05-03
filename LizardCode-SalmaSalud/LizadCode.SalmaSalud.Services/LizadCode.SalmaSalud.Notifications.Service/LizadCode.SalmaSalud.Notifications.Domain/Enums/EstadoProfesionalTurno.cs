using System.ComponentModel;

namespace LizadCode.SalmaSalud.Notifications.Domain.Enums
{
    public enum EstadoProfesionalTurno
    {
        [Description("Disponible")]
        Disponible = 1,
        [Description("Agendado")]
        Agendado
    }
}
