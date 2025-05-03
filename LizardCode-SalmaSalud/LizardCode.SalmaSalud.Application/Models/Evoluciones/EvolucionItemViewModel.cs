using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones
{
    public class EvolucionItemViewModel
    {
        //public int Item { get; set; }
        //public string Descripcion { get; set; }
        //public string Codigo { get; set; }
        //public double Valor { get; set; }

        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdEvolucion { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(Header = "Prestacion", ControlType = RepeaterColumnType.DropDown, Width = 350, Position = 3)]
        [RequiredEx]
        public int IdPrestacion { get; set; }
        public string Prestacion { get; set; }

        [RepeaterColumn(Header = "Código", ControlType = RepeaterColumnType.Input, Width = 350, Position = 4, Readonly = true)]
        public string Codigo { get; set; }

    }
}
