using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TiposPlantilla
    {
        [Description("Comprobante de Ventas Articulos")]

        ComprobanteVentasArticulos = 1,

        [Description("Comprobante de Ventas Automático")]
        ComprobanteVentasAutomatico,

        [Description("Comprobante de Ventas Manual")]
        ComprobanteVentasManual,

        [Description("Orden de Pago")]
        OrdenPago,

        [Description("Recibo")]
        Recibo,

        [Description("Cheque Común")]
        ChequeComun,

        [Description("Cheque Diferido")]
        ChequeDiferido,

        [Description("Retenciones")]
        Retenciones
    }
}
