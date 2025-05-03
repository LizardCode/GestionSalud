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
    public interface IEvolucionesRepository
    {
        Task<IList<TEvolucion>> GetAll<TEvolucion>(IDbTransaction transaction = null);

        Task<TEvolucion> GetById<TEvolucion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucion>(TEvolucion entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucion>(TEvolucion entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<Evolucion> GetCustomById(int idEvolucion, IDbTransaction transaction = null);
        Task<List<Evolucion>> GetEvolucionesPaciente(int idPaciente, IDbTransaction transaction = null);
        Task<EvolucionesEstadisticas> GetEstadisticas(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad);
        Task<List<EvolucionesEstadisticasFinanciador>> GetEstadisticasFinanciador(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad, IDbTransaction transaction = null);
        Task<List<EvolucionesEstadisticasEspecialidad>> GetEstadisticasEspecialidad(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad, IDbTransaction transaction = null);
    }
}