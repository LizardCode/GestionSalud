using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IArticulosMovimientosStockRepository
    {
        Task<IList<TArticuloMovimientoStock>> GetAll<TArticuloMovimientoStock>(IDbTransaction transaction = null);

        Task<TArticuloMovimientoStock> GetById<TArticuloMovimientoStock>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TArticuloMovimientoStock>(TArticuloMovimientoStock entity, IDbTransaction transaction = null);

        Task<bool> Update<TArticuloMovimientoStock>(TArticuloMovimientoStock entity, IDbTransaction transaction = null);

    }
}