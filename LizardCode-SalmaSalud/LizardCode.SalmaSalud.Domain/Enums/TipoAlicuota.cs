using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoAlicuota
    {
        [Description("I.V.A.")]
        IVA = 1,

        [Description("Impuestos Internos")]
        IVAMonotributo,

        [Description("Ingresos Brutos")]
        IngresosBrutos,

        [Description("S.U.S.S.")]
        SUSS,

        [Description("Percepción I.Br. AGIP")]
        PercepcionAGIP,

        [Description("Percepción I.Br. ARBA")]
        PercepcionARBA
    }
}
