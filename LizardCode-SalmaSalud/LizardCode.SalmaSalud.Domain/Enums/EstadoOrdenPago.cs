using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoOrdenPago
    {
        [Description("Ingresado")]
        Ingresado = 1,
        [Description("Pagado")]
        Pagado
    }
}
