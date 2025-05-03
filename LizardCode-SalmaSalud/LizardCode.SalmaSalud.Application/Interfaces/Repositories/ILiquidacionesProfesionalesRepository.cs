using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ILiquidacionesProfesionalesRepository
    {
        Task<IList<TPresupuesto>> GetAll<TPresupuesto>(IDbTransaction transaction = null);

        Task<TPresupuesto> GetById<TPresupuesto>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPresupuesto>(TPresupuesto entity, IDbTransaction transaction = null);

        Task<bool> Update<TPresupuesto>(TPresupuesto entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<LiquidacionProfesional> GetCustomById(int idLiquidacionProfesional, IDbTransaction transaction = null);
    }
}
