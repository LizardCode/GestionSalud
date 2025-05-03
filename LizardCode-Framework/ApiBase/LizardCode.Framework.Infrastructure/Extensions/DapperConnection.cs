using Dapper.Contrib.Extensions;
using System.Data;

namespace LizardCode.Framework.Infrastructure.Extensions
{
    public static class DapperConnection
    {
        public static async Task<T> InvokeAsync<T>(this IDbConnection connection, string method, params object[] parameters)
        {
            return await InvokeAsync<T, T>(connection, method, parameters);
        }

        public static async Task<TOut> InvokeAsync<TIn, TOut>(this IDbConnection connection, string method, params object[] parameters)
        {
            var task = (Task)typeof(SqlMapperExtensions)
                .GetMethod(method)
                .MakeGenericMethod(typeof(TIn))
                .Invoke(
                    null,
                    parameters?
                        .ToList()
                        .Prepend(connection)
                        .ToArray()
                );

            await task.ConfigureAwait(false);

            return (TOut)task.GetType().GetProperty("Result").GetValue(task);
        }
    }
}
