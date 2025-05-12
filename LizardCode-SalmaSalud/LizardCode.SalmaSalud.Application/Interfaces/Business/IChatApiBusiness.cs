using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.AuditoriasChatApiViewModel;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IChatApiBusiness
    {
        Task<AuditoriaChatApiViewModel> Get(int idAuditoriaChatApi);
        Task<DataTablesResponse<AuditoriaChatApi>> GetAll(DataTablesRequest request);
        Task<DataTablesResponse<AuditoriaChatApi>> GetLast(DataTablesRequest request);
        Task<AuditoriaChatApiTotales> ObtenerTotalesByEstado();
        Task SendMessage(EventosChatApi evento, string telefono, string mensaje, int? idEmpresa, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageBienvenida(string telefono, string nombre, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageCodigoAcceso(string telefono, string nombre, string codigo, DateTime vencimiento, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageDemandaEspontanea(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageGuardia(string telefono, DateTime fechaTurno, string paciente, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageLlamar(string telefono, string consultorio, string paciente, string profesional, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageSobreTurnoAsignado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageSolicitudTurnoAsignado(string telefono, DateTime fechaTurno, string paciente, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoAsignado(string telefono, DateTime fechaTurno, string nombre, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoCancelado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoCancelado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, string motivo, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoConfirmado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoGuardiaCancelado(string telefono, string paciente, string motivo, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoReAgendado(string telefono, DateTime fechaTurno, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
        Task SendMessageTurnoRecepcionado(string telefono, string consultorio, string paciente, string profesional, string especialidad, int? idPaciente = null, IDbTransaction transaction = null);
    }
}
