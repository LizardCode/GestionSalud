using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.Financiadores;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IFinanciadoresBusiness
    {
        Task<FinanciadorViewModel> Get(int idFinanciador);
        Task<DataTablesResponse<Custom.Financiador>> GetAll(DataTablesRequest request);
        Task New(FinanciadorViewModel model);
        Task Remove(int idFinanciador);
        Task Update(FinanciadorViewModel model);
        Task<string> ValidarNroCUIT(string cuit, int? idFinanciador);
        Task<Custom.Contribuyente> GetPadron(string cuit);
        Task<IList<FinanciadorPlan>> GetAllByFinanciadorId(int idFinanciador);
        //Task<List<FinanciadorPrestacionViewModel>> ProcesarExcel(IFormFile file);
        Task<IList<FinanciadorPrestacion>> GetAllPrestacionesByFinanciadorId(int idFinanciador);
        Task<FinanciadorPrestacion> GetFinanciadorPrestacionById(int idFinanciadorPrestacion);
        Task<FinanciadorPlan> GetPlanById(int idFinanciadorPlan);
        Task ADMIN_CreateClientes();
        Task<IList<FinanciadorPrestacion>> GetAllPrestacionesByIdFinanciadorAndIdPlan(int idFinanciador, int idFinanciadorPlan);
    }
}
