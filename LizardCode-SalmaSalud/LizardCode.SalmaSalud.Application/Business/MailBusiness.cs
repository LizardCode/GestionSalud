using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.SendGrid;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class MailBusiness : IMailBusiness
    {
        public async Task EnviarMailBienvenidaPaciente(string pacienteEmail, string pacienteNombre)
        {
            var apiKey = "SendGrid:ApiKey".FromAppSettings<string>(notFoundException: false);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SendGrid:FromName".FromAppSettings<string>(notFoundException: true);

            var templatePath = "Pacientes:TemplateBienvenida".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            //template = template.Replace("[PACIENTE_CODIGO]", codigo);

            var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Bienvenido",
                                                        pacienteEmail, pacienteNombre, template, null, null);
        }

        public async Task EnviarMailCodigoAccesoPaciente(string codigo, string pacienteEmail, string pacienteNombre)
        {
            var apiKey = "SendGrid:ApiKey".FromAppSettings<string>(notFoundException: false);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SendGrid:FromName".FromAppSettings<string>(notFoundException: true);

            var templatePath = "Pacientes:TemplateCodigoAcceso".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[PACIENTE_CODIGO]", codigo);

            var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Código de Accesso",
                                                        pacienteEmail, pacienteNombre, template, null, null);
        }

        public async Task EnviarMailAutogestionPaciente(string pacienteHash, string pacienteEmail, string pacienteNombre)
        {
            var pacienteUrl = "Pacientes:UrlConsulta".FromAppSettings(string.Empty, true) + pacienteHash;

            var apiKey = "SendGrid:ApiKey".FromAppSettings(string.Empty);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings(string.Empty, true);
            var fromName = "SendGrid:FromName".FromAppSettings(string.Empty, true);

            var templatePath = "Pacientes:TemplateConsulta".FromAppSettings(string.Empty, true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[PACIENTE_LINK]", pacienteUrl);

            await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Novedades en su Historia Clínica",
                                                        pacienteEmail, pacienteNombre, template, null, null);
        }

        public async Task EnviarMailRecetasPaciente(string pacienteEmail, string pacienteNombre, Dictionary<string, string> recetas)
        {
            var apiKey = "SendGrid:ApiKey".FromAppSettings<string>(notFoundException: false);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SendGrid:FromName".FromAppSettings<string>(notFoundException: true);

            var templatePath = "Pacientes:TemplateRecetas".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());

            var code = await new SendGridMail().SendSingleEmailWithBase64Attachments(apiKey, from, fromName, "Recetas",
                                                        pacienteEmail, pacienteNombre, template, recetas, null);
        }

        public async Task EnviarMailTurnoAsignadoPaciente(string pacienteEmail, string pacienteNombre, string fechaAsigancion, string especialidad, string observaciones)
        {
            var apiKey = "SendGrid:ApiKey".FromAppSettings<string>(notFoundException: false);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SendGrid:FromName".FromAppSettings<string>(notFoundException: true);

            var templatePath = "Pacientes:TemplateTurnoAsignado".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);
            var host = HttpContextHelper.Current.Request.Host.Host;

            template = template.Replace("[DOMAIN]", host.ToUpperInvariant());
            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[FECHA_ASIGNACION]", fechaAsigancion.ToUpperInvariant());
            template = template.Replace("[ESPECIALIDAD]", especialidad.ToUpperInvariant());
            template = template.Replace("[OBSERVACIONES]", observaciones);
            //template = template.Replace("[PACIENTE_CODIGO]", codigo);

            var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Turno Asignado",
                                                        pacienteEmail, pacienteNombre, template, null, null);
        }

        public async Task EnviarMailSolicitudTurnoCanceladaPaciente(string pacienteEmail, string pacienteNombre, string especialidad)
        {
            var apiKey = "SendGrid:ApiKey".FromAppSettings<string>(notFoundException: false);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SendGrid:FromName".FromAppSettings<string>(notFoundException: true);

            var templatePath = "Pacientes:TemplateSolicitudTurnoCancelada".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            var host = HttpContextHelper.Current.Request.Host.Host;

            template = template.Replace("[DOMAIN]", host.ToUpperInvariant());
            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[ESPECIALIDAD]", especialidad.ToUpperInvariant());
            //template = template.Replace("[PACIENTE_CODIGO]", codigo);

            var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Solicitud de Turno Cancelada",
                                                        pacienteEmail, pacienteNombre, template, null, null);
        }

        private string GetTemplate(string templatePath)
        {
            var crp = AppDomain.CurrentDomain.GetData("WebRootPath").ToString();

            var path = Path.Combine(crp, templatePath);

            string template = string.Empty;

            if (!File.Exists(path))
                throw new Exception("No se encontró el template.");

            using (StreamReader reader = new(path))
                template = reader.ReadToEnd();

            return template;
        }
    }
}
