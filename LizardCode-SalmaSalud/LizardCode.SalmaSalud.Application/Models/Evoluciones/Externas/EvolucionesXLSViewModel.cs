using LizardCode.Framework.Helpers.Excel;
using LizardCode.Framework.Helpers.Excel.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones
{
    [Sheet("Evoluciones")]
    public class EvolucionesXLSViewModel
    {
        [HeaderCustom(0, "Fecha", CellType.String)]
        public DateTime Fecha { get; set; }

        [HeaderCustom(1, "Documento", CellType.String)]
        public string Documento { get; set; }

        [HeaderCustom(2, "Nombre", CellType.String)]
        public string Nombre { get; set; }

        [HeaderCustom(3, "Telefono", CellType.String)]
        public string Telefono { get; set; }

        [HeaderCustom(4, "Email", CellType.String)]
        public string Email { get; set; }

        [HeaderCustom(5, "Fecha Nacimiento", CellType.String)]
        public DateTime FechaNacimiento { get; set; }

        [HeaderCustom(6, "Plan", CellType.String)]
        public string Plan { get; set; }

        [HeaderCustom(7, "Plan", CellType.String)]
        public string Afiliado { get; set; }

        [HeaderCustom(8, "Diagnostico", CellType.String)]
        public string Diagnostico { get; set; }

        [HeaderCustom(9, "Pieza", CellType.String)]
        public int Pieza { get; set; }

        [HeaderCustom(10, "Prestacion", CellType.String)]
        public string Prestacion { get; set; }

        [HeaderCustom(11, "Prestacion Particular", CellType.String)]
        public string PrestacionParticular { get; set; }
    }
}
