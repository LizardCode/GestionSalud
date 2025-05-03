using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.CargaManual
{
    [Sheet("Comprobantes")]
    public class CargaManualXLS
    {
        [HeaderCustom(0, "CUIT", CellType.String)]
        public string CUIT { get; set; }

        [HeaderCustom(1, "Proveedor", CellType.String)]
        public string Proveedor { get; set; }

        [HeaderCustom(2, "Fecha", CellType.String)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(3, "Comprobante", CellType.String)]
        public string Comprobante { get; set; }

        [HeaderCustom(4, "Numero", CellType.String)]
        public string Numero { get; set; }

        [HeaderCustom(5, "Cta_NoGravado", CellType.String)]
        public string Cta_NoGravado { get; set; }

        [HeaderCustom(6, "NoGravado", CellType.Numeric)]
        public double NoGravado { get; set; }

        [HeaderCustom(7, "Cta", CellType.String)]
        public string Cta { get; set; }

        [HeaderCustom(8, "Neto", CellType.Numeric)]
        public double Neto { get; set; }

        [HeaderCustom(9, "Alicuota", CellType.Numeric)]
        public double Alicuota { get; set; }

        [HeaderCustom(10, "Cta2", CellType.String)]
        public string Cta2 { get; set; }

        [HeaderCustom(11, "Neto2", CellType.Numeric)]
        public double Neto2 { get; set; }

        [HeaderCustom(12, "Alicuota2", CellType.Numeric)]
        public double Alicuota2 { get; set; }

        [HeaderCustom(13, "Cta_Percepcion", CellType.String)]
        public string Cta_Percepcion { get; set; }

        [HeaderCustom(14, "Percepcion", CellType.Numeric)]
        public double Percepcion { get; set; }

        [HeaderCustom(15, "Cta_Percepcion2", CellType.String)]
        public string Cta_Percepcion2 { get; set; }

        [HeaderCustom(16, "Percepcion2", CellType.Numeric)]
        public double Percepcion2 { get; set; }

        [HeaderCustom(17, "ImpuestosInternos", CellType.Numeric)]
        public double ImpuestosInternos { get; set; }

        [HeaderCustom(18, "Total", CellType.Numeric)]
        public double Total { get; set; }

        [HeaderCustom(19, "CAE", CellType.String)]
        public string CAE { get; set; }

        [HeaderCustom(20, "VencimientoCAE", CellType.String)]
        public DateTime VencimientoCAE { get; set; }

    }
}
