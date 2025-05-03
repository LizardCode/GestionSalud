using System.ComponentModel;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum ListaRetencionesPercepciones
    {
        [Description("Retención Ganancias - Proveedores")]
        GananciasProveedores = 1,

        [Description("Retención I. Brutos - Proveedores")]
        IngresosBrutosProveedores,

        [Description("Retención I.V.A. - Proveedores")]
        IVAProveedores,

        [Description("Retención S.U.S.S. - Proveedores")]
        SUSSProveedores,

        [Description("Percepción I. Brutos - Proveedores")]
        PercepcionIngresosBrutosProveedores,

        [Description("Retención Ganancias - Clientes")]
        GananciasClientes,

        [Description("Retención I. Brutos - Clientes")]
        IngresosBrutosClientes,

        [Description("Retención I.V.A. - Clientes")]
        IVAClientes,

        [Description("Retención S.U.S.S. - Clientes")]
        SUSSClientes,

        [Description("Percepción I. Brutos - Clientes")]
        PercepcionIngresosBrutosClientes,
    }
}
