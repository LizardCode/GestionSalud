namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum ImpuestosAFIPRespInscripto
    {
        IVA = 30,
        IVA2 = 2415,
        IVAG = 2852,
        IVA_ADICIONAL_INSCR = 2422,
        IVARG3298 = 130,
        IVAMOLIENDA = 144,
        IVAPROM = 167,
        REINTEGROIVA = 197,
        REINTEGROIVADTO = 205,
        IVA_PRODUCTORES_GANADO = 254,
        IVA_MERCADOS_MAYORISTAS = 330,
        IVA_ADUANA = 729,
        IVA_GANADO_PORCINO = 731,
        IVA_RESTAURANTES = 733,
        IVA_RETENCIONES = 735,
        IVA_POLLOS_PARRILLEROS = 738,
        IVA_CAL_CEMENTO = 739,
        IVA_HARINAS = 760,
        IVA_ANTIDUMPING = 2860
    }

    public enum ImpuestosAFIPMonotributo
    {
        MONOTRIBUTO = 20,
        MONOTRIBUTO_AUTONOMO = 21,
        MONOTRIBUTO_SEGSOCIAL = 22,
        MONOTRIBUTO_INTEG_SOCIEDAD = 23,
        MONOTRIBUTO_OBRA_SOCIAL = 24,
        DJ_INFORMATIVA_MONOTRIBUTO = 199,
        MONEDA_EXT_MONOTRIBUTO = 371
    }

    public enum ImpuestosAFIPExento
    {
        EXENTO_EN_GANANCIAS = 12,
        IVA_EXENTO = 32
    }
}
