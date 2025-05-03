using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum RubrosContablesMaestro
    {
        [Description("ACTIVO")]
        Activo = 100000,

        [Description("PASIVO")]
        Pasivo = 200000,

        [Description("PATRIMONIO NETO")]
        PatrimonioNeto = 300000,

        [Description("PERDIDAS Y GANANCIAS")]
        EstadoDeResultados = 400000,

    }
}
