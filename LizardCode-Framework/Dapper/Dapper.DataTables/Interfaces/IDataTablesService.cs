using Dapper.DataTables.Models;
using System.Threading.Tasks;

namespace Dapper.DataTables.Interfaces
{
    public interface IDataTablesService
    {
        Task<DataTablesResponse<T>> Resolve<T>(DataTablesRequest request) where T : class, new();
        Task<DataTablesResponse<T>> Resolve<T>(DataTablesRequest request, string initialQuery, DynamicParameters initialParameters, bool withSoftDelete = false, string staticWhere = null) where T : class, new();
    }
}