using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.PlanillaGastos
{
    [Sheet("Gastos")]
    public class PlanillaGastosXLSViewModel
    {
        [HeaderCustom(0, "CUIT", CellType.String)]
        public string CUIT { get; set; }

        [HeaderCustom(1, "Fecha", CellType.String)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(2, "Comprobante", CellType.String)]
        public string Comprobante { get; set; }

        [HeaderCustom(3, "Numero", CellType.String)]
        public string Numero { get; set; }

        [HeaderCustom(4, "Detalle", CellType.String)]
        public string Detalle { get; set; }

        [HeaderCustom(5, "NoGravado", CellType.Numeric)]
        public double NoGravado { get; set; }

        [HeaderCustom(6, "Subtotal", CellType.Numeric)]
        public double Subtotal { get; set; }

        [HeaderCustom(7, "Alicuota", CellType.Numeric)]
        public double Alicuota { get; set; }

        [HeaderCustom(8, "Subtotal2", CellType.Numeric)]
        public double Subtotal2 { get; set; }

        [HeaderCustom(9, "Alicuota2", CellType.Numeric)]
        public double Alicuota2 { get; set; }

        [HeaderCustom(10, "Percepcion", CellType.Numeric)]
        public double Percepcion { get; set; }

        [HeaderCustom(11, "Percepcion2", CellType.Numeric)]
        public double Percepcion2 { get; set; }

        [HeaderCustom(12, "ImpuestosInternos", CellType.Numeric)]
        public double ImpuestosInternos { get; set; }

        [HeaderCustom(13, "Total", CellType.Numeric)]
        public double Total { get; set; }

        [HeaderCustom(14, "CAE", CellType.String)]
        public string CAE { get; set; }

        [HeaderCustom(15, "VencimientoCAE", CellType.String)]
        public DateTime VencimientoCAE { get; set; }

    }
}
