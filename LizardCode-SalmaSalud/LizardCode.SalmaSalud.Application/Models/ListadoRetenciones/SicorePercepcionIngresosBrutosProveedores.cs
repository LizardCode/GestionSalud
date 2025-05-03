using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Models;
using System;

namespace umeral1.Gestion.Application.Models.ListadoRetenciones
{
    public class SicorePercepcionIngresosBrutosProveedores : ISicoreCiti
    {
        [SubstringAttribute(iNro: 1, attrType: SubstringAttributeType.String, iLen: 3)]
        public string Regimen { get; set; }

        [SubstringAttribute(iNro: 2, attrType: SubstringAttributeType.String, iLen: 11)]
        public string CUIT { get; set; }

        [SubstringAttribute(iNro: 3, attrType: SubstringAttributeType.DateTime, iLen: 10)]
        public DateTime FechaRetencion { get; set; }

        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.String, iLen: 8)]
        public string Sucursal { get; set; }

        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.String, iLen: 8)]
        public string NroComprobante { get; set; }

        [SubstringAttribute(iNro: 5, attrType: SubstringAttributeType.Double, iLen: 16)]
        public double ImportePercepcion { get; set; }
    }
}
