using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoCobro
    {
        [Description("Efectivo")]
        Efectivo = 1,
        [Description("Cheque")]
        Cheque,
        [Description("Transferencia")]
        Transferencia,
        [Description("Documento")]
        Documento
    }
}
