using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCtePrv;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISdoCtaCtePrvBusiness
    {
        Task<SdoCtaCtePrvViewModel> Get(int idSdoCtaCtePrv);
        Task<DataTablesResponse<Domain.EntitiesCustom.SdoCtaCtePrv>> GetAll(DataTablesRequest request);
        Task<SdoCtaCtePrvViewModel> GetCustom(int idSdoCtaCtePrv);
        Task New(SdoCtaCtePrvViewModel model);
        Task<List<SdoCtaCtePrvDetalle>> ProcesarExcel(IFormFile file);
        Task Remove(int idSdoCtaCtePrv);
        Task Update(SdoCtaCtePrvViewModel model);
    }
}
