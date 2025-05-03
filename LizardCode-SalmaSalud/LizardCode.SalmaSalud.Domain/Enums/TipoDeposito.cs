using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoDeposito
    {
        [Description("Efectivo")]
        Efectivo = 1,
        [Description("Cheque Común")]
        ChequeComun,
        [Description("Cheque Diferido")]
        ChequeDiferido,
        [Description("E-Cheque Común")]
        EChequeComun,
        [Description("E-Cheque Diferido")]
        EChequeDiferido,
        [Description("Cheque de Terceros")]
        ChequeTerceros,
        [Description("Transferencia")]
        Transferencia
    }
}
