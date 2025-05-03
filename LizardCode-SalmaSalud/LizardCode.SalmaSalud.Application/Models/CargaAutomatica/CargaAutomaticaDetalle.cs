using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CargaAutomatica
{
    public class CargaAutomaticaDetalle
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdComprobanteCompra { get; set; }

        [MasterDetailColumn(Header = "Item", Position = 2, Width = 40)]
        public int Item { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Estimado", Width = 150, Position = 3)]
        public string Estimado { get; set; }


        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 4)]
        public string Descripcion { get; set; }
        
        [RequiredEx]
        [MasterDetailColumn(Header = "Moneda Comprobante", Width = 200, Position = 5)]
        public string Moneda { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Alícuota", Width = 110, Position = 6)]
        [AutoNumericConstraint(AutoNumericConstraintType.Percentage, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Alicuota { get; set; }

        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 7)]
        [RequiredEx]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

    }
}
