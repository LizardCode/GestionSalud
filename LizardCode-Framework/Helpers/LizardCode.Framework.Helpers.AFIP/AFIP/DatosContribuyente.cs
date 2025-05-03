using ContribuyenteLookups;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.AFIP.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LizardCode.Framework.Helpers.AFIP
{
    public class DatosContribuyente
    {
        private string _token { get; set; }
        private string _sign { get; set; }
        private string _cuit { get; set; }
        private string _idPersona { get; set; }

        PersonaServiceA5 personaService = null;
        private Logger _logger;

        public DatosContribuyente(string token, string sign, string cuit, string idPersona)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicy) => true;
            _logger = LogManager.GetCurrentClassLogger();

            _token = token;
            _sign = sign;
            _cuit = cuit;
            _idPersona = idPersona;
        }

        public async Task<Common.Clases.Padron> GetPadron(string url)
        {
            personaService = new PersonaServiceA5Client(endpointConfiguration: PersonaServiceA5Client.EndpointConfiguration.PersonaServiceA5Port, remoteAddress: url);

            try
            {
                var padron = await personaService.getPersonaAsync(new getPersona(_token, _sign, long.Parse(_cuit), long.Parse(_idPersona)));
                if (padron.personaReturn.errorConstancia?.error.Any() ?? false)
                {
                    var personaPadron = new Common.Clases.Padron();
                    var errores = padron.personaReturn.errorConstancia.error.Select(err => new Errores
                    {
                        Codigo = 0,
                        MensajeError = err
                    })
                    .ToList();

                    personaPadron.Errores = string.Join(",", errores.Select(err => err.MensajeError));
                    return personaPadron;
                }

                if (padron.personaReturn.errorMonotributo?.error.Any() ?? false)
                {
                    var personaPadron = new Common.Clases.Padron();
                    var errores = padron.personaReturn.errorMonotributo.error.Select(err => new Errores
                    {
                        Codigo = 0,
                        MensajeError = err
                    })
                    .ToList();

                    personaPadron.Errores = string.Join(",", errores.Select(err => err.MensajeError));
                    return personaPadron;
                }

                if (padron.personaReturn.errorRegimenGeneral?.error.Any() ?? false)
                {
                    var personaPadron = new Common.Clases.Padron();
                    var errores = padron.personaReturn.errorRegimenGeneral.error.Select(err => new Errores
                    {
                        Codigo = 0,
                        MensajeError = err
                    })
                    .ToList();

                    personaPadron.Errores = string.Join(",", errores.Select(err => err.MensajeError));
                    return personaPadron;
                }

                var persona = padron.personaReturn.datosGenerales;

                return new Common.Clases.Padron
                {
                    Apellido = persona.apellido,
                    Nombre = persona.nombre,
                    RazonSocial = persona?.razonSocial ?? $"{persona.nombre} {persona.apellido}",
                    CodigoPostal = persona?.domicilioFiscal?.codPostal ?? string.Empty,
                    Direccion = persona?.domicilioFiscal?.direccion.ToUpper().Trim() ?? string.Empty,
                    Localidad = persona.domicilioFiscal?.localidad?.ToUpper().Trim() ?? string.Empty,
                    Provincia = persona.domicilioFiscal?.descripcionProvincia.ToUpper().Trim() ?? string.Empty,
                    Impuestos = padron.personaReturn.datosRegimenGeneral?.impuesto?.Select(i => i.idImpuesto).ToList() ?? new List<int>() { 20 }, //Monotributo
                    CUIT = _cuit
                };
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                throw new PadronException(ex.Message);
            }
        }
    }
}
