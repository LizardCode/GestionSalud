using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SdoCtaCtePrv
{
    public class SdoCtaCtePrvDetalle
    {
        public SdoCtaCtePrvDetalle()
        {
            Fecha = DateTime.Now.ToString("dd/MM/yyyy");
            Vencimiento = DateTime.Now.ToString("dd/MM/yyyy");
        }

        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdSdoCtaCteCliComprobanteVenta { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(Header = "Proveedor", ControlType = RepeaterColumnType.Select2, Width = 0, Position = 3)]
        [RequiredEx]
        public int IdProveedor { get; set; }
        public string CUIT { get; set; }

        [RepeaterColumn(ControlType = RepeaterColumnType.Input, Header = "Fecha", Width = 95, Position = 4)]
        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public string Fecha { get; set; }

        [RepeaterColumn(ControlType = RepeaterColumnType.Input, Header = "Vencimiento", Width = 95, Position = 5)]
        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public string Vencimiento { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Comprobante", Width = 120, Position = 6)]
        public int IdComprobante { get; set; }
        public string Comprobante { get; set; }

        [RepeaterColumn(ControlType = RepeaterColumnType.Input, Header = "Sucursal", Width = 80, Position = 7)]
        [RequiredEx]
        public string Sucursal { get; set; }

        [RepeaterColumn(ControlType = RepeaterColumnType.Input, Header = "Numero", Width = 80, Position = 8)]
        [RequiredEx]
        public string Numero { get; set; }

        [RepeaterColumn(Header = "Neto Gravado", ControlType = RepeaterColumnType.Currency, Width = 120, Position = 9)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double NetoGravado { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Alicuota", Width = 120, Position = 10)]
        public double IdAlicuota { get; set; }
        public string Alicuota { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Percepciones", Width = 100, Position = 11)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double Percepciones { get; set; }

        [RepeaterColumn(Header = "Total", ControlType = RepeaterColumnType.Currency, Width = 120, Position = 12, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double Total { get; set; }
    }
}
