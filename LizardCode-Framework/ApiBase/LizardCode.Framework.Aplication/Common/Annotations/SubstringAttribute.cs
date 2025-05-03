using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Common.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SubstringAttribute : Attribute
    {
        public int Nro { get; init; }
        public int Len { get; init; }
        public SubstringAttributeType AttrType { get; set; }
        public string Format { get; set; }
        public string PadLeft { get; set; }
        public bool SinPuntoDecimal { get; set; }

        public SubstringAttribute(int iNro, SubstringAttributeType attrType, int iLen, string format = "", string padLeft = "", bool sinPuntoDecimal = false)
        {
            Nro = iNro;
            Len = iLen;
            AttrType = attrType;
            Format = format;
            PadLeft = padLeft;
            SinPuntoDecimal = sinPuntoDecimal;
        }
    }
}
