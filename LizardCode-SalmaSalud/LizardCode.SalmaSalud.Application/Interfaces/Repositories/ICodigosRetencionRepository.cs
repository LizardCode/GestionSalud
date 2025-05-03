using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionRepository
    {
        Task<IList<TCodigosRetencion>> GetAll<TCodigosRetencion>(IDbTransaction transaction = null);

        Task<TCodigosRetencion> GetById<TCodigosRetencion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCodigosRetencion>(TCodigosRetencion entity, IDbTransaction transaction = null);

        Task<bool> Update<TCodigosRetencion>(TCodigosRetencion entity, IDbTransaction transaction = null);

        Task<IList<TCodigosRetencion>> GetAllByTipo<TCodigosRetencion>(int? idTipoRetencion, IDbTransaction transaction = null);

        Task<Domain.EntitiesCustom.CodigosRetencion> GetByIdCustom(int idCodigoRetencion, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
    }
}