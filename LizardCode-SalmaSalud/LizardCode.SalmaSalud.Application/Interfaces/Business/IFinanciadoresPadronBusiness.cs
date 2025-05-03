using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.FinanciadoresPadron;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IFinanciadoresPadronBusiness
    {
        Task<FinanciadorPadronViewModel> Get(int idFinanciadorPadron);
        Task<DataTablesResponse<FinanciadorPadron>> GetAll(DataTablesRequest request, int idFinanciador);
        Task<Domain.Entities.FinanciadorPadron> GetByDocumento(string documento, int idFinanciador);
        Task<PadronImportarExcelResultViewModel> ImportarPrestacionesExcel(IFormFile file, int idFinanciador);
        Task New(FinanciadorPadronViewModel model, int idFinanciador);
        Task Remove(int idFinanciadorPadron);
        Task Update(FinanciadorPadronViewModel model, int idFinanciador);
        Task<bool> ValidarDocumento(string codigo, int idFinanciador);
    }
}
