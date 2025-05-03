using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.AnulaComprobantesVenta
{
    public class AnulaComprobantesVentaDetalle
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdComprobanteVenta { get; set; }

        [MasterDetailColumn(Header = "Item", Position = 2, Width = 40)]
        public int Item { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 3)]
        public string Descripcion { get; set; }
        
        [RequiredEx]
        [MasterDetailColumn(Header = "Alícuota", Width = 110, Position = 4)]
        public string Alicuota { get; set; }

        [MasterDetailColumn(Header = "Importe", Width = 150, Position = 5)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        public double Importe { get; set; }

    }
}
