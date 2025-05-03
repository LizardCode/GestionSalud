using System.Data;

namespace LizardCode.Framework.Application.Interfaces.Repositories
{

    public interface IBaseRepository
    {
        Task<IList<T>> GetAll<T>(IDbTransaction transaction = null);
        Task<T> GetById<T>(int id, IDbTransaction transaction = null);
        Task<long> Insert<T>(T entity, IDbTransaction transaction = null);
        Task<bool> Update<T>(T entity, IDbTransaction transaction = null);
        Task<bool> Delete<T>(T entity, IDbTransaction transaction = null);
        Task<bool> DeleteById<T>(int id, IDbTransaction transaction = null);
        Task<IList<T>> GetAllQueryAsync<T>(string query, IDbTransaction transaction = null);
        Task QueryAsync(string query, IDbTransaction transaction = null);
        Task<T> GetOneQueryAsync<T>(string query, IDbTransaction transaction = null);
    }
}
