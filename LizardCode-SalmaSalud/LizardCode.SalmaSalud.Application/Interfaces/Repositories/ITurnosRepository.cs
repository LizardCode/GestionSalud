using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITurnosRepository
    {
        Task<TTurno> GetById<TTurno>(int id, IDbTransaction transaction = null);
        Task<long> Insert<TTurno>(TTurno entity, IDbTransaction transaction = null);
        Task<bool> Update<TTurno>(TTurno entity, IDbTransaction transaction = null);

        //DataTablesCustomQuery GetProfesionalesDisponiblesByEspecialidad();
        DataTablesCustomQuery GetProfesionalesDisponiblesByEspecialidad(string desde, string hasta, int idEmpresa, int idEspecialidad, int idProfesional);

        DataTablesCustomQuery GetTuenosDisponiblesByProfesional(DateTime desde, DateTime hasta, int idEmpresa, int idProfesional);

        Task<IList<Turno>> GetTurnosDisponibles(int idEmpresa, DateTime fecha, int idEspecialidad, int idProfesional);

        Task<IList<TurnoDisponible>> GetTurnosDisponiblesPorDia(int idEmpresa, DateTime desde, DateTime hasta, int idEspecialidad, int idProfesional);
        Task<IList<Turno>> GetPrimerosTurnosDisponibles(int idEmpresa, DateTime desde, int idEspecialidad, int idProfesional);
        //Task<List<Turno>> GetTurnosHoy(int idEmpresa, IDbTransaction transaction = null);

        DataTablesCustomQuery GetTurnos(IDbTransaction transaction = null);

        Task<List<Turno>> GetTurnosByIdPaciente(int idPaciente, IDbTransaction transaction = null);
        Task<Turno> GetByIdCustom(int idTurno, IDbTransaction transaction = null);
        DataTablesCustomQuery GetSalaEspera(IDbTransaction transaction = null);
        DataTablesCustomQuery GetTurnosReagendar(int idEmpresa, int idProfesional, int idEspecialidad, IDbTransaction transaction = null);
        Task<TurnosTotales> GetTotalesByEstado(int idEmpresa, int idProfesional, int idPaciente);
        Task<List<Turno>> GetSobreTurnos(DateTime desde, DateTime hasta, int idEmpresa, int idProfesional, IDbTransaction transaction = null);
        Task<Turno> GetByIdProfesionalTurnoCustom(int idProfesionalTurno, IDbTransaction transaction = null);
        Task<IList<Turno>> GetTurnosByFecha(int idEmpresa, DateTime fecha, int idProfesional = 0, IDbTransaction transaction = null);
        Task<List<Select2Custom>> GetMedicamentos(string q);
        Task<TurnosTotales> GetTotalesDashboard(int idEmpresa, int idProfesional);
        DataTablesCustomQuery GetGuardia(IDbTransaction transaction = null);
        Task<List<Turno>> GetReporteTurnos(Dictionary<string, object> filters);
        Task<TurnosEstadisticasAusentes> GetEstadisticasAusentes(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad);
        Task<List<TurnosEstadisticasEstados>> GetEstadisticasEstados(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad, IDbTransaction transaction = null);
    }
}
