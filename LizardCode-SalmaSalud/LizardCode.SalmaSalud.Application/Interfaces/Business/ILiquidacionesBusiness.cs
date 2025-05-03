using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.LiquidacionesProfesionales;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ILiquidacionesProfesionalesBusiness
    {
        Task<DataTablesResponse<Custom.LiquidacionProfesional>> GetAll(DataTablesRequest request);

        Task<LiquidacionProfesionalViewModel> Get(int idLiquidacionProfesional);
        Task<Custom.LiquidacionProfesional> GetCustomById(int idLiquidacionProfesional);

        Task New(LiquidacionProfesionalViewModel model);
        Task Remove(int idLiquidacionProfesional);
        Task<List<LiquidacionProfesionalPrestacionViewModel>> GetPrestacionesALiquidar(DateTime desde, DateTime hasta, int idProfesional);
    }
}
