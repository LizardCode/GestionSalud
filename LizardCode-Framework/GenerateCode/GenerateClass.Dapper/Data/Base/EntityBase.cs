using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenerateClass.Dapper.Data
{
    public class EntityBase<TEntity> : ConnectionBase where TEntity : class
    {
        public bool buffered { get; set; }

        public EntityBase(string connectionStr)
        {
            _connectionStr = connectionStr;

            buffered = false;

        }

        /// <summary>
        /// Creates new connection.
        /// </summary>
        /// <param name="connectionStr">The connection string.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>

        private TEntity _getOneValue(string procedureNameOrQuery, object oParams, CommandType cmdType)
        {
            TEntity result;
            using (var sqlConnection = ConnectionFactory())
            {
                result = sqlConnection.QueryFirstOrDefault<TEntity>(procedureNameOrQuery, oParams, commandTimeout: _commandTimeout, commandType: cmdType);
            }
            return result;
        }


        /// <summary>
        /// Gets the one value from query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionStr">The connection string.</param>
        /// <param name="query">The query.</param>
        /// <param name="oParams">The o parameters.</param>
        /// <returns></returns>
        public TEntity GetOneValueFromQuery(string query, object oParams)
        {
            return _getOneValue(query, oParams, CommandType.Text);
        }

        /// <summary>
        /// Gets all query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="oParams">The o parameters.</param>
        /// <param name="cmdType">Type of the command.</param>
        /// <returns></returns>
        public List<TEntity> GetAllQuery(string query, object oParams)
        {
            using (var sqlConnection = ConnectionFactory())
            {
                return sqlConnection.Query<TEntity>(query, param: oParams, commandTimeout: _commandTimeout, commandType: CommandType.Text, buffered: buffered).ToList();
            }
           
        }

        /// <summary>
        /// Executes from query asynchronous.
        /// </summary>
        /// <param name="connectionStr">The connection string.</param>
        /// <param name="query">The query.</param>
        /// <param name="oParams">The o parameters.</param>
        /// <param name="transaction">The transaction.</param>
        public void ExecuteFromQueryAsync(string query, object oParams, IDbTransaction transaction = null)
        {
            using (var sqlConnection = ConnectionFactory())
            {
                sqlConnection.Execute(query, oParams, commandTimeout: _commandTimeout, transaction: transaction, commandType: CommandType.Text);
            }
        }

    }
}

