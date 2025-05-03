using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoRecibo
    {
        [Description("Recibo Común")]
        ReciboComun = 1,
        [Description("Anticipo")]
        Anticipo
    }
}
