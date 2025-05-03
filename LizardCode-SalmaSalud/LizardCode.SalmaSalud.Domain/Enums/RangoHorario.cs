using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum RangoHorario
    {
        [Description("07-09 hs.")]
        R0709 = 1,
        [Description("09-11 hs.")]
        R0911 = 2,
        [Description("11-13 hs.")]
        R1113 = 3,
        [Description("13-15 hs.")]
        R1315 = 4,
        [Description("15-17 hs.")]
        R1517 = 5,
        [Description("17-19 hs.")]
        R1719 = 6
    }
}
