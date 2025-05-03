using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Recibos
{
    public class RecibosImputacion
    {
        [RepeaterColumn(RepeaterColumnType.Check, Width = 100, Header = "Seleccionar", Position = 1)]
        public int Seleccionar { get; set; }

        [RepeaterColumn(Header = "Id", Position = 2, Hidden = true)]
        public int IdComprobanteVenta { get; set; }

        [RepeaterColumn(Header = "Tipo de Comprobante", Width = 200, Position = 3, Readonly = true)]
        public string TipoComprobante { get; set; }

        [RepeaterColumn(Header = "N° Comprobante", Width = 0, Position = 4, Readonly = true)]
        public string NumeroComprobante { get; set; }

        [RepeaterColumn(Header = "Total", Width = 130, Position = 5, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Total { get; set; }

        [RepeaterColumn(Header = "Saldo", Width = 130, Position = 6, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Saldo { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe", Width = 130, Position = 7)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Cotizacion", Hidden = true, Position = 8)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

    }
}
