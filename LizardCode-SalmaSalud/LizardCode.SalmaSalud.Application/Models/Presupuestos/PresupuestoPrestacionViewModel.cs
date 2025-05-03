using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Presupuestos
{
    public class PresupuestoPrestacionViewModel
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdPresupuesto { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true, Hidden = true)]
        public int Item { get; set; }

        [RepeaterColumn(Header = "Prestación", ControlType = RepeaterColumnType.Select2, Width = 0, Position = 3)]
        [RequiredEx]
        public int IdPrestacion { get; set; }
        public string Descripcion { get; set; }

        [RepeaterColumn(Header = "Pieza", Position = 4, Width = 40)]
        [RepeaterRemote("ValidarPieza", "Evoluciones", ErrorMessage = "Nro de Pieza incorrecta")]
        public int Pieza { get; set; }

        //[RepeaterColumn(Header = "Código", ControlType = RepeaterColumnType.Input, Width = 65, Position = 5, Readonly = true)]
        public string Codigo { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Valor", Width = 75, Position = 5, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        public double Valor { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Co-Pago", Width = 75, Position = 6)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        public double CoPago { get; set; }
    }
}
