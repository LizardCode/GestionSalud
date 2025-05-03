using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Financiadores
{
    [Sheet("Prestaciones")]
    public class FinanciadorPrestacionXLSViewModel
    {
        [HeaderCustom(0, "Codigo Plan", CellType.String)]
        public string CodigoPlan { get; set; }

        [HeaderCustom(1, "Descripcion", CellType.String)]
        public string Descripcion { get; set; }

        [HeaderCustom(2, "Valor", CellType.String)]
        public double Valor { get; set; }

        [HeaderCustom(3, "Codigo", CellType.String)]
        public string Nomenclador { get; set; }

        [HeaderCustom(4, "Codigo Interno", CellType.String)]
        public string NomencladorInterno { get; set; }

        [HeaderCustom(5, "Valor Fijo", CellType.String)]
        public double ValorFijo { get; set; }

        [HeaderCustom(6, "Porcentaje", CellType.String)]
        public double Porcentaje { get; set; }

        [HeaderCustom(7, "CoPago", CellType.String)]
        public double CoPago { get; set; }
    }
}
