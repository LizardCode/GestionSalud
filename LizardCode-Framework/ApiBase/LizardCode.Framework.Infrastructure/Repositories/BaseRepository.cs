using Dapper;
using Dapper.Contrib.Extensions;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.Framework.Application.Interfaces.Repositories;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.Infrastructure.Extensions;
using System.Data;

namespace LizardCode.Framework.Infrastructure.Repositories
{
    public abstract class BaseRepository : IBaseRepository
    {
        protected readonly IDbContext _context;


        public BaseRepository(IDbContext context)
        {
            _context = context;
        }


        public virtual async Task<IList<T>> GetAll<T>(IDbTransaction transaction = null)
        {
            var softDeleteMapping = typeof(T).SoftDeleteMapping();

            if (softDeleteMapping.Item1.IsNull())
                return await _context.Connection.InvokeAsync<T, List<T>>(nameof(SqlMapperExtensions.GetAllAsync), null, null);

            var tableMapping = typeof(T).TableMapping();

            if (tableMapping.IsNull())
                throw new ArgumentNullException($"Decorador 'Table' en '{nameof(T)}' inexistente");

            var query = $"SELECT * FROM [{tableMapping}] WHERE [{softDeleteMapping.Item1}] <> @{softDeleteMapping.Item1}Value";
            var parameters = new DynamicParameters();
            parameters.Add($"{softDeleteMapping.Item1}Value", softDeleteMapping.Item2);

            var results = await _context.Connection.QueryAsync<T>(query, parameters, transaction);

            return results.AsList();
        }


        public virtual async Task<T> GetOneQueryAsync<T>(string query, IDbTransaction transaction = null)
        {

            var results = await _context.Connection.QueryFirstOrDefaultAsync<T>(query, new DynamicParameters(), transaction);

            return results;
        }

        public virtual async Task<T> GetById<T>(int id, IDbTransaction transaction = null) =>
            await _context.Connection.InvokeAsync<T>(nameof(SqlMapperExtensions.GetAsync), id, transaction, null);

        public virtual DataTablesCustomQuery GetAllCustomQuery(QueryBuilder queryBuilder)
        {
            var query = $"SELECT * FROM ({queryBuilder.Sql}) [CustomQueryContainer]";
            return new(query, queryBuilder.Parameters.DapperParameters);
        }

        public virtual async Task<bool> Update<T>(T entity, IDbTransaction transaction = null) =>
            await _context.Connection.InvokeAsync<T, bool>(nameof(SqlMapperExtensions.UpdateAsync), entity, transaction, null);

        public virtual async Task<long> Insert<T>(T entity, IDbTransaction transaction = null) =>
            await _context.Connection.InvokeAsync<T, int>(nameof(SqlMapperExtensions.InsertAsync), entity, transaction, null, null); //TODO: ver salida de int a long

        public virtual async Task<bool> Delete<T>(T entity, IDbTransaction transaction = null) =>
            await _context.Connection.InvokeAsync<T, bool>(nameof(SqlMapperExtensions.DeleteAsync), entity, transaction, null);

        public virtual async Task<bool> DeleteById<T>(int id, IDbTransaction transaction = null)
        {
            var entity = await GetById<T>(id);
            return await Delete(entity, transaction);
        }

        public virtual async Task<IList<T>> GetAllQueryAsync<T>(string query, IDbTransaction transaction = null)
        {

            var results = await _context.Connection.QueryAsync<T>(query, new DynamicParameters(), transaction);

            return results.AsList();
        }

        public virtual async Task QueryAsync(string query, IDbTransaction transaction = null)
        {
            await _context.Connection.ExecuteAsync(query, transaction: transaction);
        }

    }
}
