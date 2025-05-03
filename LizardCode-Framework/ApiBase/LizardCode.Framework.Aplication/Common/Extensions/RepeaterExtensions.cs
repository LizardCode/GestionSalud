using System.Linq.Expressions;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class RepeaterExtensions
    {
        public static Expression<Func<TModel, TProperty>> ItemPropertyExpression<TModel, TItem, TProperty>(this Func<TModel, TProperty> input, string propertyName)
        {
            var pe = Expression.Parameter(typeof(TModel), "m");
            var ex = Expression.Lambda<Func<TModel, TProperty>>(
                Expression.MakeMemberAccess(
                    Expression.Convert(
                        Expression.Property(
                            Expression.MakeMemberAccess(pe, typeof(TModel).GetProperty("Items")),
                            typeof(IList<object>).GetProperty("Item"),
                            Expression.Constant(0)
                        ),
                        typeof(TItem)
                    ),
                    typeof(TItem).GetProperty(propertyName)
                ),
                pe
            );

            return ex;
        }
    }
}
