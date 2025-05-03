using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoPrestacion
    {
        [Description("PRESTACIÓN")]
        Prestacion = 1,

        [Description("CO-PAGO")]
        CoPago
    }
}
