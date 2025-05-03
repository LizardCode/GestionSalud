using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SubdiarioCobros
{
    [Sheet("Subdiario Cobros")]
    public class SubdiarioCobrosXLSViewModel
    {
        [HeaderCustom(0, "Fecha", CellType.String)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(1, "Comprobante", CellType.String)]
        public string Comprobante { get; set; }

        [HeaderCustom(2, "Numero", CellType.String)]
        public string Numero { get; set; }

        [HeaderCustom(3, "Cliente", CellType.String)]
        public string Cliente { get; set; }

        [HeaderCustom(4, "CUIT", CellType.String)]
        public string CUIT { get; set; }

        [HeaderCustom(5, "Total", CellType.Numeric)]
        public double Total { get; set; }

        [HeaderCustom(6, "Cod. Cuenta", CellType.String)]
        public string CodigoCuenta { get; set; }

        [HeaderCustom(7, "Cuenta", CellType.String)]
        public string NombreCuenta { get; set; }

        [HeaderCustom(8, "Importe", CellType.Numeric)]
        public double Importe { get; set; }
    }
}
