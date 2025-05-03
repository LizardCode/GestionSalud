using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SaldoInicioBanco
{
    public class SaldoInicioBancoAnticiposProveedores
	{
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdSaldoInicioBanco { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Width = 100, Position = 2)]
        public DateTime Fecha { get; set; }

        [MasterDetailColumn(Header = "Proveedor", Width = 0, Position = 3)]
        [RequiredEx]
        public int IdProveedor { get; set; }
        public string Proveedor { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Hidden = true, Position = 4)]
        public string Descripcion { get; set; }

        [MasterDetailColumn(Header = "Moneda", Hidden = true, Position = 5)]
        [RequiredEx]
        public string IdMoneda { get; set; }
        public string Moneda { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Cotización",  Hidden = true, Position = 6)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 7)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }
    }
}
