using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SubdiarioCompras
{
    [Sheet("Subdiario Compras")]
    public class SubdiarioComprasXLSViewModel
    {
        [HeaderCustom(0, "Fecha", CellType.String)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(1, "Comprobante", CellType.String)]
        public string Comprobante { get; set; }

        [HeaderCustom(2, "Sucursal", CellType.String)]
        public string Sucursal { get; set; }

        [HeaderCustom(3, "Numero", CellType.String)]
        public string Numero { get; set; }

        [HeaderCustom(4, "Cliente", CellType.String)]
        public string Cliente { get; set; }

        [HeaderCustom(5, "CUIT", CellType.String)]
        public string CUIT { get; set; }

        [HeaderCustom(6, "TipoIVA", CellType.String)]
        public string TipoIVA { get; set; }

        [HeaderCustom(7, "C. Costos", CellType.String)]
        public string CentroCostos { get; set; }

        [HeaderCustom(8, "Total", CellType.Numeric)]
        public double Total { get; set; }

        [HeaderCustom(9, "Cod. Cuenta", CellType.String)]
        public string CodigoCuenta { get; set; }

        [HeaderCustom(10, "Cuenta", CellType.String)]
        public string NombreCuenta { get; set; }

        [HeaderCustom(11, "Importe", CellType.Numeric)]
        public double Importe { get; set; }
    }
}
