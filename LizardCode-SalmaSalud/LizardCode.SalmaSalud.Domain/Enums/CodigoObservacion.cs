using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum CodigoObservacion
    {
        [Description("CTA. VENTA DE SERVICIOS")]
        VENTA_DE_SERVICIOS = 1,

        [Description("CTA. BANCOS")]
        BANCOS,

        [Description("CTA. GASTOS BANCARIOS")]
        GASTOS_BANCARIOS,

        [Description("CTA. CREDITO FISCAL I.V.A.")]
        CREDITO_FISCAL_IVA,

        [Description("CTA. DEBITO FISCAL I.V.A.")]
        DEBITO_FISCAL_IVA,

        [Description("CTA. BONIFICACION")]
        BONIFICACION,

        [Description("CTA. VENTA DE ARTICULOS")]
        VENTA_DE_ARTICULOS,

        [Description("CTA. INTEGRADORA CLIENTES")]
        INTEGRADORA_CLIENTES,

        [Description("CTA. INTEGRADORA PROVEEDORES")]
        INTEGRADORA_PROVEEDORES,

        [Description("CTA. CAJA")]
        CAJA,

        [Description("CTA. RETENCION DE IVA CLIENTES")]
        RETENCION_IVA_CLIENTES,

        [Description("CTA. RETENCION DE I. BRUTOS CLIENTES")]
        RETENCION_ING_BRUTOS_CLIENTES,

        [Description("CTA. RETENCION DE GANANCIAS CLIENTES")]
        RETENCION_GANANCIAS_CLIENTES,

        [Description("CTA. VALORES A DEPOSITAR")]
        VALORES_A_DEPOSITAR,

        [Description("CTA. MONEDA EXTRANJERA")]
        MONEDA_EXTRANJERA,

        [Description("CTA. CHEQUES DIFERIDOS")]
        CHEQUES_DIFERIDOS,

        [Description("CTA. CHEQUES RECHAZADOS")]
        CHEQUES_RECHAZADOS,

        [Description("CTA. FONDO FIJO")]
        FONDO_FIJO,

        [Description("CTA. PERCEPCION IVA")]
        PERCEPCION_IVA,

        [Description("CTA. PERCEPCION I. BRUTOS")]
        PERCEPCION_ING_BRUTOS,

        [Description("CTA. RETENCION DE IVA")]
        RETENCION_IVA,

        [Description("CTA. RETENCION I. BRUTOS")]
        RETENCION_ING_BRUTOS,

        [Description("CTA. RETENCION GANANCIAS")]
        RETENCION_GANANCIAS,

        [Description("CTA. DIFERENCIA DE CAMBIO")]
        DIFERENCIA_CAMBIO,

        [Description("CTA. RESULTADO DEL EJERCICIO")]
        RESULTADO_EJERCICIO,

        [Description("CTA. IVA A PAGAR")]
        IVA_A_PAGAR,

        [Description("CTA. IVA SALDO A FAVOR TECNICO")]
        IVA_SALDO_A_FAVOR_TECNICO,

        [Description("CTA. IVA SALDO A FAVOR L/DISPONIBILIDAD")]
        IVA_SALDO_A_FAVOR_LIB_DISPONIBILIDAD,

        [Description("CTA. ING. BRUTOS A PAGAR")]
        ING_BRUTOS_A_PAGAR,

        [Description("CTA. ING. BRUTOS")]
        ING_BRUTOS,

        [Description("CTA. RETENCION DE SUSS")]
        RETENCION_SUSS,

        [Description("CTA. ANTICIPOS A PROVEEDORES")]
        ANTICIPOS_PROVEEDORES,

        [Description("CTA. ANTICIPOS DE CLIENTES")]
        ANTICIPOS_CLIENTES,

        [Description("CTA. REDONDEO")]
        REDONDEO,

        [Description("CTA. PERCEP. IVA CLIENTES")]
        PERCEP_IVA_CLIENTES,

        [Description("CTA. PERCEP. I.BRUTOS CLIENTES")]
        PERCEP_ING_BRUTOS_CLIENTES,

        [Description("CTA. PERCEPCION I.B. CLIENTES CABA")]
        PERCEPCION_ING_BRUTOS_CLIENTES_CABA,

        [Description("CTA. RETENCION I.B. CABA")]
        RETENCION_ING_BRUTOS_CABA,

        [Description("CTA. RETENCION SUSS CLIENTES")]
        RETENCION_SUSS_CLIENTES,

        [Description("CTA. COMPRA DE ARTICULOS")]
        COMPRA_ARTICULOS,

        [Description("CTA. DOCUMENTOS EN CARTERA")]
        DOCUMENTOS_EN_CARTERA,

        [Description("CTA. COSTO MERCADERIA VENDIDA")]
        COSTO_MERCADERIA_VENDIDA,

        [Description("CTA. IMPUESTOS INTERNOS")]
        IMPUESTOS_INTERNOS
    }
}
