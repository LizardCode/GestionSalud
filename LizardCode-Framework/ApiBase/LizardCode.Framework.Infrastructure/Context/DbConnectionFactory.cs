using LizardCode.Framework.Application.Interfaces.Context;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace LizardCode.Framework.Infrastructure.Context
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private string _connectionKey;


        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionKey = "DefaultConnection";
        }

        public DbConnectionFactory(IConfiguration configuration, string connectionKey)
        {
            _configuration = configuration;
            _connectionKey = connectionKey;
        }

        public SqlConnection Create()
        {
            return Create(_connectionKey);
        }

        public SqlConnection Create(string name)
        {
            var conn = _configuration.GetConnectionString(name);
            return new SqlConnection(conn);
        }
    }
}