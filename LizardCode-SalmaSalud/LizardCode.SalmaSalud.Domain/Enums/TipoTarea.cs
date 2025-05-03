using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoTarea
    {
        [Description("Agrupador")]
        Agrupador = 0,

        [Description("Gross Income")]
        GrossIncome = 1,

        [Description("Fee de Agencia")]
        FeeAgencia
    }
}
