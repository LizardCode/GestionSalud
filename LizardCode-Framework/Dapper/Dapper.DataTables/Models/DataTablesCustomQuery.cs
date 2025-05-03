namespace Dapper.DataTables.Models
{
    public class DataTablesCustomQuery
    {
        public string Sql { get; }
        public DynamicParameters Parameters { get; }


        public DataTablesCustomQuery(string sql, DynamicParameters parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }
    }
}
