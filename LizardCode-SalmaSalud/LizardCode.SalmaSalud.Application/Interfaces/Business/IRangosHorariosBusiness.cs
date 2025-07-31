using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.RangosHorarios;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IRangosHorariosBusiness
    {
        Task<RangosHorariosViewModel> Get(int idRubro);
        Task<DataTablesResponse<Custom.TipoRangoHorario>> GetAll(DataTablesRequest request);
        Task New(RangosHorariosViewModel model);
        Task Remove(int idRango);
        Task Update(RangosHorariosViewModel model);
    }
}
