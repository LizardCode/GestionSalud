using System.ComponentModel;

namespace LizardCode.SalmaSalud.Appointments.Domain.Enums
{
    public enum EstadoProfesionalTurno
    {
        [Description("Disponible")]
        Disponible = 1,
        [Description("Agendado")]
        Agendado
    }
}
