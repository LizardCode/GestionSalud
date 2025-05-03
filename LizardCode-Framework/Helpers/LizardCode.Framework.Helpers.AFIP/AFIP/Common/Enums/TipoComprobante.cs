using System.ComponentModel;

namespace LizardCode.Framework.Helpers.AFIP.Common.Enums
{
    public enum TipoComprobante
    {
        [Description("Factura A")]
        FacturaA = 1,

        [Description("Factura B")]
        FacturaB = 6,

        [Description("Factura C")]
        FacturaC = 11,

        [Description("Nota de Crédito A")]
        NCreditoA = 3,

        [Description("Nota de Crédito B")]
        NCreditoB = 8,

        [Description("Nota de Crédito C")]
        NCreditoC = 13,

        [Description("Nota de Débito A")]
        NDebitoA = 2,

        [Description("Nota de Débito B")]
        NDebitoB = 7,

        [Description("Nota de Débito C")]
        NDebitoC = 12,

        [Description("Ticket")]
        Ticket = 10,

        [Description("Factura MiPyme A")]
        FacturaMiPymeA = 201,

        [Description("Factura MiPyme B")]
        FacturaMiPymeB = 206,

        [Description("Factura MiPyme C")]
        FacturaMiPymeC = 211,

        [Description("Nota de Crédito MiPyme A")]
        NCreditoMiPymeA = 203,

        [Description("Nota de Crédito MiPyme B")]
        NCreditoMiPymeB = 208,

        [Description("Nota de Crédito MiPyme C")]
        NCreditoMiPymeC = 213,

        [Description("Nota de Débito MiPyme A")]
        NDebitoMiPymeA = 202,

        [Description("Nota de Débito MiPyme B")]
        NDebitoMiPymeB = 207,

        [Description("Nota de Débito MiPyme C")]
        NDebitoMiPymeC = 212
    }
}
