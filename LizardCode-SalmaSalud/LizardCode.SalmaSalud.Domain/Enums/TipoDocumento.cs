using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoDocumento
    {
        [Description("CUIT")]
        CUIT = 80,

        [Description("CUIL")]
        CUIL = 86,

        [Description("CDI")]
        CDI = 87,

        [Description("LIBRETA DE ENROLAMIENTO")]
        LibretaEnrolamiento = 89,

        [Description("LIBRETA CIVICA")]
        LibretaCivica = 90,

        [Description("PASAPORTE")]
        Pasaporte = 94,

        [Description("DNI")]
        DNI = 96
    }
}
