using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.FacturacionAutomatica
{
    public class FacturacionAutomaticaDetalle
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdComprobanteVenta { get; set; }

        [MasterDetailColumn(Header = "", Position = 2, Hidden = true)]
        public int? IdEvolucionPrestacion { get; set; }

        [MasterDetailColumn(Header = "", Position = 3, Hidden = true)]
        public int? IdEvolucionOtraPrestacion { get; set; }



        [MasterDetailColumn(Header = "Item", Position = 4, Width = 40)]
        public int Item { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 5)]
        public string Descripcion { get; set; }
        
        [RequiredEx]
        [MasterDetailColumn(Header = "Moneda Comprobante", Width = 200, Position = 6)]
        public string IdMoneda { get; set; }
        public string Moneda { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Alícuota", Width = 110, Position = 7)]
        public string Alicuota { get; set; }

        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 8)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        public double Importe { get; set; }

    }
}
