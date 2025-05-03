using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CargaManual
{
    public class CargaManualPercepciones
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdComprobanteCompra { get; set; }

        [MasterDetailColumn(Header = "Item", Position = 2, Width = 40)]
        public int Item { get; set; }
        
        [RequiredEx]
        [MasterDetailColumn(Header = "Cuenta Contable", Width = 0, Position = 3)]
        public int IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }

        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 4)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

    }
}
