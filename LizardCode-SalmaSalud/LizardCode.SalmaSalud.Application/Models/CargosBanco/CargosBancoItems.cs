using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.SalmaSalud.Application.Models.CargosBanco
{
    public class CargosBancoItems
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdCargoBanco { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Cuenta", Width = 200, Position = 3)]
        [RequiredEx]
        public int IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Detalle", Width = 0, Position = 4)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RepeaterRemote("ValidacionRemota", "CargosBanco", AdditionalFields = "Importe,IdCuentaContable", ErrorMessage = "Ejemplo Error")]
        [RequiredEx]
        public string Detalle { get; set; }

        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Alicuota", Width = 80, Position = 5)]
        public double IdAlicuota { get; set; }
        public string Alicuota { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe", Width = 150, Position = 6)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Total", Position = 7, Width = 150, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Total { get; set; }

    }
}
