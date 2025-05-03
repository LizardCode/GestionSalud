using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IEstadoResultadosBusiness
    {
        Task<List<Custom.EstadoResultado>> GetAll(DataTablesRequest request);
    }
}