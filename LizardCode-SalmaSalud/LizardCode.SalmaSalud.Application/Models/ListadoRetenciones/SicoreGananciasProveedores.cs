using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Models;
using System;

namespace umeral1.Gestion.Application.Models.ListadoRetenciones
{
    public class SicoreGananciasProveedores : ISicoreCiti
    {
        [SubstringAttribute(iNro: 1, attrType: SubstringAttributeType.String, iLen: 2)]
        public string CodigoComprobante { get; set; }

        [SubstringAttribute(iNro: 2, attrType: SubstringAttributeType.DateTime, iLen: 10, Format = "dd/MM/yyyy")]
        public DateTime FechaEmisionComprobante { get; set; }

        [SubstringAttribute(iNro: 3, attrType: SubstringAttributeType.String, iLen: 16)]
        public string NroComprobante { get; set; }

        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.Double, iLen: 16)]
        public double ImporteNetoGravado { get; set; }

        [SubstringAttribute(iNro: 5, attrType: SubstringAttributeType.String, iLen: 4)]
        public string CodImpuesto { get; set; }

        [SubstringAttribute(iNro: 6, attrType: SubstringAttributeType.String, iLen: 3)]
        public string Regimen { get; set; }

        [SubstringAttribute(iNro: 7, attrType: SubstringAttributeType.String, iLen: 1)]
        public string CodigoOperacion { get; set; }

        [SubstringAttribute(iNro: 8, attrType: SubstringAttributeType.Double, iLen: 14)]
        public double BaseImponible { get; set; }

        [SubstringAttribute(iNro: 9, attrType: SubstringAttributeType.DateTime, iLen: 10, Format = "dd/MM/yyyy")]
        public DateTime FechaEmisionRetencion { get; set; }

        [SubstringAttribute(iNro: 10, attrType: SubstringAttributeType.String, iLen: 2)]
        public string CodigoCondicion { get; set; }

        [SubstringAttribute(iNro: 11, attrType: SubstringAttributeType.String, iLen: 1)]
        public string CodSujetosSuspendidos { get; set; }

        [SubstringAttribute(iNro: 12, attrType: SubstringAttributeType.Double, iLen: 14)]
        public double ImporteRetencion { get; set; }

        [SubstringAttribute(iNro: 13, attrType: SubstringAttributeType.String, iLen: 6)]
        public string PorcentajeExclusion { get; set; }

        [SubstringAttribute(iNro: 14, attrType: SubstringAttributeType.String, iLen: 10)]
        public string FechaEmisionBoletin { get; set; }

        [SubstringAttribute(iNro: 15, attrType: SubstringAttributeType.String, iLen: 2)]
        public string TipoDocumentoDelRetenido { get; set; }

        [SubstringAttribute(iNro: 16, attrType: SubstringAttributeType.String, iLen: 20)]
        public string NroDocumentoDelRetenido { get; set; }

        [SubstringAttribute(iNro: 17, attrType: SubstringAttributeType.String, iLen: 14)]
        public string NroCertificadoOriginal { get; set; }
    }
}
