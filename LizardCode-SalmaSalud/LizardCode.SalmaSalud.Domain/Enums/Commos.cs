using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum Commons
    {
        [Description("S")]
        Si = 1,
        [Description("N")]
        No
    }

    public enum Monedas
    {
        [Description("PES")]
        MonedaLocal = 1,
    }

    public enum Modulos
    {
        [Description("CLI")]
        Clientes = 1,

        [Description("PRO")]
        Proveedores,

        [Description("CAJ")]
        CajaBancos
    }

    public enum Letras
    {
        [Description("A")]
        A = 1,

        [Description("B")]
        B = 2,

        [Description("C")]
        C = 3,

        [Description("E")]
        E = 4,

        [Description("M")]
        M = 5,

        [Description("X")]
        X = 6
    }

    public enum Genero
    {
        [Description("M")]
        Masculino,
        [Description("F")]
        Femenino,
        [Description("E")]
        Empresa
    }
}
