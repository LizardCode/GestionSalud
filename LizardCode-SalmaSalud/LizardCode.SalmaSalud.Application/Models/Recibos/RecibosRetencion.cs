using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Recibos
{
    public class RecibosRetencion
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdRecibo { get; set; }

        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 2)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Categoria", Width = 0, Position = 3)]
        public int IdCategoria { get; set; }
        public string Categoria { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Cuenta Contable", Hidden = true, Position = 4)]
        public int IdCuentaContable { get; set; }

        [AutoNumericConstraint(AutoNumericConstraintType.Numeric, decimalPlaces: 0)]
        [MasterDetailColumn(Header = "Número", Width = 170, Position = 5)]
        public string NroRetencion { get; set; }

        [MasterDetailColumn(Header = "Base Imponible", Width = 150, Position = 6)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double? BaseImponible { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 7)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }


    }
}
