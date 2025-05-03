using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoOrdenPago
    {
        [Description("Proveedores")]
        Proveedores = 1,
        [Description("Gastos")]
        Gastos,
        [Description("Anticipo")]
        Anticipo,
        [Description("Varios")]
        Varios
    }
}
