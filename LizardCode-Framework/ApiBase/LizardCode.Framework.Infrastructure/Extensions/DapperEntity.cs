using Dapper.Contrib.Extensions;
using System.Reflection;

namespace LizardCode.Framework.Infrastructure.Extensions
{
    public static class DapperEntity
    {
        public static string TableMapping<TEntity>(this TEntity entity) where TEntity : class, new()
        {
            return TableMapping(typeof(TEntity));
        }

        public static string TableMapping(this Type entity)
        {
            var tableAttr = entity
                .GetCustomAttributes(typeof(TableAttribute), false)
                .Cast<TableAttribute>()
                .FirstOrDefault();

            return tableAttr?.Name;
        }

        public static (string, object) SoftDeleteMapping<TEntity>(this TEntity entity) where TEntity : class, new()
        {
            return SoftDeleteMapping(typeof(TEntity));
        }

        public static (string, object) SoftDeleteMapping(this Type entity)
        {
            var propertyInfo = entity
                .GetProperties()
                .SingleOrDefault(property => property.GetCustomAttributes(typeof(SoftDeleteAttribute), false).Any());

            if (propertyInfo == null)
                return default;

            var propertyAttr = propertyInfo
                .GetCustomAttributes(typeof(SoftDeleteAttribute), false)
                .Cast<SoftDeleteAttribute>()
                .FirstOrDefault();

            return (propertyInfo.Name, propertyAttr.Value);
        }

        public static string Table(this Type entityType)
        {
            var attribute = entityType.GetCustomAttribute<TableAttribute>();

            if (attribute == null)
                return null;

            return attribute.Name;
        }
    }
}
