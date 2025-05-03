using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace GenerateClass.Dapper.Data
{
    public class TableData: EntityBase<DTable>
    {

        #region Properties

        private const string resxFile = @".\Temp\ResourceTemp.resx";

        private ResXResourceSet resxSet { get; set; }

        #endregion

        public TableData(string connectionStr): base(connectionStr)
        {
            resxSet = new ResXResourceSet(resxFile);
        
        }

        public List<DTable> GetAll()
        {
            var sql = resxSet.GetString("GetTables");

            return GetAllQuery(sql, new Dictionary<string, object>());
        }
    }
}
