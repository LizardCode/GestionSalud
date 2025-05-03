using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace GenerateClass.Dapper.Data
{
    public class ColumnData : EntityBase<DColumn>
    {

        #region Properties

        private const string resxFile = @".\Temp\ResourceTemp.resx";

        private ResXResourceSet resxSet { get; set; }

        #endregion

        public ColumnData(string connectionStr): base(connectionStr)
        {
            resxSet = new ResXResourceSet(resxFile);
        
        }


        /// <summary>Gets all column.</summary>
        /// <param name="table">The table.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public List<DColumn> GetAllColumn(string table)
        {
            var sql = resxSet.GetString("GetTableColumn");

            var parameter = new Dictionary<string, object>
            {
                { "TableName", table }
            };

            return GetAllQuery(sql, parameter);

        }
    }
}
