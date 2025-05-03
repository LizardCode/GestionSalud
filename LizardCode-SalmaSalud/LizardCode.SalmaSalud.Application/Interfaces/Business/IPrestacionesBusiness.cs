using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.Prestaciones;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPrestacionesBusiness
    {
        Task<PrestacionViewModel> Get(int idPrestacion);
        Task<DataTablesResponse<Prestacion>> GetAll(DataTablesRequest request);
        Task<PrestacionesImportarExcelResultViewModel> ImportarPrestacionesExcel(IFormFile file);
        Task New(PrestacionViewModel model);
        Task Remove(int idPrestacion);
        Task Update(PrestacionViewModel model);
        Task<bool> ValidarCodigo(string codigo);
    }
}
