using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum CategoriaRetencion
    {
        [Description("Ganancias")]
        Ganancias = 1,

        [Description("I.V.A.")]
        IVA,

        [Description("Ingresos Brutos")]
        IngresosBrutos,

        [Description("S.U.S.S.")]
        SUSS,

    }
}
