using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPlanillaGastosRepository
    {
        Task<IList<TPlanillaGasto>> GetAll<TPlanillaGasto>(IDbTransaction transaction = null);

        Task<TPlanillaGasto> GetById<TPlanillaGasto>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPlanillaGasto>(TPlanillaGasto entity, IDbTransaction transaction = null);

        Task<bool> Update<TPlanillaGasto>(TPlanillaGasto entity, IDbTransaction transaction = null);

        Task<Domain.EntitiesCustom.PlanillaGastos> GetCustomByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<List<Select2Custom>> GetItemsGastos(int anno, int mes, int numero, string moneda, int idEmpresa);
    }
}