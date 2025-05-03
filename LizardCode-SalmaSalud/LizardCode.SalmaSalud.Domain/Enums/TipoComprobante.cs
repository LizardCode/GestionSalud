using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum TipoComprobante
    {
        [Description("Automática de Clientes")]
        AutomaticaClientes = 1,

        [Description("Manual de Clientes")]
        ManualClientes,

        [Description("Manual de Articulos")]
        ManualArticulos,

        [Description("Automática de Proveedores")]
        AutomaticaProveedores,

        [Description("Manual de Proveedores")]
        ManualProveedores,

        [Description("Automática Anula Factura Clientes")]
        AutomaticaAnulaFacturaClientes,

        [Description("Automática Anula Factura Proveedores")]
        AutomaticaAnulaFacturaProveedores,

        [Description("Gastos de Proveedores")]
        GastosProveedores,

        [Description("Cargos Bancarios")]
        CargosBancarios,

        [Description("Interfaz")]
        Interfaz
    }
}
