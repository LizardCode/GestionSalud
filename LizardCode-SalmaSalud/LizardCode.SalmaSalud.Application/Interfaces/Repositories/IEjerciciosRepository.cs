using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEjerciciosRepository
    {
        Task<IList<TEjercicio>> GetAll<TEjercicio>(IDbTransaction transaction = null);

        Task<TEjercicio> GetById<TEjercicio>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEjercicio>(TEjercicio entity, IDbTransaction transaction = null);

        Task<bool> Update<TEjercicio>(TEjercicio entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<string> GetLastCodigoByIdEmpresa(int idEmpresa);

        Task<bool> ValidateFechaEjercicio(int idEjercicio, DateTime fecha, int idEmpresa);

        Task<bool> ValidateFechaEnOtroEjercicio(DateTime fecha, int idEmpresa);

        Task<List<Ejercicio>> GetAllByIdEmpresa(int idEmpresa);

        Task<bool> EjercicioCerrado(int idEjercicio, int idEmpresa);

        Task<Ejercicio> GetCurrentByFechaIdEmpresa(DateTime fecha, int idEmpresa, IDbTransaction tran = null);
    }
}