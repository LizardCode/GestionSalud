using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISdoCtaCteCliBusiness
    {
        Task<SdoCtaCteCliViewModel> Get(int idSdoCtaCteCli);
        Task<DataTablesResponse<Domain.EntitiesCustom.SdoCtaCteCli>> GetAll(DataTablesRequest request);
        Task<SdoCtaCteCliViewModel> GetCustom(int idSdoCtaCteCli);
        Task New(SdoCtaCteCliViewModel model);
        Task<List<SdoCtaCteCliDetalle>> ProcesarExcel(IFormFile file);
        Task Remove(int idSdoCtaCteCli);
        Task Update(SdoCtaCteCliViewModel model);
    }
}
