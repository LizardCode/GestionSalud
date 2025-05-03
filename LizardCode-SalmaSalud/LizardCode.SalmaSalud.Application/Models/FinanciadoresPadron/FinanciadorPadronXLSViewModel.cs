using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;

namespace LizardCode.SalmaSalud.Application.Models.FinanciadoresPadron
{
    [Sheet("Afiliados")]
    public class FinanciadorPadronXLSViewModel
    {
        [HeaderCustom(0, "Documento", CellType.String)]
        public string Documento { get; set; }

        [HeaderCustom(1, "Afiliado", CellType.String)]
        public string FinanciadorNro{ get; set; }

        [HeaderCustom(2, "Nombre", CellType.String)]
        public string Nombre { get; set; }
    }
}