using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoInterfaz
    {
        [Description("Mis Comprobantes AFIP")]
        MisComprobantesAFIP = 0,
        [Description("Linkside Facturante.com")]
        LinksideFacturante,
        [Description("Interfaz Custom DAWASOFT")]
        CustomInterfaz
    }
}
