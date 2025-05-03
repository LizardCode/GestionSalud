using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.ChatApi;
using LizardCode.Framework.Helpers.ChatApi.Responses;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Helpers;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.AuditoriasChatApiViewModel;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ChatApiBusiness : BaseBusiness, IChatApiBusiness
    {
        private readonly IChatApiHelper _chatApiHelper;
        private readonly IWAppApiHelper _wAppApiHelper;
        private readonly ILogger<ChatApiBusiness> _logger;
        private readonly IAuditoriasChatApiRepository _auditoriasChatApiRepository;

        public ChatApiBusiness(IChatApiHelper chatApiHelper,
                                IWAppApiHelper wAppApiHelper,
                                ILogger<ChatApiBusiness> logger,
                                IAuditoriasChatApiRepository auditoriasChatApiRepository)
        {
            _chatApiHelper = chatApiHelper;
            _wAppApiHelper = wAppApiHelper;
            _logger = logger;
            _auditoriasChatApiRepository = auditoriasChatApiRepository;
        }

        public async Task SendMessageBienvenida(string telefono, string nombre, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Bienvenido a *{1}*. ",
                                nombre,
                                "[NOMBRE INSTITUCION]");

            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.Bienvenida,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageCodigoAcceso(string telefono, string nombre, string codigo, DateTime vencimiento, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Ha solicitado el siguiente código de acceso: *{1}*. El mismo será válido hasta: *{2}*. Si no solicito el mismo, simplemente ignore el mensaje. *Recuerde no compratir este código con nadie*",
                                nombre,
                                codigo,
                                vencimiento.ToString("dd/MM/yyyy hh:mm"));

            //message += " " + _portalText;

            await SendMessage(EventosChatApi.CodigoAcceso,
                                            "549" + telefono,
                                            message,
                                            null,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageTurnoAsignado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos agendado un turno para el *{1}* a las *{2}* con el profesional *{3}* para la especialidad *{4}*. ",
                                paciente, 
                                fechaTurno.ToString("dd/MM/yyyy"),
                                fechaTurno.ToString("HH:mm"),
                                profesional,
                                especialidad);

            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.TurnoAsignado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente, 
                                transaction);

        }

        public async Task SendMessageTurnoReAgendado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Su turno con el profesional *{1}* para la especialidad *{2}* se ha RE-AGENDADO para el *{3}* a las *{4}*hs.",
                                paciente,
                                profesional,
                                especialidad,
                                fechaTurno.ToString("dd/MM/yyyy"),
                                fechaTurno.ToString("HH:mm"));


            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.TurnoReAgendado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageTurnoCancelado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos CANCELADO un turno para el *{1}* a las *{2}* con el profesional *{3}* para la especialidad *{4}*.",
                                            paciente,
                                            fechaTurno.ToString("dd/MM/yyyy"),
                                            fechaTurno.ToString("HH:mm"),
                                            profesional,
                                            especialidad);

            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.TurnoCancelado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageTurnoCancelado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, string motivo, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos CANCELADO un turno para el *{1}* a las *{2}* con el profesional *{3}* para la especialidad *{4}*. Motivo: {5}",
                                            paciente,
                                            fechaTurno.ToString("dd/MM/yyyy"),
                                            fechaTurno.ToString("HH:mm"),
                                            profesional,
                                            especialidad,
                                            motivo);

            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.TurnoCancelado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageTurnoConfirmado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos CONFIRMADO un turno para el *{1}* a las *{2}* con el profesional *{3}* para la especialidad *{4}*.",
                                            paciente,
                                            fechaTurno.ToString("dd/MM/yyyy"),
                                            fechaTurno.ToString("HH:mm"),
                                            profesional,
                                            especialidad);


            //message += " " + _portalText;

            await SendMessage(EventosChatApi.TurnoConfirmado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageTurnoRecepcionado(string telefono, string consultorio, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Su turno con el profesional *{1}* para la especialidad *{2}* será atendido en el consultorio *{3}*.",
                                            paciente,
                                            profesional,
                                            especialidad,
                                            consultorio);


            //message += " " + _portalText;

            await SendMessage(EventosChatApi.TurnoRecepcionado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageSobreTurnoAsignado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos agendado un SOBRETURNO para el *{1}* a las *{2}* con el profesional *{3}* para la especialidad *{4}*.",
                                            paciente,
                                            fechaTurno.ToString("dd/MM/yyyy"),
                                            fechaTurno.ToString("HH:mm"),
                                            profesional,
                                            especialidad);

            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.SobreTurnoAsignado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageDemandaEspontanea(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos recibido su solicitud de atención con el profesional *{1}* para la especialidad *{2}* el *{3}* a las *{4}*.",
                                            paciente,
                                            profesional,
                                            especialidad,
                                            fechaTurno.ToString("dd/MM/yyyy"),
                                            fechaTurno.ToString("HH:mm"));

            message += " " + GetPortalText();

            await SendMessage(EventosChatApi.DemandaEspontanea,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageGuardia(string telefono, DateTime fechaTurno, string paciente, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Ha sido recepcionado el *{1}* a las *{2}* para atención en Guardia. Aguarde a ser llamado en la sala indicada por recepción.",
                                            paciente,
                                            fechaTurno.ToString("dd/MM/yyyy"),
                                            fechaTurno.ToString("HH:mm"));

            //message += " " + GetPortalText();

            await SendMessage(EventosChatApi.Guardia,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageLlamar(string telefono, string consultorio, string paciente, string profesional, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. El profesional *{1}* lo espera en el consultorio *{2}*.",
                                            paciente,
                                            profesional,
                                            consultorio);


            //message += " " + _portalText;

            await SendMessage(EventosChatApi.GuardiaLlamado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessageTurnoGuardiaCancelado(string telefono, string paciente, string motivo, int? idPaciente = null, IDbTransaction transaction = null)
        {
            var message = string.Format("Estimado *{0}*. Hemos CANCELADO su turno en GUARDIA. Motivo: *{1}*.",
                                            paciente,
                                            motivo);

            //message += " " + GetPortalText();

            await SendMessage(EventosChatApi.GuardiaCancelado,
                                            "549" + telefono,
                                            message,
                                            _permissionsBusiness.Value.User.IdEmpresa,
                                            idPaciente,
                                transaction);

        }

        public async Task SendMessage_OLD(EventosChatApi evento, string telefono, string mensaje, int? idEmpresa, int? idPaciente = null, IDbTransaction transaction = null)
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
                    IdEmpresa = idEmpresa == 0 ? null : idEmpresa,
                    IdEvento = (int)evento,
                    IdEstadoAuditoriaChatApi = estado,
                    Telefono = telefono,
                    Mensaje = evento == EventosChatApi.CodigoAcceso ? "(SOLICITUD DE CÓDIGO DE ACCESO)" : mensaje,
                    Respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(sendResponse),
                    IdPaciente = idPaciente,
                    Id = id
                }, transaction);
            }            
        }

        public async Task SendMessage(EventosChatApi evento, string telefono, string mensaje, int? idEmpresa, int? idPaciente = null, IDbTransaction transaction = null)
        {
            string id = null;
            var estado = (int)EstadoAuditoriaChatApi.Error;
            WAppApiResponse sendResponse = null;

            telefono = telefono.Replace("549", string.Empty);

            try
            {
                sendResponse = await _wAppApiHelper.SendMessage(telefono, mensaje);

                if (sendResponse != null && sendResponse.Status == "success")
                {
                    estado = (int)EstadoAuditoriaChatApi.Enviado;
                    id = sendResponse.Data?.InstanceId;
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
                    IdEmpresa = idEmpresa == 0 ? null : idEmpresa,
                    IdEvento = (int)evento,
                    IdEstadoAuditoriaChatApi = estado,
                    Telefono = telefono,
                    Mensaje = evento == EventosChatApi.CodigoAcceso ? "(SOLICITUD DE CÓDIGO DE ACCESO)" : mensaje,
                    Respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(sendResponse),
                    IdPaciente = idPaciente,
                    Id = id
                }, transaction);
            }
        }

        public async Task<AuditoriaChatApiViewModel> Get(int idAuditoriaChatApi)
        {
            var auditoria = await _auditoriasChatApiRepository.GetByIdCustom(idAuditoriaChatApi);

            var model = _mapper.Map<AuditoriaChatApiViewModel>(auditoria);

            return model;
        }

        public async Task<DataTablesResponse<Custom.AuditoriaChatApi>> GetAll(DataTablesRequest request)
        {
            var customQuery = _auditoriasChatApiRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();


            if (filters.ContainsKey("IdEvento"))
                builder.Append($"AND IdEvento = {filters["IdEvento"]}");

            if (filters.ContainsKey("IdEstadoChatApi"))
                builder.Append($"AND IdEstadoAuditoriaChatApi = {filters["IdEstadoChatApi"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                builder.Append($"AND Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            builder.Append($"AND idEmpresa = {_permissionsBusiness.Value.User.IdEmpresa} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.AuditoriaChatApi>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<DataTablesResponse<Custom.AuditoriaChatApi>> GetLast(DataTablesRequest request)
        {
            var customQuery = _auditoriasChatApiRepository.GetAllCustomQuery(limit: 10);
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND idEmpresa = {_permissionsBusiness.Value.User.IdEmpresa} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.AuditoriaChatApi>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<Custom.AuditoriaChatApiTotales> ObtenerTotalesByEstado()
            => await _auditoriasChatApiRepository.GetTotalesByEstado(_permissionsBusiness.Value.User.IdEmpresa);

        private string GetPortalText()
        {
            var host = HttpContextHelper.Current.Request.Host;
            
            return $"Puede gestionar sus turnos y ver su historia clínica desde: https://{host}/portal-pacientes/login";
        }
    }
}
