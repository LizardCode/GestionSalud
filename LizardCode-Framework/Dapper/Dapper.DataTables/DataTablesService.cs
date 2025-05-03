using Dapper.Contrib.Extensions;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Internal;
using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dapper.DataTables
{
    //
    // https://github.com/peterschlosser/DataTablesCoreMVCDapper
    //
    public class DataTablesService : IDataTablesService
    {
        protected readonly IDbConnection _connection;
        private string _finalQuery;


        public DataTablesService(IDbConnection connection)
        {
            _connection = connection;
            _finalQuery = string.Empty;
        }


        public async Task<DataTablesResponse<T>> Resolve<T>(DataTablesRequest request) where T : class, new()
        {
            var entityInfo = EntityInfo<T>();
            var query = $"SELECT * FROM [{entityInfo.Table}]";

            return await Resolve<T>(request, query, null, true);
        }

        public async Task<DataTablesResponse<T>> Resolve<T>(DataTablesRequest request, string initialQuery, DynamicParameters initialParameters, bool withSoftDelete = false, string staticWhere = null) where T : class, new()
        {
            _finalQuery = initialQuery;

            var entityInfo = (withSoftDelete ? EntityInfo<T>() : default);
            var softDeleteParameters = Parameters(entityInfo?.SoftDelete, null, initialParameters);
            var total = await Count(softDeleteParameters, entityInfo?.SoftDelete);

            if (request == null)
            {
                var results = await _connection.QueryAsync<T>(initialQuery);

                return new DataTablesResponse<T>
                {
                    Draw = 0,
                    RecordsTotal = total,
                    RecordsFiltered = 0,
                    Data = results,
                    Error = null
                };
            }

            Where<T>(request, entityInfo?.SoftDelete, staticWhere);

            var parameters = Parameters(entityInfo?.SoftDelete, request, initialParameters);
            var filtered = await Count(parameters, entityInfo?.SoftDelete);

            Order(request);
            SkipTake(request);

            var data = await Results<T>(parameters);

            return new DataTablesResponse<T>
            {
                Draw = request.Draw,
                RecordsTotal = total,
                RecordsFiltered = filtered,
                Data = data,
                Error = request.Error
            };
        }


        private void Order(DataTablesRequest request)
        {
            var order = $" ORDER BY 1";

            if (request.Order == null)
            {
                _finalQuery += order;
                return;
            }

            if (request.Order.Any())
            {
                var columns = request.Order
                    .Select(o =>
                    {
                        var column = request.Columns[o.Column];

                        return new
                        {
                            Name = $"[{column.Name ?? column.Data}]",
                            Direction = o.Dir.ToUpper()
                        };
                    });

                order = $" ORDER BY {string.Join(",", columns.Select(s => $"{s.Name} {s.Direction}"))}";
            }

            _finalQuery += order;
        }

        private void SkipTake(DataTablesRequest request)
        {
            var offset = Math.Max(0, request.Start);
            var fetch = Math.Max(0, request.Length);

            _finalQuery += $" OFFSET {offset} ROWS {(fetch > 0 ? $"FETCH NEXT {fetch} ROWS ONLY" : "")}";
        }

        private void Where<T>(DataTablesRequest request, SoftDeleteInfo softDelete, string staticWhere) where T : class, new()
        {
            var where = "1=1";
            var regex = Regex.IsMatch(_finalQuery, @"(?<!\()where(?![\s\S]*[\)])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var instruction = regex ? "AND" : "WHERE";
            var operations = Enumerable.Empty<FilterOperation>();

            if (softDelete != null)
                where = $"([{softDelete.Property}] <> @{softDelete.Property}Value)";

            if (!string.IsNullOrWhiteSpace(request.Filters) && string.IsNullOrWhiteSpace(staticWhere))
                operations = operations.Append(new FilterOperation
                {
                    Operation = " AND ",
                    Conditions = request.ParseFilters()
                        .Select(s => new FilterCondition(s.Key, "LIKE", $"@{s.Key}Value"))
                        .AsList()
                });

            if (!string.IsNullOrWhiteSpace(request.Search?.Value))
                operations = operations.Append(new FilterOperation
                {
                    Operation = " OR ",
                    Conditions = request.Columns
                        .Where(w => w.Searchable && (!string.IsNullOrWhiteSpace(w.Name) || !string.IsNullOrWhiteSpace(w.Data)))
                        .Select(s => new FilterCondition(s.Name ?? s.Data, "LIKE", "@SearchToken"))
                        .AsList()
                });

            if (operations.Any())
            {
                var properties = typeof(T).GetProperties();
                var whereFields = string.Join(
                    " OR ",
                    operations.Select(condition =>
                    {
                        var fields = string.Join(
                            condition.Operation,
                            condition.Conditions.Select(column =>
                            {
                                var prop = properties.First(f => f.Name.Equals(column.FieldName, StringComparison.OrdinalIgnoreCase));
                                return Type.GetTypeCode(prop.PropertyType) switch
                                {
                                    TypeCode.DateTime => $"FORMAT([{prop.Name}], 'dd/MM/yyyy HH:mm:ss') {column.Operation} {column.Token}",
                                    _ => $"[{prop.Name}] {column.Operation} {column.Token}"
                                };
                            })
                        );

                        return condition.Operation.Equals(" AND ") ? $"({fields})" : fields;
                    })
                );

                where = $"{where} AND ({whereFields})";
            }

            if (!string.IsNullOrWhiteSpace(staticWhere))
                where = $"{where} AND ({staticWhere.TrimStart('A', 'N', 'D')})";

            _finalQuery += $" {instruction} {where}";
        }


        private async Task<int> Count(DynamicParameters parameters = null, SoftDeleteInfo softDelete = null)
        {
            var query = _finalQuery;

            if (softDelete != null)
            {
                var instruction = _finalQuery.ToUpper().Contains("WHERE") ? "AND" : "WHERE";
                query = $"{_finalQuery} {instruction} [{softDelete.Property}] <> @{softDelete.Property}Value";
            }

            var subQuery = $"SELECT COUNT(1) FROM ({query}) [T];";

            return await _connection.QuerySingleAsync<int>(subQuery, parameters);
        }

        private async Task<IList<T>> Results<T>(DynamicParameters parameters = null) where T : class, new()
        {
            var results = await _connection.QueryAsync<T>(_finalQuery, parameters);

            return results.AsList();
        }


        private EntityInfo EntityInfo<T>() where T : class, new()
        {
            var type = typeof(T);
            var classAttr = type.GetCustomAttributes(typeof(TableAttribute), true); // false) ;
            var name = ((TableAttribute)classAttr.First()).Name;
            var softDeleteInfo = default(SoftDeleteInfo);

            var softDeleteProperty = type
                .GetProperties()
                .SingleOrDefault(pi => pi.GetCustomAttributes(typeof(SoftDeleteAttribute), false).Any());

            if (softDeleteProperty != null)
            {
                var propertyAttr = softDeleteProperty
                    .GetCustomAttributes(typeof(SoftDeleteAttribute), false)
                    .SingleOrDefault();

                softDeleteInfo = new SoftDeleteInfo
                {
                    Property = softDeleteProperty.Name,
                    Value = ((SoftDeleteAttribute)propertyAttr).Value
                };
            }

            return new EntityInfo
            {
                Table = name,
                SoftDelete = softDeleteInfo
            };
        }

        private DynamicParameters Parameters(SoftDeleteInfo softDelete, DataTablesRequest request = null, DynamicParameters initialParameters = null)
        {
            var parameters = new DynamicParameters(initialParameters);

            if (softDelete != null)
                parameters.Add($"@{softDelete.Property}Value", softDelete.Value);

            if (request == null)
                return parameters;

            if (!string.IsNullOrWhiteSpace(request.Filters))
                request.ParseFilters()
                    .AsList()
                    .ForEach(filter =>
                        parameters.Add($"@{filter.Key}Value", $"%{filter.Value}%")
                    );

            if (!string.IsNullOrWhiteSpace(request.Search?.Value))
                parameters.Add("@SearchToken", $"%{request.Search?.Value}%");

            return parameters;
        }
    }
}
