using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.AperturaAutoCuentas
{
    public class AperturaAutoCuentasDetalle
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdAsiento { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Cuenta", Width = 300, Position = 3)]
        [RequiredEx]
        public int IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Detalle", Width = 0, Position = 4)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RequiredEx]
        public string Detalle { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Débitos", Width = 200, Position = 5)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double Debitos { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Créditos", Width = 200, Position = 6)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double Creditos { get; set; }

    }
}
