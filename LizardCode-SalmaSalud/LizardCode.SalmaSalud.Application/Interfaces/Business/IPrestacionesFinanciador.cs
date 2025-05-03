using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPrestacionesFinanciadorBusiness
    {
        Task<PrestacionViewModel> Get(int idPrestacion);
        Task<DataTablesResponse<FinanciadorPrestacion>> GetAll(DataTablesRequest request, int idFinanciador);
        Task<Domain.Entities.FinanciadorPrestacion> GetByCodigo(string codigo);
        Task<PrestacionesImportarExcelResultViewModel> ImportarPrestacionesExcel(IFormFile file, int idFinanciador);
        Task New(PrestacionViewModel model, int idFinanciador);
        Task Remove(int idPrestacion);
        Task Update(PrestacionViewModel model, int idFinanciador);
        Task<bool> ValidarCodigo(string codigo);
    }
}
