using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPlantillasRepository
    {
        Task<IList<TPlantilla>> GetAll<TPlantilla>(IDbTransaction transaction = null);
        DataTablesCustomQuery GetAllCustomQuery();
        Task<TPlantilla> GetById<TPlantilla>(int id, IDbTransaction transaction = null);
        Task<long> Insert<TPlantilla>(TPlantilla entity, IDbTransaction transaction = null);
        Task<bool> Update<TPlantilla>(TPlantilla entity, IDbTransaction transaction = null);
        Task<List<PlantillaEtiqueta>> GetPlantillaEtiquetasByTipo(int idTipoPlantilla, IDbTransaction transaction = null);
        Task<List<Plantilla>> GetPlantillasByTipo(int idEmpresa, int idTipoPlantilla, IDbTransaction transaction = null);
    }
}