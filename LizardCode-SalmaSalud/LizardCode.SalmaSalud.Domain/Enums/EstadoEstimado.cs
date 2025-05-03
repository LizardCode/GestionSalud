using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoEstimado
    {
        [Description("Emitido")]
        Emitido = 1,

        [Description("Aprobado por Cliente")]
        AprobadoCliente,

        [Description("Aprobado para Facturar")]
        AprobadoFacturar,

        [Description("Facturado Parcial")]
        FacturadoParcial,

        [Description("Facturado Total")]
        FacturadoTotal,
    }

}
