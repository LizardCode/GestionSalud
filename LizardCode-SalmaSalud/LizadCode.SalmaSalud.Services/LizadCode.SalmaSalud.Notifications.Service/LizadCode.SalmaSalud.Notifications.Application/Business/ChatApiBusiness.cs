using Dawa.Framework.Helpers.ChatApi;
using Dawa.Framework.Helpers.ChatApi.Responses;
using Microsoft.Extensions.Logging;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Business;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories;
using LizadCode.SalmaSalud.Notifications.Domain.Entities;
using LizadCode.SalmaSalud.Notifications.Domain.Enums;
using System.Data;

namespace LizadCode.SalmaSalud.Notifications.Application.Business
{
    public class ChatApiBusiness : IChatApiBusiness
    {
        private readonly IChatApiHelper _chatApiHelper;
        private readonly ILogger<ChatApiBusiness> _logger;
        private readonly IAuditoriasChatApiRepository _auditoriasChatApiRepository;

        public ChatApiBusiness(IChatApiHelper chatApiHelper,
                                ILogger<ChatApiBusiness> logger,
                                IAuditoriasChatApiRepository auditoriasChatApiRepository)
        {
            _chatApiHelper = chatApiHelper;
            _logger = logger;
            _auditoriasChatApiRepository = auditoriasChatApiRepository;
        }

        public async Task SendMessage(EventosChatApi evento, string telefono, string mensaje, int idEmpresa, int? idPaciente = null, IDbTransaction transaction = null)
        {
            string id = null;
            var estado = (int)EstadoAuditoriaChatApi.Error;
            SendMessageResponse sendResponse = null;

            try
            {
                sendResponse = await _chatApiHelper.SendMessage(telefono, mensaje);

                if (sendResponse != null && sendResponse.Status == "OK") { 
                    estado = (int)EstadoAuditoriaChatApi.Enviado;
                    id = sendResponse.Id;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                //transaction.Rollback();
            }
            finally
            {
                await _auditoriasChatApiRepository.Insert(new AuditoriaChatApi
                {
                    Fecha = DateTime.Now,
                    IdEmpresa = idEmpresa,
                    IdEvento = (int)evento,
                    IdEstadoAuditoriaChatApi = estado,
                    Telefono = telefono,
                    Mensaje = mensaje,
                    Respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(sendResponse),
                    IdPaciente = idPaciente,
                    Id = id
                }, transaction);
            }            
        }
    }
}
