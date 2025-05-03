
using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoFeriado
    {
        [Description("FERIADO")]
        Feriado = 1,
        [Description("EVENTO")]
        Evento,
        [Description("OTRO")]
        Otro
    }
}
