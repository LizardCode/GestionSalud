using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoAFIP
    {
        [Description("Inicial")]
        Inicial = 1,

        [Description("Observado")]
        Observado,

        [Description("Error")]
        Error,

        [Description("Autorizado")]
        Autorizado,

        [Description("Documento Sin CAE")]
        DocumentoSinCAE,
    }
}
