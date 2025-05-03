using System;

namespace LizardCode.Framework.Helpers.AFIP.Common
{
    public class WSAA
    {
        public string CodigoWS { get; init; }
        public long Cuit { get; init; }
        public string Sign { get; init; }
        public string Token { get; init; }
        public DateTime Generated { get; init; }
        public DateTime Expiration { get; init; }
        public UInt32 UniqueID { get; init; }

        public WSAA()
        {

        }

        public WSAA(string codigoWS, long cuit, string sign, string token, DateTime generated, DateTime expiration)
        {
            CodigoWS = codigoWS;

            Cuit = cuit;
            Sign = sign;
            Token = token;

            Generated = generated;
            Expiration = expiration;

            UniqueID = 1;
        }
    }
}
