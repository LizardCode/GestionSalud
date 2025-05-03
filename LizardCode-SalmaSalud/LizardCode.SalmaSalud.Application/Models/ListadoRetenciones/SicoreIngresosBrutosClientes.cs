using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Models;
using System;

namespace umeral1.Gestion.Application.Models.ListadoRetenciones
{
    public class SicoreIngresosBrutosClientes : ISicoreCiti
    {
        [SubstringAttribute(iNro: 1, attrType: SubstringAttributeType.String, iLen: 3)]
        public string CodJurisdiccion { get; set; }

        [SubstringAttribute(iNro: 2, attrType: SubstringAttributeType.String, iLen: 11)]
        public string CUIT { get; set; }

        [SubstringAttribute(iNro: 3, attrType: SubstringAttributeType.DateTime, iLen: 10)]
        public DateTime FechaPercepcion { get; set; }

        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.String, iLen: 4)]
        public string Sucursal { get; set; }

        [SubstringAttribute(iNro: 5, attrType: SubstringAttributeType.String, iLen: 8)]
        public string NroComprobante { get; set; }

        [SubstringAttribute(iNro: 6, attrType: SubstringAttributeType.String, iLen: 1)]
        public string TipoComprobantePercepcion { get; set; }

        [SubstringAttribute(iNro: 7, attrType: SubstringAttributeType.String, iLen: 1)]
        public string LetraComprobante { get; set; }

        [SubstringAttribute(iNro: 8, attrType: SubstringAttributeType.Double, iLen: 11)]
        public double ImportePercepcion { get; set; }
    }
}
