using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoRecibo
    {
        [Description("Ingresado")]
        Ingresado = 1,
        [Description("Finalizado")]
        Finalizado,
        [Description("Anulado")]
        Anulado
    }
}
