using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CargaManual
{
    public class CargaManualDetalle
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdComprobanteCompra { get; set; }

        [MasterDetailColumn(Header = "Item", Position = 2, Width = 40)]
        public int Item { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Cuenta Contable", Width = 300, Position = 3)]
        public int IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 4)]
        public string Descripcion { get; set; }
        
        [RequiredEx]
        [MasterDetailColumn(Header = "Alícuota", Width = 110, Position = 5)]
        public int IdAlicuota { get; set; }
        public string Alicuota { get; set; }


        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 6)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

    }
}
