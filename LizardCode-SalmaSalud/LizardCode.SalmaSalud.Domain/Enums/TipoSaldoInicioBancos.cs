using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoSaldoInicioBancos
    {
        [Description("Cheques")]
        Cheque = 1,
        [Description("Anticipos Clientes")]
        AnticiposClientes,
        [Description("Anticipos Proveedores")]
        AnticiposProveedores
    }
}
