using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Models;

namespace LizardCode.SalmaSalud.Application.Models.SubdiarioIVAVentas
{
    public class CitiVentaAlicuota : ISicoreCiti
    {
        [SubstringAttribute(iNro: 1, attrType: SubstringAttributeType.String, iLen: 3, PadLeft = "0")]
        public int TipoComprobante { get; set; }
        [SubstringAttribute(iNro: 2, attrType: SubstringAttributeType.String, iLen: 5)]
        public string PuntoVenta { get; set; }
        [SubstringAttribute(iNro: 3, attrType: SubstringAttributeType.String, iLen: 20, PadLeft = "0")]
        public string NroComprobante { get; set; }
        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImporteNetoGravado { get; set; }
        [SubstringAttribute(iNro: 5, attrType: SubstringAttributeType.Integer, iLen: 4)]
        public int Alicuota { get; set; }
        [SubstringAttribute(iNro: 6, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImpuestoLiquidado { get; set; }
    }
}
