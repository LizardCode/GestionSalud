using System.ComponentModel;

namespace LizardCode.Framework.Application.Common.Enums
{
    public enum TipoUsuario
    {
        [Description("ADMIN")]
        Administrador = 1,
        [Description("ADMINISTRACION")]
        Administracion = 2,
        [Description("CUENTAS")]
        Cuentas = 3,
        [Description("TESORERIA")]
        Tesoreria = 4,
        [Description("CUENTAS POR PAGAR")]
        CuentasPorPagar = 5,
        [Description("CUENTAS POR COBRAR")]
        CuentasPorCobrar = 6,
        [Description("PROFESIONAL")]
        Profesional = 7,
        [Description("RECEPCION")]
        Recepcion = 8,
        [Description("PACIENTE")]
        Paciente = 9,
        [Description("PROFESIONAL_EXTERNO")]
        ProfesionalExterno = 10
    }
}
