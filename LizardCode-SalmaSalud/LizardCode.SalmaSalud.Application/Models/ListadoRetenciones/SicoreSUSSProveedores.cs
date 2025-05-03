using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Models;
using System;

namespace umeral1.Gestion.Application.Models.ListadoRetenciones
{
    public class SicoreSUSSProveedores : ISicoreCiti
    {
        [SubstringAttribute(iNro: 1, attrType: SubstringAttributeType.String, iLen: 11)]
        public string CUIT { get; set; }

        [SubstringAttribute(iNro: 2, attrType: SubstringAttributeType.DateTime, iLen: 10)]
        public DateTime FechaRetencion { get; set; }

        [SubstringAttribute(iNro: 3, attrType: SubstringAttributeType.String, iLen: 10)]
        public string NroRetencion { get; set; }

        [SubstringAttribute(iNro: 4, attrType: SubstringAttributeType.Double, iLen: 15)]
        public double ImporteRetencion { get; set; }
    }
}
