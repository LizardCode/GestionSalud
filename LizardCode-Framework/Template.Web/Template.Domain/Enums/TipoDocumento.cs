using System.ComponentModel;

namespace Template.Domain.Enums
{
    public enum TipoDocumento
    {
        [Description("DNI")]
        Dni = 1,

        [Description("LE")]
        Le,

        [Description("LC")]
        Lc,

        [Description("CIPF")]
        Cipf,

        [Description("Pasaporte")]
        Pasaporte
    }
}
