using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IArchivosRepository
    {
        Task<IList<TArchivo>> GetAll<TArchivo>(IDbTransaction transaction = null);

        Task<TArchivo> GetById<TArchivo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TArchivo>(TArchivo entity, IDbTransaction transaction = null);

        Task<bool> Update<TArchivo>(TArchivo entity, IDbTransaction transaction = null);

        Task<bool> DeleteById<TArchivo>(int id, IDbTransaction transaction = null);
    }
}
