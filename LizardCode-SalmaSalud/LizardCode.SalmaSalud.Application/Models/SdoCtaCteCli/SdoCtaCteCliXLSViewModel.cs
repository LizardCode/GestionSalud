using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli
{
    [Sheet("Saldo Inicio")]
    public class SdoCtaCteCliXLSViewModel
    {
        [HeaderCustom(0, "CUIT", CellType.String)]
        public string CUIT { get; set; }

        [HeaderCustom(1, "Fecha", CellType.Numeric)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(2, "Vencimiento", CellType.Numeric)]
        public DateTime Vencimiento { get; set; }

        [HeaderCustom(3, "Comprobante", CellType.String)]
        public string Comprobante { get; set; }

        [HeaderCustom(4, "Sucursal", CellType.String)]
        public string Sucursal { get; set; }

        [HeaderCustom(5, "Numero", CellType.String)]
        public string Numero { get; set; }

        [HeaderCustom(6, "Neto", CellType.String)]
        public double Neto { get; set; }

        [HeaderCustom(7, "Alicuota", CellType.String)]
        public double Alicuota { get; set; }

        [HeaderCustom(8, "Total", CellType.String)]
        public double Total{ get; set; }
    }
}
