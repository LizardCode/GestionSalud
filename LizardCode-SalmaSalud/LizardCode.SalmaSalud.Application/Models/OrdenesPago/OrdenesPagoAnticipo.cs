using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.OrdenesPago
{
    public class OrdenesPagoAnticipo
    {
        [RepeaterColumn(RepeaterColumnType.Check, Width = 100, Header = "Seleccionar", Position = 1)]
        public bool Seleccionar { get; set; }

        [RepeaterColumn(Header = "Id", Position = 2, Hidden = true)]
        public int IdOrdenPago { get; set; }

        [RepeaterColumn(Header = "Descripción", Width = 0, Position = 3, Readonly = true)]
        public string Descripcion { get; set; }

        [RepeaterColumn(Header = "Saldo a Imputar", Width = 130, Position = 4, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Saldo { get; set; }

        [RepeaterColumn(Header = "Importe", Width = 130, Position = 5, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }
    }
}
