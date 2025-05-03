using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoCheque
    {
        [Description("Común")]
        Comun = 1,
        [Description("Diferido")]
        Diferido,
        [Description("E-Cheque Común")]
        E_ChequeComun,
        [Description("E-Cheque Diferido")]
        E_ChequeDiferido,
        [Description("Terceros")]
        Terceros
    }
}
