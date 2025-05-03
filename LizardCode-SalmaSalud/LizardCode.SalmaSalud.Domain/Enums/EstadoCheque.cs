using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoCheque
    {
        [Description("Sin Librar")]
        SinLibrar = 1,
        [Description("Librado")]
        Librado,
        [Description("Debitado/Depositado")]
        Debitado_Depositado,
        [Description("Anulado")]
        Anulado,
        [Description("Rechazado")]
        Rechazado,
        [Description("Debitado/Rechazado")]
        Debitado_Rechazado,
        [Description("En Cartera")]
        EnCartera,
        [Description("Entregado")]
        Entregado
    }
}
