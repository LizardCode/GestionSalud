using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoCondicion
    {
        [Description("Ventas")]
        Ventas = 1,
        [Description("Compras")]
        Compras
    }
}
