using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoTurno
    {
        [Description("T")]
        Turno = 0,

        [Description("ST")]
        SobreTurno = 1,

        [Description("DE")]
        DemandaEspontanea = 2,

        [Description("GU")]
        Guardia = 3
    }
}
