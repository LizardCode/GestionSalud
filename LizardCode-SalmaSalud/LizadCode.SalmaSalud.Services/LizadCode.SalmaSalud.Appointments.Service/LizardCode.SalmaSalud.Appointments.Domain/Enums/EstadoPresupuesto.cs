using System.ComponentModel;

namespace LizardCode.SalmaSalud.Appointments.Domain.Enums
{
    public enum EstadoPresupuesto
    {
        [Description("Abierto")]
        Abierto = 1,
        [Description("Aprobado")]
        Aprobado,
        [Description("Rechazado")]
        Rechazado,
        [Description("Vencido")]
        Vencido
    }
}
