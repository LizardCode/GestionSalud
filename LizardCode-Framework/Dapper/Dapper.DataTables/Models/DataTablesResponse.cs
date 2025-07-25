﻿using System.Collections.Generic;

namespace Dapper.DataTables.Models
{
    public class DataTablesResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IEnumerable<T> Data { get; set; }
        public string Error { get; set; }

        public DataTablesResponse()
        {
            Data = new List<T>();
        }
    }
}
