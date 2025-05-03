namespace Dapper.DataTables.Models
{
    public class DataTablesColumnInfo
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTablesSearchInfo Search { get; set; }
    }
}
