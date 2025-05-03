using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoTelefono
    {
        [Description("Móvil")]
        Movil = 1,

        [Description("Casa")]
        Casa,

        [Description("Trabajo")]
        Trabajo,

        [Description("Otro")]
        Otro
    }
}
