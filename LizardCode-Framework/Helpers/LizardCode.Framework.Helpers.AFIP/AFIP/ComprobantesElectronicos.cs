using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.AFIP.Common;
using LizardCode.Framework.Helpers.AFIP.Common.Clases;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WSFE;

namespace LizardCode.Framework.Helpers.AFIP
{
    public class ComprobantesElectronicos
    {
        private string _token { get; set; }
        private string _sign { get; set; }
        private string _cuit { get; set; }

        ServiceSoapClient objWSFEV1 = null;
        FEAuthRequest authRequest = null;
        private Logger _logger;

        public ComprobantesElectronicos(string token, string sign, string cuit)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            _token = token;
            _sign = sign;
            _cuit = cuit;
        }

        public void Conectar(string url)
        {
            objWSFEV1 = new ServiceSoapClient(endpointConfiguration: ServiceSoapClient.EndpointConfiguration.ServiceSoap12, remoteAddress: url);
            objWSFEV1.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom,
                CustomCertificateValidator = new CustomCertificateValidator(_logger)
            };
            authRequest = new FEAuthRequest();

            authRequest.Token = _token;
            authRequest.Sign = _sign;
            authRequest.Cuit = long.Parse(_cuit);
        }

        public async Task<List<PtoVenta>> GetPtoVentas()
        {
            try
            {
                var ptosVentaRequest = new FEParamGetPtosVentaRequest(new FEParamGetPtosVentaRequestBody(authRequest));
                var ePtoVentaResponse = await objWSFEV1.FEParamGetPtosVentaAsync(ptosVentaRequest);

                return ePtoVentaResponse.Body.FEParamGetPtosVentaResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<List<CbteTipo>> GetTiposCbteAsync()
        {
            try
            {
                var tiposCbteRequest = new FEParamGetTiposCbteRequest(new FEParamGetTiposCbteRequestBody(authRequest));
                var objConsultaTiposCbte = await objWSFEV1.FEParamGetTiposCbteAsync(tiposCbteRequest);

                return objConsultaTiposCbte.Body.FEParamGetTiposCbteResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<List<ConceptoTipo>> GetTiposConceptoAsync()
        {
            try
            {

                var conceptoTipoRequest = new FEParamGetTiposConceptoRequest(new FEParamGetTiposConceptoRequestBody(authRequest));
                var objConsultaTiposConcepto = await objWSFEV1.FEParamGetTiposConceptoAsync(conceptoTipoRequest);

                return objConsultaTiposConcepto.Body.FEParamGetTiposConceptoResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<List<DocTipo>> GetTiposDocAsync()
        {
            try
            {
                var docTipoRequest = new FEParamGetTiposDocRequest(new FEParamGetTiposDocRequestBody(authRequest));
                var objConsultaTiposDoc = await objWSFEV1.FEParamGetTiposDocAsync(docTipoRequest);

                return objConsultaTiposDoc.Body.FEParamGetTiposDocResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<List<IvaTipo>> GetTiposIvaAsync()
        {
            try
            {
                var ivaTipoRequest = new FEParamGetTiposIvaRequest(new FEParamGetTiposIvaRequestBody(authRequest));
                var objConsultaTiposIva = await objWSFEV1.FEParamGetTiposIvaAsync(ivaTipoRequest);

                return objConsultaTiposIva.Body.FEParamGetTiposIvaResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public List<Moneda> GetTiposMonedas()
        {
            try
            {
                var monedaRequest = new FEParamGetTiposMonedasRequest(new FEParamGetTiposMonedasRequestBody(authRequest));
                var objConsultaMoneda = objWSFEV1.FEParamGetTiposMonedasAsync(monedaRequest).GetAwaiter().GetResult();

                return objConsultaMoneda.Body.FEParamGetTiposMonedasResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<List<OpcionalTipo>> GetTiposOpcionalAsync()
        {
            try
            {
                var opcionalTipoRequest = new FEParamGetTiposOpcionalRequest(new FEParamGetTiposOpcionalRequestBody(authRequest));
                var objConsultaTiposOpcional = await objWSFEV1.FEParamGetTiposOpcionalAsync(opcionalTipoRequest);

                return objConsultaTiposOpcional.Body.FEParamGetTiposOpcionalResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<List<TributoTipo>> GetTiposTributosAsync()
        {
            try
            {
                var tributoRequest = new FEParamGetTiposTributosRequest(new FEParamGetTiposTributosRequestBody(authRequest));
                var objConsultaTributo = await objWSFEV1.FEParamGetTiposTributosAsync(tributoRequest);

                return objConsultaTributo.Body.FEParamGetTiposTributosResult.ResultGet;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);

            }
        }

        public async Task<CAE> GetCompConsultar(int CbteTipo, int PtoVta, int CbteNro)
        {
            try
            {
                FECompConsultaReq objFECompConsultaReq = new FECompConsultaReq();
                objFECompConsultaReq.CbteNro = CbteNro;
                objFECompConsultaReq.CbteTipo = CbteTipo;
                objFECompConsultaReq.PtoVta = PtoVta;

                var objFECompConsultaRequest = new FECompConsultarRequest(new FECompConsultarRequestBody(authRequest, objFECompConsultaReq));
                var objFECompConsultaResponse = await objWSFEV1.FECompConsultarAsync(objFECompConsultaRequest);

                var cae = new CAE();

                if (objFECompConsultaResponse != null)
                {
                    var objCompConsulta = objFECompConsultaResponse.Body.FECompConsultarResult;

                    if (objCompConsulta.ResultGet != null)
                    {
                        cae.NroCAE = objCompConsulta.ResultGet.CodAutorizacion;
                        cae.CAEFchVto = objCompConsulta.ResultGet.FchVto;
                        cae.Resultado = objCompConsulta.ResultGet.Resultado;
                    }

                    if (objCompConsulta.Errors?.Any() ?? false)
                    {
                        cae.Errores = objCompConsulta.Errors.Select(err => new Errores
                        {
                            Codigo = err.Code,
                            MensajeError = err.Msg
                        })
                        .ToList();

                        cae.Error = string.Join(",", cae.Errores.Select(obs => obs.MensajeError));

                    }
                }
                return cae;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<int> GetCompUltimoAutorizadoAsync(int PtoVta, int CbteTipo)
        {
            try
            {
                FECompUltimoAutorizadoRequest objFECompUltimoAutorizadoReq = new FECompUltimoAutorizadoRequest(new FECompUltimoAutorizadoRequestBody(authRequest, PtoVta, CbteTipo));
                var objFERecuperaLastCbteResponse = await objWSFEV1.FECompUltimoAutorizadoAsync(objFECompUltimoAutorizadoReq);

                return objFERecuperaLastCbteResponse.Body.FECompUltimoAutorizadoResult.CbteNro;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }

        public async Task<CAE> CAESolicitarAsync(Comprobantes comprobante)
        {
            try
            {
                FECAERequest objFECAERequest = new FECAERequest();

                objFECAERequest.FeCabReq = new FECAECabRequest
                {
                    CantReg = 1,
                    CbteTipo = comprobante.CbteTipo,
                    PtoVta = comprobante.PtoVta
                };

                FECAEDetRequest objFECAEDetRequest = new FECAEDetRequest
                {
                    Concepto = comprobante.Concepto,
                    DocTipo = comprobante.DocTipo,
                    DocNro = comprobante.DocNro,
                    CbteDesde = comprobante.CbteDesde,
                    CbteHasta = comprobante.CbteHasta,
                    CbteFch = comprobante.CbteFch.ToString("yyyyMMdd"),
                    FchVtoPago = comprobante.FchVtoPago.HasValue ? comprobante.FchVtoPago.Value.ToString("yyyyMMdd") : string.Empty,
                    ImpTotal = Math.Round(comprobante.ImpTotal, 2),
                    ImpTotConc = Math.Round(comprobante.ImpTotConc, 2),
                    ImpNeto = Math.Round(comprobante.ImpNeto, 2),
                    ImpOpEx = Math.Round(comprobante.ImpOpEx, 2),
                    ImpTrib = Math.Round(comprobante.ImpTrib, 2),
                    ImpIVA = Math.Round(comprobante.ImpIVA, 2),
                    FchServDesde = comprobante.FchServDesde.ToString("yyyyMMdd"),
                    FchServHasta = comprobante.FchServHasta.ToString("yyyyMMdd"),
                    MonId = comprobante.MonId,
                    MonCotiz = comprobante.MonCotiz, 
                };

                if (comprobante.PeriodoAsocFchServDesde.HasValue)
                {
                    objFECAEDetRequest.PeriodoAsoc.FchDesde = comprobante.PeriodoAsocFchServDesde.Value.ToString("yyyyMMdd");
                }

                if (comprobante.PeriodoAsocFchServHasta.HasValue)
                {
                    objFECAEDetRequest.PeriodoAsoc.FchHasta = comprobante.PeriodoAsocFchServHasta.Value.ToString("yyyyMMdd");
                }

                objFECAEDetRequest.Iva = new List<AlicIva>();
                objFECAEDetRequest.Iva.AddRange(
                    comprobante.TipoAlic.Select(alicuota => new AlicIva
                    {
                        Id = alicuota.Id,
                        BaseImp = Math.Round(alicuota.BaseImponible, 2),
                        Importe = Math.Round(alicuota.Importe, 2)
                    })
                );

                if (comprobante.TipoTrib.Count > 0)
                {
                    objFECAEDetRequest.Tributos = new List<Tributo>();
                    objFECAEDetRequest.Tributos.AddRange(
                        comprobante.TipoTrib.Select(tributo => new Tributo
                        {
                            Id = tributo.Id,
                            Desc = tributo.Descripcion,
                            BaseImp = tributo.BaseImponible,
                            Alic = tributo.Alicuota,
                            Importe = Math.Round(tributo.Importe, 2)
                        })
                    );
                }

                if (comprobante.ComprobantesAsociados.Count > 0)
                {
                    objFECAEDetRequest.CbtesAsoc = new List<CbteAsoc>();
                    objFECAEDetRequest.CbtesAsoc.AddRange(
                        comprobante.ComprobantesAsociados.Select(cte => new CbteAsoc
                        {
                            Nro = cte.Nro,
                            Cuit = cte.Cuit,
                            Tipo = cte.Tipo,
                            CbteFch = cte.CbteFch.ToString("yyyyMMdd"),
                            PtoVta = cte.PtoVta
                        })
                    );
                }

                if (comprobante.Opcionales.Count > 0)
                {
                    objFECAEDetRequest.Opcionales = new List<Opcional>();
                    objFECAEDetRequest.Opcionales.AddRange(
                        comprobante.Opcionales.Select(opcional => new Opcional
                        {
                            Id = opcional.Id,
                            Valor = opcional.Valor
                        })
                    );
                }

                objFECAERequest.FeDetReq = new List<FECAEDetRequest>
                {
                    objFECAEDetRequest
                };

                var objCAESolicitarRequest = new FECAESolicitarRequest(new FECAESolicitarRequestBody(authRequest, objFECAERequest));
                var objFECAEResponse = await objWSFEV1.FECAESolicitarAsync(objCAESolicitarRequest);

                CAE cae = new CAE();

                var memoryStream = new MemoryStream();
                var xmlSerializerRequest = new XmlSerializer(objCAESolicitarRequest.GetType());
                xmlSerializerRequest.Serialize(memoryStream, objCAESolicitarRequest);
                cae.XMLRequest = Encoding.UTF8.GetString(memoryStream.ToArray());

                memoryStream = new MemoryStream();
                var xmlSerializerResponse = new XmlSerializer(objFECAEResponse.GetType());
                xmlSerializerResponse.Serialize(memoryStream, objFECAEResponse);
                cae.XMLResponse = Encoding.UTF8.GetString(memoryStream.ToArray());

                if (objFECAEResponse.Body.FECAESolicitarResult != null)
                {
                    
                    if (objFECAEResponse.Body.FECAESolicitarResult.FeDetResp.Any())
                    {
                        var CAEResp = objFECAEResponse.Body.FECAESolicitarResult.FeDetResp.Single();
                        
                        cae.NroCAE = CAEResp.CAE;
                        cae.CAEFchVto = CAEResp.CAEFchVto;
                        cae.Resultado = CAEResp.Resultado;

                        if (CAEResp.Observaciones?.Any() ?? false)
                        {
                            cae.Observaciones = CAEResp.Observaciones.Select(obs => new Observaciones
                            {
                                Codigo = obs.Code,
                                MensajeObservacion = obs.Msg
                            })
                            .ToList();

                            cae.Observacion = string.Join(",", cae.Observaciones.Select(obs => obs.MensajeObservacion));
                        }
                    }

                    if (objFECAEResponse.Body.FECAESolicitarResult.Errors?.Any() ?? false)
                    {
                        cae.Errores = objFECAEResponse.Body.FECAESolicitarResult.Errors.Select(err => new Errores
                        {
                            Codigo = err.Code,
                            MensajeError = err.Msg
                        })
                        .ToList();

                        cae.Error = string.Join(",", cae.Errores.Select(obs => obs.MensajeError));

                    }

                    return cae;
                }
                else
                {
                    cae = new CAE
                    {
                        Resultado = "E",
                        Error = "El Servicio no devolvio Resultados"
                    };
                }

                return cae;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new WSFEException(ex.Message);
            }
        }
    }
}
