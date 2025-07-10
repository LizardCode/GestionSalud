using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.SendGrid;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
//using System.Net;
//using System.Net.Mail;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class MailBusiness : IMailBusiness
    {
        public async Task EnviarMailBienvenidaPaciente(string pacienteEmail, string pacienteNombre)
        {
            var templatePath = "Pacientes:TemplateBienvenida".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            //template = template.Replace("[PACIENTE_CODIGO]", codigo);

            //var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Bienvenido",
            //                                            pacienteEmail, pacienteNombre, template, null, null);

            await SendMail(pacienteEmail, "Bienvenido", template, pacienteNombre);
        }

        public async Task EnviarMailCodigoAccesoPaciente(string codigo, string pacienteEmail, string pacienteNombre)
        {
            var templatePath = "Pacientes:TemplateCodigoAcceso".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[PACIENTE_CODIGO]", codigo);

            //var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Código de Acceso",
            //                                            pacienteEmail, pacienteNombre, template, null, null);

            await SendMail(pacienteEmail, "Código de Acceso", template, pacienteNombre);
        }

        public async Task EnviarMailAutogestionPaciente(string pacienteHash, string pacienteEmail, string pacienteNombre)
        {
            var pacienteUrl = "Pacientes:UrlConsulta".FromAppSettings(string.Empty, true) + pacienteHash;

            var templatePath = "Pacientes:TemplateConsulta".FromAppSettings(string.Empty, true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[PACIENTE_LINK]", pacienteUrl);

            //await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Novedades en su Historia Clínica",
            //                                            pacienteEmail, pacienteNombre, template, null, null);

            await SendMail(pacienteEmail, "Novedades en su Historia Clínica", template, pacienteNombre);
        }

        public async Task EnviarMailRecetasPaciente(string pacienteEmail, string pacienteNombre, Dictionary<string, string> recetas)
        {
            var templatePath = "Pacientes:TemplateRecetas".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());

            //var code = await new SendGridMail().SendSingleEmailWithBase64Attachments(apiKey, from, fromName, "Recetas",
            //                                            pacienteEmail, pacienteNombre, template, recetas, null);

            await SendMail(pacienteEmail, "Recetas", template, pacienteNombre);
        }

        public async Task EnviarMailTurnoAsignadoPaciente(string pacienteEmail, string pacienteNombre, string fechaAsigancion, string especialidad, string observaciones)
        {
            var templatePath = "Pacientes:TemplateTurnoAsignado".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);
            var host = HttpContextHelper.Current.Request.Host.Host;

            template = template.Replace("[DOMAIN]", host.ToUpperInvariant());
            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[FECHA_ASIGNACION]", fechaAsigancion.ToUpperInvariant());
            template = template.Replace("[ESPECIALIDAD]", especialidad.ToUpperInvariant());
            template = template.Replace("[OBSERVACIONES]", observaciones);
            //template = template.Replace("[PACIENTE_CODIGO]", codigo);

            //var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Turno Asignado",
            //                                            pacienteEmail, pacienteNombre, template, null, null);

            await SendMail(pacienteEmail, "Turno Asignado", template, pacienteNombre);
        }

        public async Task EnviarMailSolicitudTurnoCanceladaPaciente(string pacienteEmail, string pacienteNombre, string especialidad)
        {
            var templatePath = "Pacientes:TemplateSolicitudTurnoCancelada".FromAppSettings<string>(notFoundException: true);
            var template = GetTemplate(templatePath);

            var host = HttpContextHelper.Current.Request.Host.Host;

            template = template.Replace("[DOMAIN]", host.ToUpperInvariant());
            template = template.Replace("[PACIENTE_NOMBRE]", pacienteNombre.ToUpperInvariant());
            template = template.Replace("[ESPECIALIDAD]", especialidad.ToUpperInvariant());
            //template = template.Replace("[PACIENTE_CODIGO]", codigo);

            //var code = await new SendGridMail().SendSingleEmail(apiKey, from, fromName, "Solicitud de Turno Cancelada",
            //                                            pacienteEmail, pacienteNombre, template, null, null);

            await SendMail(pacienteEmail, "Solicitud de Turno Cancelada", template, pacienteNombre);
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

        private async Task SendMail(string to, string subject, string bodyMessage, string toName = "", Dictionary<string, string> attachments = null)
        {
            var useSMTP = "useSMTP".FromAppSettings<bool>(notFoundException: false, defaultValue: false);
            if (useSMTP)
            {
                await SendSMTPMail(to, subject, bodyMessage);
            }
            else
            {
                await SendSendGridMail(to, subject, bodyMessage, toName, attachments);
            }
        }

        //private async Task SendSMTPMail(string to, string subject, string bodyMessage)
        //{
        //    var host = "SMTP:Host".FromAppSettings<string>(notFoundException: true);
        //    var port = "SMTP:Port".FromAppSettings<int>(notFoundException: true);

        //    var from = "SMTP:From".FromAppSettings<string>(notFoundException: true);
        //    var fromName = "SMTP:FromName".FromAppSettings<string>(notFoundException: true);
        //    var pass = "SMTP:Pass".FromAppSettings<string>(notFoundException: true);

        //    using (SmtpClient client = new SmtpClient(host, port))
        //    {
        //        client.EnableSsl = true;
        //        client.Credentials = new NetworkCredential(from, pass);

        //        MailMessage message = new MailMessage();
        //        message.From = new MailAddress(from, fromName);
        //        message.To.Add(to);
        //        message.Subject = subject;
        //        message.Body = bodyMessage;

        //        client.Send(message);
        //    }
        //}

        private async Task SendSMTPMail(string to, string subject, string bodyMessage)
        {
            var host = "SMTP:Host".FromAppSettings<string>(notFoundException: true);
            var port = "SMTP:Port".FromAppSettings<int>(notFoundException: true);

            var from = "SMTP:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SMTP:FromName".FromAppSettings<string>(notFoundException: true);
            var pass = "SMTP:Pass".FromAppSettings<string>(notFoundException: true);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, from));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = bodyMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(from, pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        private async Task SendSendGridMail(string to, string subject, string bodyMessage, string toName = "", Dictionary<string, string> attachments = null)
        {
            var apiKey = "SendGrid:ApiKey".FromAppSettings<string>(notFoundException: false);

            if (apiKey.IsNull())
                return;

            var from = "SendGrid:From".FromAppSettings<string>(notFoundException: true);
            var fromName = "SendGrid:FromName".FromAppSettings<string>(notFoundException: true);

            await new SendGridMail().SendSingleEmailWithBase64Attachments(apiKey, from, fromName, subject,
                                                    to, toName, bodyMessage, attachments, null);
        }
    }
}
