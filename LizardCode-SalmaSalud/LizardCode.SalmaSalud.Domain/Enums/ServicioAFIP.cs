using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum ServicioAFIP
    {
        [Description("wsfe")]
        WEB_SERVICE_FACTURA_ELECTRONICA = 1,

        [Description("ws_sr_padron_a5")]
        WEB_SERVICE_PADRON,

        [Description("wscdc")]
        WEB_SERVICE_CONSULTA_COMPROBANTES
    }

    public enum ConceptosComprobantesAFIP
    {
        COMPROBANTE_BIENES = 1,
        COMPROBANTE_SERVICIOS = 2
    }
}
