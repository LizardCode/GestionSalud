using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.PlanillaGastos;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPlanillaGastosBusiness
    {
        Task<PlanillaGastosViewModel> Get(int idPlanillaGastos);
        Task<DataTablesResponse<Custom.PlanillaGastos>> GetAll(DataTablesRequest request);
        Task New(PlanillaGastosViewModel model);
        Task Remove(int idPlanillaGastos);
        Task Update(PlanillaGastosViewModel model);
        Task<Custom.PlanillaGastos> GetCustom(int idAsiento);
        //Task<List<Select2Custom>> GetItemsGastos(string annoMes, int numero, string moneda);
        //Task<double> GetSaldoItemGastos(string annoMes, int numero, int item);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<List<PlanillaGastosDetalle>> ProcesarExcel(IFormFile file);
    }
}