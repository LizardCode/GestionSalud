using Newtonsoft.Json;
using NLog;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static LizardCode.Framework.Helpers.SendGrid.Enums;

namespace LizardCode.Framework.Helpers.SendGrid
{

    public class SendGridMail
    {
        private readonly Logger _log;


        public SendGridMail()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public async Task<HttpStatusCode> SendSingleEmail(string apiKey, string from, string fromName, string subject, string to, string toName, string body, Dictionary<string, string> attachments = null, Dictionary<string, string> customArgs = null)
        {
            var client = new SendGridClient(apiKey);

            var msg = MailHelper.CreateSingleEmail(
                from: new EmailAddress(from, fromName),
                to: new EmailAddress(to, toName),
                subject: subject,
                plainTextContent: null,
                htmlContent: body
            );

            if (attachments != null && attachments.Count > 0)
            {
                foreach (KeyValuePair<string, string> files in attachments)
                {
                    var bytes = File.ReadAllBytes(files.Value);
                    var file = Convert.ToBase64String(bytes);
                    msg.AddAttachment(files.Key, file);
                }
            }

            if (customArgs != null && customArgs.Count > 0)
            {
                msg.Personalizations.First().CustomArgs = customArgs;
            }

            _log.Info("Enviando email...");

            var response = await client.SendEmailAsync(msg);
            var responseBody = await response.Body.ReadAsStringAsync();

            _log.Info($"Email enviado: {response.StatusCode}-{responseBody}");

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendSingleEmailWithBase64Attachments(string apiKey, string from, string fromName, string subject, string to, string toName, string body, Dictionary<string, string> attachments, Dictionary<string, string> customArgs = null)
        {
            var client = new SendGridClient(apiKey);

            var msg = MailHelper.CreateSingleEmail(
                from: new EmailAddress(from, fromName),
                to: new EmailAddress(to, toName),
                subject: subject,
                plainTextContent: null,
                htmlContent: body
            );

            if (attachments != null && attachments.Count > 0)
            {
                foreach (KeyValuePair<string, string> files in attachments)
                {
                    msg.AddAttachment(files.Key, files.Value);
                }
            }

            if (customArgs != null && customArgs.Count > 0)
            {
                msg.Personalizations.First().CustomArgs = customArgs;
            }

            _log.Info("Enviando email...");

            var response = await client.SendEmailAsync(msg);
            var responseBody = await response.Body.ReadAsStringAsync();

            _log.Info($"Email enviado: {response.StatusCode}-{responseBody}");

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendSingleEmailToMultipleRecipients(string apiKey, string from, string fromName, string subject, Dictionary<string, string> to, string body, Dictionary<string, string> attachments = null, Dictionary<string, string> customArgs = null)
        {
            var client = new SendGridClient(apiKey);

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
                from: new EmailAddress(from, fromName),
                tos: to.Select(s => new EmailAddress(s.Key, s.Value)).ToList(),
                subject: subject,
                plainTextContent: null,
                htmlContent: body
            );

            if (attachments != null)
            {
                foreach (KeyValuePair<string, string> files in attachments)
                {
                    var bytes = File.ReadAllBytes(files.Value);
                    var file = Convert.ToBase64String(bytes);
                    msg.AddAttachment(files.Key, file);
                }
            }

            if (customArgs != null && customArgs.Count > 0)
            {
                msg.Personalizations.First().CustomArgs = customArgs;
            }

            return (await client.SendEmailAsync(msg)).StatusCode;
        }

        public async Task<HttpStatusCode> SendSingleEmailToMultipleRecipients(string apiKey, string from, string fromName, string subject, List<Tuple<string, string>> to, string body, List<Tuple<string, string>> attachments = null, Dictionary<string, string> customArgs = null)
        {
            var client = new SendGridClient(apiKey);

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
                from: new EmailAddress(from, fromName),
                tos: to.Select(s => new EmailAddress(s.Item1, s.Item2)).ToList(),
                subject: subject,
                plainTextContent: null,
                htmlContent: body
            );

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var files in attachments)
                {
                    var bytes = File.ReadAllBytes(files.Item2);
                    var file = Convert.ToBase64String(bytes);
                    msg.AddAttachment(files.Item1, file);
                }
            }

            if (customArgs != null && customArgs.Count > 0)
            {
                msg.Personalizations.First().CustomArgs = customArgs;
            }

            return (await client.SendEmailAsync(msg)).StatusCode;
        }


        public async Task<bool> DeliveredSingleEmail(string apiKey, string email)
        {
            var client = new SendGridClient(apiKey);
            var queryParams = "{'limit':1,'query':'to_email%3D%22" + email + "%22'}";

            var response = await client.RequestAsync(
                method: SendGridClient.Method.GET,
                urlPath: "messages",
                queryParams: queryParams
            );

            var result = await response.Body.ReadAsStringAsync();

            var values = JsonConvert.DeserializeAnonymousType(result, new { messages = new List<Messages>() });

            if (values == null || values.messages == null || values.messages.Count == 0)
            {
                return false;
            }

            switch (values.messages.First().Status)
            {
                case StatusEmailCode.not_delivered:
                    return false;

                case StatusEmailCode.delivered:
                    return true;

                default:
                    return false;
            }
        }
    }
}
