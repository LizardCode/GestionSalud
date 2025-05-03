using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Models;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SubdiarioIVAVentas
{
    public class CitiVenta : ISicoreCiti
    {
        [SubstringAttribute(iNro: 1, attrType: SubstringAttributeType.DateTime, iLen: 8, Format = "yyyyMMdd")]
        public DateTime Fecha { get; set; }
        [SubstringAttribute(iNro: 2, attrType: SubstringAttributeType.String, iLen: 3, PadLeft = "0")]
        public string TipoComprobante { get; set; }
        [SubstringAttribute(iNro: 3, attrType: SubstringAttributeType.String, iLen: 5)]
        public string PuntoVenta { get; set; }
        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.String, iLen: 20, PadLeft = "0")]
        public string NroComprobanteDesde { get; set; }
        [SubstringAttribute(iNro: 5, attrType: SubstringAttributeType.String, iLen: 20, PadLeft = "0")]
        public string NroComprobanteHasta { get; set; }
        [SubstringAttribute(iNro: 6, attrType: SubstringAttributeType.String, iLen: 2)]
        public string CodDocumentoComprador { get; set; }
        [SubstringAttribute(iNro: 7, attrType: SubstringAttributeType.String, iLen: 20, PadLeft = "0")]
        public string NroIdentificaiconComprador { get; set; }
        [SubstringAttribute(iNro: 8, attrType: SubstringAttributeType.String, iLen: 30)]
        public string ApellidoNombreComprador { get; set; }
        [SubstringAttribute(iNro: 9, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImporteTotal { get; set; }
        [SubstringAttribute(iNro: 10, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImporteConceptos { get; set; }
        [SubstringAttribute(iNro: 11, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImportePercepcion { get; set; }
        [SubstringAttribute(iNro: 12, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImporteOperacioneExentas { get; set; }
        [SubstringAttribute(iNro: 13, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double PercepcionIVA { get; set; }
        [SubstringAttribute(iNro: 14, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double PercepcionIBrutos { get; set; }
        [SubstringAttribute(iNro: 15, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double PercepcionImpuestosMunicipales { get; set; }
        [SubstringAttribute(iNro: 16, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double ImporteImpuestosInternos { get; set; }
        [SubstringAttribute(iNro: 17, attrType: SubstringAttributeType.String, iLen: 3)]
        public string Moneda { get; set; }
        [SubstringAttribute(iNro: 18, attrType: SubstringAttributeType.Double, iLen: 10, Format = "####.000000", PadLeft = " ", SinPuntoDecimal = true)]
        public double TipoCambio { get; set; }
        [SubstringAttribute(iNro: 19, attrType: SubstringAttributeType.Integer, iLen: 1)]
        public int CantidadAlicuotas { get; set; }
        [SubstringAttribute(iNro: 20, attrType: SubstringAttributeType.String, iLen: 1)]
        public string CodigoOperacion { get; set; }
        [SubstringAttribute(iNro: 21, attrType: SubstringAttributeType.Double, iLen: 15, Format = "#############.00", PadLeft = " ", SinPuntoDecimal = true)]
        public double OtrosTributos { get; set; }
        [SubstringAttribute(iNro: 22, attrType: SubstringAttributeType.DateTime, iLen: 8, Format = "yyyyMMdd")]
        public DateTime VtoPago { get; set; }
    }
}
