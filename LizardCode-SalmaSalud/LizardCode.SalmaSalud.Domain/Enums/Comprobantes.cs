using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum Comprobantes
    {
        [Description("FACTURA A")]
        FACTURA_A = 1,
        [Description("FACTURA B")]
        FACTURA_B,
        [Description("FACTURA C")]
        FACTURA_C,
        [Description("FACTURA E")]
        FACTURA_E,
        [Description("N/CREDITO A")]
        NCREDITO_A,
        [Description("N/CREDITO B")]
        NCREDITO_B,
        [Description("N/CREDITO C")]
        NCREDITO_C,
        [Description("N/CREDITO E")]
        NCREDITO_E,
        [Description("N/DEBITO A")]
        NDEBITO_A,
        [Description("N/DEBITO B")]
        NDEBITO_B,
        [Description("N/DEBITO C")]
        NDEBITO_C,
        [Description("N/DEBITO E")]
        NDEBITO_E,
        [Description("TICKET A")]
        TICKET_A,
        [Description("TICKET B")]
        TICKET_B,
        [Description("TICKET C")]
        TICKET_C,
        [Description("FACTURA MIPYME A")]
        FACTURA_MIPYME_A,
        [Description("N/DEBITO MIPYME A")]
        NDEBITO_MIPYME_A,
        [Description("N/CREDITO MIPYME A")]
        NCREDITO_MIPYME_A,
        [Description("FACTURA MIPYME B")]
        FACTURA_MIPYME_B,
        [Description("N/DEBITO MIPYME B")]
        NDEBITO_MIPYME_B,
        [Description("N/CREDITO MIPYME B")]
        NCREDITO_MIPYME_B,
        [Description("FACTURA MIPYME C")]
        FACTURA_MIPYME_C,
        [Description("N/DEBITO MIPYME C")]
        NDEBITO_MIPYME_C,
        [Description("N/CREDITO MIPYME C")]
        NCREDITO_MIPYME_C,
        [Description("FACTURAS M")]
        FACTURAS_M,
        [Description("N/DEBITO M")]
        NDEBITO_M,
        [Description("N/CREDITO M")]
        NCREDITO_M,
        [Description("OTROS COMPROBANTES")]
        OTROS_COMPROBANTES


    }
}
