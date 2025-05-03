using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Models.Reportes;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ITurnosBusiness
    {
        //Task<DataTablesResponse<Custom.TurnoProfesional>> GetProfesionalesDisponiblesByEspecialidad(DataTablesRequest request);
        Task<DataTablesResponse<Custom.TurnoProfesional>> GetAll(DataTablesRequest request);

        Task<List<TurnoCalendarioEvent>> GetTurnosDisponiblesPorDia(DateTime desde, DateTime hasta, int idEspecialidad, int idProfesional);

        Task<List<Custom.Turno>> GetTurnosDisponibles(DateTime fecha, int idEspecialidad, int idProfesional);
        Task Agendar(int idProfesionalTurno, int idPaciente);
        Task<List<Custom.Turno>> GetPrimerosTurnosDisponibles(int idEspecialidad, int idProfesional);
        Task Asignar(AsignarViewModel model, bool noActualizarPaciente = false);
        Task<DataTablesResponse<Custom.Turno>> GetTurnosHoy(DataTablesRequest request);
        Task<DataTablesResponse<Custom.Turno>> GetSalaEspera(DataTablesRequest request);
        //Task<List<Custom.Turno>> GetTurnosHoy(DataTablesRequest request);

        Task Cancelar(CancelarViewModel model);
        Task Confirmar(int IdTurno);
        Task Recepcionar(RecepcionarViewModel IdTurno);
        Task<Custom.Turno> GetCustomById(int idTurno);
        Task ReAgendar(int idTurno, int idProfesionalTurno);
        Task<DataTablesResponse<Custom.Turno>> GetTurnosReAgendar(int idEspecialidad, int idProfesional, DataTablesRequest request);
        Task<DataTablesResponse<Custom.Turno>> GetTurnos(DataTablesRequest request);
        Task<TurnosTotales> ObtenerTotalesByEstado();
        Task<List<ProfesionalTurnoEvent>> GetAgendaSobreTurnos(DateTime desde, DateTime hasta, int idProfesional);
        Task AsignarSobreTurno(AsignarSobreTurnoViewModel model, bool noActualizarPaciente = false);
        Task CancelarAgenda(CancelarAgendaViewModel model);
        Task DemandaEspontanea(DemandaEspontaneaViewModel model, bool noActualizarPaciente = false);
        Task<DataTablesResponse<Custom.TurnoHistorial>> GetHistorial(int idTurno, DataTablesRequest request);
        Task<List<Custom.Turno>> GetTurnosPaciente();
        Task<List<Custom.Select2Custom>> GetMedicamentos(string q);
        Task<TurnosTotales> ObtenerTotalesDashboard();
        Task<DataTablesResponse<Custom.Turno>> GetTurnosProximos(DataTablesRequest request);
        Task<DataTablesResponse<Custom.Turno>> GetGuardia(DataTablesRequest request);
        Task Guardia(GuardiaViewModel model, bool noActualizarPaciente = false);
        Task Llamar(LlamarViewModel model);
        Task<List<Custom.Turno>> GetReporteTurnos(DataTablesRequest request);
        Task<TurnosEstadisticasAusentes> GetEstadisticasAusentes(TurnosEstadisticasViewModel filters);
        Task<List<TurnosEstadisticasEstados>> GetEstadisticasEstados(TurnosEstadisticasViewModel filters);
    }
}
