using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITiposAsientosCuentasRepository
    {
        Task<IList<TipoAsientoCuenta>> GetAll<TipoAsientoCuenta>(IDbTransaction transaction = null);

        Task<TipoAsientoCuenta> GetById<TipoAsientoCuenta>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TipoAsientoCuenta>(TipoAsientoCuenta entity, IDbTransaction transaction = null);

        Task<bool> Update<TipoAsientoCuenta>(TipoAsientoCuenta entity, IDbTransaction transaction = null);

        Task<bool> RemoveAllByIdTipoAsiento(int idTipoAsiento, IDbTransaction transaction = null);
    }
}
