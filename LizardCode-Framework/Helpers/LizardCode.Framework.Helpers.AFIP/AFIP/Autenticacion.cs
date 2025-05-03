using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WSAA;
using static WSAA.LoginCMSClient;

namespace LizardCode.Framework.Helpers.AFIP
{
    public class Autenticacion
    {
        public DateTime _generationTime { get; set; }
        public DateTime _expirationTime { get; set; }
        public UInt32 _globalUniqueID { get; set; }

        private LoginCMSClient loginCMSClient;

        public string _CUIT { get; set; }
        public string _remoteAddress { get; set; }
        public string _certificado { get; set; }
        public string _privateKey { get; set; }

        private Logger _logger;

        public Autenticacion(string certificado, string privateKey, string remoteAddress, string CUIT)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            _generationTime = new DateTime(1900, 1, 1);
            _expirationTime = new DateTime(1900, 1, 1);
            _globalUniqueID = 1;

            _certificado = certificado;
            _privateKey = privateKey;
            _remoteAddress = remoteAddress;
            _CUIT = CUIT;
        }

        public Autenticacion(DateTime? generationTime, DateTime? expirationTime, uint? globalUniqueID, string certificado, string privateKey, string remoteAddress, string CUIT)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            _generationTime = generationTime ?? new DateTime(1900, 1, 1);
            _expirationTime = expirationTime ?? new DateTime(1900, 1, 1);
            _globalUniqueID = globalUniqueID ?? 1;

            _certificado = certificado;
            _privateKey = privateKey;
            _remoteAddress = remoteAddress;
            _CUIT = CUIT;
        }

        public async Task<Common.WSAA> ObtenerToken(string codigoWS)
        {
            try
            {
                XmlDocument XmlLoginTicketRequest = null;
                XmlDocument XmlLoginTicketResponse = null;

                var XmlStrLoginTicketRequestTemplate = @"
                <loginTicketRequest>
                    <header>
                        <uniqueId></uniqueId>
                        <generationTime></generationTime>
                        <expirationTime></expirationTime>
                    </header>
                    <service></service>
                </loginTicketRequest>";

                XmlNode xmlNodoUniqueId;
                XmlNode xmlNodoGenerationTime;
                XmlNode xmlNodoExpirationTime;
                XmlNode xmlNodoService;

                string cmsFirmadoBase64;

                XmlLoginTicketRequest = new XmlDocument();
                XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

                xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId");
                xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime");
                xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime");
                xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service");

                xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-20).ToString("s");
                xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+20).ToString("s");
                xmlNodoUniqueId.InnerText = Convert.ToString(_globalUniqueID);
                xmlNodoService.InnerText = codigoWS;

                byte[] msgBytes;
                X509Certificate2 certFirmante;
                SignedCms cmsFirmado;
                CmsSigner cmsFirmante;
                byte[] encodedSignedCms;

                if (_certificado.IsNull())
                {
                    throw new WSAAException(@"Certificado AFIP Inexistente.");
                }

                if (_remoteAddress.IsNull())
                {
                    throw new WSAAException(@"URL WebService AFIP para el Servicio WSAA Inexistente.");
                }

                RSACryptoServiceProvider rsaKey = DecodeRSAPrivateKey(Convert.FromBase64String(_privateKey ?? throw new WSAAException(@"Private Key del Certificado Inexistente.")));
                certFirmante = new X509Certificate2(Convert.FromBase64String(_certificado));

                Encoding EncodedMsg = Encoding.UTF8;
                msgBytes = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);
                ContentInfo infoContenido = new ContentInfo(msgBytes);
                cmsFirmado = new SignedCms(infoContenido);
                cmsFirmante = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certFirmante, rsaKey);
                cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly;
                cmsFirmado.ComputeSignature(cmsFirmante, false);

                encodedSignedCms = cmsFirmado.Encode();
                cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);

                loginCMSClient = new LoginCMSClient(endpointConfiguration: EndpointConfiguration.LoginCms, remoteAddress: _remoteAddress);
                ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicy) => true;
                var loginCmsRequest = new loginCmsRequest(new loginCmsRequestBody(cmsFirmadoBase64));
                await loginCMSClient.OpenAsync();
                var loginTicketResponse = await loginCMSClient.loginCmsAsync(loginCmsRequest);
                loginCMSClient.Close();

                string xmlLoginResponse = loginTicketResponse.Body.loginCmsReturn;

                XmlLoginTicketResponse = new XmlDocument();
                XmlLoginTicketResponse.LoadXml(xmlLoginResponse);

                _generationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
                _generationTime = _generationTime.AddMinutes(-2);
                _expirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);

                return new Common.WSAA
                {
                    CodigoWS = codigoWS,
                    Cuit = long.Parse(_CUIT),
                    Expiration = _expirationTime,
                    Generated = _generationTime,
                    Sign = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText,
                    Token = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText
                };

            }
            catch(Exception ex)
            {
                if (loginCMSClient != null)
                    loginCMSClient.Abort();
                _logger.Log(LogLevel.Error, ex);
                throw new WSAAException(ex.Message);
            }
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }
            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

    }
}
