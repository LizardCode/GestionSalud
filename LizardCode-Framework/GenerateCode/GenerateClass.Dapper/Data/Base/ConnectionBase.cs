using System;
using System.Data;
using System.Data.SqlClient;

namespace GenerateClass.Dapper.Data
{
    public class ConnectionBase
    {
        public string _connectionStr { get; set; }
        public int? _commandTimeout { get; set; }

        public IDbConnection ConnectionFactory()
        {
            if (string.IsNullOrEmpty(_connectionStr))
            {
                throw new Exception(string.Format("The configuration key '{0}' could not be accessed.Please add an entry in the web.config file with the key '{0}' containing the connection string.", _connectionStr));
            }

            IDbConnection sqlConnection = new SqlConnection(_connectionStr);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
