using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IGuardiasRepository
    { 
        Task<IList<TGuardia>> GetAll<TGuardia>(IDbTransaction transaction = null);

        Task<TGuardia> GetById<TGuardia>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TGuardia>(TGuardia entity, IDbTransaction transaction = null);

        Task<bool> Update<TGuardia>(TGuardia entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Guardia> GetCustomById(int idGuardia, IDbTransaction transaction = null);
        Task<List<Guardia>> GetGuardiasALiquidar(DateTime desde, DateTime hasta, int idProfesional, IDbTransaction transaction = null);
        Task<bool> ValidarGuardiaLiquidada(int idGuardia, IDbTransaction transaction = null);
    }
}
