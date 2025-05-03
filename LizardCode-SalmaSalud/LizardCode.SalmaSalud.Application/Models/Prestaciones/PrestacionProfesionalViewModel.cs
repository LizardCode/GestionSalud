using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Prestaciones
{
    public class PrestacionProfesionalViewModel
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdPrestacion { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true, Hidden = true)]
        public int Item { get; set; }

        [RepeaterColumn(Header = "Profesional", ControlType = RepeaterColumnType.Select2, Width = 0, Position = 3)]
        [RequiredEx]
        public int IdProfesional { get; set; }
        public string Profesional { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Valor", Width = 125, Position = 4)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        public double ValorFijo { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Porcentaje", Width = 125, Position = 5)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 2)]
        public double Porcentaje { get; set; }
    }
}
