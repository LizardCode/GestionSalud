using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.AFIP.Common;
using LizardCode.Framework.Helpers.AFIP.Common.Clases;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WSCDC;

namespace LizardCode.Framework.Helpers.AFIP
{
    public  class ConsultaComprobantes
    {
        private string _token { get; set; }
        private string _sign { get; set; }
        private string _cuit { get; set; }

        ServiceSoapClient objWSCDC = null;
        CmpAuthRequest authRequest = null;
        private Logger _logger;

        public ConsultaComprobantes(string token, string sign, string cuit)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            _token = token;
            _sign = sign;
            _cuit = cuit;
        }

        public void Conectar(string url)
        {

            objWSCDC = new ServiceSoapClient(endpointConfiguration: ServiceSoapClient.EndpointConfiguration.ServiceSoap12, remoteAddress: url);
            objWSCDC.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom,
                CustomCertificateValidator = new CustomCertificateValidator(_logger)
            };
            authRequest = new CmpAuthRequest();

            authRequest.Token = _token;
            authRequest.Sign = _sign;
            authRequest.Cuit = long.Parse(_cuit);
        }

        public async Task<ComprobanteConstatar> ConsutlaComprobantes(ComprobantesConsulta consultaComprobantes)
        {
            try
            {
                var datosComprobante = new CmpDatos
                {
                    CbteModo = "CAE",
                    CbteNro = consultaComprobantes.CbteNro,
                    CbteFch = consultaComprobantes.CbteFch.ToString("yyyyMMdd"),
                    CbteTipo = consultaComprobantes.CbteTipo,
                    PtoVta = consultaComprobantes.PtoVta,
                    CodAutorizacion = consultaComprobantes.CodAutorizacion,
                    CuitEmisor = consultaComprobantes.CuitEmisor,
                    DocNroReceptor = consultaComprobantes.DocNroReceptor,
                    DocTipoReceptor = consultaComprobantes.DocTipoReceptor,
                    ImpTotal = Math.Round(consultaComprobantes.ImpTotal, 2)
                };

                var consultaCompRequest = new ComprobanteConstatarRequest(new ComprobanteConstatarRequestBody(authRequest, datosComprobante));
                var consultaComp = await objWSCDC.ComprobanteConstatarAsync(consultaCompRequest);

                ComprobanteConstatar comprobanteConstatar = new ComprobanteConstatar();

                var memoryStream = new MemoryStream();
                var xmlSerializerRequest = new XmlSerializer(consultaCompRequest.GetType());
                xmlSerializerRequest.Serialize(memoryStream, consultaCompRequest);
                comprobanteConstatar.XMLRequest = Encoding.UTF8.GetString(memoryStream.ToArray());

                memoryStream = new MemoryStream();
                var xmlSerializerResponse = new XmlSerializer(consultaComp.GetType());
                xmlSerializerResponse.Serialize(memoryStream, consultaComp);
                comprobanteConstatar.XMLResponse = Encoding.UTF8.GetString(memoryStream.ToArray());

                if(consultaComp.Body.ComprobanteConstatarResult != null)
                {
                    var CmpResp = consultaComp.Body.ComprobanteConstatarResult.CmpResp;
                    comprobanteConstatar.Resultado = consultaComp.Body.ComprobanteConstatarResult.Resultado;
                    comprobanteConstatar.CbteModo = CmpResp.CbteModo;
                    comprobanteConstatar.CuitEmisor = CmpResp.CuitEmisor;
                    comprobanteConstatar.PtoVta = CmpResp.PtoVta;
                    comprobanteConstatar.CbteTipo = CmpResp.CbteTipo;
                    comprobanteConstatar.CbteNro = CmpResp.CbteNro;
                    comprobanteConstatar.CbteFch = CmpResp.CbteFch;
                    comprobanteConstatar.ImpTotal = CmpResp.ImpTotal;
                    comprobanteConstatar.CodAutorizacion = CmpResp.CodAutorizacion;
                    comprobanteConstatar.DocTipoReceptor = CmpResp.DocTipoReceptor;
                    comprobanteConstatar.DocNroReceptor = CmpResp.DocNroReceptor;

                    if (consultaComp.Body.ComprobanteConstatarResult.Errors?.Any() ?? false)
                    {
                        comprobanteConstatar.Errores = consultaComp.Body.ComprobanteConstatarResult.Errors.Select(err => new Errores
                        {
                            Codigo = err.Code,
                            MensajeError = err.Msg
                        })
                        .ToList();

                        comprobanteConstatar.Error = string.Join(",", comprobanteConstatar.Errores.Select(obs => obs.MensajeError));

                    }

                    if (consultaComp.Body.ComprobanteConstatarResult.Observaciones?.Any() ?? false)
                    {
                        comprobanteConstatar.Observaciones = consultaComp.Body.ComprobanteConstatarResult.Observaciones.Select(obs => new Observaciones
                        {
                            Codigo = obs.Code,
                            MensajeObservacion = obs.Msg
                        })
                        .ToList();

                        comprobanteConstatar.Observacion = string.Join(",", comprobanteConstatar.Observaciones.Select(obs => obs.MensajeObservacion));
                    }
                }

                return comprobanteConstatar;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSCDCException(ex.Message);
            }
        }


    }
}
