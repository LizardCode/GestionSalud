using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoAsientoAuto
    {
        [Description("Asiento Apertura de Cuentas")]
        AsientoAperturaCuentas = 1,

        [Description("Asiento Cierre de Cuentas Patrimoniales")]
        AsientoCierreCuentasPatrimoniales,

        [Description("Asiento Cierre de Cuentas de Resultados")]
        AsientoCierreCuentasResultados

    }
}
