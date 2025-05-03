using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoEstimadoItemCliente
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
        FacturadoTotal
    }

    public enum EstadoEstimadoItemProveedor
    {
        [Description("Emitido")]
        Emitido = 6,

        [Description("Pendiente de Recibir Factura")]
        PendienteRecibirFactura,

        [Description("Facturado Parcial")]
        FacturadoParcial,

        [Description("Facturado Total")]
        FacturadoTotal
    }
}
