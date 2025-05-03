using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Prestaciones
{
    [Sheet("Prestaciones")]
    public class PrestacionXLSViewModel
    {
        [HeaderCustom(0, "Descripcion", CellType.String)]
        public string Descripcion { get; set; }

        [HeaderCustom(1, "Valor", CellType.String)]
        public double Valor { get; set; }

        [HeaderCustom(2, "Codigo", CellType.String)]
        public string Codigo { get; set; }

        [HeaderCustom(3, "Valor Fijo", CellType.String)]
        public double ValorFijo { get; set; }

        [HeaderCustom(4, "Porcentaje", CellType.String)]
        public double Porcentaje { get; set; }
    }
}
