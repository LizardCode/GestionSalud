using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CodigosRetencion
{
    public class CodigosRetencionDetalle
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdCodigoRetencion { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe Desde", Width = 200, Position = 3)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteDesde { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe Hasta", Width = 200, Position = 4)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteHasta { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe Retención", Width = 200, Position = 5)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteRetencion { get; set; }

        [RepeaterColumn(RepeaterColumnType.Number, Header = "Mas %", Width = 200, Position = 6)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double MasPorcentaje { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Sobre el Excedente", Width = 0, Position = 7)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double SobreExcedente { get; set; }

    }
}
