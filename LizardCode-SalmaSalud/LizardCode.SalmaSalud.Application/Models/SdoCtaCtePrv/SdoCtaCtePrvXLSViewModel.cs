using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SdoCtaCtePrv
{
    [Sheet("Saldo Inicio")]
    public class SdoCtaCtePrvXLSViewModel
    {
        [HeaderCustom(0, "CUIT", CellType.String)]
        public string CUIT { get; set; }

        [HeaderCustom(1, "Fecha", CellType.String)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(2, "Vencimiento", CellType.String)]
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

        [HeaderCustom(8, "Percepciones", CellType.String)]
        public double Percepciones { get; set; }

        [HeaderCustom(9, "Total", CellType.String)]
        public double Total { get; set; }
    }
}
