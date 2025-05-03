using System.Data.SqlClient;

namespace LizardCode.Framework.Application.Interfaces.Context
{
    public interface IDbConnectionFactory
    {
        SqlConnection Create();
        SqlConnection Create(string connectionName);
    }
}
