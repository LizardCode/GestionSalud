using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dapper.DataTables.Models
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTablesSearchInfo Search { get; set; }
        public List<DataTablesSortInfo> Order { get; set; }
        public List<DataTablesColumnInfo> Columns { get; set; }
        public string Error { get; set; } = "";
        public string Filters { get; set; }


        public Dictionary<string, object> ParseFilters()
        {
            if (string.IsNullOrEmpty(Filters))
                return new();

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(Filters);
        }
    }
}
