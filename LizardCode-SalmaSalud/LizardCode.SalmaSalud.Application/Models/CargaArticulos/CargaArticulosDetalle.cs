using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CargaArticulos
{
    public class CargaArticulosDetalle
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdComprobanteCompra { get; set; }

        [MasterDetailColumn(Header = "Item", Position = 2, Width = 40)]
        public int Item { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Articulo", Width = 300, Position = 3)]
        public int IdArticulo { get; set; }
        public string Articulo { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 4)]
        public string Descripcion { get; set; }
        
        [RequiredEx]
        [MasterDetailColumn(Header = "Alícuota", Width = 110, Position = 5)]
        public int IdAlicuota { get; set; }
        public string Alicuota { get; set; }

        [MasterDetailColumn(Header = "Precio Unitario", Width = 110, Position = 6)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double PrecioUnitario { get; set; }

        [MasterDetailColumn(Header = "Cantidad", Width = 110, Position = 7)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cantidad { get; set; }


        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 8)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

    }
}
