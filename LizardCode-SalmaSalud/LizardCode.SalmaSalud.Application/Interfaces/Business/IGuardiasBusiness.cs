using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Guardias;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IGuardiasBusiness
    {
        Task<DataTablesResponse<Custom.Guardia>> GetAll(DataTablesRequest request);

        Task<GuardiasViewModel> Get(int idGuardia);
        Task<Custom.Guardia> GetCustomById(int idGuardia);

        Task New(GuardiasViewModel model);
        Task Remove(int idGuardia);
    }
}