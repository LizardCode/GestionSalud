using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoIVA
    {
        [Description("Consumidor Final")]
        ConsumidorFinal = 1,

        [Description("Responsable Inscripto")]
        ResponsableInscripto,

        [Description("Monotributo")]
        Monotributo,

        [Description("Exento")]
        Exento,

        [Description("Exento No Gravado")]
        ExentoNoGravado,

        [Description("Exterior")]
        Exterior

    }
}
